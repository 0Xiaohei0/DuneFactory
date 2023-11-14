using CodeMonkey.Utils;
using StarterAssets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    public GridXZ<GridObject> grid;
    public List<PlacedObjectTypeSO> placedObjectTypeSOList;
    public PlacedObjectTypeSO placedObjectTypeSO;

    public PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down;
    public ThirdPersonController thirdPersonController;
    public StarterAssetsInputs _input;

    private void Awake()
    {
        Instance = this;
        int gridWidth = 2000;
        int gridHeight = 2000;
        float cellSize = 1f;
        Vector3 origin = new Vector3(-gridWidth / 2 * cellSize, 0, -gridHeight / 2 * cellSize);
        //Vector3 origin = Vector3.zero;
        grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, origin, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));

        thirdPersonController = FindFirstObjectByType<ThirdPersonController>();
        _input = FindFirstObjectByType<StarterAssetsInputs>();

        placedObjectTypeSO = null;
    }

    private void OnEnable()
    {
        EventManager.OnBuildingSelected += HandleBuildingSelected;
    }
    void OnDisable()
    {
        EventManager.OnBuildingSelected -= HandleBuildingSelected;
    }

    private void Update()
    {
        if (_input.confirm && placedObjectTypeSO != null)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                SpawnStructure(thirdPersonController.mouseWorldPosition);
            }
            _input.confirm = false;
        }
        else if (_input.confirm && placedObjectTypeSO == null)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 mousePosition = Input.mousePosition;
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f)) { }
                {
                    hit = raycastHit;
                }
                if (hit.collider != null)
                {
                    PlacedObject placedObject = hit.collider.gameObject.GetComponentInParent<PlacedObject>();

                    if (placedObject != null)
                    {
                        // Clicked on something
                        if (placedObject is Smelter)
                        {
                            SmelterUI.Instance.Show(placedObject as Smelter);
                        }
                        else if (placedObject is MiningMachine)
                        {
                            MiningMachineUI.Instance.Show(placedObject as MiningMachine);
                        }
                        else if (placedObject is Assembler)
                        {
                            AssemblerUI.Instance.Show(placedObject as Assembler);
                        }
                        else if (placedObject is StructureAssembler)
                        {
                            StructureAssemblerUI.Instance.Show(placedObject as StructureAssembler);
                        }/*
                    else if (placedObject is Storage)
                    {
                        StorageUI.Instance.Show(placedObject as Storage);
                    }*/
                        else if (placedObject is Grabber)
                        {
                            GrabberUI.Instance.Show(placedObject as Grabber);
                        }
                    }
                }
            }
            _input.confirm = false;
        }
        if (_input.rotate)
        {
            RotateStructure();
            _input.rotate = false;
        }
        if (_input.demolish)
        {
            DemolishStructure();
            _input.demolish = false;
        }
    }

    public class GridObject
    {
        private GridXZ<GridObject> grid;
        public int x;
        public int z;
        private PlacedObject placedObject;

        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetPlacedObject(PlacedObject placedObject)
        {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, z);
        }
        public PlacedObject GetPlacedObject()
        {
            return placedObject;
        }

        public void ClearPlacedObject()
        {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, z);
        }

        public bool CanBuild()
        {
            return placedObject == null;
        }
        public override string ToString()
        {
            return x + ", " + z + "/n" + placedObject;
        }
        public void TriggerGridObjectChanged()
        {
            grid.TriggerGridObjectChanged(x, z);
        }
    }

    public PlacedObject SpawnStructure(Vector3 position, bool useItem = true)
    {
        grid.GetXZ(position, out int x, out int z);

        List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, z), dir);

        GridObject gridObject = grid.GetGridObject(x, z);
        PlacedObject placedObject = null;

        bool canBuild = true;
        // check if has enough items
        if (GlobalStorage.GetBuildingCount(placedObjectTypeSO) < 0)
        {
            canBuild = false;
            UtilsClass.CreateWorldTextPopup("No building left!", position);
        }
        // check if placement location is empty
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
            {
                canBuild = false;
                UtilsClass.CreateWorldTextPopup("Cannot build here!", position);
                break;
            }
        }
        if (canBuild)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

            placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, z), dir, placedObjectTypeSO);

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
            }
            placedObject.GridSetupDone();
            if (useItem)
            {
                GlobalStorage.RemoveBuilding(placedObjectTypeSO);
            }
            OnObjectPlaced?.Invoke(placedObject, EventArgs.Empty);
        }
        else
        {
        }
        return placedObject;
    }

    private void RotateStructure()
    {
        print("rotating structure");
        dir = PlacedObjectTypeSO.GetNextDir(dir);
    }

    private void DemolishStructure()
    {
        GridObject gridObject = grid.GetGridObject(thirdPersonController.mouseWorldPosition);
        PlacedObject placedObject = gridObject.GetPlacedObject();
        if (placedObject != null)
        {
            GlobalStorage.AddBuilding(placedObject.placedObjectTypeSO);
            placedObject.DestroySelf();

            List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
            }
        }
    }

    private void DeselectObjectType()
    {
        placedObjectTypeSO = null; RefreshSelectedObjectType();
    }

    private void RefreshSelectedObjectType()
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    public Vector3 GetMouseWorldSnappedPosition()
    {
        Vector3 mousePosition = thirdPersonController.mouseWorldPosition;
        grid.GetXZ(mousePosition, out int x, out int z);

        if (placedObjectTypeSO != null)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        }
        else
        {
            return mousePosition;
        }
    }
    public Quaternion GetPlacedObjectRotation()
    {
        if (placedObjectTypeSO != null)
        {
            return Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
        }
        else
        {
            return Quaternion.identity;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO()
    {
        return placedObjectTypeSO;
    }

    public void SetPlacedObjectTypeSO(PlacedObjectTypeSO type)
    {
        placedObjectTypeSO = type;
        RefreshSelectedObjectType();
    }

    public bool IsValidGridPosition(Vector2Int gridPosition)
    {
        return grid.IsValidGridPosition(gridPosition);
    }

    public GridObject GetGridObject(Vector2Int gridPosition)
    {
        return grid.GetGridObject(gridPosition.x, gridPosition.y);
    }

    public GridObject GetGridObject(Vector3 worldPosition)
    {
        return grid.GetGridObject(worldPosition);
    }

    private void HandleBuildingSelected(PlacedObjectTypeSO selectedBuilding)
    {
        // Handle the building selection here
        // For example, you could instantiate the building in the game world
        //Debug.Log("Building selected: " + selectedBuilding.nameString);
        SetPlacedObjectTypeSO(selectedBuilding);
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        return grid.GetWorldPosition(gridPosition.x, gridPosition.y);
    }

    public Vector3 GetWorldPositionCentre(Vector2Int gridPosition)
    {
        return grid.GetWorldPositionCentre(gridPosition.x, gridPosition.y);
    }
}
