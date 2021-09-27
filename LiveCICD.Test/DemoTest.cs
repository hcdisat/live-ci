using LiveCICD.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LiveCICD.Test
{
    public class DemoTest
    {
        [Fact]
        public void Test1()
        {
            Assert.True(1 == 1);
        }

        [Fact]
        public async Task CustomerIntegrationTest()
        {
            // create db context
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            
            var optionsBuilder = new DbContextOptionsBuilder<CustomerContext>();
            optionsBuilder.UseSqlServer(configuration["ConnectionsStrings:DefaultConnection"]);
            
            var context = new CustomerContext(optionsBuilder.Options);

            // Delete all customers in the DB
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            // create Controller
            var controller = new CustomersController(context);

            // Add customer
            await controller.Add(new Customer { CustomerName = "FooBar" });

            // check: Does GetAll return the added customer?
            var customers = (await controller.GetAll()).ToArray();
            Assert.Single(customers);
            Assert.Equal("FooBar", customers[0].CustomerName);
        }
    }
}
