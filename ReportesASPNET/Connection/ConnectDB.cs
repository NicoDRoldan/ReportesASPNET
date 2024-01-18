namespace ReportesASPNET.Connection
{
    public class ConnectDB
    {
        private readonly IConfiguration _configuration;

        public ConnectDB(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ConnectionString()
        {
            string connect = _configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine(connect); 
            return connect;
        }
    }
}
