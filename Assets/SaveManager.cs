using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using static GridBuildingSystem;
using UnityEditor.Experimental.GraphView;
using System.IO;
using UnityEngine.Playables;

public class SaveManager : MonoBehaviour
{
    private static string SavePath;

    public GridBuildingSystem gridBuildingSystem;
    private StarterAssetsInputs _input;

    // Start is called before the first frame update
    void Start()
    {
        _input = FindObjectOfType<StarterAssetsInputs>();
        SavePath = Path.Combine(Application.persistentDataPath, "save.json");
        GameData loadedGameData = SaveSystem.LoadGameData();
    }

    // Update is called once per frame
    void Update()
    {
        if (_input.save)
        {
            GameData gameData = new GameData();

            gameData.grid = gridBuildingSystem.grid;

            SaveSystem.SaveGameData(gameData);
            _input.save = false;
        }
    }

    public class GameData
    {
        public GridXZ<GridObject> grid;
    }

}
