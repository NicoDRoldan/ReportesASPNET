using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using ReportesASPNET.Pages.Precios;

namespace ReportesASPNET.Pages.Articulos
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<ArticuloInfo> listaArticulos = new List<ArticuloInfo>();

        public void OnGet()
        {
            try
            {
                string query = "SELECT a.id_Articulo, Nombre, Precio, Rubro, Activo, Descripcion, a.Fecha FROM Articulos a LEFT JOIN Precios p on p.id_Articulo = a.id_Articulo";

                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    sqlConnection.Open();

                    using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ArticuloInfo info = new ArticuloInfo();
                                info.idArticulo = reader.GetInt32(0);
                                info.nombre = reader.GetString(1);
                                info.precio = reader.IsDBNull(2) ? null : reader.GetDecimal(2);
                                info.rubro = reader.GetString(3);
                                info.activo = reader.GetBoolean(4);
                                info.descripcion = reader.IsDBNull(5) ? null : reader.GetString(5);
                                info.fecha = reader.IsDBNull(6) ? null : reader.GetDateTime(6);

                                listaArticulos.Add(info);
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

    public class ArticuloInfo
    {
        public int idArticulo;
        public string nombre;
        public string rubro;
        public bool activo;
        public string? descripcion;
        public DateTime? fecha;
        public decimal? precio;
    }
}
