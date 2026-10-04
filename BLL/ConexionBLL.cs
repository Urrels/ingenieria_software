namespace BLL
{
    public class ConexionBLL
    {
        private readonly DAL.ConexionDAL _dal = new DAL.ConexionDAL();

        public bool Disponible() => _dal.Probar();
    }
}
