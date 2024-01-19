using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using ReportesASPNET.Models;

namespace ReportesASPNET.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly CategoriaModels _categoria;
        
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            _categoria = new CategoriaModels(configuration);
        }

        public IActionResult Index()
        {
            List<UsuariosModels> Usuarios = new List<UsuariosModels>();
            List<CategoriaModels> Categorias = new List<CategoriaModels>();

            try
            {
                string query = "SELECT id_Usuario, Usuario, Password, c.Nombre [Categoria], Estado, Fecha, u.Nombre, Apellido FROM Usuarios u INNER JOIN Categorias c ON c.id_Categoria = u.id_Categoria ORDER BY id_Usuario;";

                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Defaultconnection")))
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UsuariosModels usuario = new UsuariosModels();
                                CategoriaModels categoria = new CategoriaModels(_configuration);

                                usuario.Id_Usuario = reader.GetInt32(0);
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

            return View(Usuarios);
        }

        public IActionResult Create()
        {
            List<CategoriaModels> listaCategorias = _categoria.TraerCategorias();
            UsuariosModels usuarios = new UsuariosModels();
            ViewBag.listaCategorias = listaCategorias;

            return View(usuarios);
        }

        [HttpPost]
        public IActionResult Create(UsuariosModels usuarioModel)
        {
            List<CategoriaModels> listaCategorias = _categoria.TraerCategorias();
            ViewBag.listaCategorias = listaCategorias;
            
            try
            {
                usuarioModel.Fecha = DateTime.Today;
                usuarioModel.Estado = true;

                if (!ModelState.IsValid)
                {
                    string query = "INSERT INTO Usuarios (Usuario, id_Categoria, Password, Estado, Fecha, Nombre, Apellido) " +
                        "VALUES(@Usuario, @Categoria, @Password, @Estado, @Fecha, @Nombre, @Apellido); " +
                        "SELECT SCOPE_IDENTITY();";

                    using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                    {
                        sqlConnection.Open();
                        using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                        {
                            sqlCommand.Parameters.AddWithValue("@Usuario", usuarioModel.Usuario);
                            sqlCommand.Parameters.AddWithValue("@Password", usuarioModel.Password);
                            sqlCommand.Parameters.AddWithValue("@Categoria", usuarioModel.Id_Categoria);
                            sqlCommand.Parameters.AddWithValue("@Estado", usuarioModel.Estado);
                            sqlCommand.Parameters.AddWithValue("@Fecha", usuarioModel.Fecha);
                            sqlCommand.Parameters.AddWithValue("@Nombre", usuarioModel.Nombre);
                            sqlCommand.Parameters.AddWithValue("@Apellido", usuarioModel.Apellido);

                            sqlCommand.ExecuteNonQuery();
                        }
                    }

                    TempData["SuccessMessage"] = "Se agregó el usuario.";
                    return RedirectToAction("Index"); // Redirige a la acción Index del controlador para mostrar la lista de usuarios
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Erroressage"] = "Ocurrió un error al agregar el usuario.";
            }

            return View(usuarioModel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            List<CategoriaModels> listaCategorias = _categoria.TraerCategorias();
            UsuariosModels usuario = new UsuariosModels();
            ViewBag.listaCategorias = listaCategorias;

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
                                _categoria.Nombre = reader.GetString(2);
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
                Console.WriteLine(ex.Message);
                TempData["Erroressage"] = "Ocurrió un error al agregar el usuario.";
            }

            return View(usuario);
        }

        [HttpPost]
        public IActionResult Edit(UsuariosModels usuariosModels)
        {
            List<CategoriaModels> listaCategorias = _categoria.TraerCategorias();
            ViewBag.listaCategorias = listaCategorias;

            try
            {
                if (!ModelState.IsValid)
                {
                    using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                    {
                        sqlConnection.Open();
                        string query = "UPDATE Usuarios " +
                            "SET Usuario=@Usuario, id_Categoria=@Categoria, Estado=@Estado, Nombre=@NombreUser, Apellido=@ApellidoUser " +
                            "WHERE id_Usuario=@id_Usuario;";

                        using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                        {
                            sqlCommand.Parameters.AddWithValue("@Usuario", usuariosModels.Usuario);
                            sqlCommand.Parameters.AddWithValue("@Categoria", usuariosModels.Id_Categoria);
                            sqlCommand.Parameters.AddWithValue("@Estado", usuariosModels.Estado);
                            sqlCommand.Parameters.AddWithValue("@NombreUser", usuariosModels.Nombre);
                            sqlCommand.Parameters.AddWithValue("@ApellidoUser", usuariosModels.Apellido);
                            sqlCommand.Parameters.AddWithValue("@id_Usuario", usuariosModels.Id_Usuario);

                            sqlCommand.ExecuteNonQuery();
                        }
                    }
                    TempData["SuccessMessage"] = "Se editó el usuario.";
                    return RedirectToAction("Index"); // Redirige a la acción Index del controlador para mostrar la lista de usuarios
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Erroressage"] = "Ocurrió un error al agregar el usuario.";
            }

            return View(usuariosModels);
        }
    }
}