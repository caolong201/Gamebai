using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabsManager : MonoBehaviour
{
    private List<Tab> lstTab;
    private Tab seleted;
    [SerializeField] private int indexTabDefaultSelected = 0;
    [SerializeField] private Color selectedColor, unSelectColor;

    // Start is called before the first frame update
    void Start()
    {
        lstTab = new List<Tab>(GetComponentsInChildren<Tab>(true));
        seleted = lstTab[indexTabDefaultSelected];

        for (int i = 0; i < lstTab.Count; i++)
        {
            lstTab[i].Init(i);
            lstTab[i].onTabSelect += OnTabSelect;

            if (lstTab[i] == seleted) UpdateTab(lstTab[i], true);
            else UpdateTab(lstTab[i], false);
        }
    }

    //delegate callback
    private void OnTabSelect(int index)
    {
        Debug.Log("OnTabSelect: " + index);
        for (int i = 0; i < lstTab.Count; i++)
        {
            if (i == index)
            {
                seleted = lstTab[i];
                UpdateTab(lstTab[i], true);
            }
            else
            {
                UpdateTab(lstTab[i], false);
            }
        }
    }

    private void UpdateTab(Tab tab, bool isSelected)
    {
        tab.UpdateTab(isSelected);
    }

    public Tab GetTabSelected()
    {
        return seleted;
    }
}
