using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGhost : MonoBehaviour
{
    public static TutorialGhost Instance { get; private set; }
    public List<Transform> visuals;
    public Material ghostMaterial;
    private void Awake()
    {
        Instance = this;
    }
    public void ShowGhost(PlacedObjectTypeSO placedObjectTypeSO, Vector2Int gridPosition, PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down)
    {
        if (placedObjectTypeSO != null)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = GridBuildingSystem.Instance.GetWorldPosition(gridPosition) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * GridBuildingSystem.Instance.grid.GetCellSize();
            Transform visual = Instantiate(placedObjectTypeSO.visual, placedObjectWorldPosition, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));
            visuals.Add(visual);
            SetMaterialRecursive(visual.gameObject);
        }
    }
    private void SetMaterialRecursive(GameObject targetGameObject)
    {
        Renderer renderer = targetGameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = ghostMaterial;
        }
        foreach (Transform child in targetGameObject.transform)
        {
            SetMaterialRecursive(child.gameObject);
        }
    }
    public void HideGhost(Vector2Int gridPosition)
    {
        foreach (Transform child in visuals)
        {
            if (GridBuildingSystem.Instance.GetXZ(child.transform.position) == gridPosition)
            {
                Destroy(child.gameObject);
                visuals.Remove(child);
                break;
            }
        }
    }
    public void HideAllGhosts()
    {
        foreach (Transform child in visuals)
        {
            Destroy(child.gameObject);
        }
        visuals.Clear();
    }
}
