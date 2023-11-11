using System;
using UnityEngine;
using UnityEngine.UI;

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

        worldItem.transform.Find("Canvas").Find("Image").GetComponent<Image>().sprite = itemScriptableObject.sprite;
        worldItem.transform.Find("Mesh").GetComponent<Renderer>().material.color = itemScriptableObject.color;

        return worldItem;
    }

    private void Update()
    {
        // transform.position = Vector3.Lerp(transform.position, GridBuildingSystem.Instance.GetWorldPosition(gridPosition), Time.deltaTime * 10f);
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
