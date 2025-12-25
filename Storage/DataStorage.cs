using Sem3_kurs.Collections;
using System.IO;
public class DataStorage
{
    private readonly string _filePath;

    public DataStorage(string filePath)
    {
        _filePath = filePath;
    }

    public void Save(EstateAgency agency)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(agency,
            new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true
            });

        File.WriteAllText(_filePath, json);
    }

    public EstateAgency Load()
    {
        if (!File.Exists(_filePath)) return new EstateAgency();

        var json = File.ReadAllText(_filePath);
        return System.Text.Json.JsonSerializer.Deserialize<EstateAgency>(json)
               ?? new EstateAgency();
    }
}
