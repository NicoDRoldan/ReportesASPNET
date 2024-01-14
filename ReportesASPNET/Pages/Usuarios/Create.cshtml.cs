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

        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public UsuariosModels usuario = new UsuariosModels();
        public List<CategoriaModels> listaCategorias { get; set; }

        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }

        public void OnPost()
        {
            listaCategorias = TraerCategorias();

            usuario.Usuario = Request.Form["Usuario"];
            usuario.Password = Request.Form["PasswordUser"];
            usuario.Nombre = Request.Form["NombreUser"];
            usuario.Apellido = Request.Form["ApellidoUser"];
            CategoriaModels categoriaSeleccionada = listaCategorias.FirstOrDefault(c => c.Nombre == Request.Form["Categoria"]);
            usuario.Fecha = DateTime.Today;
            usuario.Estado = true;

            if (usuario.Usuario.Length == 0 || usuario.Nombre.Length == 0 || usuario.Password.Length == 0 || usuario.Apellido.Length == 0)
            {
                errorMessage = "�Completar todos los campos!";
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

            successMessage = "Se agreg� el usuario.";

        }

        public List<CategoriaModels> TraerCategorias()
        {
            List<CategoriaModels> listaCategorias = new List<CategoriaModels>();

            string query = "SELECT * FROM Categorias";

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            while (sqlDataReader.Read())
                            {
                                //Instancia un nuevo objeto categoria por cada vez que leo.
                                CategoriaModels categoria = new CategoriaModels();

                                categoria.Id_Categoria = sqlDataReader.GetInt32(0);
                                categoria.Nombre = sqlDataReader.GetString(1);

                                //Agrego el objeto obtenido en el select recorrido, a una lista de categorias. 
                                listaCategorias.Add(categoria);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return listaCategorias;
        }
    }
}