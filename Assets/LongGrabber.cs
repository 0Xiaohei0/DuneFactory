using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlacedObjectTypeSO;
using static UnityEngine.UI.Image;

public class LongGrabber : PlacedObject
{
    private enum State
    {
        Cooldown,
        WaitingForItemToGrab,
        MovingToDropItem,
        DroppingItem,
    }

    [SerializeField] private Vector2Int grabPosition;
    [SerializeField] private Vector2Int dropPosition;
    [SerializeField] Vector3 grabWorldPosition;
    [SerializeField] Vector3 dropWorldPosition;
    [SerializeField] float padding = 0.3f;
    [SerializeField] private WorldItem holdingItem;
    [SerializeField] public ItemSO grabFilterItemSO;
    [SerializeField] public ItemSO[] dropFilterItemSO;
    [SerializeField] private float timer;
    [SerializeField] private string textString = "";
    [SerializeField] private State state;


    public float TIME_TO_DROP_ITEM = 0.5f;

    protected override void Setup()
    {
        grabPosition = origin + PlacedObjectTypeSO.GetDirForwardVector(dir) * -2;
        dropPosition = origin + PlacedObjectTypeSO.GetDirForwardVector(dir) * 2;

        grabWorldPosition = GridBuildingSystem.Instance.GetWorldPositionCentre(grabPosition) + new Vector3(0, padding, 0);
        dropWorldPosition = GridBuildingSystem.Instance.GetWorldPositionCentre(dropPosition) + new Vector3(0, padding, 0);

        state = State.Cooldown;
        /*
                transform.Find("GrabberVisual").Find("ArrowGrab").gameObject.SetActive(false);
                transform.Find("GrabberVisual").Find("ArrowDrop").gameObject.SetActive(false);*/

        grabFilterItemSO = GameAssets.i.itemSO_Refs.any;
    }

    public override string ToString()
    {
        return textString;
    }


    private void Update()
    {
        switch (state)
        {
            default:
            case State.Cooldown:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    state = State.WaitingForItemToGrab;
                }
                break;
            case State.WaitingForItemToGrab:
                PlacedObject grabPlacedObject = GridBuildingSystem.Instance.GetGridObject(grabPosition).GetPlacedObject();
                PlacedObject dropPlacedObject = GridBuildingSystem.Instance.GetGridObject(dropPosition).GetPlacedObject();

                if (grabPlacedObject != null && dropPlacedObject != null)
                {
                    // Objects exist on both places
                    // Type of object that can be dropped
                    dropFilterItemSO = new ItemSO[] { GameAssets.i.itemSO_Refs.none };

                    if (dropPlacedObject is IItemStorage)
                    {
                        dropFilterItemSO = (dropPlacedObject as IItemStorage).GetItemSOThatCanStore();
                    }
                    if (dropPlacedObject is IWorldItemSlot)
                    {
                        dropFilterItemSO = (dropPlacedObject as IWorldItemSlot).GetItemSOThatCanStore();
                    }

                    //ItemSO.DebugFilter(dropFilterItemSO);
                    // Combine Drop and Grab filters
                    dropFilterItemSO = ItemSO.GetCombinedFilter(new ItemSO[] { grabFilterItemSO }, dropFilterItemSO);

                    if (ItemSO.IsItemSOInFilter(GameAssets.i.itemSO_Refs.none, dropFilterItemSO))
                    {
                        // Cannot drop any item, so dont grab anything
                        break;
                    }

                    // Is Grab PlacedObject a Item Storage?
                    if (grabPlacedObject is IItemStorage)
                    {
                        IItemStorage itemStorage = grabPlacedObject as IItemStorage;
                        if (itemStorage.TryGetStoredItem(dropFilterItemSO, out ItemSO itemScriptableObject))
                        {
                            holdingItem = WorldItem.Create(grabPosition, itemScriptableObject);
                            holdingItem.SetGridPosition(grabPosition);

                            state = State.MovingToDropItem;
                            timer = TIME_TO_DROP_ITEM;
                        }
                        else
                        {
                        }
                    }

                    // Is Grab PlacedObject a WorldItemSlot?
                    if (grabPlacedObject is IWorldItemSlot)
                    {
                        IWorldItemSlot worldItemSlot = grabPlacedObject as IWorldItemSlot;
                        if (worldItemSlot.TryGetWorldItem(dropFilterItemSO, out holdingItem))
                        {
                            holdingItem.SetGridPosition(grabPosition);

                            state = State.MovingToDropItem;
                            timer = TIME_TO_DROP_ITEM;
                        }
                    }
                }
                break;
            case State.MovingToDropItem:
                timer -= Time.deltaTime;
                float percentageComplete = TIME_TO_DROP_ITEM - timer / TIME_TO_DROP_ITEM;
                if (holdingItem != null)
                {
                    holdingItem.transform.position = Vector3.Lerp(grabWorldPosition, dropWorldPosition, percentageComplete);
                }
                if (percentageComplete >= 1f)
                {
                    state = State.DroppingItem;
                }
                break;
            case State.DroppingItem:
                dropPlacedObject = GridBuildingSystem.Instance.GetGridObject(dropPosition).GetPlacedObject();
                // Does it have a place to drop the item?
                if (dropPlacedObject != null)
                {
                    // Is it a World Item Slot?
                    if (dropPlacedObject is IWorldItemSlot)
                    {
                        IWorldItemSlot worldItemSlot = dropPlacedObject as IWorldItemSlot;
                        // Try to Set World Item
                        if (worldItemSlot.TrySetWorldItem(holdingItem))
                        {
                            // It worked, drop item
                            holdingItem.SetGridPosition(worldItemSlot.GetGridPosition());
                            holdingItem = null;

                            state = State.Cooldown;
                            float COOLDOWN_TIME = .2f;
                            timer = COOLDOWN_TIME;

                        }
                        else
                        {
                            // Cannot drop, slot must be full
                            // Continue trying...
                        }
                    }

                    // Is it a Item Storage?
                    if (dropPlacedObject is IItemStorage)
                    {
                        IItemStorage itemStorage = dropPlacedObject as IItemStorage;
                        // Try to Set World Item
                        if (itemStorage.TryStoreItem(holdingItem.GetItemSO()))
                        {
                            // It worked, drop item, destroy world item
                            holdingItem.DestroySelf();
                            holdingItem = null;

                            state = State.Cooldown;
                            float COOLDOWN_TIME = .2f;
                            timer = COOLDOWN_TIME;
                        }
                        else
                        {
                            // Cannot drop, storage must be full
                            // Continue trying...
                        }
                    }
                }
                break;
        }
    }

    public ItemSO GetGrabFilterItemSO()
    {
        return grabFilterItemSO;
    }

    public void SetGrabFilterItemSO(ItemSO grabFilterItemSO)
    {
        this.grabFilterItemSO = grabFilterItemSO;
    }

}
