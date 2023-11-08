using System;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public GameObject item;
    public ItemSO itemSO;
    private Vector2Int gridPosition;

    public static WorldItem Create(Vector2Int gridPosition, ItemSO itemScriptableObject)
    {
        Transform worldItemTransform = Instantiate(GameAssets.i.pfWorldItem, GridBuildingSystem.Instance.GetWorldPositionCentre(gridPosition), Quaternion.identity);

        WorldItem worldItem = worldItemTransform.GetComponent<WorldItem>();
        worldItem.SetGridPosition(gridPosition);
        worldItem.itemSO = itemScriptableObject;

        return worldItem;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, GridBuildingSystem.Instance.GetWorldPosition(gridPosition), Time.deltaTime * 10f);
    }

    public void SetGridPosition(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    private void Awake()
    {
        item = gameObject;
    }

    public ItemSO GetItemSO()
    {
        return itemSO;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
