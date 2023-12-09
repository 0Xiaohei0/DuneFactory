using UniGLTF;
using UnityEngine;

public class EnergyProducer : EnergyBuilding
{
    private float tickTimer = 0f;
    private Transform structureSound;
    void Awake()
    {
        energyRate = Mathf.Abs(energyRate);
        structureSound = transform.Find("StructureSound");
    }
    void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= energyManager.TickInterval)
        {
            if (isOn)
            {
                energyManager.AddProduction(energyRate);
            }
            tickTimer = 0f;
        }
        if (structureSound != null)
        {
            structureSound.gameObject.SetActive(isOn);
        }
    }
}
