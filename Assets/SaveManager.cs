using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using static GridBuildingSystem;
using UnityEditor.Experimental.GraphView;
using System.IO;
using UnityEngine.Playables;
using static SaveManager.GameData.GridData;

public class SaveManager : MonoBehaviour
{
    private static string SavePath;

    public GridBuildingSystem gridBuildingSystem;
    private StarterAssetsInputs _input;

    // Start is called before the first frame update
    void Start()
    {
        gridBuildingSystem = FindObjectOfType<GridBuildingSystem>();
        _input = FindObjectOfType<StarterAssetsInputs>();
        SavePath = Path.Combine(Application.persistentDataPath, "save.json");
        GameData loadedGameData = SaveSystem.LoadGameData();
        LoadGameSave(loadedGameData);
    }

    // Update is called once per frame
    void Update()
    {
        if (_input.save)
        {
            _input.save = false;
            GameData gameData = new GameData();

            gameData.gridData.width = gridBuildingSystem.grid.width;
            gameData.gridData.height = gridBuildingSystem.grid.height;
            gameData.gridData.originPosition = gridBuildingSystem.grid.originPosition;
            gameData.gridData.cellSize = gridBuildingSystem.grid.cellSize;
            gameData.gridData.gridArray = new GridObjectData[gameData.gridData.width, gameData.gridData.height];
            for (int x = 0; x < gameData.gridData.gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gameData.gridData.gridArray.GetLength(1); z++)
                {
                    gameData.gridData.gridArray[x, z] = new GridObjectData();
                    gameData.gridData.gridArray[x, z].x = gridBuildingSystem.grid.gridArray[x, z].x;
                    gameData.gridData.gridArray[x, z].z = gridBuildingSystem.grid.gridArray[x, z].z;
                    if (gridBuildingSystem.grid.gridArray[x, z].GetPlacedObject() == null) continue;
                    gameData.gridData.gridArray[x, z].placedObjectName = gridBuildingSystem.grid.gridArray[x, z].GetPlacedObject().name;
                    gameData.gridData.gridArray[x, z].placedObjectOrigin = gridBuildingSystem.grid.gridArray[x, z].GetPlacedObject().GetOrigin();
                }
            }
            SaveSystem.SaveGameData(gameData);
        }
    }

    public class GameData
    {
        public GridData gridData;
        public GameData()
        {
            gridData = new GridData();
        }
        public class GridData
        {
            public int width;
            public int height;
            public float cellSize;
            public Vector3 originPosition;
            public GridObjectData[,] gridArray;

            public class GridObjectData
            {
                public int x;
                public int z;
                public string placedObjectName;
                public Vector2Int placedObjectOrigin;
            }
        }
    }

    public void LoadGameSave(GameData gameData)
    {
        print(gameData);

    }
}
