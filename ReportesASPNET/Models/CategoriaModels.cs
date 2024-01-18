using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ReportesASPNET.Connection;

namespace ReportesASPNET.Models
{
    public class CategoriaModels
    {
        public int Id_Categoria { get; set; }
        public string Nombre { get; set; }
        public UsuariosModels Usuarios { get; set; }

        private readonly IConfiguration _configuration;

        public CategoriaModels(IConfiguration configuration)
        {
            _configuration = configuration;
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
                                CategoriaModels categoria = new CategoriaModels(_configuration);

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
