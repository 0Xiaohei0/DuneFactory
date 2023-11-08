using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : PlacedObject, IWorldItemSlot
{
    public Belt Belt;

    void Start()
    {
        Belt = GetComponentInChildren<Belt>();
    }

    void Update()
    {

    }

    public bool IsSpaceTaken()
    {
        return Belt.isSpaceTaken;
    }
    public void SetBeltItem(WorldItem item)
    {
        Belt.beltItem = item;
    }

    public WorldItem GetWorldItem()
    {
        return Belt.beltItem;
    }

    public bool IsEmpty()
    {
        return Belt.beltItem == null;
    }

    public bool TrySetWorldItem(WorldItem worldItem)
    {
        if (IsEmpty())
        {
            this.Belt.beltItem = worldItem;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveWorldItem()
    {
        Belt.beltItem = null;
    }

    public override void DestroySelf()
    {
        if (!IsEmpty())
        {
            Belt.beltItem.DestroySelf();
        }
        base.DestroySelf();
    }

    public bool TryGetWorldItem(ItemSO[] filterItemSO, out WorldItem worldItem)
    {
        if (IsEmpty())
        {
            // Nothing to grab
            worldItem = null;
            return false;
        }
        else
        {
            // Check if this WorldItem matches the filter OR there's no filter
            if (ItemSO.IsItemSOInFilter(GetWorldItem().GetItemSO(), filterItemSO) ||
                ItemSO.IsItemSOInFilter(GameAssets.i.itemSO_Refs.any, filterItemSO))
            {
                // Return this World Item and Remove it
                worldItem = GetWorldItem();
                RemoveWorldItem();
                return true;
            }
            else
            {
                // This itemType does not match the filter
                worldItem = null;
                return false;
            }
        }
    }

    public ItemSO[] GetItemSOThatCanStore()
    {
        return new ItemSO[] { GameAssets.i.itemSO_Refs.any };
    }

    public Vector2Int GetGridPosition()
    {
        return origin;
    }
}
