using FinancialAssistant.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FinancialAssistant
{
    public class FinancialAssistantContext : DbContext
    {
        public DbSet<Board> Boards { get; set; }
        public DbSet<Market> Markets { get; set; }
        public DbSet<Tool> Tools { get; set; }
        public DbSet<TradeDate> TradeDates { get; set; }
        public FinancialAssistantContext()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(CreateConnectionString()).UseLazyLoadingProxies();
        }

        private string CreateConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            SqlUser? sqlUser = JsonConvert.DeserializeObject<SqlUser>(File.ReadAllText("SQL.json")!);

            builder.DataSource = sqlUser.DataBase;
            builder.InitialCatalog = sqlUser.InitialCatalog;
            builder.UserID = sqlUser.UserId;
            builder.Password = sqlUser.Password;

            return builder.ConnectionString;
        }
        private class SqlUser
        {
            public string DataBase { get; set; }
            public string InitialCatalog { get; set; }
            public string UserId { get; set; }
            public string Password { get; set; }
        }
    }
}
