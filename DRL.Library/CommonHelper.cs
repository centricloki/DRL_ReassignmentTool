using System;

using Microsoft.Extensions.Configuration;

namespace DRL.Library
{
    public class CommonHelper
    {
        private readonly IConfigurationRoot Configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();

        public string GetDefaultConnectionString()
        {
            //string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //return connectionString;
            return Configuration.GetConnectionString("DefaultConnection");
        }
        public string GetProdConnectionString()
        {
            return Configuration.GetConnectionString("ProdConnection");
        }

        public string GetKeyBasedValue(string ParentKey, string ChildKey)
        {
            return Configuration.GetSection(ParentKey).GetSection(ChildKey).Value;
        }
    }
}