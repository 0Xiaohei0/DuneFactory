using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class DialogManager : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public GameObject dialogueBox;
    public List<DialogueSO> beginningDialogs;
    public List<DialogueSO> OnPlaceExtractorDialogs;
    public List<DialogueSO> OnPlaceSolarPanelDialogs;
    public List<DialogueSO> OnProduceIronBarsDialogs;

    public bool extractorPlaced;
    public bool solarPanelPlaced;
    public VideoClip speaking;
    public VideoClip idle;
    public VideoPlayer videoPlayer;
    AudioSource audioSource;
    private Queue<DialogueSO> sentences; // Queue to store sentences

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sentences = new Queue<DialogueSO>();
        GridBuildingSystem.Instance.OnObjectPlaced += OnbuildingPlaced;
        DisplayDialog(beginningDialogs);
    }
    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            videoPlayer.clip = idle;
            videoPlayer.Play();
        }
    }
    public void DisplayDialog(List<DialogueSO> dialogList)
    {
        dialogueBox.SetActive(true);

        sentences.Clear();

        foreach (DialogueSO dialogueSO in dialogList)
        {
            sentences.Enqueue(dialogueSO);
        }

        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    {
        videoPlayer.clip = speaking;
        videoPlayer.Play();
        if (sentences.Count == 0)
        {
            CloseDialogue();
            return;
        }

        DialogueSO dialogue = sentences.Dequeue();
        dialogueText.text = dialogue.sentence;
        audioSource.clip = dialogue.audioClip;
        audioSource.Play();
    }
    void CloseDialogue()
    {
        audioSource.Stop();
        dialogueBox.SetActive(false);
    }

    void OnbuildingPlaced(object sender, EventArgs e)
    {
        if (!extractorPlaced && sender is MiningMachine)
        {
            print("First Extractor Placed!");
            extractorPlaced = true;
            DisplayDialog(OnPlaceExtractorDialogs);
        }
        if (!solarPanelPlaced && sender is SolarPanel)
        {
            print("First SolarPanel Placed!");
            solarPanelPlaced = true;
            DisplayDialog(OnPlaceSolarPanelDialogs);
        }
    }
}
