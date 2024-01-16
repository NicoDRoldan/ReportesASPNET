using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using ReportesASPNET.Models;

namespace ReportesASPNET.Pages.Usuarios
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<UsuariosModels> Usuarios = new List<UsuariosModels>();
        public List<CategoriaModels> Categorias = new List<CategoriaModels>();

        public void OnGet()
        {
            try
            {
                string query = "SELECT id_Usuario, Usuario, Password, c.Nombre [Categoria], Estado, Fecha, u.Nombre, Apellido FROM Usuarios u INNER JOIN Categorias c ON c.id_Categoria = u.id_Categoria ORDER BY id_Usuario;";

                using(SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Defaultconnection")))
                {
                    sqlConnection.Open();

                    using(SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        using(SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UsuariosModels usuario = new UsuariosModels();
                                CategoriaModels categoria = new CategoriaModels();

                                int usuarioId = reader.GetInt32(0);

                                usuario.Id_Usuario = usuarioId;
                                usuario.Usuario = reader.GetString(1);
                                usuario.Password = reader.GetString(2);
                                usuario.Estado = reader.GetBoolean(4);
                                usuario.Fecha = reader.IsDBNull(5) ? null : reader.GetDateTime(5);
                                usuario.Nombre = reader.GetString(6);
                                usuario.Apellido = reader.GetString(7);

                                categoria.Nombre = reader.GetString(3);
                                usuario.Categoria = categoria;

                                Usuarios.Add(usuario);
                            }
                        }
                    }
                    sqlConnection.Close();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
