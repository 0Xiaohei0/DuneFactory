using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }
    public GameObject popupPrefab; // Assign your popup prefab in the Inspector
    private void Awake()
    {
        Instance = this;
    }
    public void ShowPopup(string message, float delay = 3.0f)
    {
        GameObject popupInstance = Instantiate(popupPrefab, transform);
        TMP_Text messageText = popupInstance.GetComponentInChildren<TMP_Text>();
        if (messageText != null)
        {
            messageText.text = message;
        }
        Destroy(popupInstance, delay);
    }
}
