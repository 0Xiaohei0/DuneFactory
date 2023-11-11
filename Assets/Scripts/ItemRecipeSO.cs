using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ItemRecipeSO : ScriptableObject
{
    [JsonIgnore]
    public List<RecipeItem> outputItemList;
    [JsonIgnore]
    public List<RecipeItem> inputItemList;
    [JsonIgnore]
    public float craftingEffort;


    [System.Serializable]
    public struct RecipeItem
    {

        public ItemSO item;
        public int amount;

    }

}
