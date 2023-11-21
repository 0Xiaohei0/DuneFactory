using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public int saveIdxToLoad = 0;
    public bool saveSelected = false;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}
