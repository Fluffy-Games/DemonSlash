using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("Panels")]
    [SerializeField] public GameObject introPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] public GameObject retryPanel;
    [SerializeField] private GameObject nextLevelPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject gemObject;
    [SerializeField] private Image fadePanel;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI demonText;
    [SerializeField] private TextMeshProUGUI demonTextEnd;
    [SerializeField] private TextMeshProUGUI diamondText;
    [SerializeField] private TextMeshProUGUI diamondTextEnd;
    [SerializeField] private TextMeshProUGUI levelTextIntro;
    [SerializeField] private TextMeshProUGUI levelTextGame;
    [SerializeField] private TextMeshProUGUI totalDiamondTextGame;
    [SerializeField] private List<TextMeshProUGUI> introLevelTexts;

    [Header("AnimatedGem")] 
    [SerializeField] private GameObject animatedGem;
    [SerializeField] private Transform targetPosition;
    [SerializeField] [Range(0.5f, 1.5f)] float minAnimDuration;
    [SerializeField] [Range(1.5f, 2.1f)] float maxAnimDuration;
    [SerializeField] float spread;
    [SerializeField] Ease easeType;

    private int _gemValue = 40;
    private Queue<GameObject> _coinsQueue = new Queue<GameObject>();
    
    [SerializeField] private Slider powerBar;

    private float _demonFontSize = 70f;
    private float _diamondFontSize = 70f;
    private float _diamondTotalFontSize = 50f;

    private void Start()
    {
        
    }

    public void StartLevel()
    {
        introPanel.GetComponent<Button>().interactable = false;
        introPanel.SetActive(false);
        gemObject.SetActive(false);
        gamePanel.SetActive(true);
        LevelTextUpdate(levelTextGame);
        PrepareCoins();
    }
    public void LevelProgress(float value)
    {
        //progressBar.fillAmount = value;
    }
    public void IntroPanel()
    {
        introPanel.SetActive(true);
        gamePanel.SetActive(false);
        LevelTextUpdate(levelTextIntro);
    }
    public void UpdateIntroLevelTexts()
    {
        introLevelTexts[0].text = (LevelManager.Instance.Index-2).ToString();
        introLevelTexts[1].text = (LevelManager.Instance.Index - 1).ToString();
        introLevelTexts[2].text = (LevelManager.Instance.Index ).ToString();
        introLevelTexts[3].text = (LevelManager.Instance.Index + 1).ToString();
        introLevelTexts[4].text = (LevelManager.Instance.Index + 2).ToString();

        if (LevelManager.Instance.Index <= 2)
        {
            if (LevelManager.Instance.Index == 1)
            {
                introLevelTexts[0].text = "";
                introLevelTexts[1].text = "";
                introLevelTexts[2].text = (LevelManager.Instance.Index + 0).ToString();
                introLevelTexts[3].text = (LevelManager.Instance.Index + 1).ToString();
                introLevelTexts[4].text = (LevelManager.Instance.Index + 2).ToString();
            }

            if (LevelManager.Instance.Index == 2)
            {
                introLevelTexts[0].text = "";
                introLevelTexts[1].text = (LevelManager.Instance.Index -1).ToString();
                introLevelTexts[2].text = (LevelManager.Instance.Index + 0).ToString();
                introLevelTexts[3].text = (LevelManager.Instance.Index + 1).ToString();
                introLevelTexts[4].text = (LevelManager.Instance.Index + 2).ToString();
            }
        }
    }

    public void RetryPanel()
    {
        retryPanel.SetActive(true);
        gamePanel.SetActive(false);
        introPanel.GetComponent<Button>().interactable = true;
    }
    public void WinPanel()
    {
        nextLevelPanel.SetActive(true);
        gamePanel.SetActive(false);
        introPanel.GetComponent<Button>().interactable = true;
    }
    
    public IEnumerator LevelLoadRoutine(int index, bool nextLevel)
    {
        if (nextLevel)
        {
            yield return new WaitForSeconds(2f);
        }
        nextLevelPanel.SetActive(false);
        gamePanel.SetActive(false);
        PlayerController.Instance.CloseConfetti();
        StartCoroutine(FadePanelRout(0, 1));
        yield return new WaitForSeconds(1f);
        StartCoroutine(FadePanelRout(1, 0));
        LevelManager.Instance.ManageLevel(index);
        GameManager.Instance.CurrentGameState = GameManager.GameState.Prepare;
        PlayerController.Instance.ResetModelPos();
        IntroPanel();
        powerBar.value = 0;
        introPanel.GetComponent<Button>().enabled = true;
        yield return new WaitForSeconds(1f);
    }

    public void DemonSlashCountUpdate(int demonSlashCount)
    {
        demonText.text = $"{demonSlashCount}";
        demonTextEnd.text = "+" + $"{demonSlashCount}";
        StartCoroutine(CustomPunchScale(demonText, _demonFontSize));
    }

    public void LevelTextUpdate(TextMeshProUGUI text)
    {
        text.text = $"{"LEVEL "+LevelManager.Instance.Index}";
    }

    public void PowerBarUpdate(float value)
    {
        powerBar.value += value;
    }
    public void DiamondCountUpdate(int diamondCount)
    {
        diamondText.text = $"{diamondCount}";
        diamondTextEnd.text = "+" + $"{diamondCount}";
        StartCoroutine(CustomPunchScale(diamondText, _diamondFontSize));
    }

    private IEnumerator CustomPunchScale(TextMeshProUGUI text, float origin)
    {
        float target = origin * 1.75f;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime * 7f;
            text.fontSize = Mathf.Lerp(origin, target, timer);
            yield return null;
            if (timer >= 1f)
            {
                timer = 0f;
                while (true)
                {
                    timer += Time.deltaTime * 7f;
                    text.fontSize = Mathf.Lerp(target, origin, timer);
                    yield return null;
                    if (timer >= 1f)
                    {
                        break;
                    }
                }
                break;
            }
            
        }
    }

    private IEnumerator FadePanelRout(float a, float b)
    {
        fadePanel.gameObject.SetActive(true);
        Color color = fadePanel.color;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(a, b, timer);
            fadePanel.color = color;
            yield return null;
            if (timer >= 1f)
            {
                break;
            }
        }
        fadePanel.gameObject.SetActive(false);
    }

    public void TotalDiamond(int diamond)
    {
        totalDiamondTextGame.text = $"{diamond}";
        StartCoroutine(CustomPunchScale(totalDiamondTextGame, _diamondTotalFontSize));
    }
    
    
    void PrepareCoins()
    {
        GameObject gem;
        for (int i = 0; i < _gemValue; i++)
        {
            gem = Instantiate(animatedGem, nextLevelPanel.transform, true);
            gem.SetActive(false);
            _coinsQueue.Enqueue(gem);
        }
    }
    public void GemAnimate(Transform collectedCoinPosition)
    {
        bool first = false;
        gemObject.SetActive(true);
        for (int i = 0; i < _gemValue; i++)
        {
            //check if there's coins in the pool
            if (_coinsQueue.Count > 0)
            {
                //extract a coin from the pool
                GameObject coin = _coinsQueue.Dequeue();
                coin.SetActive(true);

                //move coin to the collected coin pos
                coin.transform.position = collectedCoinPosition.position + new Vector3(Random.Range(-spread, spread), 0f, 0f);

                //animate coin to target position
                float duration = Random.Range(minAnimDuration, maxAnimDuration);
                coin.transform.DOMove(targetPosition.position, duration)
                    .SetEase(easeType)
                    .OnComplete(() => {
                        //executes whenever coin reach target position
                        coin.SetActive(false);
                        _coinsQueue.Enqueue(coin);
                        if (!first)
                        {
                            first = true;
                            PlayerController.Instance.SumDiamond();
                        }
                    });
            }
        }
    }

    public void ShopPanelOpen(bool value)
    {
        shopPanel.SetActive(value);
    }
}
