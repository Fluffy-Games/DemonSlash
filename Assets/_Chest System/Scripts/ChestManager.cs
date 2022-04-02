using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChestManager : MonoBehaviour
{
    public GameObject animatedGemPrefab;
    public GameObject ChestBlackPanelFadeOut; // for Fade out in chest room
    public GameObject ChestBlackPanelFadeIn;// for Fade In in chest room
    public GameObject GetKeyBtn;
    public GameObject nextBtn;
    public GameObject KeysBG;
    public GameObject noThanks;
    public GameObject[] SecurePanel;// SecurePanel of all chests present in chest room
    public GameObject[] GemImages;
    Queue<GameObject> coinsQueue = new Queue<GameObject>();

    [Space]
    public Transform target;
    private Transform targetPosition;

    [Space]
    [Header("Max coin to Instantiate")]

    public int maxPoolCoins = 30;

    [Space]
    [Header("For Random coins amount")]
    [Header("Make sure that you max and min coins value is not greater than maxPoolCoins")]

    [Range(0, 20)]
    public int minCoins;

    [Range(0, 20)]
    public int maxCoins;
    int index;


    [Space]
    [Header("Animation settings")]

    [SerializeField] [Range(0.5f, 0.9f)] float minAnimDuration;
    [SerializeField] [Range(0.9f, 2f)] float maxAnimDuration;
    [SerializeField] float spread;

    [Space]
    [SerializeField] Ease easeType;

    [Space]
    public TextMeshProUGUI gemTxt;

    [Space]
    public Sprite KeysSprite_Fill;
    public Sprite KeysSprite_Empty;

    [Space]
    public bool AdIsLoaded = true;
    private bool turnOffGetKeyBtn;

    [Space]
    public Animator[] ChestsAnimator;// animator of all chests present in chest room

    [Space]
    public Image[] keys;

    [Space]
    public TextMeshProUGUI[] texts;

    [Space]
    public string Keys_ = "Keys";
    public string Gems_ = "Gems";
    public string loadScene;

    private void Awake()
    {
        targetPosition = target;

        //prepare pool
        PrepareCoins();
    }



    private void Start() {
        gemTxt.text = PlayerPrefs.GetInt(Gems_, 0).ToString();
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].gameObject.SetActive(false);
            GemImages[i].SetActive(false);
            SecurePanel[i].SetActive(false);
        }
        OpenChestPanel();
        CheckForKeys();
        Randomtext();
    }

    void PrepareCoins()
    {
        GameObject gem;
        for (int i = 0; i < maxPoolCoins; i++)
        {
            gem = Instantiate(animatedGemPrefab);
            //coin.transform.parent = transform;
            gem.SetActive(false);
            coinsQueue.Enqueue(gem);
        }
    }

    void Animate(Vector3 collectedCoinPosition, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            //check if there's coins in the pool
            if (coinsQueue.Count > 0)
            {
                //extract a coin from the pool
                GameObject coin = coinsQueue.Dequeue();
                coin.SetActive(true);

                //move coin to the collected coin pos
                coin.transform.position = collectedCoinPosition + new Vector3(Random.Range(-spread, spread), 0f, 0f);

                //animate coin to target position
                float duration = Random.Range(minAnimDuration, maxAnimDuration);
                coin.transform.DOMove(targetPosition.position, duration)
                .SetEase(easeType)
                .OnComplete(() => {
                    //executes whenever coin reach target position
                    coin.SetActive(false);
                    coinsQueue.Enqueue(coin);

                    PlayerPrefs.SetInt(Gems_,PlayerPrefs.GetInt(Gems_, 0) + 1);
                    gemTxt.text = PlayerPrefs.GetInt(Gems_, 0).ToString();
                });
            }
        }
    }



    public void CheckForBtnOnorOff()
    {
        if(PlayerPrefs.GetInt(Keys_, 0) >= 1)
        {
            GetKeyBtn.SetActive(false);
            KeysBG.SetActive(true);
            noThanks.SetActive(false);
            nextBtn.SetActive(false);
            return;
        }
        else
        {
            if(AdIsLoaded /*replace AdIsLoaded variable to check your ad is loaded or not*/ && !turnOffGetKeyBtn)
            {
                GetKeyBtn.SetActive(true);
                KeysBG.SetActive(false);
                StartCoroutine(TurnOffOn_noThanksBtn(4,true));
                nextBtn.SetActive(false);
                return;
            }
            else
            {
                GetKeyBtn.SetActive(false);
                KeysBG.SetActive(false);
                noThanks.SetActive(false);
                nextBtn.SetActive(true);
                return;
            }
        }
    }

    IEnumerator TurnOffOn_noThanksBtn(int duration,bool TurnOffOn)
    {
        yield return new WaitForSeconds(duration);
        if(!KeysBG.activeInHierarchy)
            noThanks.SetActive(TurnOffOn);
        
    }

    //Update the image of keys in scene
    public void CheckForKeys()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            if(PlayerPrefs.GetInt(Keys_, 0) >= 1)
            {
                keys[i].sprite = KeysSprite_Fill;
            }
            else if(i ==0)
            {
                keys[i].sprite = KeysSprite_Empty;

            }

            if(PlayerPrefs.GetInt(Keys_, 0) >= 2 )
            {
                keys[i].sprite = KeysSprite_Fill;
            }
            else if(i == 1)
            {
                keys[i].sprite = KeysSprite_Empty;

            }

            if(PlayerPrefs.GetInt(Keys_, 0) == 3 )
            {
                keys[i].sprite = KeysSprite_Fill;
            }
            else if(i == 2)
            {
                keys[i].sprite = KeysSprite_Empty;

            }
        }
    }

    //open  chests based upon there index
    public void OpenChest(int index) 
    {
        
        if(PlayerPrefs.GetInt(Keys_, 0) >= 1 && !SecurePanel[index].activeInHierarchy)
        {
            
            SoundManager.Instance.PlaySound(SoundManager.Instance.AddKey);
            SoundManager.Instance.PlaySound(SoundManager.Instance.OpenChest);
            SecurePanel[index].SetActive(true);
            PlayerPrefs.SetInt(Keys_, PlayerPrefs.GetInt(Keys_, 0) - 1);
            CheckForBtnOnorOff();
            CheckForKeys();
            ChestsAnimator[index].SetTrigger("Open");

            StartCoroutine(AddGems(index));
        }
        else if(!SecurePanel[index].activeInHierarchy)
        {
            ChestsAnimator[index].SetTrigger("Shake");
            SoundManager.Instance.PlaySound(SoundManager.Instance.chestLocked);
        }
        
    }

    IEnumerator AddGems(int i)
    {
        yield return new WaitForSeconds(1.15f);
        AddCoins(new Vector3(texts[i].rectTransform.position.x, 
            texts[i].rectTransform.position.y, 40), int.Parse(texts[i].text));
    }

    //add coin to PlayerPrefs
    private void AddCoins(Vector3 collectedCoinPosition, int amount)
    {
        Animate(collectedCoinPosition, amount);
    }

    //for random reward in chests
    public void Randomtext()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            int rand = Random.Range(minCoins,maxCoins);
            texts[i].text = rand.ToString();
        }
    }
    public void Get3Keys()
    {
        PlayerPrefs.SetInt(Keys_, 3);
        CheckForBtnOnorOff();
        GetKeyBtn.SetActive(false);
        turnOffGetKeyBtn = true;
        CheckForKeys();
    }

    public void OpenChestPanel()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.OpenChestRoom);
        index = 0;
        turnOffGetKeyBtn = false;
        for (int i = 0; i < ChestsAnimator.Length; i++)
        {
            index = i;
            SecurePanel[i].SetActive(false);
            texts[i].gameObject.SetActive(false);
            GemImages[i].SetActive(false);
        }
        Invoke("WaitForOpeningChest",0.75f);
        ChestBlackPanelFadeOut.SetActive(true);
        ChestBlackPanelFadeIn.SetActive(false);
    }

    void WaitForOpeningChest()
    {

        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].gameObject.SetActive(true);
            GemImages[i].SetActive(true);
        }
    }


    public void CloseChestPanel()
    {
        ChestBlackPanelFadeOut.SetActive(false);
        ChestBlackPanelFadeIn.SetActive(true);
        StartCoroutine(WaitForClose());
    }
    IEnumerator WaitForClose()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(loadScene);
        
    }
}
