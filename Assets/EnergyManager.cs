using TMPro;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance { get; private set; }
    public float TotalEnergy { get; private set; }
    public float MaxEnergy = 1000; // Max energy capacity
    public float TickInterval = 1.0f; // Time in seconds for each tick

    [SerializeField] public float energyProducedThisTick = 0;
    [SerializeField] public float energyConsumedThisTick = 0;

    public TMP_Text EnergyProducedText; // The parent transform for stat UI
    public TMP_Text EnergyConsumedText; // The parent transform for stat UI

    void Start()
    {
        Instance = this;
        TotalEnergy = 0;
        InvokeRepeating("ProcessTick", TickInterval, TickInterval); // Repeatedly call ProcessTick every TickInterval seconds
    }

    public void AddProduction(float amount)
    {
        energyProducedThisTick += amount;
    }

    public void AddConsumption(float amount)
    {
        energyConsumedThisTick += amount;
    }

    private void ProcessTick()
    {
        TotalEnergy += energyProducedThisTick;
        TotalEnergy -= energyConsumedThisTick;

        if (TotalEnergy < 0)
        {
            float efficiencyRatio = energyProducedThisTick / energyConsumedThisTick;
            TotalEnergy = 0;
            AdjustBuildingEfficiency(efficiencyRatio);
        }
        else if (TotalEnergy > MaxEnergy)
        {
            TotalEnergy = MaxEnergy;
        }
        UpdateUI();

        // Reset for the next tick
        energyProducedThisTick = 0;
        energyConsumedThisTick = 0;
    }

    private void AdjustBuildingEfficiency(float efficiencyRatio)
    {
        // Adjust the efficiency of consumer buildings here based on efficiencyRatio
    }

    private void UpdateUI()
    {
        EnergyProducedText.text = energyProducedThisTick.ToString() + "kWh";
        EnergyConsumedText.text = energyConsumedThisTick.ToString() + "kWh";
    }
}
