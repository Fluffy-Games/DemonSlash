using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> swordObjects;
    [SerializeField] private List<GameObject> swordUi;

    private int _unlockIndex;
    private int _swordIndex;
    private void Start()
    {
        _swordIndex = PlayerPrefs.GetInt("swordIndex", 0);
        _unlockIndex = PlayerPrefs.GetInt("unlockIndex", 1);
        UnlockItem();
    }
    
    private void UpdateSwords()
    {
        foreach (var item in swordObjects)
        {
            item.SetActive(false);
        }
        swordObjects[_swordIndex].SetActive(true);
    }

    private void UnlockItem()
    {
        for (int i = 0; i <= _unlockIndex; i++)
        {
            swordUi[i].transform.GetChild(5).gameObject.SetActive(false);
        }
    }
    public void BuyItem(int index)
    {
        int diamond = PlayerController.Instance.Diamond;
        if (swordUi[index].GetComponent<SwordItem>().itemCost <= diamond)
        {
            diamond -= swordUi[index].GetComponent<SwordItem>().itemCost;
            PlayerController.Instance.UpdateTotalDiamond(diamond);
            swordUi[index].GetComponent<SwordItem>().BoughtItem(1);
        }
    }
    public void EquipItem(int index)
    {
        _swordIndex = index;
        PlayerPrefs.SetInt("swordIndex", _swordIndex);
        foreach (var item in swordUi)
        {
            item.transform.GetChild(3).gameObject.SetActive(true);
        }
        swordUi[_swordIndex].transform.GetChild(3).gameObject.SetActive(false);
        UpdateSwords();
    }
}
