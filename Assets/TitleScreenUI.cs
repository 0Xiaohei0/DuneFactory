using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenUI : MonoBehaviour
{
    public void StartGame()
    {
        // Replace "GameScene" with the name of your game scene
        SceneManager.LoadScene("Playground");
    }

    public void QuitGame()
    {
        // This will quit the game. Note: This will not stop the game in the Unity editor.
        Application.Quit();
    }
}
