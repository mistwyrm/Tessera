using Microsoft.Extensions.Configuration;
using MySqlConnector;
namespace Tessera.Models
{
    class sqlConnection
    {
        IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

        public MySqlConnection GetSqlConnection()
        {
            return new MySqlConnection(config.GetConnectionString("Connection"));
        }
    }
}
