using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AquaticFarmUI : AssemblerUI
{
    public static new AquaticFarmUI Instance { get; private set; }

    public override void Awake()
    {
        base.Awake();
        Instance = this;
    }
}
