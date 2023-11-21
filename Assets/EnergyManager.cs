using TMPro;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance { get; private set; }
    public float TotalEnergy { get; private set; }
    public float MaxEnergy = 1000; // Max energy capacity
    public float TickInterval = 1.0f; // Time in seconds for each tick
    public float ExcessEnergy = 0;
    public float ExcessEnergyRate = 0;
    public float NoEnergyThreshold = 0.75f;
    public int NoEnergyDuration = 3;

    [SerializeField] public float energyProducedThisTick = 0;
    [SerializeField] public float energyConsumedThisTick = 0;

    [SerializeField] private float efficiencyRatio;
    [SerializeField] private int noEnergyTickCount;

    public TMP_Text EnergyProducedText; // The parent transform for stat UI
    public TMP_Text EnergyConsumedText; // The parent transform for stat UI

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
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
            ExcessEnergyRate = TotalEnergy - MaxEnergy;
            ExcessEnergy += ExcessEnergyRate;
            TotalEnergy = MaxEnergy;
        }

        efficiencyRatio = energyProducedThisTick / energyConsumedThisTick;
        if (efficiencyRatio <= NoEnergyThreshold)
        {
            noEnergyTickCount++;
        }
        else
        {
            noEnergyTickCount = 0;
        }
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
            consumer.SetPowerStatus(noEnergyTickCount < NoEnergyDuration); // True if enough power, false otherwise
            consumer.gameObject.GetComponent<PlacedObject>().SetEffciencyMultiplier(Mathf.Min(efficiencyRatio, 1));
        }
    }

    private void UpdateUI()
    {
        EnergyProducedText.text = energyProducedThisTick.ToString() + "kWh";
        EnergyConsumedText.text = energyConsumedThisTick.ToString() + "kWh";
    }
}
