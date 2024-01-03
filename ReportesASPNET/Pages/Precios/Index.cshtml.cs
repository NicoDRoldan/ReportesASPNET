using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReportesASPNET.Pages.Precios
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            try
            {
                string connectionString = "";
            }
            catch (Exception ex)
            {

            }
        }
    }

    public class Precios
    {
        public int id_Articulo;
        public decimal precio;
        public DateTime? fecha;
    }
}
