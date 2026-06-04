namespace SeguridadYServicios
{
    /// <summary>
    /// Contrato para cualquier entidad que soporte dígitos verificadores.
    /// Devuelve los valores de sus atributos en orden canónico fijo,
    /// excluyendo el propio DVH para evitar circularidad.
    /// </summary>
    public interface IEntidadVerificable
    {
        string[] ObtenerAtributosParaDVH();
    }
}
