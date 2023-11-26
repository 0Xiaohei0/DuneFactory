using CodeMonkey.Utils;
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
    public List<string> Objectives;
    public int maxObjectives = 5;
    public GameObject objectivePanel;

    public bool extractorPlaced;
    public bool solarPanelPlaced;
    public bool structureAssemblerPlaced;
    public bool furnacePlaced;
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
        UpdateObjectiveList();
    }
    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            videoPlayer.clip = idle;
            videoPlayer.Play();
        }
    }
    private void UpdateObjectiveList()
    {
        Transform ObjectivesContainer = transform.Find("ObjectivePanel");
        Transform ObjectivesTemplate = ObjectivesContainer.Find("Template");
        ObjectivesTemplate.gameObject.SetActive(false);

        // Destory old transforms
        foreach (Transform transform in ObjectivesContainer)
        {
            if (transform != ObjectivesTemplate)
            {
                Destroy(transform.gameObject);
            }
        }

        for (int i = 0; i < Mathf.Min(Objectives.Count, maxObjectives); i++)
        {
            string objective = Objectives[i];
            Transform recipeTransform = Instantiate(ObjectivesTemplate, ObjectivesContainer);
            recipeTransform.gameObject.SetActive(true);
            recipeTransform.Find("Text").GetComponent<TMP_Text>().text = objective;
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
            extractorPlaced = true;
            DisplayDialog(OnPlaceExtractorDialogs);
            RemoveObjective("Build Extractor");
        }
        if (!solarPanelPlaced && sender is SolarPanel)
        {
            solarPanelPlaced = true;
            DisplayDialog(OnPlaceSolarPanelDialogs);
            RemoveObjective("Build Solar Panels");
        }
        if (!furnacePlaced && sender is Smelter)
        {
            furnacePlaced = true;
            //DisplayDialog(OnPlaceSolarPanelDialogs);
            RemoveObjective("Build Furnace");
        }
        if (!structureAssemblerPlaced && sender is StructureAssembler)
        {
            structureAssemblerPlaced = true;
            //DisplayDialog(OnPlaceSolarPanelDialogs);
            RemoveObjective("Build Structure Assembler");
        }
    }

    void RemoveObjective(string objectiveName)
    {
        Objectives.Remove(objectiveName);
        UpdateObjectiveList();
    }
}
