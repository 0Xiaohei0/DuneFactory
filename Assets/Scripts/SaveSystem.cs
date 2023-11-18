using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using static SaveManager;

public static class SaveSystem
{
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "save.json");

    public static void SaveGameData(GameData data)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        string json = JsonConvert.SerializeObject(data, settings);
        File.WriteAllText(SavePath, json);
        Debug.Log("Game data saved to " + SavePath);
    }

    public static GameData LoadGameData()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            GameData data = JsonConvert.DeserializeObject<GameData>(json, settings);
            Debug.Log("Game data loaded from " + SavePath);
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found in " + SavePath);
            return new GameData(); // Return a new instance if no save file exists.
        }
    }
    public static GameData LoadGameDataFromString(string json)
    {
        if (File.Exists(SavePath))
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            GameData data = JsonConvert.DeserializeObject<GameData>(json, settings);
            Debug.Log("Game data loaded from " + SavePath);
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found in " + SavePath);
            return new GameData(); // Return a new instance if no save file exists.
        }
    }
}
