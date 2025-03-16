namespace Meteo.IntegrationTests.PostgreSQL
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using Xunit;
    using Meteo.Persistence.PostgreSql;

    /// <summary>
    /// Unit tests for ApplicationDbContextFactory.
    /// </summary>
    public class ApplicationDbContextFactoryTests
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContextFactory _factory;

        /// <summary>
        /// Initializes a new instance of the ApplicationDbContextFactoryTests class.
        /// </summary>
        public ApplicationDbContextFactoryTests()
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                {"ConnectionStrings:DefaultConnection", "Host=localhost;Database=testdb;Username=testuser;Password=testpassword"},
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _factory = new ApplicationDbContextFactory(_configuration);
        }

        /// <summary>
        /// Tests that CreateDbContext returns a valid DbContext instance when a valid connection string is provided.
        /// </summary>
        [Fact]
        public void Should_Create_DbContext_When_ConnectionString_Is_Valid()
        {
            var dbContext = _factory.CreateDbContext([]);

            dbContext.Should().NotBeNull();
            dbContext.Should().BeOfType<ApplicationDbContext>();
        }

        /// <summary>
        /// Tests that CreateDbContext throws an exception when an invalid connection string is provided.
        /// </summary>
        [Fact]
        public void Should_Throw_When_ConnectionString_Is_Invalid()
        {
            var invalidConfig = new Mock<IConfiguration>();
            invalidConfig.Setup(c => c["ConnectionStrings:DefaultConnection"])
                    .Returns("");

            var factory = new ApplicationDbContextFactory(invalidConfig.Object);

            Func<ApplicationDbContext> act = () => factory.CreateDbContext([]);

            act.Should().Throw<Exception>(); // This can be a database connection exception
        }
    }
}
