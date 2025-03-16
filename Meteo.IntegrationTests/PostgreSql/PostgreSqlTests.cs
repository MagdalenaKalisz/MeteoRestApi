
namespace Meteo.IntegrationTests.PostgreSQL
{
    using System.Threading.Tasks;
    using Testcontainers.PostgreSql;
    using Xunit;
    using Npgsql;
    /// <summary>
    /// This class contains tests for the PostgreSqlContainer class.
    /// </summary>
    public class PostgreSqlTests : IAsyncLifetime
    {
        /// <summary>
        /// The PostgreSqlContainer instance to be tested.
        /// </summary>
        private readonly PostgreSqlContainer _postgresContainer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PostgreSqlTests()
        {
            _postgresContainer = new PostgreSqlBuilder()
                .WithImage("postgres:15")
                .WithDatabase("testdb")
                .WithUsername("testuser")
                .WithPassword("testpassword")
                .WithPortBinding(5433, 5432)
                .Build();
        }

        /// <summary>
        /// Initializes the PostgreSqlContainer instance before running the tests.
        /// </summary>
        public async Task InitializeAsync()
        {
            await _postgresContainer.StartAsync();
        }

        /// <summary>
        /// Disposes the PostgreSqlContainer instance after running the tests.
        /// </summary>
        /// <returns></returns>
        public async Task DisposeAsync()
        {
            await _postgresContainer.DisposeAsync();
        }

        /// <summary>
        /// Tests that the PostgreSqlContainer can connect to the database.
        /// </summary>
        [Fact]
        public async Task CanConnectToDatabase()
        {
            await using var connection = new NpgsqlConnection(_postgresContainer.GetConnectionString());
            await connection.OpenAsync();

            Assert.Equal(System.Data.ConnectionState.Open, connection.State);
        }
    }
}