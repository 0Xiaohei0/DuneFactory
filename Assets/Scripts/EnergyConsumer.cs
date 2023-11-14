using UnityEngine;

public class EnergyConsumer : EnergyBuilding
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
            energyManager.AddConsumption(energyRate);
            tickTimer = 0f;
        }
    }
}
