using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CodeMonkey.Utils;

public class MiningMachineUI : MonoBehaviour
{

    public static MiningMachineUI Instance { get; private set; }

    [SerializeField] private List<ItemSO> itemScriptableObjectList;
    private Dictionary<ItemSO, Transform> recipeButtonDic;
    private MiningMachine miningMachine;
    private TextMeshProUGUI miningItemText;
    private Image craftingProgressBar;

    private void Awake()
    {
        Instance = this;

        transform.Find("CloseBtn").GetComponent<Button_UI>().ClickFunc = () =>
        {
            Hide();
        };

        miningItemText = transform.Find("MiningItem").Find("Text").GetComponent<TextMeshProUGUI>();
        craftingProgressBar = transform.Find("CraftingProgressBar").Find("Bar").GetComponent<Image>();
        craftingProgressBar.fillAmount = 0f;

        SetupRecipes();

        Hide();
    }

    private void Update()
    {
        UpdateCraftingProgress();
    }

    private void UpdateCraftingProgress()
    {
        if (miningMachine != null)
        {
            craftingProgressBar.fillAmount = miningMachine.GetCraftingProgressNormalized();
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

        recipeButtonDic = new Dictionary<ItemSO, Transform>();

        // Build transforms
        for (int i = 0; i < itemScriptableObjectList.Count; i++)
        {
            ItemSO itemScriptableObject = itemScriptableObjectList[i];
            Transform recipeTransform = Instantiate(recipeTemplate, recipeContainer);
            recipeTransform.gameObject.SetActive(true);

            recipeButtonDic[itemScriptableObject] = recipeTransform;

            recipeTransform.Find("Icon").GetComponent<Image>().sprite = itemScriptableObject.sprite;
            //recipeTransform.Find("Text").GetComponent<TextMeshProUGUI>().text = itemRecipeScriptableObject.name;

            recipeTransform.GetComponent<Button_UI>().ClickFunc = () =>
            {
                if (miningMachine != null)
                {
                    miningMachine.SetMiningResourceItem(itemScriptableObject);
                    UpdateSelectedRecipe();
                }
            };
        }

        UpdateSelectedRecipe();
    }
    private void UpdateSelectedRecipe()
    {
        int index = 0;
        foreach (ItemSO itemScriptableObject in recipeButtonDic.Keys)
        {
            if (miningMachine != null && miningMachine.GetMiningResourceItem() == itemScriptableObject)
            {

                // This one is selected
                recipeButtonDic[itemScriptableObject].Find("Selected").gameObject.SetActive(true);
            }
            else
            {
                // Not selected
                recipeButtonDic[itemScriptableObject].Find("Selected").gameObject.SetActive(false);
            }
        }
        UpdateOutputs();
    }
    private void UpdateOutputs()
    {
        Transform outputsContainer = transform.Find("OutputsContainer");
        Transform outputsTemplate = outputsContainer.Find("Template");
        outputsTemplate.gameObject.SetActive(false);

        // Destory old transforms
        foreach (Transform transform in outputsContainer)
        {
            if (transform != outputsTemplate)
            {
                Destroy(transform.gameObject);
            }
        }

        if (miningMachine != null && miningMachine.GetMiningResourceItem() != null)
        {
            ItemSO itemScriptableObject = miningMachine.GetMiningResourceItem();

            Transform outputTransform = Instantiate(outputsTemplate, outputsContainer);
            outputTransform.gameObject.SetActive(true);

            outputTransform.Find("Icon").GetComponent<Image>().sprite = itemScriptableObject.sprite;
            outputTransform.Find("Text").GetComponent<TextMeshProUGUI>().text = miningMachine.GetItemStoredCount(GameAssets.i.itemSO_Refs.any).ToString();
        }
    }

    private void MiningMachine_OnItemStorageCountChanged(object sender, System.EventArgs e)
    {
        UpdateText();
    }

    private void UpdateText()
    {
        if (miningMachine.GetMiningResourceItem() != null)
        {
            miningItemText.text = miningMachine.GetMiningResourceItem().itemName;
        }
    }


    public void Show(MiningMachine miningMachine)
    {
        gameObject.SetActive(true);

        if (this.miningMachine != null)
        {
            this.miningMachine.OnItemStorageCountChanged -= MiningMachine_OnItemStorageCountChanged;
        }

        this.miningMachine = miningMachine;

        if (miningMachine != null)
        {
            //transform.Find("MiningItem").Find("Icon").GetComponent<Image>().sprite = miningMachine.GetMiningResourceItem()?.sprite;

            miningMachine.OnItemStorageCountChanged += MiningMachine_OnItemStorageCountChanged;
        }

        UpdateText();
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        if (this.miningMachine != null)
        {
            this.miningMachine.OnItemStorageCountChanged -= MiningMachine_OnItemStorageCountChanged;
        }

        miningMachine = null;
    }

}
