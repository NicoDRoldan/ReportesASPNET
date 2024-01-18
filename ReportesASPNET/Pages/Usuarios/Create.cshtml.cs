using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using ReportesASPNET.Models;

namespace ReportesASPNET.Pages.Usuarios
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly CategoriaModels _categoria;

        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _categoria = new CategoriaModels(configuration);
        }

        public List<CategoriaModels> listaCategorias { get; set; }

        public UsuariosModels usuario = new UsuariosModels();
        public CategoriaModels categoria => _categoria;

        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
            listaCategorias = categoria.TraerCategorias();
        }

        public void OnPost()
        {
            listaCategorias = categoria.TraerCategorias();

            int categoriaIdSeleccionada;

            usuario.Usuario = Request.Form["Usuario"];
            usuario.Password = Request.Form["PasswordUser"];
            usuario.Nombre = Request.Form["NombreUser"];
            usuario.Apellido = Request.Form["ApellidoUser"];

            if (int.TryParse(Request.Form["Categoria"], out categoriaIdSeleccionada))
            {
                CategoriaModels categoriaSeleccionada = listaCategorias.FirstOrDefault(c => c.Id_Categoria == categoriaIdSeleccionada);
                usuario.Id_Categoria = categoriaSeleccionada.Id_Categoria;
            }
            
            usuario.Fecha = DateTime.Today;
            usuario.Estado = true;

            if (usuario.Usuario.Length == 0 || usuario.Nombre.Length == 0 || usuario.Password.Length == 0 || usuario.Apellido.Length == 0)
            {
                errorMessage = "¡Completar todos los campos!";
                return;
            }

            try
            {
                string query = "INSERT INTO Usuarios (Usuario, id_Categoria, Password, Estado, Fecha, Nombre, Apellido) " +
                    "VALUES(@Usuario, @Categoria, @Password, @Estado, @Fecha, @Nombre, @Apellido); " +
                    "SELECT SCOPE_IDENTITY();";

                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@Usuario", usuario.Usuario);
                        sqlCommand.Parameters.AddWithValue("@Password", usuario.Password);
                        sqlCommand.Parameters.AddWithValue("@Categoria", usuario.Id_Categoria);
                        sqlCommand.Parameters.AddWithValue("@Estado", usuario.Estado);
                        sqlCommand.Parameters.AddWithValue("@Fecha", usuario.Fecha);
                        sqlCommand.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                        sqlCommand.Parameters.AddWithValue("@Apellido", usuario.Apellido);

                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            usuario.Usuario = "";
            usuario.Password = "";
            usuario.Nombre = "";
            usuario.Apellido = "";
            usuario.Id_Categoria = 0;

            successMessage = "Se agregó el usuario.";

        }

    }
}