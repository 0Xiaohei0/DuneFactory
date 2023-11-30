using CodeMonkey.Utils;
using System;
using System.Collections.Generic;
using TMPro;
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
    public List<DialogueSO> OnPlaceFurnacelDialogs;
    public List<DialogueSO> OnPlaceInserterDialogs;
    public List<DialogueSO> OnProduceIronBarsDialogs;
    public List<DialogueSO> OnProduceExtractorDialogs;
    public List<DialogueSO> OnLevel1Dialogs;
    public List<DialogueSO> OnLevel2Dialogs;
    public List<DialogueSO> OnFinishGameDialogs;
    public List<string> Objectives;
    public int maxObjectives = 5;
    public GameObject objectivePanel;

    public bool extractorPlaced;
    public bool solarPanelPlaced;
    public bool structureAssemblerPlaced;
    public bool furnacePlaced;
    public bool inserterPlaced;
    public bool inserter2Placed;
    public bool ironBarProduced;
    public bool extractorProduced;
    public bool furnaceProduced;
    public bool inserterProduced;
    public bool assemblerProduced;
    public bool solarPanelProduced;
    public Vector2Int extractorPosition = new Vector2Int(498, 506);
    public Vector2Int SolarPanelPosition = new Vector2Int(498, 502);
    public Vector2Int furnacePosition = new Vector2Int(501, 506);
    public Vector2Int inserterPosition = new Vector2Int(500, 506);
    public Vector2Int structureAssemplerPosition = new Vector2Int(506, 505);
    public Vector2Int inserterPosition2 = new Vector2Int(498, 506);

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
        TerrainManager.Instance.OnTerraformPercentageCalculated += OnTerraformPercentageCalculated;
        ProgressionManager.Instance.OnLevelUp += OnLevelup;
        DisplayDialog(beginningDialogs);
        UpdateObjectiveList();
        TutorialGhost.Instance.QueueGhostList(TutorialGhost.Instance.ExtractorBuilding);
        TutorialGhost.Instance.QueueGhostList(TutorialGhost.Instance.SolarPanelBuilding);
        TutorialGhost.Instance.QueueGhostList(TutorialGhost.Instance.FurnaceBuilding);
        TutorialGhost.Instance.QueueGhostList(TutorialGhost.Instance.FurnaceInserter);
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
            MiningMachine extractor = sender as MiningMachine;
            if (extractor.origin == extractorPosition)
            {
                extractorPlaced = true;
                DisplayDialog(OnPlaceExtractorDialogs);
                RemoveObjective("Build Extractor");
            }
        }
        if (!solarPanelPlaced && sender is SolarPanel)
        {
            SolarPanel solarPanel = sender as SolarPanel;
            if (solarPanel.origin == SolarPanelPosition)
            {
                solarPanelPlaced = true;
                DisplayDialog(OnPlaceSolarPanelDialogs);
                RemoveObjective("Build Solar Panels");
            }
        }
        if (sender is Smelter)
        {
            Smelter smelter = sender as Smelter;
            smelter.OnItemStorageCountChanged += Smelter_OnItemStorageCountChanged;
            if (!furnacePlaced && smelter.origin == furnacePosition)
            {
                furnacePlaced = true;
                DisplayDialog(OnPlaceFurnacelDialogs);
                RemoveObjective("Build Furnace");
            }
        }
        if (sender is StructureAssembler)
        {
            StructureAssembler structureAssembler = sender as StructureAssembler;
            structureAssembler.OnBuildingProduced += StructureAssembler_OnBuildingProduced;
            if (!structureAssemblerPlaced)
            {
                structureAssemblerPlaced = true;
                RemoveObjective("Build Structure Assembler");
            }
        }
        if (sender is Grabber)
        {
            Grabber grabber = sender as Grabber;

            if (!inserterPlaced && grabber.origin == inserterPosition && grabber.GetDir() == PlacedObjectTypeSO.Dir.Right)
            {
                inserterPlaced = true;
                DisplayDialog(OnPlaceInserterDialogs);
            }
            if (!inserter2Placed && grabber.origin == inserterPosition2 && grabber.GetDir() == PlacedObjectTypeSO.Dir.Right)
            {
                inserter2Placed = true;
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
                TutorialGhost.Instance.QueueGhostList(TutorialGhost.Instance.ExtractorProductionList);
            }
        }
    }
    private void StructureAssembler_OnBuildingProduced(object sender, System.EventArgs e)
    {
        if (Objectives.Contains("Produce Extractors"))
        {
            if ((object)sender == (GameAssets.i.placedObjectTypeSO_Refs.miningMachine))
            {
                DisplayDialog(OnProduceExtractorDialogs);
                RemoveObjective("Produce Extractors");
                TutorialGhost.Instance.QueueGhostList(TutorialGhost.Instance.BasicBuildingList);
                TutorialGhost.Instance.QueueGhostList(TutorialGhost.Instance.IronBarUpgrade);
                TutorialGhost.Instance.QueueGhostList(TutorialGhost.Instance.CircuitBoardProduction);
                TutorialGhost.Instance.QueueGhostList(TutorialGhost.Instance.MegneticRingProduction);
                TutorialGhost.Instance.QueueGhostList(TutorialGhost.Instance.GeothermalGenerator);
                TutorialGhost.Instance.QueueGhostList(TutorialGhost.Instance.TerraformingBuilding);
            }
        }
        if (Objectives.Contains("Produce Inserters"))
        {
            if ((object)sender == (GameAssets.i.placedObjectTypeSO_Refs.grabber))
            {
                RemoveObjective("Produce Inserters");
            }
        }
        if (Objectives.Contains("Produce Furnaces"))
        {
            if ((object)sender == (GameAssets.i.placedObjectTypeSO_Refs.smelter))
            {
                RemoveObjective("Produce Furnaces");
            }
        }
        if (Objectives.Contains("Produce Assemblers"))
        {
            if ((object)sender == (GameAssets.i.placedObjectTypeSO_Refs.assembler))
            {
                RemoveObjective("Produce Assemblers");
            }
        }
        if (Objectives.Contains("Produce Solar Panels"))
        {
            if ((object)sender == (GameAssets.i.placedObjectTypeSO_Refs.solarPanel))
            {
                RemoveObjective("Produce Solar Panels");
            }
        }
        if (Objectives.Contains("Produce Atmospheric Extractor"))
        {
            if ((object)sender == (GameAssets.i.placedObjectTypeSO_Refs.AtmosphericExtractor))
            {
                RemoveObjective("Produce Atmospheric Extractor");
            }
        }
        if (Objectives.Contains("Produce GeoThermal Generator"))
        {
            if ((object)sender == (GameAssets.i.placedObjectTypeSO_Refs.GeoThermalGenerator))
            {
                RemoveObjective("Produce GeoThermal Generator");
            }
        }
        if (Objectives.Contains("Produce Aquatic Farm"))
        {
            if ((object)sender == (GameAssets.i.placedObjectTypeSO_Refs.AquaticFarm))
            {
                RemoveObjective("Produce Aquatic Farm");
            }
        }
        if (Objectives.Contains("Produce Soil Enrichment Plant"))
        {
            if ((object)sender == (GameAssets.i.placedObjectTypeSO_Refs.SoilEnrichmentPlant))
            {
                RemoveObjective("Produce Soil Enrichment Plant");
            }
        }
    }
    void OnTerraformPercentageCalculated(object sender, EventArgs e)
    {
        TerrainManager terrainManager = sender as TerrainManager;
        if (Objectives.Contains("Terraform 10% of the desert") && terrainManager.GetAverageDensity() >= 0.1)
        {
            RemoveObjective("Terraform 10% of the desert");
            DisplayDialog(OnFinishGameDialogs);
        }
    }
    void OnLevelup(object sender, EventArgs e)
    {
        ProgressionManager progressManager = sender as ProgressionManager;
        if (progressManager.currentLevel == 1)
        {
            DisplayDialog(OnLevel1Dialogs);
        }
        else if (progressManager.currentLevel == 2)
        {
            DisplayDialog(OnLevel2Dialogs);
        }
    }
    void RemoveObjective(string objectiveName)
    {
        Objectives.Remove(objectiveName);
        UpdateObjectiveList();
    }
}
