using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    internal class Acceso
    {
        private SqlConnection conexion;
        private SqlTransaction transaccion;

        private const string NombreCadenaConexion = "BDCAPAS";

        public void Abrir()
        {
            conexion = new SqlConnection(CadenaConexion());
            conexion.Open();
        }

        public void Cerrar()
        {
            conexion?.Close();
            conexion = null;
        }

        public void IniciarTx()
        {
            transaccion = conexion.BeginTransaction();
        }

        public void ConfirmarTX()
        {
            transaccion.Commit();
            transaccion = null;
        }

        public void DeshacerTX()
        {
            if (transaccion == null) return;
            transaccion.Rollback();
            transaccion = null;
        }

        private static string CadenaConexion()
        {
            ConnectionStringSettings configuracion = ConfigurationManager.ConnectionStrings[NombreCadenaConexion];
            if (configuracion == null)
                throw new InvalidOperationException(
                    $"Falta la cadena de conexión '{NombreCadenaConexion}' en App.config.");
            return configuracion.ConnectionString;
        }


        private SqlCommand CrearComando(string sql, List<SqlParameter> parametros = null)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = sql;
            cmd.Connection = conexion;
            if (transaccion != null)
            {
                cmd.Transaction = transaccion;
            }
            if (parametros != null)
            {
                cmd.Parameters.AddRange(parametros.ToArray());
            }
            return cmd;
        }

        public int Escribir(string sql, List<SqlParameter> parametros = null)
        {
            SqlCommand cmd = CrearComando(sql, parametros);
            int filas = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return filas;
        }


        public DataTable Leer(string sql, List<SqlParameter> parametros = null)
        {
            SqlDataAdapter adaptador = new SqlDataAdapter();
            adaptador.SelectCommand = CrearComando(sql, parametros);
            DataTable tabla = new DataTable();

            adaptador.Fill(tabla);

            return tabla;

        }



        public SqlParameter CrearParametro(string nombre, string valor)
        {
            SqlParameter p = new SqlParameter();
            p.Value = valor;
            p.ParameterName = nombre;
            p.DbType = DbType.String;
            return p;
        }
        public SqlParameter CrearParametro(string nombre, int valor)
        {
            SqlParameter p = new SqlParameter();
            p.Value = valor;
            p.ParameterName = nombre;
            p.DbType = DbType.Int32;
            return p;
        }

    }
}
