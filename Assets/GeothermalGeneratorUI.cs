using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeothermalGeneratorUI : MonoBehaviour
{
    public static GeothermalGeneratorUI Instance { get; private set; }



    [SerializeField] private List<ItemRecipeSO> itemRecipeScriptableObjectList;

    private Dictionary<ItemSO, Transform> recipeButtonDic;
    private GeothermalGenerator geothermalGenerator;
    private Image craftingProgressBar;

    private void Awake()
    {
        Instance = this;

        transform.Find("CloseBtn").GetComponent<Button_UI>().ClickFunc = () =>
        {
            Hide();
        };

        craftingProgressBar = transform.Find("CraftingProgressBar").Find("Bar").GetComponent<Image>();
        craftingProgressBar.fillAmount = 0f;
        Hide();
    }

    private void Update()
    {
        UpdateCraftingProgress();
    }

    private void UpdateCraftingProgress()
    {
        if (geothermalGenerator != null)
        {
            craftingProgressBar.fillAmount = geothermalGenerator.GetCraftingProgressNormalized();
        }
        else
        {
            craftingProgressBar.fillAmount = 0f;
        }
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

        if (geothermalGenerator != null && geothermalGenerator.HasItemRecipe())
        {
            ItemRecipeSO itemRecipeScriptableObject = geothermalGenerator.GetItemRecipeSO();

            foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeScriptableObject.inputItemList)
            {
                Transform inputTransform = Instantiate(inputsTemplate, inputsContainer);
                inputTransform.gameObject.SetActive(true);

                inputTransform.Find("Icon").GetComponent<Image>().sprite = recipeItem.item.sprite;
                inputTransform.Find("Text").GetComponent<TextMeshProUGUI>().text = geothermalGenerator.GetItemStoredCount(recipeItem.item).ToString();
            }
        }
    }

    private void Assembler_OnItemStorageCountChanged(object sender, System.EventArgs e)
    {
        UpdateInputs();
    }

    public void Show(GeothermalGenerator geothermalGenerator)
    {
        gameObject.SetActive(true);

        if (this.geothermalGenerator != null)
        {
            // Unsub from previous Assembler
            this.geothermalGenerator.OnItemStorageCountChanged -= Assembler_OnItemStorageCountChanged;
        }

        this.geothermalGenerator = geothermalGenerator;

        if (geothermalGenerator != null)
        {
            // Sub for item changes
            geothermalGenerator.OnItemStorageCountChanged += Assembler_OnItemStorageCountChanged;
        }
        UpdateInputs();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
