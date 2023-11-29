using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGhost : MonoBehaviour
{
    public static TutorialGhost Instance { get; private set; }
    public List<Transform> visuals;
    public Material ghostMaterial;
    public List<BuildingData> ExtractorProductionList;
    public List<BuildingData> BasicBuildingList;
    public Queue<List<BuildingData>> BuildingDataQueue;

    public class BuildingData
    {
        public PlacedObjectTypeSO type;
        public Vector2Int gridPosition;
        public PlacedObjectTypeSO.Dir dir;
        public BuildingData(PlacedObjectTypeSO type, int gridx, int gridz, PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down)
        {
            this.type = type;
            this.gridPosition = new Vector2Int(gridx, gridz);
            this.dir = dir;
        }
    }

    private void Awake()
    {
        Instance = this;
        GridBuildingSystem.Instance.OnObjectPlaced += OnBuildingPlaced;
        BuildingDataQueue = new Queue<List<BuildingData>>();
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

        };
    }
    public void ShowGhost(PlacedObjectTypeSO placedObjectTypeSO, Vector2Int gridPosition, PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down)
    {
        if (placedObjectTypeSO != null)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = GridBuildingSystem.Instance.GetWorldPosition(gridPosition) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * GridBuildingSystem.Instance.grid.GetCellSize();
            Transform visual = Instantiate(placedObjectTypeSO.visual, placedObjectWorldPosition, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));
            visuals.Add(visual);
            SetMaterialRecursive(visual.gameObject);
        }
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
    public void HideGhost(Vector2Int gridPosition)
    {
        foreach (Transform child in visuals)
        {
            if (GridBuildingSystem.Instance.GetXZ(child.transform.position) == gridPosition)
            {
                Destroy(child.gameObject);
                visuals.Remove(child);
                break;
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
        BuildingDataQueue.Enqueue(buildingList);
        print("Adding " + buildingList.Count + " buildings to queue of length: " + BuildingDataQueue);
        if (buildingList.Count == 1)
        {
            ShowGhostList(buildingList);
        }
    }
    private void ShowGhostList(List<BuildingData> buildingList)
    {
        foreach (BuildingData building in buildingList)
        {
            ShowGhost(building.type, building.gridPosition, building.dir);
        }
    }
    public void OnBuildingPlaced(object caller, EventArgs e)
    {
        PlacedObject building = caller as PlacedObject;
        if (building != null)
        {
            HideGhost(building.origin);
            if (visuals.Count == 0 && BuildingDataQueue.Count != 0)
            {
                ShowGhostList(BuildingDataQueue.Dequeue());
            }
        }
    }
}
