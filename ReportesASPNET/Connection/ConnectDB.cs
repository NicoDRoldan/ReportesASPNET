namespace ReportesASPNET.Connection
{
    public class ConnectDB
    {
        public string ConnectionString()
        {
            return "Data Source=26.188.233.195,1433;Initial Catalog=Reportes;User ID=sa;Password=cinettorcel;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        }
    }
}
