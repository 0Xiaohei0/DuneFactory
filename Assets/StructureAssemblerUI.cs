using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StructureAssemblerUI : MonoBehaviour
{

    public static StructureAssemblerUI Instance { get; private set; }



    [SerializeField] public List<PlacedObjectTypeSO> itemRecipeScriptableObjectList;

    private Dictionary<PlacedObjectTypeSO, Transform> recipeButtonDic;
    private StructureAssembler structureAssembler;
    private Image craftingProgressBar;
    private TextMeshProUGUI powerSatisfactionText;

    private void Awake()
    {
        Instance = this;

        transform.Find("CloseBtn").GetComponent<Button_UI>().ClickFunc = () =>
        {
            Hide();
        };

        craftingProgressBar = transform.Find("CraftingProgressBar").Find("Bar").GetComponent<Image>();
        craftingProgressBar.fillAmount = 0f;
        powerSatisfactionText = transform.Find("PowerSatisfiedContainer").Find("PowerSatisfiedText").GetComponent<TextMeshProUGUI>();

        SetupRecipes();

        Hide();
    }

    private void Update()
    {
        UpdateCraftingProgress();
        UpdatePowerSatisfaction();
    }
    private void UpdatePowerSatisfaction()
    {
        if (structureAssembler != null)
        {
            powerSatisfactionText.text = Mathf.RoundToInt(structureAssembler.powerSaticfactionMultiplier * 100) + "%";
        }
        else
        {
            powerSatisfactionText.text = 0 + "%";
        }
    }
    private void UpdateCraftingProgress()
    {
        if (structureAssembler != null)
        {
            craftingProgressBar.fillAmount = structureAssembler.GetCraftingProgressNormalized();
        }
        else
        {
            craftingProgressBar.fillAmount = 0f;
        }
    }

    private void SetupRecipes()
    {
        Transform recipeContainer = transform.Find("RecipeContainer");
        Transform recipeTemplate = recipeContainer.Find("Template");
        recipeTemplate.gameObject.SetActive(false);

        // Destory old transforms
        foreach (Transform transform in recipeContainer)
        {
            if (transform != recipeTemplate)
            {
                Destroy(transform.gameObject);
            }
        }

        recipeButtonDic = new Dictionary<PlacedObjectTypeSO, Transform>();

        // Build transforms
        for (int i = 0; i < itemRecipeScriptableObjectList.Count; i++)
        {
            PlacedObjectTypeSO itemRecipeScriptableObject = itemRecipeScriptableObjectList[i];
            Transform recipeTransform = Instantiate(recipeTemplate, recipeContainer);
            recipeTransform.gameObject.SetActive(true);

            recipeButtonDic[itemRecipeScriptableObject] = recipeTransform;

            recipeTransform.Find("Icon").GetComponent<Image>().sprite = itemRecipeScriptableObject.icon;
            //recipeTransform.Find("Text").GetComponent<TextMeshProUGUI>().text = itemRecipeScriptableObject.name;

            recipeTransform.GetComponent<Button_UI>().ClickFunc = () =>
            {
                if (structureAssembler != null)
                {
                    structureAssembler.SetItemRecipeScriptableObject(itemRecipeScriptableObject);
                    UpdateSelectedRecipe();
                }
            };
        }

        UpdateSelectedRecipe();
    }

    private void UpdateSelectedRecipe()
    {
        foreach (PlacedObjectTypeSO itemRecipeScriptableObject in recipeButtonDic.Keys)
        {
            if (structureAssembler != null && structureAssembler.GetItemRecipeSO() == itemRecipeScriptableObject)
            {
                // This one is selected
                recipeButtonDic[itemRecipeScriptableObject].Find("Selected").gameObject.SetActive(true);
            }
            else
            {
                // Not selected
                recipeButtonDic[itemRecipeScriptableObject].Find("Selected").gameObject.SetActive(false);
            }
        }

        UpdateInputs();
    }

    private void UpdateInputs()
    {
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

        if (structureAssembler != null && structureAssembler.HasItemRecipe())
        {
            PlacedObjectTypeSO itemRecipeScriptableObject = structureAssembler.GetItemRecipeSO();

            foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeScriptableObject.inputItemList)
            {
                Transform inputTransform = Instantiate(inputsTemplate, inputsContainer);
                inputTransform.gameObject.SetActive(true);

                inputTransform.Find("Icon").GetComponent<Image>().sprite = recipeItem.item.sprite;
                inputTransform.Find("Text").GetComponent<TextMeshProUGUI>().text = structureAssembler.GetItemStoredCount(recipeItem.item).ToString();
            }
        }
    }

    private void Assembler_OnItemStorageCountChanged(object sender, System.EventArgs e)
    {
        UpdateInputs();
    }

    public void Show(StructureAssembler structureAssembler)
    {
        gameObject.SetActive(true);

        if (this.structureAssembler != null)
        {
            // Unsub from previous Assembler
            this.structureAssembler.OnItemStorageCountChanged -= Assembler_OnItemStorageCountChanged;
        }

        this.structureAssembler = structureAssembler;

        if (structureAssembler != null)
        {
            // Sub for item changes
            structureAssembler.OnItemStorageCountChanged += Assembler_OnItemStorageCountChanged;
        }
        SetupRecipes();
        UpdateSelectedRecipe();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
