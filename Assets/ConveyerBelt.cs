using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : PlacedObject, IWorldItemSlot
{
    public Belt belt;

    void Start()
    {
        belt = GetComponentInChildren<Belt>();
    }

    void Update()
    {

    }


    public WorldItem GetWorldItem()
    {
        return belt.beltItem;
    }

    public bool IsEmpty()
    {
        return !belt.isSpaceTaken;
    }

    public bool TrySetWorldItem(WorldItem worldItem)
    {
        print("IsEmpty(): " + IsEmpty());
        if (IsEmpty())
        {
            belt.beltItem = worldItem;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveWorldItem()
    {
        belt.beltItem.DestroySelf();
        belt.beltItem = null;
    }

    public override void DestroySelf()
    {
        if (!IsEmpty())
        {
            belt.beltItem.DestroySelf();
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
