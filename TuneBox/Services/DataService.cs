using System.Text.Json;
using TuneBox.Models;

namespace TuneBox.Services
{
    public class DataService
    {
        private readonly string _filePath = "data.json";

        public DataService(IWebHostEnvironment environment)
        {
            _filePath = Path.Combine(
                environment.ContentRootPath,
                "Data",
                "data.json"
            );
        }

        public TuneBoxData LoadData()
        {
            if (!File.Exists(_filePath))
            {
                return new TuneBoxData();
            }

            var json = File.ReadAllText(_filePath);

            return JsonSerializer.Deserialize<TuneBoxData>(json)
                   ?? new TuneBoxData();
        }

        public void SaveData(TuneBoxData data)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(data, options);

            File.WriteAllText(_filePath, json);
        }
    }
}
