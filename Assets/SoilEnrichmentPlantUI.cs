using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoilEnrichmentPlantUI : MonoBehaviour
{
    public static SoilEnrichmentPlantUI Instance { get; private set; }



    [SerializeField] private List<ItemRecipeSO> itemRecipeScriptableObjectList;

    private Dictionary<ItemSO, Transform> recipeButtonDic;
    private SoilEnrichmentPlant soilEnrichmentPlant;
    private Image craftingProgressBar;
    private TextMeshProUGUI powerSatisfactionText;

    public virtual void Awake()
    {
        Instance = this;

        transform.Find("CloseBtn").GetComponent<Button_UI>().ClickFunc = () =>
        {
            Hide();
        };

        craftingProgressBar = transform.Find("CraftingProgressBar").Find("Bar").GetComponent<Image>();
        craftingProgressBar.fillAmount = 0f;
        powerSatisfactionText = transform.Find("PowerSatisfiedContainer").Find("PowerSatisfiedText").GetComponent<TextMeshProUGUI>();
        Hide();
    }

    private void Update()
    {
        UpdateCraftingProgress();
        UpdatePowerSatisfaction();
    }
    private void UpdatePowerSatisfaction()
    {
        if (soilEnrichmentPlant != null)
        {
            powerSatisfactionText.text = Mathf.RoundToInt(soilEnrichmentPlant.powerSaticfactionMultiplier * 100) + "%";
        }
        else
        {
            powerSatisfactionText.text = 0 + "%";
        }
    }

    private void UpdateCraftingProgress()
    {
        if (soilEnrichmentPlant != null)
        {
            craftingProgressBar.fillAmount = soilEnrichmentPlant.GetCraftingProgressNormalized();
        }
        else
        {
            craftingProgressBar.fillAmount = 0f;
        }
    }

    private void UpdateInputs()
    {
        print("updating input for soil");
        Transform inputsContainer = transform.Find("InputsContainer");
        Transform inputsTemplate = inputsContainer.Find("Template");
        inputsTemplate.gameObject.SetActive(false);

        // Destory old transforms
        foreach (Transform transform in inputsContainer)
        {
            if (transform != inputsTemplate)
            {
                Destroy(transform.gameObject);
            }
        }

        if (soilEnrichmentPlant != null && soilEnrichmentPlant.HasItemRecipe())
        {
            ItemRecipeSO itemRecipeScriptableObject = soilEnrichmentPlant.GetItemRecipeSO();

            foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeScriptableObject.inputItemList)
            {
                Transform inputTransform = Instantiate(inputsTemplate, inputsContainer);
                inputTransform.gameObject.SetActive(true);

                inputTransform.Find("Icon").GetComponent<Image>().sprite = recipeItem.item.sprite;
                inputTransform.Find("Text").GetComponent<TextMeshProUGUI>().text = soilEnrichmentPlant.GetItemStoredCount(recipeItem.item).ToString();
            }
        }
    }

    private void Assembler_OnItemStorageCountChanged(object sender, System.EventArgs e)
    {
        UpdateInputs();
    }

    public void Show(SoilEnrichmentPlant soilEnrichmentPlant)
    {
        gameObject.SetActive(true);

        if (this.soilEnrichmentPlant != null)
        {
            // Unsub from previous Assembler
            this.soilEnrichmentPlant.OnItemStorageCountChanged -= Assembler_OnItemStorageCountChanged;
        }

        this.soilEnrichmentPlant = soilEnrichmentPlant;

        if (soilEnrichmentPlant != null)
        {
            // Sub for item changes
            soilEnrichmentPlant.OnItemStorageCountChanged += Assembler_OnItemStorageCountChanged;
        }
        UpdateInputs();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
