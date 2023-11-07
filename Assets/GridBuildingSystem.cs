using CodeMonkey.Utils;
using StarterAssets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    public GridXZ<GridObject> grid;
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList;
    private PlacedObjectTypeSO placedObjectTypeSO;

    private PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down;
    public ThirdPersonController thirdPersonController;
    public StarterAssetsInputs _input;

    private void Awake()
    {
        Instance = this;
        int gridWidth = 10;
        int gridHeight = 10;
        float cellSize = 1f;
        //Vector3 origin = new Vector3(-gridWidth / 2 * cellSize, 0, -gridHeight / 2 * cellSize);
        Vector3 origin = Vector3.zero;
        grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, origin, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));

        thirdPersonController = FindFirstObjectByType<ThirdPersonController>();
        _input = FindFirstObjectByType<StarterAssetsInputs>();

        placedObjectTypeSO = placedObjectTypeSOList[0];
    }

    private void Update()
    {
        if (_input.confirm)
        {
            SpawnStructure(thirdPersonController.mouseWorldPosition);
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
    }

    public void SpawnStructure(Vector3 position)
    {
        grid.GetXZ(position, out int x, out int z);

        List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, z), dir);

        GridObject gridObject = grid.GetGridObject(x, z);

        bool canBuild = true;
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
            {
                canBuild = false;
                break;
            }
        }
        if (canBuild)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

            PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, z), dir, placedObjectTypeSO);

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
            }

            OnObjectPlaced?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            UtilsClass.CreateWorldTextPopup("Cannot build here!", position);
        }
    }

    private void RotateStructure()
    {
        dir = PlacedObjectTypeSO.GetNextDir(dir);
    }

    private void DemolishStructure()
    {
        GridObject gridObject = grid.GetGridObject(thirdPersonController.mouseWorldPosition);
        PlacedObject placedObject = gridObject.GetPlacedObject();
        if (placedObject != null)
        {
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
}
