using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GameAssets : MonoBehaviour
{

    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            return _i;
        }
    }

    private void Awake()
    {
        _i = this;
    }

    public enum PlacedObjectName
    {
        CONVEYER_BELT,
        MINING_MACHINE,
        SMELTER,
        GRABBER,
        ASSEMBLER,
        STORAGE
    }


    [System.Serializable]
    public class PlacedObjectTypeSO_Refs
    {

        public PlacedObjectTypeSO conveyorBelt;
        public PlacedObjectTypeSO miningMachine;
        public PlacedObjectTypeSO smelter;
        public PlacedObjectTypeSO grabber;
        public PlacedObjectTypeSO assembler;
        public PlacedObjectTypeSO storage;

        public PlacedObjectTypeSO FindPlacedObjectTypeSOByName(string name)
        {
            // Use reflection to get all fields of PlacedObjectTypeSO_Refs
            FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                // Check if the field is of the type PlacedObjectTypeSO
                if (field.FieldType == typeof(PlacedObjectTypeSO))
                {
                    // Cast the field value to PlacedObjectTypeSO
                    PlacedObjectTypeSO placedObjTypeSO = field.GetValue(this) as PlacedObjectTypeSO;

                    // Check if the name property matches the provided name
                    if (placedObjTypeSO != null && placedObjTypeSO.nameString == name)  // Replace 'name' with the actual property you want to compare
                    {
                        return placedObjTypeSO;
                    }
                }
            }

            // Return null if no match is found
            return null;
        }

    }

    public PlacedObjectTypeSO_Refs placedObjectTypeSO_Refs;



    [System.Serializable]
    public class ItemSO_Refs
    {

        public ItemSO ironOre;
        public ItemSO copperOre;
        public ItemSO ironBar;
        public ItemSO copperBar;
        public ItemSO circuitBoard;
        public ItemSO sand;
        public ItemSO siliconBar;
        public ItemSO solarCell;

        public ItemSO any;
        public ItemSO none;

        public ItemSO FindItemSOByName(string name)
        {
            // Use reflection to get all fields of ItemSO_Refs
            FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                // Check if the field is of the type ItemSO
                if (field.FieldType == typeof(ItemSO))
                {
                    // Cast the field value to ItemSO
                    ItemSO itemSO = field.GetValue(this) as ItemSO;

                    // Check if the itemName matches the provided name
                    if (itemSO != null && itemSO.itemName == name)
                    {
                        return itemSO;
                    }
                }
            }

            // Return null if no match is found
            return null;
        }
    }


    public ItemSO_Refs itemSO_Refs;



    [System.Serializable]
    public class ItemRecipeSO_Refs
    {

        public ItemRecipeSO ironBar;
        public ItemRecipeSO copperBar;
        public ItemRecipeSO circuitBoard;
        public ItemRecipeSO siliconBar;
        public ItemRecipeSO solarCell;

        public ItemRecipeSO FindItemRecipeSOByName(string name)
        {
            // Use reflection to get all fields of ItemRecipeSO_Refs
            FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                // Check if the field is of the type ItemRecipeSO
                if (field.FieldType == typeof(ItemRecipeSO))
                {
                    // Cast the field value to ItemRecipeSO
                    ItemRecipeSO itemRecipeSO = field.GetValue(this) as ItemRecipeSO;

                    // Check if the recipeName property matches the provided name
                    if (itemRecipeSO != null && itemRecipeSO.recipeName == name)  // Replace 'recipeName' with the actual property you want to compare
                    {
                        return itemRecipeSO;
                    }
                }
            }

            // Return null if no match is found
            return null;
        }
    }


    public ItemRecipeSO_Refs itemRecipeSO_Refs;


    public Transform pfWorldItem;
    public Transform pfBeltDebugVisualNode;
    public Transform pfBeltDebugVisualLine;

    public Transform fxBuildingPlaced;

    public Transform sndBuilding;

    public Material LightRingMaterialOn;
    public Material LightRingMaterialOff;
}
