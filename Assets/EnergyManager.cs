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

    [SerializeField] private float efficiencyRatio;

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
            TotalEnergy = 0;
        }
        else if (TotalEnergy > MaxEnergy)
        {
            TotalEnergy = MaxEnergy;
        }

        efficiencyRatio = energyProducedThisTick / energyConsumedThisTick;
        AdjustBuildingEfficiency(efficiencyRatio);

        UpdateUI();

        // Reset for the next tick
        energyProducedThisTick = 0;
        energyConsumedThisTick = 0;
    }

    private void AdjustBuildingEfficiency(float efficiencyRatio)
    {
        foreach (var consumer in FindObjectsOfType<EnergyConsumer>())
        {
            consumer.SetPowerStatus(efficiencyRatio >= 0.75f); // True if enough power, false otherwise
            consumer.gameObject.GetComponent<PlacedObject>().SetEffciencyMultiplier(Mathf.Min(efficiencyRatio, 1));
        }
    }

    private void UpdateUI()
    {
        EnergyProducedText.text = energyProducedThisTick.ToString() + "kWh";
        EnergyConsumedText.text = energyConsumedThisTick.ToString() + "kWh";
    }
}
