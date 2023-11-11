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
using static UnityEngine.UI.Image;
using System.Runtime.CompilerServices;

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
        loadNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (_input.save)
        {
            _input.save = false;
            WriteGameSave();
        }
        else if (_input.load)
        {
            _input.load = false;
            LoadGameSave();
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
                public PlacedObjectTypeSO.Dir dir;
                public string itemRecipeName; // Assembler
                public string miningResourceItemName; // Miner
            }
        }
    }
    public void WriteGameSave()
    {
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
                PlacedObject placedObject = gridBuildingSystem.grid.gridArray[x, z].GetPlacedObject();
                if (placedObject == null) continue;
                gameData.gridData.gridArray[x, z].placedObjectName = placedObject.GetPlacedObjectTypeSO().nameString;
                gameData.gridData.gridArray[x, z].placedObjectOrigin = placedObject.GetOrigin();
                gameData.gridData.gridArray[x, z].dir = placedObject.GetDir();


                if (placedObject is MiningMachine)
                {
                    MiningMachine miningMachine = placedObject as MiningMachine;
                    gameData.gridData.gridArray[x, z].miningResourceItemName = miningMachine.GetMiningResourceItem().itemName;
                }
                else if (placedObject is Assembler)
                {
                    Assembler assembler = placedObject as Assembler;
                    gameData.gridData.gridArray[x, z].itemRecipeName = assembler.GetItemRecipeSO().recipeName;
                }
                else if (placedObject is Smelter)
                {
                    Smelter smelter = placedObject as Smelter;
                    gameData.gridData.gridArray[x, z].itemRecipeName = smelter.GetItemRecipeSO().recipeName;
                }
            }
        }
        SaveSystem.SaveGameData(gameData);
    }
    public void LoadGameSave()
    {
        GameData gameData = SaveSystem.LoadGameData();
        print(gameData);
        ClearPlacableObjects();


        // load grid data
        gridBuildingSystem.grid = new GridXZ<GridObject>(gameData.gridData.width, gameData.gridData.height, gameData.gridData.cellSize, gameData.gridData.originPosition, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
        for (int x = 0; x < gameData.gridData.gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gameData.gridData.gridArray.GetLength(1); z++)
            {
                GridObjectData gridObjectData = gameData.gridData.gridArray[x, z];
                if (gridObjectData.placedObjectName == null) continue;
                // Find the PlacedObjectTypeSO with matching name
                PlacedObjectTypeSO placedObjectSO = GameAssets.i.placedObjectTypeSO_Refs.FindPlacedObjectTypeSOByName(gridObjectData.placedObjectName);
                gridBuildingSystem.placedObjectTypeSO = placedObjectSO;
                gridBuildingSystem.dir = gameData.gridData.gridArray[x, z].dir;
                PlacedObject placedObject = gridBuildingSystem.SpawnStructure(gridBuildingSystem.grid.GetWorldPosition(x, z), false);
                if (placedObject == null) continue;
                if (placedObject is MiningMachine)
                {
                    MiningMachine miningMachine = placedObject as MiningMachine;
                    miningMachine.SetMiningResourceItem(GameAssets.i.itemSO_Refs.FindItemSOByName(gameData.gridData.gridArray[x, z].miningResourceItemName));
                }
                else if (placedObject is Assembler)
                {
                    Assembler assembler = placedObject as Assembler;
                    assembler.SetItemRecipeScriptableObject(GameAssets.i.itemRecipeSO_Refs.FindItemRecipeSOByName(gameData.gridData.gridArray[x, z].itemRecipeName));
                }
                else if (placedObject is Smelter)
                {
                    Smelter smelter = placedObject as Smelter;
                    smelter.SetItemRecipeScriptableObject(GameAssets.i.itemRecipeSO_Refs.FindItemRecipeSOByName(gameData.gridData.gridArray[x, z].itemRecipeName));
                }
            }
        }
        gridBuildingSystem.placedObjectTypeSO = null;
    }

    private void ClearPlacableObjects()
    {
        // clear placed objects on grid
        PlacedObject[] allPlacedObjects = FindObjectsOfType<PlacedObject>();
        foreach (PlacedObject placedObject in allPlacedObjects)
        {
            placedObject.DestroySelf();
            for (int x = 0; x < gridBuildingSystem.grid.GetWidth(); x++)
            {
                for (int z = 0; z < gridBuildingSystem.grid.GetHeight(); z++)
                {
                    gridBuildingSystem.grid.gridArray[x, z] = new GridObject(gridBuildingSystem.grid, x, z);
                }
            }
        }
    }

    public void loadNewGame()
    {
        ClearPlacableObjects();
        GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.miningMachine, 10);
        GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.smelter, 10);
        GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.assembler, 10);
        GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.grabber, 10);
        GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 100);
    }
}
