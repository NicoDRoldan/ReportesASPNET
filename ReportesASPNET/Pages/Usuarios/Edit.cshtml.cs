using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using ReportesASPNET.Models;
using ReportesASPNET.Pages.Articulos;
using ReportesASPNET.Pages.Usuarios;

namespace ReportesASPNET.Pages.Usuarios
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly CategoriaModels _categoria;

        public EditModel(IConfiguration configuration)
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
            listaCategorias = categoria.TraerCategorias();

            usuario.Id_Usuario = int.Parse(Request.Query["id_Usuario"]);
            usuario.Usuario = Request.Form["Usuario"];

            int categoriaIdSeleccionada;

            if (int.TryParse(Request.Form["Categoria"], out categoriaIdSeleccionada))
            {
                CategoriaModels categoriaSeleccionada = listaCategorias.FirstOrDefault(c => c.Id_Categoria == categoriaIdSeleccionada);
                usuario.Id_Categoria = categoriaSeleccionada.Id_Categoria;
            }

            // Validación para el campo de Estado (?

            string boolString = Request.Form["Estado"];
            bool estado;

            if (bool.TryParse(boolString, out estado))
            {
                usuario.Estado = estado;
            }
            else
            {
                usuario.Estado = false;
            }

            usuario.Nombre = Request.Form["NombreUser"];

            usuario.Apellido = Request.Form["ApellidoUser"];

            // Se hace la conexión y se realizan las querys correspondientes.

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    sqlConnection.Open();
                    string query = "UPDATE Usuarios " +
                        "SET Usuario=@Usuario, id_Categoria=@Categoria, Estado=@Estado, Nombre=@NombreUser, Apellido=@ApellidoUser " +
                        "WHERE id_Usuario=@id_Usuario;";

                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@Usuario", usuario.Usuario);
                        sqlCommand.Parameters.AddWithValue("@Categoria", usuario.Id_Categoria);
                        sqlCommand.Parameters.AddWithValue("@Estado", usuario.Estado);
                        sqlCommand.Parameters.AddWithValue("@NombreUser", usuario.Nombre);
                        sqlCommand.Parameters.AddWithValue("@ApellidoUser", usuario.Apellido);
                        sqlCommand.Parameters.AddWithValue("@id_Usuario", usuario.Id_Usuario);

                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            Response.Redirect("/Usuarios/Index");

        }
    }
}
