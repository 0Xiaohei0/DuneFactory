using StarterAssets;
using UnityEngine;
using static GridBuildingSystem;
using static SaveManager.GameData.GridData;
using System;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public bool processInput = true;
    public GridBuildingSystem gridBuildingSystem;
    private StarterAssetsInputs _input;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        if (!processInput) return;
        gridBuildingSystem = FindObjectOfType<GridBuildingSystem>();
        _input = FindObjectOfType<StarterAssetsInputs>();
        GameSettings gameSettings = FindAnyObjectByType<GameSettings>();
        if (gameSettings != null && gameSettings.saveSelected == true)
        {
            LoadGameSave(SaveSystem.LoadGameData(gameSettings.saveIdxToLoad));
        }
        loadNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (!processInput) return;
        if (_input.save)
        {
            _input.save = false;
            WriteGameSave();
        }
        else if (_input.load)
        {
            _input.load = false;
            LoadGameSave(SaveSystem.LoadGameData(0));
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
                public string placedObjectName;
                public Vector2Int placedObjectOrigin;
                public PlacedObjectTypeSO.Dir dir;
                public string itemRecipeName; // Assembler
                public string miningResourceItemName; // Miner
                public int grabberRange;
            }
        }
    }
    public void WriteGameSave(int idx = 0)
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
                PlacedObject placedObject = gridBuildingSystem.grid.gridArray[x, z].GetPlacedObject();
                if (placedObject == null) continue;
                gameData.gridData.gridArray[x, z] = new GridObjectData();
                gameData.gridData.gridArray[x, z].placedObjectName = placedObject.GetPlacedObjectTypeSO().nameString;
                gameData.gridData.gridArray[x, z].placedObjectOrigin = placedObject.GetOrigin();
                gameData.gridData.gridArray[x, z].dir = placedObject.GetDir();


                if (placedObject is MiningMachine)
                {
                    MiningMachine miningMachine = placedObject as MiningMachine;
                    gameData.gridData.gridArray[x, z].miningResourceItemName = miningMachine.GetMiningResourceItem()?.itemName;
                }
                else if (placedObject is AtmosphericExtractor)
                {
                    AtmosphericExtractor atmosphericExtractor = placedObject as AtmosphericExtractor;
                    gameData.gridData.gridArray[x, z].miningResourceItemName = atmosphericExtractor.GetMiningResourceItem()?.itemName;
                }
                else if (placedObject is Assembler)
                {
                    Assembler assembler = placedObject as Assembler;
                    gameData.gridData.gridArray[x, z].itemRecipeName = assembler.GetItemRecipeSO()?.recipeName;
                }
                else if (placedObject is Smelter)
                {
                    Smelter smelter = placedObject as Smelter;
                    gameData.gridData.gridArray[x, z].itemRecipeName = smelter.GetItemRecipeSO()?.recipeName;
                }
                else if (placedObject is StructureAssembler)
                {
                    StructureAssembler structureAssembler = placedObject as StructureAssembler;
                    gameData.gridData.gridArray[x, z].itemRecipeName = structureAssembler.GetItemRecipeSO()?.nameString;
                }
                else if (placedObject is Grabber)
                {
                    Grabber grabber = placedObject as Grabber;
                    gameData.gridData.gridArray[x, z].grabberRange = grabber.getRange();
                }
            }
        }

        SaveStatus saveStatus = new SaveStatus();
        saveStatus.saveTime = DateTime.Now.ToString();
        saveStatus.statusString = "Lv " + ProgressionManager.Instance.currentLevel.ToString();
        SaveSystem.SaveGameData(gameData, idx);
        SaveSystem.SaveSaveStatus(saveStatus, idx);
    }
    public void LoadGameSave(GameData gameData)
    {
        print(gameData);
        ClearPlacableObjects();


        // load grid data
        gridBuildingSystem.grid = new GridXZ<GridObject>(gameData.gridData.width, gameData.gridData.height, gameData.gridData.cellSize, gameData.gridData.originPosition, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
        for (int x = 0; x < gameData.gridData.gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gameData.gridData.gridArray.GetLength(1); z++)
            {
                GridObjectData gridObjectData = gameData.gridData.gridArray[x, z];
                if (gridObjectData == null) continue;
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
                else if (placedObject is AtmosphericExtractor)
                {
                    AtmosphericExtractor atmosphericExtractor = placedObject as AtmosphericExtractor;
                    atmosphericExtractor.SetMiningResourceItem(GameAssets.i.itemSO_Refs.FindItemSOByName(gameData.gridData.gridArray[x, z].miningResourceItemName));
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
                else if (placedObject is StructureAssembler)
                {
                    StructureAssembler structureAssembler = placedObject as StructureAssembler;
                    structureAssembler.SetItemRecipeScriptableObject(GameAssets.i.placedObjectTypeSO_Refs.FindPlacedObjectTypeSOByName(gameData.gridData.gridArray[x, z].itemRecipeName));
                }
                else if (placedObject is Grabber)
                {
                    Grabber grabber = placedObject as Grabber;
                    if (gameData.gridData.gridArray[x, z].grabberRange < 1)
                        grabber.SetRange(1);
                    else
                        grabber.SetRange(gameData.gridData.gridArray[x, z].grabberRange);
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
        bool testing = false;
        if (testing)
        {
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.miningMachine, 100);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.smelter, 100);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.assembler, 100);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.grabber, 200);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 100);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.solarPanel, 200);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.structureAssembler, 50);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.GeoThermalGenerator, 100);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.AtmosphericExtractor, 100);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.SoilEnrichmentPlant, 100);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.AquaticFarm, 100);
            ProgressionManager.Instance.LevelUp();
            ProgressionManager.Instance.LevelUp();
        }
        else
        {
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.miningMachine, 10);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.smelter, 10);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.assembler, 10);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.grabber, 20);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 100);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.solarPanel, 20);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.structureAssembler, 5);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.GeoThermalGenerator, 0);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.AtmosphericExtractor, 0);
            GlobalStorage.AddBuilding(GameAssets.i.placedObjectTypeSO_Refs.SoilEnrichmentPlant, 0);
        }
    }

    public SaveStatus GetSaveStatus(int slot)
    {
        return SaveSystem.LoadSaveStatus(slot);
    }

    public class SaveStatus
    {
        public string statusString;
        public string saveTime;
    }
}
