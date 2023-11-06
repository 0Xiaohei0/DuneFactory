using CodeMonkey.Utils;
using System.Collections.Generic;
using UnityEngine;


public class GridBuildingSystem : MonoBehaviour
{
    public GridXZ<GridObject> grid;
    [SerializeField] private PlacedObjectTypeSO placedObjectTypeSO;

    private void Awake()
    {
        int gridWidth = 10;
        int gridHeight = 10;
        float cellSize = 2f;
        grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, Vector3.zero, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
    }

    public class GridObject
    {
        private GridXZ<GridObject> grid;
        private int x;
        private int z;
        private Transform transform;

        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetTransform(Transform transform)
        {
            this.transform = transform;
            grid.TriggerGridObjectChanged(x, z);
        }

        public void ClearTransform()
        {
            transform = null;
        }

        public bool CanBuild()
        {
            return transform == null;
        }
        public override string ToString()
        {
            return x + ", " + z + "/n" + transform;
        }
    }

    public void SpawnStructure(Vector3 position)
    {
        grid.GetXZ(position, out int x, out int z);

        List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, z), PlacedObjectTypeSO.Dir.Down);

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
            Transform builtTransform = Instantiate(placedObjectTypeSO.prefab, grid.GetWorldPosition(x, z), Quaternion.identity);

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetTransform(builtTransform);
            }
            gridObject.SetTransform(builtTransform);
        }
        else
        {
            UtilsClass.CreateWorldTextPopup("Cannot build here!", position);
        }

    }
}
