using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SaveManager;

public class TitleScreenManager : SaveManager
{
    private void Start()
    {
        print(Resources.Load<TextAsset>("Assets/saves/TitleSave"));
        GameData saveData = SaveSystem.LoadGameDataFromString(Resources.Load<TextAsset>("saves/TitleSave").text);
        gridBuildingSystem.processInput = false;
        LoadGameSave(saveData);
    }
    void Update()
    { }
}
