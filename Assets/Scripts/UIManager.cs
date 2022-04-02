using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("Panels")]
    [SerializeField] public GameObject introPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] public GameObject retryPanel;
    [SerializeField] private GameObject nextLevelPanel;
    [SerializeField] private TextMeshProUGUI demonText;
    [SerializeField] private TextMeshProUGUI diamondText;
    [SerializeField] private TextMeshProUGUI levelTextIntro;
    [SerializeField] private TextMeshProUGUI levelTextGame;
    
    [SerializeField] private Slider powerBar;

    private float demonFontSize;
    private float diamondFontSize;

    private void Start()
    {
        demonFontSize = demonText.fontSize;
        diamondFontSize = diamondText.fontSize;
    }

    public void StartLevel()
    {
        introPanel.GetComponent<Button>().interactable = false;
        introPanel.SetActive(false);
        gamePanel.SetActive(true);
        LevelTextUpdate(levelTextGame);
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

    public void RetryPanel()
    {
        retryPanel.SetActive(true);
        gamePanel.SetActive(false);
    }
    public void WinPanel()
    {
        nextLevelPanel.SetActive(true);
        gamePanel.SetActive(false);
        introPanel.GetComponent<Button>().interactable = true;
    }
    
    public IEnumerator LevelLoadRoutine(int index)
    {
        nextLevelPanel.SetActive(false);
        gamePanel.SetActive(false);
        yield return new WaitForSeconds(1f);
        LevelManager.Instance.ManageLevel(index);
        GameManager.Instance.CurrentGameState = GameManager.GameState.Prepare;
        PlayerController.Instance.ResetModelPos();
        IntroPanel();
        powerBar.value = 0;
        introPanel.GetComponent<Button>().enabled = true;
    }

    public void DemonSlashCountUpdate(int demonSlashCount)
    {
        demonText.text = $"{demonSlashCount}";
        StartCoroutine(CustomPunchScale(demonText, demonFontSize));
    }

    public void LevelTextUpdate(TextMeshProUGUI text)
    {
        text.text = $"{"LEVEL "+LevelManager.Instance.Index}";
    }

    public void PowerBarUpdate(int value)
    {
        if (value > 0)
        {
            powerBar.value += 0.05f;
        }
        else
        {
            powerBar.value -= 0.05f;
        }
    }
    public void DiamondCountUpdate(int diamondCount)
    {
        diamondText.text = $"{diamondCount}";
        StartCoroutine(CustomPunchScale(diamondText, diamondFontSize));
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
}
