namespace Meteo.Persistence.Json
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    /// <summary>
    /// Simple JSON file-based database.
    /// </summary>
    public sealed class JsonDatabase<T>
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = false,
            IncludeFields = true,
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDatabase{T}"/> class.
        /// </summary>
        /// <param name="filePath"></param>
        public JsonDatabase(string filePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

            _filePath = filePath;
            EnsureFileExists();
        }

        /// <summary>
        /// Reads data from JSON file.
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> ReadAsync()
        {
            await using FileStream stream = File.OpenRead(_filePath);
            return await JsonSerializer.DeserializeAsync<List<T>>(stream, _serializerOptions) ?? [];
        }

        /// <summary>
        /// Writes data to JSON file.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task WriteAsync(List<T> data)
        {
            ArgumentNullException.ThrowIfNull(data);

            await using FileStream stream = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(stream, data, _serializerOptions);
        }

        /// <summary>
        /// Ensures the JSON file exists.
        /// </summary>
        private void EnsureFileExists()
        {
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }
    }
}
