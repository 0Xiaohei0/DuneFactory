using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smelter : PlacedObject, IItemStorage
{

    public event EventHandler OnItemStorageCountChanged;


    [SerializeField] private ItemRecipeSO itemRecipeSO;
    [SerializeField] private ItemStackList inputItemStackList;
    [SerializeField] public ItemStackList outputItemStackList;
    [SerializeField] private float craftingProgress;

    protected override void Setup()
    {
        //Debug.Log("Smelter.Setup()");
        inputItemStackList = new ItemStackList();
        outputItemStackList = new ItemStackList();
    }

    public override string ToString()
    {
        string str = "";
        str += inputItemStackList.ToString();
        str += outputItemStackList.ToString();
        return str;
    }

    private void Update()
    {
        bool hasEnoughItemsToCraft = HasEnoughItemsToCraft();
        SetLight(hasEnoughItemsToCraft);
        EnergyConsumer energyConsumer = transform.GetComponent<EnergyConsumer>();
        if (energyConsumer != null)
        {
            energyConsumer.isOn = hasEnoughItemsToCraft;
        }
        if (!HasItemRecipe()) return;

        if (hasEnoughItemsToCraft)
        {
            craftingProgress += Time.deltaTime * powerSaticfactionMultiplier;

            if (craftingProgress >= itemRecipeSO.craftingEffort)
            {
                // Item crafting complete
                craftingProgress = 0f;

                // Add Crafted Output Items
                foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.outputItemList)
                {
                    outputItemStackList.AddItemToItemStack(recipeItem.item, recipeItem.amount);
                }

                // Consume Input Items
                foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.inputItemList)
                {
                    ItemStack itemStack = inputItemStackList.GetItemStackWithItemType(recipeItem.item);
                    itemStack.amount -= recipeItem.amount;
                }

                OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
                TriggerGridObjectChanged();
            }
        }
    }

    public float GetCraftingProgressNormalized()
    {
        if (HasItemRecipe())
        {
            return craftingProgress / itemRecipeSO.craftingEffort;
        }
        else
        {
            return 0f;
        }
    }

    public int GetItemStoredCount(ItemSO filterItemSO)
    {
        int amount = 0;

        amount += outputItemStackList.GetItemStoredCount(filterItemSO);
        amount += inputItemStackList.GetItemStoredCount(filterItemSO);

        return amount;
    }

    public bool TryGetStoredItem(ItemSO[] filterItemSO, out ItemSO itemSO)
    {
        if (!HasItemRecipe())
        {
            itemSO = null;
            return false;
        }

        if (ItemSO.IsItemSOInFilter(GameAssets.i.itemSO_Refs.any, filterItemSO) ||
            ItemSO.IsItemSOInFilter(itemRecipeSO.outputItemList[0].item, filterItemSO))
        {
            // If filter matches any or filter matches this itemType
            ItemStack itemStack = outputItemStackList.GetItemStackWithItemType(itemRecipeSO.outputItemList[0].item);
            if (itemStack != null)
            {
                if (itemStack.amount > 0)
                {
                    itemStack.amount -= 1;
                    itemSO = itemStack.itemSO;
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
        else
        {
            itemSO = null;
            return false;
        }
    }

    public ItemSO[] GetItemSOThatCanStore()
    {
        if (!HasItemRecipe()) return new ItemSO[] { GameAssets.i.itemSO_Refs.none };

        List<ItemSO> canStoreItemSOList = new List<ItemSO>();
        foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.inputItemList)
        {
            canStoreItemSOList.Add(recipeItem.item);
        }

        return canStoreItemSOList.ToArray();
    }

    public bool TryStoreItem(ItemSO itemSO)
    {
        if (!HasItemRecipe()) return false;

        foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.inputItemList)
        {
            if (itemSO == recipeItem.item)
            {
                // Can add item to input stack?
                if (inputItemStackList.CanAddItemToItemStack(itemSO))
                {
                    inputItemStackList.AddItemToItemStack(itemSO);
                    OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
                    TriggerGridObjectChanged();
                    return true;
                }
                else
                {
                    // It's this item but cannot fit in stack
                    return false;
                }
            }
        }
        return false;
    }

    private bool HasEnoughItemsToCraft()
    {
        if (!HasItemRecipe()) return false;

        foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.inputItemList)
        {
            ItemStack itemStack = inputItemStackList.GetItemStackWithItemType(recipeItem.item);
            if (itemStack == null)
            {
                // There's no item stack with this item type
                return false;
            }
            else
            {
                if (itemStack.amount < recipeItem.amount)
                {
                    // Not enough amount of this item type
                    return false;
                }
            }
        }
        // Everything is here, ready to craft
        return true;
    }

    public bool HasItemRecipe()
    {
        return itemRecipeSO != null;
    }

    public ItemRecipeSO GetItemRecipeSO()
    {
        return itemRecipeSO;
    }

    public void SetItemRecipeScriptableObject(ItemRecipeSO itemRecipeSO)
    {
        this.itemRecipeSO = itemRecipeSO;
    }

}
