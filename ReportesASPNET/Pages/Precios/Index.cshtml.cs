using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using ReportesASPNET.Pages.Articulos;

namespace ReportesASPNET.Pages.Precios
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Precio> listaPrecios = new List<Precio>();

        public void OnGet()
        {
            try
            {
                string query = "SELECT * FROM Precios";

                using(SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Precio precio = new Precio();

                                precio.id_Articulo = reader.GetInt32(0);
                                precio.precio = reader.GetDecimal(1);
                                precio.fecha = reader.IsDBNull(2) ? null : reader.GetDateTime(2);

                                listaPrecios.Add(precio);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    public class Precio
    {
        public int id_Articulo;
        public decimal precio;
        public DateTime? fecha;
        public ArticuloInfo articulo;
    }
}