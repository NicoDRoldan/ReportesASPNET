namespace ReportesASPNET.Models
{
    public class CategoriaModels
    {
        public int Id_Categoria { get; set; }
        public string Nombre { get; set; }
        public UsuariosModels Usuarios { get; set; }
    }
}
