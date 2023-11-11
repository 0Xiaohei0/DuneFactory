using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public BuildingCategorySO[] categories; // Your array of categories
    public GameObject buildingPrefab; // Your UI prefab for buildings
    public GameObject categoryPrefab; // Your UI prefab for buildings
    public GameObject categoriesParent; // The parent transform for category buttons
    public GameObject buildingsParent; // The parent transform for building UI elements

    private BuildingCategorySO activeCategory = null;

    // Start is assumed to set up category buttons, each with an appropriate listener
    void Start()
    {
        SetupCategoryButtons();
        buildingsParent.SetActive(false);
        GlobalStorage.OnBuildingAmountChanged += UpdateBuildingAmount;
    }

    void SetupCategoryButtons()
    {
        // Clear existing categories
        foreach (Transform child in categoriesParent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var category in categories)
        {
            // Assume you have a method to create buttons for each category
            GameObject categoryElement = Instantiate(categoryPrefab, categoriesParent.transform);
            categoryElement.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => OnCategorySelected(category));
            categoryElement.transform.Find("Button").GetComponent<Image>().sprite = category.icon;
        }
    }

    void OnCategorySelected(BuildingCategorySO category)
    {
        if (activeCategory == category)
        {
            // Toggle the visibility of the buildings panel
            buildingsParent.SetActive(!buildingsParent.activeSelf);
            if (!buildingsParent.activeSelf)
            {
                GridBuildingSystem.Instance.SetPlacedObjectTypeSO(null);
            }
        }
        else
        {
            buildingsParent.SetActive(true); // Make sure the panel is open
            activeCategory = category; // Set the new active category
            PopulateBuildings(category); // Populate with new category's buildings
        }
    }

    void OnBuildingSelected(PlacedObjectTypeSO selectedType)
    {
        // Handle the selection of a placed object type (e.g., set the current type to be placed)
        EventManager.BuildingSelected(selectedType);

    }

    void PopulateBuildings(BuildingCategorySO category)
    {
        // Clear existing buildings
        foreach (Transform child in buildingsParent.transform)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new building UI elements
        foreach (PlacedObjectTypeSO building in category.placedObjects)
        {
            GameObject buildingElement = Instantiate(buildingPrefab, buildingsParent.transform);
            buildingElement.transform.Find("NameText").GetComponent<TMP_Text>().text = building.nameString;
            buildingElement.transform.Find("IconImage").GetComponent<Image>().sprite = building.icon;
            buildingElement.transform.Find("AmountText").GetComponent<TMP_Text>().text = "x" + GlobalStorage.GetBuildingCount(building);
            buildingElement.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnBuildingSelected(building);
            });
            // Optionally add button listeners to these building elements...
        }
    }
    void UpdateBuildingAmount(object sender, System.EventArgs e)
    {
        if (activeCategory == null) return;
        PopulateBuildings(activeCategory);
    }
}
