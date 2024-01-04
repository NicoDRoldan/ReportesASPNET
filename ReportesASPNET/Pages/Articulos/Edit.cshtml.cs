using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection.PortableExecutable;

namespace ReportesASPNET.Pages.Articulos
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public EditModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ArticuloInfo articuloInfo = new ArticuloInfo();

        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
            int id = int.Parse(Request.Query["idArticulo"]);

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    sqlConnection.Open();
                    string query = "SELECT a.id_Articulo, Nombre, Precio, Rubro, Activo, Descripcion FROM Articulos a LEFT JOIN Precios p ON p.id_Articulo = a.id_Articulo WHERE a.id_Articulo = @id";
                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                articuloInfo.idArticulo = reader.GetInt32(0);
                                articuloInfo.nombre = reader.GetString(1);
                                articuloInfo.precio = reader.IsDBNull(2) ? null : reader.GetDecimal(2);
                                articuloInfo.rubro = reader.GetString(3);
                                articuloInfo.activo = reader.GetBoolean(4);
                                articuloInfo.descripcion = reader.IsDBNull(5) ? null : reader.GetString(5);
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
            // Se le pasa el valor escrito por el usuario al campo correspondiente de articuloInfo:

            articuloInfo.idArticulo = int.Parse(Request.Query["idArticulo"]);
            articuloInfo.nombre = Request.Form["nombre"];

            // Validación para el campo de Precio

            string precioString = Request.Form["precio"];

            if (!string.IsNullOrEmpty(precioString))
            {
                if(decimal.TryParse(precioString, out decimal precio))
                {
                    articuloInfo.precio = precio;
                }
                else
                {
                    articuloInfo.precio = 0;
                }
            }
            else
            {
                articuloInfo.precio = 0;
            }

            articuloInfo.rubro = Request.Form["rubro"];

            // Validación para el campo de Activo

            string boolString = Request.Form["activo"];
            bool activo;

            if (bool.TryParse(boolString, out activo))
            {
                articuloInfo.activo = activo;
            }
            else
            {
                articuloInfo.activo = false;
            }

            articuloInfo.descripcion = Request.Form["descripcion"];

            // Se valida que los campos no esten en blanco.

            if (articuloInfo.nombre.Length == 0 || articuloInfo.rubro.Length == 0)
            {
                errorMessage = "Completar todos los campos.";
                return;
            }

            // Se hace la conexión y se realizan las querys correspondientes.

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    sqlConnection.Open();
                    string query = "UPDATE Articulos " +
                        "SET nombre=@nombre, rubro=@rubro, activo=@activo, descripcion=@descripcion " +
                        "WHERE id_Articulo=@id_Articulo;";

                    string queryPrecios = "DECLARE @ID VARCHAR(10) = @id_Articulo; " +
                        "IF NOT EXISTS (SELECT TOP 1 * FROM Precios WHERE id_Articulo = @ID) " +
                        "INSERT INTO Precios (id_Articulo, Precio, Fecha) " +
                        "VALUES(@ID, @Precio, GETDATE()); " +
                        "ELSE " +
                        "UPDATE Precios SET Precio = @Precio WHERE id_Articulo = @ID;";

                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@nombre", articuloInfo.nombre);
                        sqlCommand.Parameters.AddWithValue("@rubro", articuloInfo.rubro);
                        sqlCommand.Parameters.AddWithValue("@activo", articuloInfo.activo);
                        sqlCommand.Parameters.AddWithValue("@descripcion", articuloInfo.descripcion);
                        sqlCommand.Parameters.AddWithValue("@id_Articulo", articuloInfo.idArticulo);

                        sqlCommand.ExecuteNonQuery();
                    }

                    using (SqlCommand sqlCommand = new SqlCommand(queryPrecios, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@precio", articuloInfo.precio);
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