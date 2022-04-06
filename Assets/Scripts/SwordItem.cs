using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordItem : MonoBehaviour
{
    [SerializeField] private int itemIndex;
    private int _defaultValue;
    public int itemCost;
    
    private int _bought;
    
    private void OnEnable()
    {
        if (itemIndex == 0)
        {
            _defaultValue = 1;
        }
        
        _bought = PlayerPrefs.GetInt("bought" + itemIndex, _defaultValue);
        
        UpdateUI();
    }
    
    public void BoughtItem(int value)
    {
        _bought = value;
        PlayerPrefs.SetInt("bought" + itemIndex, _bought);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_bought == 1)
        {
            transform.GetChild(5).gameObject.SetActive(false);
        }
    }
}
