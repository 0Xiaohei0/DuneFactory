using System.Collections;
using UnityEngine;

public class Belt : MonoBehaviour
{
    private static int _beltID = 0;

    public Belt beltInSequence;
    public WorldItem beltItem;
    public bool isSpaceTaken;
    public float bufferDistance = 0.5f; // The minimum distance between items.

    private BeltManager _beltManager;
    private float moveTime;

    private void Start()
    {
        _beltManager = FindObjectOfType<BeltManager>();
        beltInSequence = null;
        beltInSequence = FindNextBelt();
        gameObject.name = $"Belt: {_beltID++}";
        moveTime = 1 / _beltManager.speed; // Assuming _beltManager.speed is a measure of units per second.
    }

    private void Update()
    {
        if (beltInSequence == null)
            beltInSequence = FindNextBelt();

        if (beltItem != null && beltItem.item != null && beltInSequence != null && !beltInSequence.isSpaceTaken)
        {
            beltInSequence.isSpaceTaken = true;
            MoveItemToNextBelt(beltItem, beltInSequence);
            beltItem = null; // This item is now considered moved.
        }
    }

    private void MoveItemToNextBelt(WorldItem item, Belt nextBelt)
    {
        StartCoroutine(MoveItem(item, nextBelt));
    }

    private IEnumerator MoveItem(WorldItem item, Belt targetBelt)
    {
        Vector3 startPosition = item.item.transform.position;
        Vector3 endPosition = targetBelt.GetItemPosition();
        float elapsedTime = 0;

        while (elapsedTime < moveTime)
        {
            item.item.transform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap to the final position to ensure it's exactly in place.
        item.item.transform.position = endPosition;
        targetBelt.beltItem = item;
        isSpaceTaken = false; // This belt is now free.

        Debug.DrawLine(startPosition, endPosition, Color.green, 2f);
    }

    public Vector3 GetItemPosition()
    {
        var padding = 0.3f;
        var position = transform.position;
        return new Vector3(position.x, position.y + padding, position.z);
    }

    private Belt FindNextBelt()
    {
        Transform currentBeltTransform = transform;
        RaycastHit hit;

        var forward = transform.forward;

        Ray ray = new Ray(currentBeltTransform.position, forward);

        if (Physics.Raycast(ray, out hit, 1f))
        {
            Belt belt = hit.collider.GetComponent<Belt>();

            if (belt != null)
                return belt;
        }

        return null;
    }
}
