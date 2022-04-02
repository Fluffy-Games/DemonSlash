using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    int keys;

    int gems;

    [HideInInspector] public string Keys_ = "Keys";

    [HideInInspector] public string Gems_ = "Gems";

    public GameObject OpenChestRoom_Btn;

    public GameObject MainBlackPanelFadeIn;

    public GameObject MainBlackPanelFadeOut;

    public TextMeshProUGUI GemText;

    public Image[] KeysImage;

    public Sprite KeysSprite_Fill;

    public Sprite KeysSprite_Empty;

    public int Keys
    {
        get
        {
            return keys;
        }
        set
        {
            keys = value;
            PlayerPrefs.SetInt(Keys_, keys);
            Debug.Log("Keys: " + keys);
            CheckForKeys();
        }
    }

    public int Gems
    {
        get
        {
            return gems;
        }
        set
        {
            gems = value;
            PlayerPrefs.SetInt(Gems_, gems);
            GemText.text = gems.ToString();
            Debug.Log("Gems: " + gems);
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        Keys = PlayerPrefs.GetInt(Keys_, 0);
        Gems = PlayerPrefs.GetInt(Gems_, 0);
        if (Keys == 3)
            OpenChestRoom_Btn.SetActive(true);
        else
            OpenChestRoom_Btn.SetActive(false);

        MainBlackPanelFadeIn.SetActive(false);
        MainBlackPanelFadeOut.SetActive(true);
        
    }

    public void CheckForKeys()
    {
        if (Keys == 3)
            OpenChestRoom_Btn.SetActive(true);

        else
            OpenChestRoom_Btn.SetActive(false);

        for (int i = 0; i < KeysImage.Length; i++)
        {
            if (PlayerPrefs.GetInt(Keys_, 0) >= 1)
            {
                KeysImage[i].sprite = KeysSprite_Fill;
            }
            else if (i == 0)
            {
                KeysImage[i].sprite = KeysSprite_Empty;
            }
            if (PlayerPrefs.GetInt(Keys_, 0) >= 2)
            {
                KeysImage[i].sprite = KeysSprite_Fill;
            }
            else if (i == 1)
            {
                KeysImage[i].sprite = KeysSprite_Empty;
            }
            if (PlayerPrefs.GetInt(Keys_, 0) == 3)
            {
                KeysImage[i].sprite = KeysSprite_Fill;
            }
            else if (i == 2)
            {
                KeysImage[i].sprite = KeysSprite_Empty;
            }
        }


        
    }

    public void AddKeys()
    {
        if(Keys < 3)
        {
            Debug.Log("One key is added");
            Keys++;
            
            SoundManager.Instance.PlaySound(SoundManager.Instance.KeyCollect);
        }
        
    }

    public void RemoveKeys()
    {
        if(Keys > 0)
        {
            Debug.Log("One key is removed");
            Keys--;
        }
        
    }

    public void OpenChestRoom()
    {
        Debug.Log("Chest Room opened");
        MainBlackPanelFadeIn.SetActive(true);
        MainBlackPanelFadeOut.SetActive(false);
        Invoke("WaitForFadeInAnimation", 1);
    }

    private void WaitForFadeInAnimation()
    {
        SceneManager.LoadScene("Chest");
    }
}
