using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using static SaveManager;

public static class SaveSystem
{
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "save{0}.json");

    public static void SaveGameData(GameData data, int slot)
    {
        string slotSavePath = string.Format(SavePath, slot);
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        string json = JsonConvert.SerializeObject(data, settings);
        File.WriteAllText(slotSavePath, json);
        Debug.Log("Game data saved to " + slotSavePath);
        PopupManager.Instance.ShowPopup("Game data saved to " + slotSavePath);
    }

    public static GameData LoadGameData(int slot)
    {
        string slotSavePath = string.Format(SavePath, slot);
        if (File.Exists(slotSavePath))
        {
            string json = File.ReadAllText(slotSavePath);
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            GameData data = JsonConvert.DeserializeObject<GameData>(json, settings);
            Debug.Log("Game data loaded from " + slotSavePath);
            PopupManager.Instance.ShowPopup("Game data loaded from " + slotSavePath);
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found in " + slotSavePath);
            return new GameData(); // Return a new instance if no save file exists.
        }
    }
    public static GameData LoadGameDataFromString(string json)
    {
        var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
        GameData data = JsonConvert.DeserializeObject<GameData>(json, settings);
        Debug.Log("Game data loaded from JSON");
        PopupManager.Instance.ShowPopup("Game data loaded from JSON");
        return data;
    }
}
