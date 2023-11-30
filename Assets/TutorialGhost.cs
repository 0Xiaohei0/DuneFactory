using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class TutorialGhost : MonoBehaviour
{
    public static TutorialGhost Instance { get; private set; }
    public List<Transform> visuals;
    public Material ghostMaterial;
    public List<BuildingData> ExtractorBuilding;
    public List<BuildingData> SolarPanelBuilding;
    public List<BuildingData> FurnaceBuilding;
    public List<BuildingData> FurnaceInserter;
    public List<BuildingData> ExtractorProductionList;
    public List<BuildingData> BasicBuildingList;
    public List<BuildingData> IronBarUpgrade;
    public List<BuildingData> CircuitBoardProduction;
    public List<BuildingData> MegneticRingProduction;
    public List<BuildingData> GeothermalGenerator;
    public List<BuildingData> TerraformingBuilding;
    public List<BuildingData> currentBuildingList;
    public Queue<List<BuildingData>> BuildingDataQueue;

    public class BuildingData
    {
        public PlacedObjectTypeSO type;
        public Vector2Int gridPosition;
        public PlacedObjectTypeSO.Dir dir;
        public Transform visual;
        public BuildingData(PlacedObjectTypeSO type, int gridx, int gridz, PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down, Transform visual = null)
        {
            this.type = type;
            this.gridPosition = new Vector2Int(gridx, gridz);
            this.dir = dir;
            this.visual = visual;
        }
    }
    private void Awake()
    {
        Instance = this;
        GridBuildingSystem.Instance.OnObjectPlaced += OnBuildingPlaced;
        //SaveManager.Instance.OnGameLoaded += UpdateGhostImage;
        if (BuildingDataQueue == null)
        {
            BuildingDataQueue = new Queue<List<BuildingData>>();
        }

        ExtractorBuilding = new List<BuildingData>()
        {
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.miningMachine, 498, 506, PlacedObjectTypeSO.Dir.Down)
        };
        SolarPanelBuilding = new List<BuildingData>()
        {
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.solarPanel, 498, 502, PlacedObjectTypeSO.Dir.Down)
        };
        FurnaceBuilding = new List<BuildingData>()
        {   new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.smelter, 501, 506, PlacedObjectTypeSO.Dir.Down)
        };
        FurnaceInserter = new List<BuildingData>()
        {
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 500, 506, PlacedObjectTypeSO.Dir.Right)
        };
        ExtractorProductionList = new List<BuildingData>()
        {
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.structureAssembler, 506, 507, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 503, 506, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 505, 509, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 506, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 507, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 508, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 509, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 510, PlacedObjectTypeSO.Dir.Down)
        };
        BasicBuildingList = new List<BuildingData>()
        {
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 511, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 512, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 513, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 514, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 515, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 516, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 517, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 518, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 519, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 520, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 521, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.structureAssembler, 506, 511, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.structureAssembler, 506, 515, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.structureAssembler, 499, 511, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.structureAssembler, 499, 515, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 505, 512, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 505, 516, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 503, 512, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 503, 516, PlacedObjectTypeSO.Dir.Left),
        };
        IronBarUpgrade = new List<BuildingData>() {

            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.miningMachine, 498, 508, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.miningMachine, 498, 504, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.smelter, 501, 508, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.smelter, 501, 504, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 503, 508, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 503, 504, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 500, 504, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 500, 508, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 504, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 505, PlacedObjectTypeSO.Dir.Down),
 };

        CircuitBoardProduction = new List<BuildingData>() {
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 522, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 523, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 524, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 525, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 526, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 527, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 528, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 529, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 530, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 531, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 532, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 533, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 534, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 535, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 536, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 537, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 504, 538, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.structureAssembler, 506, 519, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 505, 520, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.structureAssembler, 499, 519, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 503, 520, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 497, 520, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 497, 521, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 497, 522, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 497, 523, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 497, 524, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 497, 525, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 497, 526, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 497, 527, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 497, 528, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 497, 529, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 497, 530, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 497, 531, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 497, 532, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 497, 519, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 497, 518, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 497, 517, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 497, 516, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.assembler, 494, 517, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.smelter, 491, 516, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.smelter, 491, 518, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.miningMachine, 488, 516, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.miningMachine, 488, 518, PlacedObjectTypeSO.Dir.Down),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 490, 518, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 490, 517, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 493, 518, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 493, 517, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 496, 517, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 498, 520, PlacedObjectTypeSO.Dir.Right),
        };
        MegneticRingProduction = new List<BuildingData>() {
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.structureAssembler, 499, 523, PlacedObjectTypeSO.Dir.Left),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 506, 524, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 506, 525, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 506, 526, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 506, 527, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 506, 528, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 506, 529, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 506, 530, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 506, 531, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 506, 532, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 506, 533, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 506, 534, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 506, 535, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 506, 536, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 506, 537, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt, 506, 538, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.assembler, 508, 524, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.assembler, 508, 527, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.smelter, 511, 527, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.smelter, 511, 524, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.miningMachine, 514, 527, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.miningMachine, 514, 524, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 513, 527, PlacedObjectTypeSO.Dir.Left),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 513, 525, PlacedObjectTypeSO.Dir.Left),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 510, 525, PlacedObjectTypeSO.Dir.Left),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 510, 527, PlacedObjectTypeSO.Dir.Left),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 509, 526, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 507, 524, PlacedObjectTypeSO.Dir.Left),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 505, 525, PlacedObjectTypeSO.Dir.Left),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 503, 524, PlacedObjectTypeSO.Dir.Left),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 498, 524, PlacedObjectTypeSO.Dir.Right),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.AtmosphericExtractor, 511, 519, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.GeoThermalGenerator, 511, 510, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 511, 518, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.AtmosphericExtractor, 520, 519, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.GeoThermalGenerator, 520, 510, PlacedObjectTypeSO.Dir.Down),
                new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 520, 518, PlacedObjectTypeSO.Dir.Down),
            };
        GeothermalGenerator = new List<BuildingData>() {
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.structureAssembler, 499, 527, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.structureAssembler, 499, 531, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 503, 528, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 503, 532, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 505, 529, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 505, 533, PlacedObjectTypeSO.Dir.Left),
        };
        TerraformingBuilding = new List<BuildingData>()
        {
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.structureAssembler, 499, 527, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.structureAssembler, 499, 531, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 503, 528, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 503, 532, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 505, 529, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 505, 533, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 498, 529, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 498, 532, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.AquaticFarm, 508, 530, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.SoilEnrichmentPlant, 509, 539, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.SoilEnrichmentPlant, 508, 539, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.AtmosphericExtractor, 517, 530, PlacedObjectTypeSO.Dir.Right),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 516, 530, PlacedObjectTypeSO.Dir.Left),
            new BuildingData(GameAssets.i.placedObjectTypeSO_Refs.grabber, 508, 538, PlacedObjectTypeSO.Dir.Up),
        };
    }
    public Transform ShowGhost(PlacedObjectTypeSO placedObjectTypeSO, Vector2Int gridPosition, PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down)
    {
        if (placedObjectTypeSO != null)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = GridBuildingSystem.Instance.GetWorldPosition(gridPosition) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * GridBuildingSystem.Instance.grid.GetCellSize();
            Transform visual = Instantiate(placedObjectTypeSO.visual, placedObjectWorldPosition, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));
            visuals.Add(visual);
            visual.parent = transform;
            SetMaterialRecursive(visual.gameObject);
            return visual;
        }
        else return null;
    }
    private void SetMaterialRecursive(GameObject targetGameObject)
    {
        Renderer renderer = targetGameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = ghostMaterial;
        }
        foreach (Transform child in targetGameObject.transform)
        {
            SetMaterialRecursive(child.gameObject);
        }
    }
    public void HideGhost(Vector2Int gridPosition, PlacedObjectTypeSO placedObjectTypeSO, PlacedObjectTypeSO.Dir dir)
    {
        //print("Hiding ghost at (" + gridPosition.x + ", " + gridPosition.y + ")");
        foreach (BuildingData building in currentBuildingList)
        {
            bool checkDir = building.type == GameAssets.i.placedObjectTypeSO_Refs.grabber ||
                            building.type == GameAssets.i.placedObjectTypeSO_Refs.conveyorBelt;
            // checks if player placed something on the ghost image
            if (gridPosition == building.gridPosition && building.type == placedObjectTypeSO && (!checkDir || building.dir == dir))
            {
                visuals.Remove(building.visual);
                if (building.visual != null)
                {
                    Destroy(building.visual.gameObject);
                }
                building.visual = null;
            }
        }
    }
    public void HideAllGhosts()
    {
        foreach (Transform child in visuals)
        {
            Destroy(child.gameObject);
        }
        visuals.Clear();
    }

    public void QueueGhostList(List<BuildingData> buildingList)
    {
        if (BuildingDataQueue == null)
        {
            BuildingDataQueue = new Queue<List<BuildingData>>();
        }
        BuildingDataQueue.Enqueue(buildingList);
        //print("Adding " + buildingList.Count + " buildings to queue of length: " + BuildingDataQueue);
        if (visuals.Count == 0)
        {
            currentBuildingList = BuildingDataQueue.Dequeue();
            ShowGhostList(currentBuildingList);
        }
    }
    private void ShowGhostList(List<BuildingData> buildingList)
    {
        foreach (BuildingData building in buildingList)
        {
            building.visual = ShowGhost(building.type, building.gridPosition, building.dir);
        }
    }
    public void OnBuildingPlaced(object caller, EventArgs e)
    {
        PlacedObject building = caller as PlacedObject;
        if (building != null)
        {
            HideGhost(building.origin, building.placedObjectTypeSO, building.dir);
            if (visuals.Count == 0 && BuildingDataQueue.Count != 0)
            {
                currentBuildingList = BuildingDataQueue.Dequeue();
                ShowGhostList(currentBuildingList);
            }
        }
    }

    public void UpdateGhostImage()
    {
        foreach (PlacedObject building in FindObjectsOfType<PlacedObject>())
        {
            OnBuildingPlaced(building, EventArgs.Empty);
        }
    }
    public void ToggleActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
