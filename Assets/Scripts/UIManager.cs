using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("Panels")]
    [SerializeField] private GameObject introPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] public GameObject retryPanel;
    [SerializeField] private GameObject nextLevelPanel;
    [SerializeField] private GameObject settingsPanel;
    
    [SerializeField] private Image progressBar;

    public void StartLevel()
    {
        introPanel.GetComponent<Button>().interactable = false;
        introPanel.SetActive(false);
        gamePanel.SetActive(true);
    }
    public void LevelProgress(float value)
    {
        //progressBar.fillAmount = value;
    }
    public void IntroPanel()
    {
        introPanel.SetActive(true);
        gamePanel.SetActive(false);
    }

    public void RetryPanel()
    {
        retryPanel.SetActive(true);
        gamePanel.SetActive(false);
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
        introPanel.GetComponent<Button>().enabled = true;
    }
}
