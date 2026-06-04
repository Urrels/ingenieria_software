using System;
using System.Collections.Generic;
using System.Data;
using SeguridadYServicios;

namespace BLL
{
    public class ResultadoIntegridad
    {
        public bool EsValido { get; set; } = true;
        public List<string> Errores { get; set; } = new List<string>();
    }

    /// <summary>
    /// Gestiona los dígitos verificadores horizontales (DVH) y verticales (DVV)
    /// para las entidades más sensibles del sistema: USUARIO, BITACORA y NODO_PERMISO.
    ///
    /// DVH (horizontal): un valor por fila, almacenado en la propia tabla.
    ///   Detecta modificaciones en cualquier atributo de un registro.
    ///
    /// DVV (vertical): un valor por columna lógica, en DIGITO_VERIFICADOR_VERTICAL.
    ///   Detecta inserciones, eliminaciones e intercambios de filas.
    ///
    /// Solución multi-tabla (USUARIO): el estado de un usuario incluye también sus
    /// perfiles (USUARIO_PERFIL). Se incorpora el atributo virtual "PERFILES" —
    /// lista ordenada de IDs de perfil — al cálculo del DVH de USUARIO. Cualquier
    /// cambio externo en USUARIO_PERFIL invalida el DVH del usuario afectado.
    ///
    /// Mecanismo genérico: el algoritmo reside en SeguridadYServicios.CalculadorDVH
    /// y opera sobre string[]. Extender a una nueva entidad requiere solo definir
    /// su lista canónica de atributos y agregar un bloque análogo en
    /// VerificarIntegridad / RecalcularIntegridad.
    /// </summary>
    public class IntegridadBLL
    {
        // Orden canónico de atributos por entidad (excluye DVH).
        // "PERFILES" en USUARIO es atributo virtual derivado de USUARIO_PERFIL.
        private static readonly string[] ColumnasUsuario =
            { "ID", "USUARIO", "PASS", "INTENTOS_FALLIDOS", "BLOQUEADO", "ROL", "PERFILES" };

        private static readonly string[] ColumnasBitacora =
            { "ID", "USUARIO", "ACCION", "FECHA" };

        private static readonly string[] ColumnasNodoPermiso =
            { "ID", "NOMBRE", "TIPO", "PADRE_ID" };

        private readonly DAL.IntegridadDAL    _dal       = new DAL.IntegridadDAL();
        private readonly DAL.UsuarioPerfilDAL _perfilDal = new DAL.UsuarioPerfilDAL();

        // -------------------------------------------------------
        // Extracción de atributos
        // -------------------------------------------------------

        private string[] ExtraerAtributosUsuario(DataRow fila, string perfilesStr)
        {
            return new string[]
            {
                fila["ID"].ToString(),
                fila["USUARIO"].ToString(),
                fila["PASS"].ToString(),
                fila["INTENTOS_FALLIDOS"].ToString(),
                Convert.ToBoolean(fila["BLOQUEADO"]) ? "1" : "0",
                fila["ROL"].ToString(),
                perfilesStr
            };
        }

        private string ObtenerPerfilesStr(int usuarioId)
        {
            List<int> ids = _perfilDal.ListarPerfilesPorUsuario(usuarioId);
            ids.Sort();
            return string.Join(",", ids);
        }

        private string[] ExtraerAtributosBitacora(DataRow fila)
        {
            return new string[]
            {
                fila["ID"].ToString(),
                fila["USUARIO"].ToString(),
                fila["ACCION"].ToString(),
                fila["FECHA"].ToString()   // SP devuelve CONVERT(VARCHAR, FECHA, 120)
            };
        }

        private string[] ExtraerAtributosNodoPermiso(DataRow fila)
        {
            return new string[]
            {
                fila["ID"].ToString(),
                fila["NOMBRE"].ToString(),
                fila["TIPO"].ToString(),
                fila["PADRE_ID"].ToString()  // SP devuelve '0' cuando es NULL
            };
        }

        // -------------------------------------------------------
        // Verificación de una tabla (método genérico interno)
        // -------------------------------------------------------

        private void VerificarTabla(
            string          nombreTabla,
            string[]        columnas,
            DataTable       datos,
            Func<DataRow, string[]> extraerAtributos,
            ResultadoIntegridad resultado)
        {
            List<string[]> todasFilas = new List<string[]>();

            foreach (DataRow fila in datos.Rows)
            {
                int id            = Convert.ToInt32(fila["ID"]);
                int dvhAlmacenado = Convert.ToInt32(fila["DVH"]);
                string[] atribs   = extraerAtributos(fila);
                int dvhCalculado  = CalculadorDVH.Calcular(atribs);

                if (dvhAlmacenado != dvhCalculado)
                {
                    resultado.EsValido = false;
                    resultado.Errores.Add(
                        $"{nombreTabla} ID={id}: DVH inválido.");
                }

                todasFilas.Add(atribs);
            }

            Dictionary<string, int> dvvsAlmacenados = _dal.ObtenerDVV(nombreTabla);

            if (dvvsAlmacenados.Count == 0 && datos.Rows.Count == 0)
                return;

            if (dvvsAlmacenados.Count == 0)
            {
                resultado.EsValido = false;
                resultado.Errores.Add(
                    $"{nombreTabla}: dígitos verificadores verticales ausentes.");
                return;
            }

            for (int c = 0; c < columnas.Length; c++)
            {
                string col       = columnas[c];
                int dvvCalculado = CalculadorDVH.CalcularVertical(todasFilas, c);

                if (!dvvsAlmacenados.ContainsKey(col) || dvvsAlmacenados[col] != dvvCalculado)
                {
                    resultado.EsValido = false;
                    resultado.Errores.Add(
                        $"{nombreTabla} columna {col}: DVV inválido. " +
                        "Posible inserción, eliminación o reordenamiento de filas.");
                }
            }
        }

        // -------------------------------------------------------
        // Recálculo de una tabla (método genérico interno)
        // -------------------------------------------------------

        private void RecalcularTabla(
            string          nombreTabla,
            string[]        columnas,
            DataTable       datos,
            Func<DataRow, string[]> extraerAtributos,
            Action<int, int> actualizarDVH)
        {
            List<string[]> todasFilas = new List<string[]>();

            foreach (DataRow fila in datos.Rows)
            {
                int id          = Convert.ToInt32(fila["ID"]);
                string[] atribs = extraerAtributos(fila);
                int dvh         = CalculadorDVH.Calcular(atribs);
                actualizarDVH(id, dvh);
                todasFilas.Add(atribs);
            }

            for (int c = 0; c < columnas.Length; c++)
            {
                int dvv = CalculadorDVH.CalcularVertical(todasFilas, c);
                _dal.ActualizarDVV(nombreTabla, columnas[c], dvv);
            }
        }

        // -------------------------------------------------------
        // API pública — verificación
        // -------------------------------------------------------

        /// <summary>
        /// Verifica la integridad de USUARIO, BITACORA y NODO_PERMISO.
        /// Se llama al iniciar la aplicación, antes del login.
        /// </summary>
        public ResultadoIntegridad VerificarIntegridad()
        {
            ResultadoIntegridad resultado = new ResultadoIntegridad();

            DataTable usuarios = _dal.ListarUsuariosParaIntegridad();
            VerificarTabla(
                "USUARIO", ColumnasUsuario, usuarios,
                fila => ExtraerAtributosUsuario(fila, ObtenerPerfilesStr(Convert.ToInt32(fila["ID"]))),
                resultado);

            DataTable bitacora = _dal.ListarBitacoraParaIntegridad();
            VerificarTabla(
                "BITACORA", ColumnasBitacora, bitacora,
                ExtraerAtributosBitacora,
                resultado);

            DataTable nodos = _dal.ListarNodosParaIntegridad();
            VerificarTabla(
                "NODO_PERMISO", ColumnasNodoPermiso, nodos,
                ExtraerAtributosNodoPermiso,
                resultado);

            return resultado;
        }

        // -------------------------------------------------------
        // API pública — recálculo por entidad
        // -------------------------------------------------------

        /// <summary>Recalcula DVH y DVV de USUARIO. Llamar tras cualquier mutación de usuarios o perfiles.</summary>
        public void RecalcularIntegridadUsuarios()
        {
            RecalcularTabla(
                "USUARIO", ColumnasUsuario,
                _dal.ListarUsuariosParaIntegridad(),
                fila => ExtraerAtributosUsuario(fila, ObtenerPerfilesStr(Convert.ToInt32(fila["ID"]))),
                _dal.ActualizarDVHUsuario);
        }

        /// <summary>Recalcula DVH y DVV de BITACORA. Llamar tras cualquier escritura en bitácora.</summary>
        public void RecalcularIntegridadBitacora()
        {
            RecalcularTabla(
                "BITACORA", ColumnasBitacora,
                _dal.ListarBitacoraParaIntegridad(),
                ExtraerAtributosBitacora,
                _dal.ActualizarDVHBitacora);
        }

        /// <summary>Recalcula DVH y DVV de NODO_PERMISO. Llamar tras cualquier mutación del árbol de permisos.</summary>
        public void RecalcularIntegridadNodos()
        {
            RecalcularTabla(
                "NODO_PERMISO", ColumnasNodoPermiso,
                _dal.ListarNodosParaIntegridad(),
                ExtraerAtributosNodoPermiso,
                _dal.ActualizarDVHNodo);
        }

        /// <summary>Recalcula las tres entidades de una sola vez. Usado en la inicialización.</summary>
        public void RecalcularIntegridad()
        {
            RecalcularIntegridadUsuarios();
            RecalcularIntegridadBitacora();
            RecalcularIntegridadNodos();
        }

        // -------------------------------------------------------
        // Bootstrap
        // -------------------------------------------------------

        /// <summary>
        /// Retorna true si el sistema de integridad fue inicializado para las tres entidades.
        /// </summary>
        public bool EstaInicializado()
        {
            return _dal.ObtenerDVV("USUARIO").Count      > 0
                && _dal.ObtenerDVV("BITACORA").Count     > 0
                && _dal.ObtenerDVV("NODO_PERMISO").Count > 0;
        }
    }
}
