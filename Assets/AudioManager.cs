using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Transform buildingPlacedAudio;
    // Start is called before the first frame update
    void Start()
    {
        GridBuildingSystem.Instance.OnObjectPlaced += OnBuildingPlaced;
    }

    public void OnBuildingPlaced(object caller, EventArgs args)
    {
        PlacedObject placedObject = caller as PlacedObject;
        Instantiate(buildingPlacedAudio, placedObject.transform);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
