using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO)
    {
        Transform placedObjectTransform = Instantiate(
                placedObjectTypeSO.prefab,
                worldPosition,
                Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject.placedObjectTypeSO = placedObjectTypeSO;
        placedObject.origin = origin;
        placedObject.dir = dir;
        placedObject.powerSaticfactionMultiplier = 1.0f;

        placedObject.Setup();

        return placedObject;
    }


    public PlacedObjectTypeSO placedObjectTypeSO;
    public Vector2Int origin;
    public PlacedObjectTypeSO.Dir dir;
    public float powerSaticfactionMultiplier;

    protected virtual void Setup()
    {
    }

    public virtual void GridSetupDone()
    {
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return placedObjectTypeSO.GetGridPositionList(origin, dir);
    }

    public virtual void DestroySelf()
    {
        Destroy(gameObject);
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO()
    {
        return placedObjectTypeSO;
    }
    public Vector2Int GetOrigin()
    {
        return origin;
    }

    public PlacedObjectTypeSO.Dir GetDir()
    {
        return dir;
    }
    protected virtual void TriggerGridObjectChanged()
    {
        foreach (Vector2Int gridPosition in GetGridPositionList())
        {
            GridBuildingSystem.Instance.GetGridObject(gridPosition).TriggerGridObjectChanged();
        }
    }
    public void SetEffciencyMultiplier(float effciencyMultiplier)
    {
        powerSaticfactionMultiplier = effciencyMultiplier;
    }
}
