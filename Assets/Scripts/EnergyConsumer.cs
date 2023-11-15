using UniGLTF;
using UnityEngine;

public class EnergyConsumer : EnergyBuilding
{
    public GameObject insufficientPowerIcon; // Assign this in the Inspector

    private bool hasSufficientPower = true;

    private float tickTimer = 0f;
    void Awake()
    {
        energyRate = Mathf.Abs(energyRate);
        insufficientPowerIcon = transform.FindDescendant("NoPower").gameObject;
    }
    void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= energyManager.TickInterval)
        {
            energyManager.AddConsumption(energyRate);
            tickTimer = 0f;
        }
        UpdateIconVisibility();
    }
    private void UpdateIconVisibility()
    {
        if (insufficientPowerIcon != null)
        {
            insufficientPowerIcon.SetActive(!hasSufficientPower);
        }
    }

    public void SetPowerStatus(bool hasPower)
    {
        hasSufficientPower = hasPower;
    }
}
