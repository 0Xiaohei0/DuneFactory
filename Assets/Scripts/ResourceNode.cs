using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : PlacedObject
{

    [SerializeField] private ItemSO itemScriptableObject;

    public ItemSO GetItemScriptableObject()
    {
        return itemScriptableObject;
    }

}
