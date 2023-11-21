using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveUI : MonoBehaviour
{
    public GameObject SaveBG;
    public GameObject LoadBG;

    private void Start()
    {
        LoadBG.SetActive(false);
    }
    public void OpenLoadMenu()
    {
        LoadBG.SetActive(true);
        UpdateSaveSlots();
    }

    public void LoadSave(int idx)
    {
        GameSettings gameSettings = FindAnyObjectByType<GameSettings>();
        if (gameSettings != null)
        {
            gameSettings.saveSelected = true;
            gameSettings.saveIdxToLoad = idx;
        }
        SceneManager.LoadScene("Playground");
    }
    public void UpdateSaveSlots()
    {
        int idx = 1;
        foreach (Transform saveSlot in LoadBG.transform.Find("SaveSlotContainer"))
        {
            Button button = saveSlot.GetComponent<Button>();
            int currentIdx = idx;
            button.onClick.AddListener(() => LoadSave(currentIdx));

            SaveManager.SaveStatus saveStatus = SaveManager.Instance.GetSaveStatus(currentIdx);
            saveSlot.Find("SlotStatus").GetComponent<TMP_Text>().text = saveStatus.statusString;
            saveSlot.Find("SlotName").GetComponent<TMP_Text>().text = "Slot " + currentIdx;
            saveSlot.Find("SaveTime").GetComponent<TMP_Text>().text = saveStatus.saveTime;
            idx++;
        }
    }
}
