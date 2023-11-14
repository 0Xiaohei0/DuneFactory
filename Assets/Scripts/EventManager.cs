using System;
using UnityEngine;

public static class EventManager
{
    // Define a delegate for building selection
    public delegate void BuildingSelectedHandler(PlacedObjectTypeSO selectedBuilding);

    // Define an event based on the delegate
    public static event BuildingSelectedHandler OnBuildingSelected;

    // Method to call the event
    public static void BuildingSelected(PlacedObjectTypeSO selectedBuilding)
    {
        OnBuildingSelected?.Invoke(selectedBuilding);
    }
}
