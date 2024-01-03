using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection.PortableExecutable;

namespace ReportesASPNET.Pages.Articulos
{
    public class EditModel : PageModel

    {
        public ArticuloInfo articuloInfo = new ArticuloInfo();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {

            int id = int.Parse(Request.Query["idArticulo"]);

            try
            {
                String connectionString = "Data Source=26.188.233.195,1433;Initial Catalog=Reportes;User ID=sa;Password=cinettorcel;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    string query = "SELECT * FROM Articulos where id_Articulo=@id";
                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                articuloInfo.idArticulo = reader.GetInt32(0);
                                articuloInfo.nombre = reader.GetString(1);
                                articuloInfo.rubro = reader.GetString(2);
                                articuloInfo.activo = reader.GetBoolean(3);
                                articuloInfo.descripcion = reader.IsDBNull(4) ? null : reader.GetString(4);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

        }

        public void OnPost()

        {
            articuloInfo.idArticulo = int.Parse(Request.Query["idArticulo"]);
            articuloInfo.nombre = Request.Form["nombre"];
            articuloInfo.rubro = Request.Form["rubro"];

            string boolString = Request.Form["activo"];
            bool activo;

            if(bool.TryParse(boolString, out activo))
            {
                articuloInfo.activo = activo;
            }
            else
            {
                articuloInfo.activo =false;
            }

            articuloInfo.descripcion = Request.Form["descripcion"];

            try
            {
                String connectionString = "Data Source=26.188.233.195,1433;Initial Catalog=Reportes;User ID=sa;Password=cinettorcel;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    string query = "UPDATE Articulos " +
                        "SET nombre=@nombre, rubro=@rubro, activo=@activo, descripcion=@descripcion " +
                        "WHERE id_Articulo=@id_Articulo";
                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@nombre", articuloInfo.nombre);
                        sqlCommand.Parameters.AddWithValue("@rubro", articuloInfo.rubro);
                        sqlCommand.Parameters.AddWithValue("@activo", articuloInfo.activo);
                        sqlCommand.Parameters.AddWithValue("@descripcion", articuloInfo.descripcion);
                        sqlCommand.Parameters.AddWithValue("@id_Articulo", articuloInfo.idArticulo);

                        sqlCommand.ExecuteNonQuery();
                    }
                }


            }

            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Articulos/Index");
        }

    }
}