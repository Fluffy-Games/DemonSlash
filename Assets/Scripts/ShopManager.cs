using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoSingleton<ShopManager>
{
    [SerializeField] private List<GameObject> swordObjects;
    [SerializeField] private List<GameObject> swordUi;
    [SerializeField] private List<GameObject> winSwordUi;

    private int _preUnlockIndex;
    private int _unlockIndex;
    private int _swordIndex;
    private void Start()
    {
        _preUnlockIndex = PlayerPrefs.GetInt("preUnlockIndex", 0);
        _swordIndex = PlayerPrefs.GetInt("swordIndex", 0);
        _unlockIndex = PlayerPrefs.GetInt("unlockIndex", 1);
        UnlockItem();
        EquipItem(_swordIndex);
    }

    public void IncreasePreUnlock()
    {
        int value = _preUnlockIndex;
        _preUnlockIndex += 20;
        PlayerPrefs.SetInt("preUnlockIndex", _preUnlockIndex);
        TextMeshProUGUI text = winSwordUi[_unlockIndex + 1].transform.GetChild(3).gameObject
            .GetComponent<TextMeshProUGUI>();
        Image image = winSwordUi[_unlockIndex + 1].transform.GetChild(2).gameObject
            .GetComponent<Image>();
        StartCoroutine(IncreasePreUnlockRout(text, image, value));
    }

    private IEnumerator IncreasePreUnlockRout(TextMeshProUGUI text, Image image, int textValue)
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i <= 20; i++)
        {
            text.text = (textValue + i) + "/100";
            image.fillAmount = ((float)textValue + i)/ 100;
            yield return new WaitForSeconds(0.025f);
        }
    }

    public void CheckPreUnlock()
    {
        if (_preUnlockIndex >= 100)
        {
            _unlockIndex++;
            PlayerPrefs.SetInt("unlockIndex", _unlockIndex);
            _preUnlockIndex = 0;
            PlayerPrefs.SetInt("preUnlockIndex", _preUnlockIndex);
            UnlockItem();
        }

        foreach (var item in winSwordUi)
        {
            item.SetActive(false);
        }
        winSwordUi[_unlockIndex + 1].SetActive(true);
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
            swordUi[i].transform.GetChild(6).gameObject.SetActive(false);
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
            item.transform.GetChild(4).gameObject.SetActive(true);
        }
        swordUi[_swordIndex].transform.GetChild(4).gameObject.SetActive(false);
        UpdateSwords();
    }
}
