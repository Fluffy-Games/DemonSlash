using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("Panels")]
    [SerializeField] private GameObject introPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject retryPanel;
    [SerializeField] private GameObject nextLevelPanel;
    [SerializeField] private GameObject settingsPanel;
    
    [SerializeField] private Image progressBar;

    public void StartLevel()
    {
        introPanel.GetComponent<Button>().interactable = false;
    }
    public void LevelProgress(float value)
    {
        progressBar.fillAmount = value;
    }
    
    public IEnumerator LevelLoadRoutine(int index)
    {
        nextLevelPanel.SetActive(false);
        gamePanel.SetActive(false);
        yield return new WaitForSeconds(1f);
        LevelManager.Instance.ManageLevel(index);
        GameManager.Instance.CurrentGameState = GameManager.GameState.Prepare;
        introPanel.GetComponent<Button>().enabled = true;
    }
}
