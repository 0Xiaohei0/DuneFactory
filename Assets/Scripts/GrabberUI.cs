using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using TMPro;

public class GrabberUI : MonoBehaviour
{

    public static GrabberUI Instance { get; private set; }



    [SerializeField] private List<ItemSO> itemSOList;
    [SerializeField] private List<int> rangeList;

    private Dictionary<ItemSO, Transform> filterButtonDic;
    private Dictionary<int, Transform> rangeButtonDic;
    private Grabber grabber;


    private void Awake()
    {
        Instance = this;

        transform.Find("CloseBtn").GetComponent<Button_UI>().ClickFunc = () =>
        {
            Hide();
        };

        SetupFilter();
        rangeList = new List<int>() { 1, 2, 3 };
        SetupRange();

        Hide();
    }
    private void SetupRange()
    {
        Transform rangeContainer = transform.Find("RangeContainer");
        Transform rangeTemplate = rangeContainer.Find("Template");
        rangeTemplate.gameObject.SetActive(false);

        // Destory old transforms
        foreach (Transform transform in rangeContainer)
        {
            if (transform != rangeTemplate)
            {
                Destroy(transform.gameObject);
            }
        }

        rangeButtonDic = new Dictionary<int, Transform>();

        // Build transforms
        for (int i = 0; i < rangeList.Count; i++)
        {
            int range = rangeList[i];
            Transform rangeTransform = Instantiate(rangeTemplate, rangeContainer);
            rangeTransform.gameObject.SetActive(true);

            rangeButtonDic[range] = rangeTransform;

            rangeTransform.Find("Text").GetComponent<TMP_Text>().text = range.ToString();

            rangeTransform.GetComponent<Button_UI>().ClickFunc = () =>
            {
                if (grabber != null)
                {
                    grabber.SetRange(range);
                    UpdateRange();
                }
            };
        }

        UpdateRange();
    }

    private void SetupFilter()
    {
        Transform filterContainer = transform.Find("FilterContainer");
        Transform filterTemplate = filterContainer.Find("Template");
        filterTemplate.gameObject.SetActive(false);

        // Destory old transforms
        foreach (Transform transform in filterContainer)
        {
            if (transform != filterTemplate)
            {
                Destroy(transform.gameObject);
            }
        }

        filterButtonDic = new Dictionary<ItemSO, Transform>();

        // Build transforms
        for (int i = 0; i < itemSOList.Count; i++)
        {
            ItemSO itemSO = itemSOList[i];
            Transform filterTransform = Instantiate(filterTemplate, filterContainer);
            filterTransform.gameObject.SetActive(true);

            filterButtonDic[itemSO] = filterTransform;

            filterTransform.Find("Icon").GetComponent<Image>().sprite = itemSO.sprite;

            filterTransform.GetComponent<Button_UI>().ClickFunc = () =>
            {
                if (grabber != null)
                {
                    grabber.SetGrabFilterItemSO(itemSO);
                    UpdateFilter();
                }
            };
        }

        UpdateFilter();
    }

    private void UpdateFilter()
    {
        foreach (ItemSO itemSO in filterButtonDic.Keys)
        {
            if (grabber != null && grabber.GetGrabFilterItemSO() == itemSO)
            {
                // This one is selected
                filterButtonDic[itemSO].Find("Selected").gameObject.SetActive(true);
            }
            else
            {
                // Not selected
                filterButtonDic[itemSO].Find("Selected").gameObject.SetActive(false);
            }
        }
    }

    private void UpdateRange()
    {
        foreach (int range in rangeButtonDic.Keys)
        {
            if (grabber != null && grabber.getRange() == range)
            {
                // This one is selected
                rangeButtonDic[range].Find("Selected").gameObject.SetActive(true);
            }
            else
            {
                // Not selected
                rangeButtonDic[range].Find("Selected").gameObject.SetActive(false);
            }
        }
    }

    public void Show(Grabber grabber)
    {
        gameObject.SetActive(true);

        this.grabber = grabber;

        UpdateFilter();
        UpdateRange();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


}
