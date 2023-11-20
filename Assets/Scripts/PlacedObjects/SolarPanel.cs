using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarPanel : PlacedObject
{
    // Start is called before the first frame update
    void Start()
    {
        EnergyProducer energyProducer = transform.GetComponent<EnergyProducer>();
        if (energyProducer != null)
        {
            energyProducer.isOn = true;
        }
    }

}
