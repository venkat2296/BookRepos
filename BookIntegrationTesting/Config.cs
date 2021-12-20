using BookClassLib.Models;
using BookWebapi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookIntegrationTesting
{
    public class TestingWebAppFactory<T> : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<BookDatabaseContext>));

                if (dbContext != null)
                    services.Remove(dbContext);

                var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

                services.AddDbContext<BookDatabaseContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryEmployeeTest");
                    options.UseInternalServiceProvider(serviceProvider);
                });
                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    using (var appContext = scope.ServiceProvider.GetRequiredService<BookDatabaseContext>())
                    {
                        try
                        {
                            appContext.Database.EnsureCreated();
                            SeedDataBase(appContext);
                        }
                        catch (Exception ex)
                        {
                            //Log errors
                            throw;
                        }
                    }
                }
            });
        }

        private void SeedDataBase(BookDatabaseContext appContext)
        {
            var _BookList = new List<Book>()
            {
                new Book(){BookNo=1,Author="venkat",Name="somerandom",Publisher="New"},
                new Book(){BookNo=2,Author="venkatesh",Name="somerandoms",Publisher="New1"},
                new Book(){BookNo=3,Author="Koushik",Name="somerandossm",Publisher="Ne2w"}


            };

            appContext.Book.AddRange(_BookList);
            appContext.SaveChanges();
        }
    }

}

