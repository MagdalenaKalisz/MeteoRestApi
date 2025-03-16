namespace Meteo.IntegrationTests.JsonDb
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Meteo.Persistence.Json;
    using Xunit;

    /// <summary>
    /// Unit tests for JsonDatabase.
    /// </summary>
    public class JsonDatabaseTests
    {
        private readonly string _testFilePath = "test_database.json";
        private readonly JsonDatabase<TestEntity> _database;

        /// <summary>
        /// Initializes a new instance of the JsonDatabaseTests class.
        /// </summary>
        public JsonDatabaseTests()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
            _database = new JsonDatabase<TestEntity>(_testFilePath);
        }

        /// <summary>
        /// Tests writing and reading data from JSON database.
        /// </summary>
        [Fact]
        public async Task Should_Write_And_Read_Data()
        {
            var testData = new List<TestEntity>
        {
            new TestEntity { Id = 1, Name = "Entity1" },
            new TestEntity { Id = 2, Name = "Entity2" },
        };

            await _database.WriteAsync(testData);
            var retrievedData = await _database.ReadAsync();

            retrievedData.Should().NotBeNull();
            retrievedData.Should().HaveCount(2);
            retrievedData[0].Name.Should().Be("Entity1");
        }

        /// <summary>
        /// Tests reading from an empty JSON database.
        /// </summary>
        [Fact]
        public async Task Should_Return_Empty_List_When_File_Is_Empty()
        {
            var retrievedData = await _database.ReadAsync();
            retrievedData.Should().BeEmpty();
        }

        /// <summary>
        /// Tests ensuring that an exception is thrown when writing null data.
        /// </summary>
        [Fact]
        public async Task Should_Throw_When_Writing_Null_Data()
        {
            Func<Task> act = async () => await _database.WriteAsync(null!);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        /// <summary>
        /// Tests ensuring the JSON file is created if it does not exist.
        /// </summary>
        [Fact]
        public void Should_Create_File_If_Not_Exists()
        {
            var filePath = "new_test_database.json";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var db = new JsonDatabase<TestEntity>(filePath);
            File.Exists(filePath).Should().BeTrue();
        }

        /// <summary>
        /// Tests ensuring an exception is thrown when the file path is invalid.
        /// </summary>
        [Fact]
        public void Should_Throw_When_FilePath_Is_Invalid()
        {
            Func<JsonDatabase<TestEntity>> act = () => new JsonDatabase<TestEntity>(null!);
            act.Should().Throw<ArgumentException>();
        }

        /// <summary>
        /// Represents a test entity for JSON serialization.
        /// </summary>
        private class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}