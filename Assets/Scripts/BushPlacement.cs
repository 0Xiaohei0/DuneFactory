using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushPlacement : MonoBehaviour
{

    [SerializeField] private Transform[] prefabArray;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int amount;
    [SerializeField] float destroyDistance = 1.5f;

    private List<Transform> spawnedTransformList;

    private void Awake()
    {
        spawnedTransformList = new List<Transform>();

        for (int i = 0; i < amount; i++)
        {
            Transform prefab = prefabArray[Random.Range(0, prefabArray.Length)];
            Vector3 spawnPosition = new Vector3(Random.Range(transform.position.x, transform.position.x + width), 0f, Random.Range(transform.position.z, transform.position.z + height));
            Transform spawnedTransform = Instantiate(prefab, spawnPosition, Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0));

            spawnedTransformList.Add(spawnedTransform);
        }
    }

    private void Start()
    {
        GridBuildingSystem.Instance.OnObjectPlaced += Instance_OnObjectPlaced;
    }

    private void Instance_OnObjectPlaced(object sender, System.EventArgs e)
    {
        print(sender);
        PlacedObject placedObject = sender as PlacedObject;

        for (int i = 0; i < spawnedTransformList.Count; i++)
        {
            Transform bushTransform = spawnedTransformList[i];

            if (Vector3.Distance(bushTransform.position, placedObject.transform.position) < destroyDistance)
            {
                // Destroy this Bush
                Destroy(bushTransform.gameObject);
                spawnedTransformList.RemoveAt(i);
                i--;
            }
        }
    }

}
