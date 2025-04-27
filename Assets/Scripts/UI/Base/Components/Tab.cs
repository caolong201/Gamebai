using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tab : MonoBehaviour
{
    public delegate void OnTabSelect(int index);
    public OnTabSelect onTabSelect;

    private int mTabIndex;
    public int TabIndex => mTabIndex;

    [SerializeField] private GameObject tabActive, tabUnActive;
    [SerializeField] private GameObject content;
    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private Color colTileActive, colTileUnActive;

    public void Init(int index)
    {
        mTabIndex = index;
    }

    public void UpdateTab(bool isSelected)
    {
        if (isSelected)
        {
            tabActive.SetActive(true);
            tabUnActive.SetActive(false);
            content.SetActive(true);
            txtTitle.color = colTileActive;
        }
        else
        {
            tabActive.SetActive(false);
            tabUnActive.SetActive(true);
            content.SetActive(false);
            txtTitle.color = colTileUnActive;

        }
    }


    public void OnTabClicked()
    {
        onTabSelect?.Invoke(mTabIndex);
    }
}
