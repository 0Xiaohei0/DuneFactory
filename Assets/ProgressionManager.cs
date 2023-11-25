using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance { get; private set; }
    public List<Level> levelList = new List<Level>();
    private Dictionary<PlacedObjectTypeSO, Transform> buildingButtonDic;
    public int currentLevel = 0;
    public Image progressFill;
    public TMP_Text levelText;
    public TMP_Text speedText;
    public Transform unlocksContainer;
    public Transform researchLevelUpPanel;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        researchLevelUpPanel.gameObject.SetActive(false);
        SetupUnlocks(unlocksContainer);
    }

    // Update is called once per frame
    void Update()
    {
        speedText.text = "$ " + EnergyManager.Instance.ExcessEnergyRate + "k / s";
        if (currentLevel < levelList.Count)
        {
            progressFill.fillAmount = EnergyManager.Instance.ExcessEnergy / levelList[currentLevel].levelCost;
            if (EnergyManager.Instance.ExcessEnergy >= levelList[currentLevel].levelCost)
            {
                LevelUp();
            }
        }
    }
    public void LevelUp()
    {
        if (currentLevel < levelList.Count - 1)
        {
            EnergyManager.Instance.ExcessEnergy = 0;
            UnlockBuildings(currentLevel);
            levelText.text = "Lv " + currentLevel.ToString();
        }
        if (currentLevel == levelList.Count - 1)
        {
            EnergyManager.Instance.ExcessEnergy = 0;
            UnlockBuildings(currentLevel);
            levelText.text = "Lv MAX";
        }
        currentLevel++;
        SetupUnlocks(unlocksContainer);
    }
    private void SetupUnlocks(Transform unlocksContainer)
    {
        if (currentLevel == levelList.Count) return;
        Transform unlocksTemplate = unlocksContainer.Find("Template");
        unlocksTemplate.gameObject.SetActive(false);

        // Destory old transforms
        foreach (Transform transform in unlocksContainer)
        {
            if (transform != unlocksTemplate)
            {
                Destroy(transform.gameObject);
            }
        }

        // Build transforms
        for (int i = 0; i < levelList[currentLevel].unlockBuildings.Count; i++)
        {
            PlacedObjectTypeSO buildingScriptableObject = levelList[currentLevel].unlockBuildings[i];
            Transform recipeTransform = Instantiate(unlocksTemplate, unlocksContainer);
            recipeTransform.gameObject.SetActive(true);
            recipeTransform.Find("Icon").GetComponent<Image>().sprite = buildingScriptableObject.icon;
            //recipeTransform.Find("Text").GetComponent<TextMeshProUGUI>().text = itemRecipeScriptableObject.name;
        }

        // Build transforms
        for (int i = 0; i < levelList[currentLevel].unlockItems.Count; i++)
        {
            ItemRecipeSO itemScriptableObject = levelList[currentLevel].unlockItems[i];
            Transform itemTransform = Instantiate(unlocksTemplate, unlocksContainer);
            itemTransform.gameObject.SetActive(true);
            itemTransform.Find("Icon").GetComponent<Image>().sprite = itemScriptableObject.outputItemList[0].item.sprite;
            itemTransform.Find("Icon").GetComponent<Image>().color = Color.white;
            //recipeTransform.Find("Text").GetComponent<TextMeshProUGUI>().text = itemRecipeScriptableObject.name;
        }
    }

    private void UnlockBuildings(int level)
    {
        for (int i = 0; i < levelList[level].unlockBuildings.Count; i++)
        {
            if (!UIManager.Instance.currentUnlockedBuildings.Contains(levelList[level].unlockBuildings[i]))
            {
                UIManager.Instance.currentUnlockedBuildings.Add(levelList[level].unlockBuildings[i]);
            }
            PlacedObjectTypeSO buildingScriptableObject = levelList[level].unlockBuildings[i];
            bool found = false;
            foreach (PlacedObjectTypeSO placedObjectTypeSO in StructureAssemblerUI.Instance.itemRecipeScriptableObjectList)
            {
                if (buildingScriptableObject.nameString == placedObjectTypeSO.nameString)
                    found = true;
            }
            if (!found)
            {
                StructureAssemblerUI.Instance.itemRecipeScriptableObjectList.Add(buildingScriptableObject);
            }
        }
        for (int i = 0; i < levelList[level].unlockItems.Count; i++)
        {
            ItemRecipeSO itemScriptableObject = levelList[level].unlockItems[i];
            bool found = false;
            foreach (ItemRecipeSO itemSO in AssemblerUI.Instance.itemRecipeScriptableObjectList)
            {
                if (itemScriptableObject.recipeName == itemSO.recipeName)
                {
                    found = true;
                }
            }
            if (!found)
            {
                AssemblerUI.Instance.itemRecipeScriptableObjectList.Add(itemScriptableObject);
            }
        }
        researchLevelUpPanel.Find("CloseBtn").GetComponent<Button_UI>().ClickFunc = () =>
        {
            researchLevelUpPanel.gameObject.SetActive(false);
        };
        SetupUnlocks(researchLevelUpPanel.Find("UnlocksContainer"));
        researchLevelUpPanel.gameObject.SetActive(true);
    }

}
