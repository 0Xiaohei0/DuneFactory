using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    ThirdPersonController thirdPersonController;
    private StarterAssetsInputs _input;
    private GridBuildingSystem gridBuildingSystem;
    // Start is called before the first frame update
    void Start()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        gridBuildingSystem = FindFirstObjectByType<GridBuildingSystem>();
        _input = GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_input.confirm)
        {
            gridBuildingSystem.SpawnStructure(thirdPersonController.mouseWorldPosition);
            _input.confirm = false;
        }
    }
}
