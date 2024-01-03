using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ReportesASPNET.Pages.Articulos
{
    public class IndexModel : PageModel
    {
        public List<ArticuloInfo> listaArticulos = new List<ArticuloInfo>();
        public void OnGet()
        {
            try
            {
                String connectionString = "Data Source=26.188.233.195,1433;Initial Catalog=Reportes;User ID=sa;Password=cinettorcel;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
                String query = "SELECT * FROM Articulos";

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
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
                                info.rubro = reader.GetString(2);
                                info.activo = reader.GetBoolean(3);
                                info.descripcion = reader.IsDBNull(4) ? null : reader.GetString(4);
                                info.fecha = reader.IsDBNull(5) ? null : reader.GetDateTime(5);

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
    }
}
