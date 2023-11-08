using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ItemSO : ScriptableObject
{

    public string itemName;
    public Sprite sprite;
    public float miningTimer;

    public static bool IsItemSOInFilter(ItemSO itemSO, ItemSO[] filterItemSOArray)
    {
        foreach (ItemSO filterItemSO in filterItemSOArray)
        {
            if (itemSO == filterItemSO)
            {
                return true;
            }
        }
        return false;
    }
}
