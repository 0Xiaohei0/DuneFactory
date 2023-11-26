using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;
using static UnityEditor.Progress;

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
    public bool ironBarProduced;
    public bool extractorProduced;
    public bool furnaceProduced;
    public bool inserterProduced;
    public bool assemblerProduced;
    public bool solarPanelProduced;
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
        if (sender is Smelter)
        {
            Smelter smelter = sender as Smelter;
            smelter.OnItemStorageCountChanged += Smelter_OnItemStorageCountChanged;
            if (!furnacePlaced)
            {
                furnacePlaced = true;
                //DisplayDialog(OnPlaceSolarPanelDialogs);
                RemoveObjective("Build Furnace");
            }
        }
        if (sender is StructureAssembler)
        {
            StructureAssembler structureAssembler = sender as StructureAssembler;
            structureAssembler.OnItemStorageCountChanged += StructureAssembler_OnItemStorageCountChanged;
            if (!structureAssemblerPlaced)
            {
                structureAssemblerPlaced = true;
                //DisplayDialog(OnPlaceSolarPanelDialogs);
                RemoveObjective("Build Structure Assembler");
            }
        }
    }
    private void Smelter_OnItemStorageCountChanged(object sender, System.EventArgs e)
    {
        Smelter smelter = sender as Smelter;
        if (!ironBarProduced)
        {
            ItemStack outputStack = smelter.outputItemStackList.GetItemStackWithItemType(GameAssets.i.itemSO_Refs.ironBar);
            if (outputStack != null && outputStack.amount > 0)
            {
                ironBarProduced = true;
                DisplayDialog(OnProduceIronBarsDialogs);
                print("First ironbar produced");
                RemoveObjective("Produce Iron Bars");
            }
        }
    }
    private void StructureAssembler_OnItemStorageCountChanged(object sender, System.EventArgs e)
    {
        StructureAssembler structureAssembler = sender as StructureAssembler;
        if (!extractorProduced)
        {
            if (structureAssembler.GetItemRecipeSO() == (GameAssets.i.placedObjectTypeSO_Refs.miningMachine))
            {
                extractorProduced = true;
                RemoveObjective("Produce Extractors");
            }
        }
        if (!inserterProduced)
        {
            if (structureAssembler.GetItemRecipeSO() == (GameAssets.i.placedObjectTypeSO_Refs.grabber))
            {
                extractorProduced = true;
                RemoveObjective("Produce Inserters");
            }
        }
        if (!furnaceProduced)
        {
            if (structureAssembler.GetItemRecipeSO() == (GameAssets.i.placedObjectTypeSO_Refs.smelter))
            {
                extractorProduced = true;
                RemoveObjective("Produce Furnaces");
            }
        }
        if (!assemblerProduced)
        {
            if (structureAssembler.GetItemRecipeSO() == (GameAssets.i.placedObjectTypeSO_Refs.assembler))
            {
                extractorProduced = true;
                RemoveObjective("Produce Assemblers");
            }
        }
        if (!solarPanelProduced)
        {
            if (structureAssembler.GetItemRecipeSO() == (GameAssets.i.placedObjectTypeSO_Refs.solarPanel))
            {
                extractorProduced = true;
                RemoveObjective("Produce Solar Panels");
            }
        }
    }

    void RemoveObjective(string objectiveName)
    {
        Objectives.Remove(objectiveName);
        UpdateObjectiveList();
    }
}
