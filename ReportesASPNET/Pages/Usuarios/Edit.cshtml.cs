using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using ReportesASPNET.Models;
using ReportesASPNET.Pages.Usuarios;

namespace ReportesASPNET.Pages.Usuarios
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public EditModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public UsuariosModels usuario = new UsuariosModels();
        public CategoriaModels categoria = new CategoriaModels();
        
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {

            CreateModel createModel = new CreateModel(_configuration);

            int id = int.Parse(Request.Query["id_Usuario"]);

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    sqlConnection.Open();
                    string query = "SELECT id_Usuario, Usuario, c.Nombre [Categoria], Estado, u.Nombre, Apellido FROM Usuarios u INNER JOIN Categorias c ON c.id_Categoria = u.id_Categoria WHERE u.id_Usuario = @id";
                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                usuario.Id_Usuario = reader.GetInt32(0);
                                usuario.Usuario = reader.GetString(1);
                                categoria.Nombre = reader.GetString(2);
                                usuario.Estado = reader.GetBoolean(3);
                                usuario.Nombre = reader.GetString(4);
                                usuario.Apellido = reader.GetString(5);
                            }
                        }
                    }
                    sqlConnection.Close();
                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public void OnPost()
        {
            CreateModel createModel = new CreateModel(_configuration);
            List<CategoriaModels> categorias = createModel.TraerCategorias();

            usuario.Id_Usuario = int.Parse(Request.Query["id_Usuario"]);
            usuario.Usuario = Request.Form["usuario"];

            int categoriaIdSeleccionada;

            if (int.TryParse(Request.Form["Categoria"], out categoriaIdSeleccionada))
            {
                CategoriaModels categoriaSeleccionada = categorias.FirstOrDefault(c => c.Id_Categoria == categoriaIdSeleccionada);
                usuario.Id_Categoria = categoriaSeleccionada.Id_Categoria;
            }

            // Validación para el campo de Estado (?

            string boolString = Request.Form["estado"];
            bool estado;

            if (bool.TryParse(boolString, out estado))
            {
                usuario.Estado = estado;
            }
            else
            {
                usuario.Estado = false;
            }

            usuario.Nombre = Request.Form["nombre"];
            usuario.Apellido = Request.Form["apellido"];

        }
    }
}
