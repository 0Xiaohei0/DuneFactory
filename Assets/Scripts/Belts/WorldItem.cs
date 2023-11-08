using System;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public GameObject item;
    public ItemSO itemSO;

    private void Awake()
    {
        item = gameObject;
    }

    public ItemSO GetItemSO()
    {
        return itemSO;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
