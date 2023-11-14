using UnityEngine;

public class EnergyProducer : EnergyBuilding
{
    private float tickTimer = 0f;
    void Awake()
    {
        energyRate = Mathf.Abs(energyRate);
    }
    void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= energyManager.TickInterval)
        {
            energyManager.AddProduction(energyRate);
            tickTimer = 0f;
        }
    }
}
