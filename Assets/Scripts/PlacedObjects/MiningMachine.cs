using System;
using System.Collections;
using System.Collections.Generic;
using UniGLTF;
using UnityEngine;

public class MiningMachine : PlacedObject, IItemStorage
{

    public event EventHandler OnItemStorageCountChanged;


    [SerializeField] private ItemSO miningResourceItem;

    [SerializeField] private float miningTimer;
    [SerializeField] private int storedItemCount;

    private void Update()
    {
        SetLight(miningResourceItem != null);
        EnergyConsumer energyConsumer = transform.GetComponent<EnergyConsumer>();
        if (energyConsumer != null)
        {
            energyConsumer.isOn = miningResourceItem != null;
        }
        if (miningResourceItem == null)
        {
            // No resources in range!

            return;
        }

        miningTimer -= Time.deltaTime * powerSaticfactionMultiplier;
        if (miningTimer <= 0f)
        {
            miningTimer += miningResourceItem.miningTimer;

            storedItemCount += 1;
            OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
            TriggerGridObjectChanged();
        }
    }

    public float GetCraftingProgressNormalized()
    {
        if (miningResourceItem != null)
        {
            return (miningResourceItem.miningTimer - miningTimer) / miningResourceItem.miningTimer;
        }
        else
        {
            return 0f;
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
        if (ItemSO.IsItemSOInFilter(GameAssets.i.itemSO_Refs.any, filterItemSO) ||
            ItemSO.IsItemSOInFilter(miningResourceItem, filterItemSO))
        {
            // If filter matches any or filter matches this itemType
            if (storedItemCount > 0)
            {
                storedItemCount--;
                itemSO = miningResourceItem;
                OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
                TriggerGridObjectChanged();
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
        return new ItemSO[] { GameAssets.i.itemSO_Refs.none };
    }

    public bool TryStoreItem(ItemSO itemScriptableObject)
    {
        return false;
    }

    public void SetMiningResourceItem(ItemSO itemSO)
    {
        miningResourceItem = itemSO;
    }

}
