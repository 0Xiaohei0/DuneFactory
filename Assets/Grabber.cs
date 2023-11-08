using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : PlacedObject
{

    public GameObject WorldItemPrefab;

    private enum State
    {
        Cooldown,
        WaitingForItemToGrab,
        MovingToDropItem,
        DroppingItem,
    }

    private Vector2Int grabPosition;
    private Vector2Int dropPosition;
    private WorldItem holdingItem;
    private float timer;
    private string textString = "";
    private State state;


    protected override void Setup()
    {
        Debug.Log("Grabber.Setup()");

        grabPosition = origin + PlacedObjectTypeSO.GetDirForwardVector(dir) * -1;
        dropPosition = origin + PlacedObjectTypeSO.GetDirForwardVector(dir);

        state = State.Cooldown;
    }

    private void Update()
    {
        Debug.Log(state);
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

                print("Grab:" + grabPlacedObject);
                print("Drop:" + dropPlacedObject);
                if (grabPlacedObject != null && dropPlacedObject != null)
                {
                    // Objects exist on both places

                    ItemSO[] dropFilterItemSO = new ItemSO[] { };
                    if (dropPlacedObject is IItemStorage)
                    {
                        dropFilterItemSO = (dropPlacedObject as IItemStorage).GetItemSOThatCanStore();
                    }
                    // Is Grab PlacedObject a Item Storage?
                    if (grabPlacedObject is IItemStorage)
                    {
                        print("is item storage");
                        IItemStorage itemStorage = grabPlacedObject as IItemStorage;
                        if (itemStorage.TryGetStoredItem(dropFilterItemSO, out ItemSO itemScriptableObject))
                        {
                            print("got stored item");
                            holdingItem = Instantiate(WorldItemPrefab, GridBuildingSystem.Instance.GetWorldPosition(grabPosition), Quaternion.identity).GetComponent<WorldItem>();

                            state = State.MovingToDropItem;
                            float TIME_TO_DROP_ITEM = .5f;
                            timer = TIME_TO_DROP_ITEM;
                        }
                        print("failed to get stored item");
                    }

                    // Is Grab PlacedObject a ConveyerBelt?
                    /*                   if (grabPlacedObject.GetComponent<ConveyerBelt>() != null)
                                       {
                                           IWorldItemSlot worldItemSlot = grabPlacedObject as IWorldItemSlot;
                                           if (worldItemSlot.TryGetWorldItem(dropFilterItemSO, out holdingItem))
                                           {
                                               holdingItem.SetGridPosition(grabPosition);

                                               state = State.MovingToDropItem;
                                               float TIME_TO_DROP_ITEM = .5f;
                                               timer = TIME_TO_DROP_ITEM;
                                           }
                                       }*/
                }
                break;
            case State.MovingToDropItem:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    state = State.DroppingItem;
                }
                break;
            case State.DroppingItem:
                dropPlacedObject = GridBuildingSystem.Instance.GetGridObject(dropPosition).GetPlacedObject();
                // Does it have a place to drop the item?
                if (dropPlacedObject != null)
                {
                    ConveyerBelt conveyerBelt = dropPlacedObject.GetComponent<ConveyerBelt>();
                    // Is it a Conveyer belt?
                    if (conveyerBelt != null)
                    {
                        // Try to drop item
                        if (!conveyerBelt.IsSpaceTaken())
                        {
                            conveyerBelt.SetBeltItem(holdingItem);
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
}
