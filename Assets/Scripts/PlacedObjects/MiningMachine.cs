using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningMachine : PlacedObject, IItemStorage
{

    public event EventHandler OnItemStorageCountChanged;

    private ItemSO miningResourceItem;

    [SerializeField] private float miningTimer;
    [SerializeField] private int storedItemCount;

    public override void GridSetupDone()
    {
        int resourceNodeSearchWidth = 2;
        int resourceNodeSearchHeight = 2;

        // Find resources within range
        for (int x = origin.x - resourceNodeSearchWidth; x < origin.x + resourceNodeSearchWidth + placedObjectTypeSO.width; x++)
        {
            for (int y = origin.y - resourceNodeSearchHeight; y < origin.y + resourceNodeSearchHeight + placedObjectTypeSO.height; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);
                if (GridBuildingSystem.Instance.IsValidGridPosition(gridPosition))
                {
                    PlacedObject placedObject = GridBuildingSystem.Instance.GetGridObject(gridPosition).GetPlacedObject();
                    if (placedObject != null)
                    {
                        if (placedObject is ResourceNode)
                        {
                            ResourceNode resourceNode = placedObject as ResourceNode;
                            miningResourceItem = resourceNode.GetItemScriptableObject();
                        }
                    }
                }
            }
        }
    }

    public override string ToString()
    {
        if (miningResourceItem == null)
        {
            return "NO RESOURCES!";
        }
        return storedItemCount.ToString();
    }

    private void Update()
    {
        if (miningResourceItem == null)
        {
            // No resources in range!
            return;
        }

        miningTimer -= Time.deltaTime;
        if (miningTimer <= 0f)
        {
            miningTimer += miningResourceItem.miningTimer;

            storedItemCount += 1;
            OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public ItemSO GetMiningResourceItem()
    {
        return miningResourceItem;
    }

    public int GetItemStoredCount(ItemSO filterItemScriptableObject)
    {
        return storedItemCount;
    }

    public bool TryGetStoredItem(ItemSO[] filterItemSO, out ItemSO itemSO)
    {
        if (ItemSO.IsItemSOInFilter(miningResourceItem, filterItemSO))
        {
            // If filter matches any or filter matches this itemType
            if (storedItemCount > 0)
            {
                storedItemCount--;
                itemSO = miningResourceItem;
                OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            else
            {
                itemSO = null;
                return false;
            }
        }
        else
        {
            itemSO = null;
            return false;
        }
    }

    public ItemSO[] GetItemSOThatCanStore()
    {
        return new ItemSO[] { };
    }

    public bool TryStoreItem(ItemSO itemScriptableObject)
    {
        return false;
    }

}
