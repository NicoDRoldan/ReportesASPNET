using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ReportesASPNET.Pages.Articulos
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ArticuloInfo articuloInfo = new ArticuloInfo();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {

        }

        public void OnPost() 
        {
            articuloInfo.nombre = Request.Form["nombre"];
            articuloInfo.rubro = Request.Form["rubro"];
            articuloInfo.activo = true;
            articuloInfo.descripcion = Request.Form["descripcion"];
            articuloInfo.fecha = DateTime.Today;

            if(articuloInfo.nombre.Length == 0 || articuloInfo.rubro.Length == 0 || articuloInfo.descripcion.Length == 0)
            {
                errorMessage = "Completar todos los campos.";
                return;
            }

            try
            {
                string query = "INSERT INTO ARTICULOS (Nombre, Rubro, Activo, Descripcion, Fecha) VALUES(@nombre,@rubro,@activo,@descripcion,@fecha); SELECT SCOPE_IDENTITY();";

                using(SqlConnection  con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    con.Open();

                    using(SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@nombre", articuloInfo.nombre);
                        cmd.Parameters.AddWithValue("@rubro", articuloInfo.rubro);
                        cmd.Parameters.AddWithValue("@activo", articuloInfo.activo);
                        cmd.Parameters.AddWithValue("@descripcion", articuloInfo.descripcion);
                        cmd.Parameters.AddWithValue("@fecha", articuloInfo.fecha);


                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {

            }

            articuloInfo.nombre = "";
            articuloInfo.rubro = "";
            articuloInfo.descripcion = "";

            successMessage = "Se añadió el artículo.";
        }
    }
}
