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
    [SerializeField] private GameObject tapPanel;
    [SerializeField] private Image fadePanel;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI demonText;
    [SerializeField] private TextMeshProUGUI demonTextEnd;
    [SerializeField] private TextMeshProUGUI diamondText;
    [SerializeField] private TextMeshProUGUI diamondTextEnd;
    [SerializeField] private TextMeshProUGUI levelTextIntro;
    [SerializeField] private TextMeshProUGUI levelTextGame;
    [SerializeField] private TextMeshProUGUI totalDiamondTextGame;
    [SerializeField] public TextMeshProUGUI upgradePowerText;
    [SerializeField] private List<TextMeshProUGUI> introLevelTexts;

    [Header("AnimatedGem")] 
    [SerializeField] private GameObject animatedGem;
    [SerializeField] private Transform targetPositionGem;
    [SerializeField] [Range(0.5f, 1.5f)] float minAnimDurationGem;
    [SerializeField] [Range(1.5f, 2.1f)] float maxAnimDurationGem;
    [SerializeField] float spreadGem;
    [SerializeField] Ease easeTypeGem;
    [Header("AnimatedEnergy")] 
    [SerializeField] private GameObject animatedEnergy;
    [SerializeField] private Transform collectedEnergyPosition;
    [SerializeField] private Transform targetPositionEnergy;
    [SerializeField] [Range(1, 2f)] float minAnimDurationEnergy;
    [SerializeField] [Range(2f, 3f)] float maxAnimDurationEnergy;
    [SerializeField] float spreadEnergy;
    [SerializeField] Ease easeTypeEnergy;

    private int _gemValue = 40;
    private int _energyValue = 6;
    private Queue<GameObject> _coinsQueue = new Queue<GameObject>();
    private Queue<GameObject> _energyQueue = new Queue<GameObject>();
    
    [SerializeField] private Slider powerBar;
    public float PowerBarValue => powerBar.value;

    private float _demonFontSize = 70f;
    private float _diamondFontSize = 70f;
    private float _diamondTotalFontSize = 50f;
    
    public void StartLevel()
    {
        introPanel.GetComponent<Button>().interactable = false;
        introPanel.SetActive(false);
        gemObject.SetActive(false);
        gamePanel.SetActive(true);
        LevelTextUpdate(levelTextGame);
        PrepareCoins();
        PrepareEnergy();
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
        if (diamond >= 1000)
        {
            totalDiamondTextGame.text = (diamond / 1000f).ToString("F1") + "k";
        }
        else
        {
            totalDiamondTextGame.text = $"{diamond}";
        }
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
                coin.transform.position = collectedCoinPosition.position + new Vector3(Random.Range(-spreadGem, spreadGem), 0f, 0f);

                //animate coin to target position
                float duration = Random.Range(minAnimDurationGem, maxAnimDurationGem);
                coin.transform.DOMove(targetPositionGem.position, duration)
                    .SetEase(easeTypeGem)
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
    void PrepareEnergy()
    {
        GameObject energy;
        for (int i = 0; i < _energyValue; i++)
        {
            energy = Instantiate(animatedEnergy, gamePanel.transform, true);
            energy.SetActive(false);
            _energyQueue.Enqueue(energy);
        }
    }
    public void EnergyAnimate()
    {
        bool first = false;
        for (int i = 0; i < _energyValue; i++)
        {
            //check if there's coins in the pool
            if (_energyQueue.Count > 0)
            {
                //extract a coin from the pool
                GameObject energy = _energyQueue.Dequeue();
                energy.SetActive(true);

                //move coin to the collected coin pos
                energy.transform.position = collectedEnergyPosition.position + new Vector3(Random.Range(-spreadEnergy, spreadEnergy), Random.Range(-spreadEnergy / 2, spreadEnergy / 2), 0f) +
                                            new Vector3((powerBar.value - 0.5f) * Screen.width * 0.7f, 0, 0);
                Vector3 tempVec = targetPositionEnergy.position +
                                  new Vector3(Random.Range(-spreadEnergy, spreadEnergy), Random.Range(-spreadEnergy / 2, spreadEnergy / 2), 0f);
                //animate coin to target position
                float duration = Random.Range(minAnimDurationEnergy, maxAnimDurationEnergy);
                energy.transform.DOMove(tempVec, duration)
                    .SetEase(easeTypeEnergy)
                    .OnComplete(() => {
                        //executes whenever coin reach target position
                        energy.SetActive(false);
                        _coinsQueue.Enqueue(energy);
                        if (!first)
                        {
                            PlayerController.Instance.FinalScaleUp();
                            first = true;
                        }
                    });
            }
        }
        PrepareEnergy();
    }

    public void ShopPanelOpen(bool value)
    {
        shopPanel.SetActive(value);
        introPanel.GetComponent<Button>().interactable = !value;
    }

    public void TapPanel(bool value)
    {
        tapPanel.SetActive(value);
    }
}
