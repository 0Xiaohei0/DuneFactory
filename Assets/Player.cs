using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    ThirdPersonController thirdPersonController;
    private StarterAssetsInputs _input;
    private Grid grid;
    // Start is called before the first frame update
    void Start()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        _input = GetComponent<StarterAssetsInputs>();
        grid = new Grid(10, 20, 2f, new Vector3(0, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (_input.confirm)
        {
            grid.SetValue(thirdPersonController.mouseWorldPosition, 56);
            _input.confirm = false;
        }
    }
}
