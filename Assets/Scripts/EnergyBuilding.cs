using UnityEngine;

public abstract class EnergyBuilding : MonoBehaviour
{
    public float energyRate; // Can be positive for production, negative for consumption

    protected EnergyManager energyManager;

    protected virtual void Start()
    {
        energyManager = FindObjectOfType<EnergyManager>();
        if (energyManager == null)
        {
            Debug.LogError("EnergyManager not found in the scene!");
        }
    }
}