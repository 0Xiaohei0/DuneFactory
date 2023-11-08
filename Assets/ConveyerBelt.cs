using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : PlacedObject
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
}
