using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class LiquidacionDAL
    {
        private readonly Acceso _acceso = new Acceso();

        private static SqlParameter Decimal(string nombre, decimal valor)
        {
            return new SqlParameter(nombre, SqlDbType.Decimal) { Precision = 12, Scale = 2, Value = valor };
        }

        public ParametrosLiquidacion ObtenerParametros()
        {
            var p = new ParametrosLiquidacion();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("PARAMETRO_LIQUIDACION_LISTAR");
                foreach (DataRow fila in tabla.Rows)
                {
                    decimal valor = Convert.ToDecimal(fila["VALOR"]);
                    switch (fila["CLAVE"].ToString())
                    {
                        case "TOLERANCIA_MINUTOS": p.ToleranciaMinutos = (int)valor; break;
                        case "REDONDEO_MINUTOS": p.RedondeoMinutos = (int)valor; break;
                        case "JORNADA_NORMAL_HORAS": p.JornadaNormalHoras = valor; break;
                        case "RECARGO_EXTRA_PCT": p.RecargoExtraPct = valor; break;
                        case "RECARGO_FERIADO_PCT": p.RecargoFeriadoPct = valor; break;
                        case "MAX_HORAS_FICHAJE_ABIERTO": p.MaxHorasFichajeAbierto = (int)valor; break;
                    }
                }
            }
            finally { _acceso.Cerrar(); }
            return p;
        }

        public void GuardarParametros(ParametrosLiquidacion p)
        {
            var valores = new Dictionary<string, decimal>
            {
                { "TOLERANCIA_MINUTOS", p.ToleranciaMinutos },
                { "REDONDEO_MINUTOS", p.RedondeoMinutos },
                { "JORNADA_NORMAL_HORAS", p.JornadaNormalHoras },
                { "RECARGO_EXTRA_PCT", p.RecargoExtraPct },
                { "RECARGO_FERIADO_PCT", p.RecargoFeriadoPct },
                { "MAX_HORAS_FICHAJE_ABIERTO", p.MaxHorasFichajeAbierto }
            };
            try
            {
                _acceso.Abrir();
                _acceso.IniciarTx();
                foreach (var kvp in valores)
                {
                    _acceso.Escribir("PARAMETRO_LIQUIDACION_GUARDAR", new List<SqlParameter>
                    {
                        _acceso.CrearParametro("@clave", kvp.Key),
                        Decimal("@valor", kvp.Value)
                    });
                }
                _acceso.ConfirmarTX();
            }
            catch
            {
                _acceso.DeshacerTX();
                throw;
            }
            finally { _acceso.Cerrar(); }
        }

        public List<EmpleadoLiquidable> ListarEmpleados()
        {
            var lista = new List<EmpleadoLiquidable>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("USUARIO_LISTAR_EMPLEADOS");
                foreach (DataRow fila in tabla.Rows)
                {
                    lista.Add(new EmpleadoLiquidable
                    {
                        Id = Convert.ToInt32(fila["ID"]),
                        Usuario = fila["USUARIO"].ToString(),
                        Nombre = fila["NOMBRE"] == DBNull.Value ? null : fila["NOMBRE"].ToString(),
                        Apellido = fila["APELLIDO"] == DBNull.Value ? null : fila["APELLIDO"].ToString()
                    });
                }
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }

        public List<ValorHoraEmpleado> ListarValoresHora()
        {
            var lista = new List<ValorHoraEmpleado>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("VALOR_HORA_LISTAR");
                foreach (DataRow fila in tabla.Rows)
                {
                    lista.Add(new ValorHoraEmpleado
                    {
                        Id = Convert.ToInt32(fila["ID"]),
                        UsuarioId = Convert.ToInt32(fila["USUARIO_ID"]),
                        Empleado = AsistenciaPersonalDAL.NombreEmpleado(fila),
                        Valor = Convert.ToDecimal(fila["VALOR"]),
                        VigenteDesde = Convert.ToDateTime(fila["VIGENTE_DESDE"])
                    });
                }
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }

        public void GuardarValorHora(int usuarioId, decimal valor, DateTime vigenteDesde, string creadoPor)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario_id", usuarioId),
                Decimal("@valor", valor),
                new SqlParameter("@vigente_desde", vigenteDesde.Date),
                _acceso.CrearParametro("@creado_por", creadoPor)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("VALOR_HORA_GUARDAR", parametros);
            }
            finally { _acceso.Cerrar(); }
        }

        public List<Feriado> ListarFeriados(DateTime desde, DateTime hasta)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@desde", desde.Date),
                new SqlParameter("@hasta", hasta.Date)
            };
            var lista = new List<Feriado>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("FERIADO_LISTAR", parametros);
                foreach (DataRow fila in tabla.Rows)
                {
                    lista.Add(new Feriado
                    {
                        Fecha = Convert.ToDateTime(fila["FECHA"]),
                        Descripcion = fila["DESCRIPCION"] == DBNull.Value ? "" : fila["DESCRIPCION"].ToString()
                    });
                }
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }

        public void GuardarFeriado(DateTime fecha, string descripcion)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@fecha", fecha.Date),
                _acceso.CrearParametro("@descripcion", descripcion ?? "")
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("FERIADO_GUARDAR", parametros);
            }
            finally { _acceso.Cerrar(); }
        }

        public void EliminarFeriado(DateTime fecha)
        {
            List<SqlParameter> parametros = new List<SqlParameter> { new SqlParameter("@fecha", fecha.Date) };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("FERIADO_ELIMINAR", parametros);
            }
            finally { _acceso.Cerrar(); }
        }

        public HashSet<(int usuarioId, DateTime fecha)> ListarExcedentesAprobados(DateTime desde, DateTime hasta)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@desde", desde.Date),
                new SqlParameter("@hasta", hasta.Date)
            };
            var conjunto = new HashSet<(int, DateTime)>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("EXCEDENTE_LISTAR", parametros);
                foreach (DataRow fila in tabla.Rows)
                    conjunto.Add((Convert.ToInt32(fila["USUARIO_ID"]), Convert.ToDateTime(fila["FECHA"]).Date));
            }
            finally { _acceso.Cerrar(); }
            return conjunto;
        }

        public void AprobarExcedente(int usuarioId, DateTime fecha, string aprobadoPor)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario_id", usuarioId),
                new SqlParameter("@fecha", fecha.Date),
                _acceso.CrearParametro("@aprobado_por", aprobadoPor)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("EXCEDENTE_APROBAR", parametros);
            }
            finally { _acceso.Cerrar(); }
        }

        public void RevocarExcedente(int usuarioId, DateTime fecha)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario_id", usuarioId),
                new SqlParameter("@fecha", fecha.Date)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("EXCEDENTE_REVOCAR", parametros);
            }
            finally { _acceso.Cerrar(); }
        }

        public List<TurnoAsignadoFecha> ListarTurnosAsignados(DateTime desde, DateTime hasta)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@desde", desde.Date),
                new SqlParameter("@hasta", hasta.Date)
            };
            var lista = new List<TurnoAsignadoFecha>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("TURNO_LISTAR_ASIGNADOS_RANGO", parametros);
                foreach (DataRow fila in tabla.Rows)
                {
                    DateTime? fecha = FechaDeFranja(Convert.ToDateTime(fila["SEMANA"]), fila["DIA"].ToString());
                    if (!fecha.HasValue) continue;
                    lista.Add(new TurnoAsignadoFecha
                    {
                        UsuarioId = Convert.ToInt32(fila["USUARIO_ID"]),
                        Empleado = AsistenciaPersonalDAL.NombreEmpleado(fila),
                        FranjaId = Convert.ToInt32(fila["FRANJA_ID"]),
                        Fecha = fecha.Value,
                        HoraInicio = (TimeSpan)fila["HORA_INICIO"],
                        HoraFin = (TimeSpan)fila["HORA_FIN"]
                    });
                }
            }
            finally { _acceso.Cerrar(); }
            return lista.FindAll(t => t.Fecha >= desde.Date && t.Fecha <= hasta.Date);
        }

        public static DateTime? FechaDeFranja(DateTime lunes, string dia)
        {
            if (!Enum.TryParse(dia, true, out DayOfWeek diaSemana)) return null;
            return lunes.Date.AddDays(((int)diaSemana - (int)DayOfWeek.Monday + 7) % 7);
        }

        public bool HayLiquidacionEnRango(DateTime desde, DateTime hasta)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@desde", desde.Date),
                new SqlParameter("@hasta", hasta.Date)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("LIQUIDACION_CONTAR_SUPERPUESTAS", parametros);
                return Convert.ToInt32(tabla.Rows[0]["CANTIDAD"]) > 0;
            }
            finally { _acceso.Cerrar(); }
        }

        public int GuardarLiquidacion(DateTime desde, DateTime hasta, string cerradaPor, List<LiquidacionDia> dias)
        {
            try
            {
                _acceso.Abrir();
                _acceso.IniciarTx();

                DataTable cabecera = _acceso.Leer("LIQUIDACION_INSERTAR", new List<SqlParameter>
                {
                    new SqlParameter("@desde", desde.Date),
                    new SqlParameter("@hasta", hasta.Date),
                    _acceso.CrearParametro("@cerrada_por", cerradaPor)
                });
                int id = Convert.ToInt32(cabecera.Rows[0]["ID"]);

                foreach (LiquidacionDia d in dias)
                {
                    _acceso.Escribir("LIQUIDACION_DIA_INSERTAR", new List<SqlParameter>
                    {
                        _acceso.CrearParametro("@liquidacion_id", id),
                        _acceso.CrearParametro("@usuario_id", d.UsuarioId),
                        _acceso.CrearParametro("@empleado", d.Empleado ?? ""),
                        new SqlParameter("@fecha", d.Fecha.Date),
                        new SqlParameter("@ingreso", SqlDbType.Time) { Value = (object)d.Ingreso ?? DBNull.Value },
                        new SqlParameter("@egreso", SqlDbType.Time) { Value = (object)d.Egreso ?? DBNull.Value },
                        _acceso.CrearParametro("@min_trabajados", d.MinTrabajados),
                        _acceso.CrearParametro("@min_asignados", d.MinAsignados),
                        _acceso.CrearParametro("@min_pagables", d.MinPagables),
                        _acceso.CrearParametro("@min_normales", d.MinNormales),
                        _acceso.CrearParametro("@min_extra", d.MinExtra),
                        _acceso.CrearParametro("@min_feriado", d.MinFeriado),
                        _acceso.CrearParametro("@min_excedente", d.MinExcedente),
                        new SqlParameter("@excedente_aprobado", SqlDbType.Bit) { Value = d.ExcedenteAprobado },
                        new SqlParameter("@ausente", SqlDbType.Bit) { Value = d.Ausente },
                        Decimal("@valor_hora", d.ValorHora),
                        Decimal("@recargo_extra_pct", d.RecargoExtraPct),
                        Decimal("@recargo_feriado_pct", d.RecargoFeriadoPct),
                        Decimal("@importe", d.Importe),
                        new SqlParameter("@observacion", SqlDbType.VarChar, 200) { Value = (object)d.Observacion ?? DBNull.Value }
                    });
                }

                _acceso.ConfirmarTX();
                return id;
            }
            catch
            {
                _acceso.DeshacerTX();
                throw;
            }
            finally { _acceso.Cerrar(); }
        }

        public List<Liquidacion> ListarLiquidaciones()
        {
            var lista = new List<Liquidacion>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("LIQUIDACION_LISTAR");
                foreach (DataRow fila in tabla.Rows)
                {
                    lista.Add(new Liquidacion
                    {
                        Id = Convert.ToInt32(fila["ID"]),
                        Desde = Convert.ToDateTime(fila["DESDE"]),
                        Hasta = Convert.ToDateTime(fila["HASTA"]),
                        Estado = fila["ESTADO"].ToString(),
                        CerradaPor = fila["CERRADA_POR"] == DBNull.Value ? null : fila["CERRADA_POR"].ToString(),
                        FechaCierre = Convert.ToDateTime(fila["FECHA_CIERRE"]),
                        PagadaPor = fila["PAGADA_POR"] == DBNull.Value ? null : fila["PAGADA_POR"].ToString(),
                        FechaPago = fila["FECHA_PAGO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(fila["FECHA_PAGO"])
                    });
                }
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }

        public List<LiquidacionDia> ListarDiasDeLiquidacion(int liquidacionId)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@liquidacion_id", liquidacionId)
            };
            var lista = new List<LiquidacionDia>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("LIQUIDACION_DIA_LISTAR", parametros);
                foreach (DataRow fila in tabla.Rows)
                {
                    lista.Add(new LiquidacionDia
                    {
                        UsuarioId = Convert.ToInt32(fila["USUARIO_ID"]),
                        Empleado = fila["EMPLEADO"].ToString(),
                        Fecha = Convert.ToDateTime(fila["FECHA"]),
                        Ingreso = fila["INGRESO"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)fila["INGRESO"],
                        Egreso = fila["EGRESO"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)fila["EGRESO"],
                        MinTrabajados = Convert.ToInt32(fila["MIN_TRABAJADOS"]),
                        MinAsignados = Convert.ToInt32(fila["MIN_ASIGNADOS"]),
                        MinPagables = Convert.ToInt32(fila["MIN_PAGABLES"]),
                        MinNormales = Convert.ToInt32(fila["MIN_NORMALES"]),
                        MinExtra = Convert.ToInt32(fila["MIN_EXTRA"]),
                        MinFeriado = Convert.ToInt32(fila["MIN_FERIADO"]),
                        MinExcedente = Convert.ToInt32(fila["MIN_EXCEDENTE"]),
                        ExcedenteAprobado = Convert.ToBoolean(fila["EXCEDENTE_APROBADO"]),
                        Ausente = Convert.ToBoolean(fila["AUSENTE"]),
                        ValorHora = Convert.ToDecimal(fila["VALOR_HORA"]),
                        RecargoExtraPct = Convert.ToDecimal(fila["RECARGO_EXTRA_PCT"]),
                        RecargoFeriadoPct = Convert.ToDecimal(fila["RECARGO_FERIADO_PCT"]),
                        Importe = Convert.ToDecimal(fila["IMPORTE"]),
                        Observacion = fila["OBSERVACION"] == DBNull.Value ? null : fila["OBSERVACION"].ToString()
                    });
                }
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }

        public void MarcarPagada(int id, string pagadaPor)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@id", id),
                _acceso.CrearParametro("@pagada_por", pagadaPor)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("LIQUIDACION_MARCAR_PAGADA", parametros);
            }
            finally { _acceso.Cerrar(); }
        }
    }
}
