using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    /* public BuildingCategorySO[] categories; // Your array of categories
     public GameObject categoryPrefab; // Your UI prefab for categories
     public GameObject buildingPrefab; // Your UI prefab for buildings
     public Transform buildingUIParent; // The parent transform in your UI where the elements should be instantiated
     public Transform categoryUIParent; // The parent transform in your UI where the elements should be instantiated

     void Start()
     {
         CreateUIElements();
     }
     void CreateCategoryUIElements()
     {
         foreach (var category in categories)
         {
             // Instantiate the category prefab
             GameObject categoryElement = Instantiate(categoryPrefab, categoryUIParent);

             // Parent for buildings under this category, could be a panel or a designated container
             Transform buildingsParent = categoryElement.transform.Find("BuildingsContainer");

             // Now instantiate UI elements for each building in this category
             foreach (var placedObjectType in category.placedObjects)
             {
                 GameObject buildingElement = Instantiate(buildingPrefab, buildingsParent);

                 // Assign the name and icon from the ScriptableObject to the building UI element
                 buildingElement.transform.Find("BuildingNameText").GetComponent<Text>().text = placedObjectType.objectName;
                 buildingElement.transform.Find("BuildingIconImage").GetComponent<Image>().sprite = placedObjectType.icon;

                 // Set up button listener or other interactive components...
             }
         }
     }
     void CreateUIElements()
     {
         foreach (var placedObjectType in placedObjectTypes)
         {
             // Instantiate the UI prefab as a child of `uiParent`
             GameObject uiElement = Instantiate(uiPrefab, uiParent);

             // Assign the name and icon from the ScriptableObject to the UI element
             uiElement.transform.Find("NameText").GetComponent<TMP_Text>().text = placedObjectType.nameString;
             uiElement.transform.Find("IconImage").GetComponent<Image>().sprite = placedObjectType.icon;

             // Optionally, you could add a button listener here if you have a button component
             // in your prefab and you want to do something when it's clicked
             Button button = uiElement.GetComponentInChildren<Button>();
             if (button != null)
             {
                 button.onClick.AddListener(() => OnPlacedObjectTypeSelected(placedObjectType));
             }
         }
         uiPrefab.SetActive(false);
     }

     void OnPlacedObjectTypeSelected(PlacedObjectTypeSO selectedType)
     {
         // Handle the selection of a placed object type (e.g., set the current type to be placed)
         Debug.Log("Selected: " + selectedType.name);
     }*/
}

