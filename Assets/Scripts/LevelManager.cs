using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField] private List<GameObject> levels;
    
    private int _index;
    private GameObject _currentLevel;
    private int _levelIndex;
    public int Index { get => _levelIndex; }
    private void OnEnable() 
    {
        //GameAnalytics.Initialize();
        _index = PlayerPrefs.GetInt("index", 0);
        _levelIndex = PlayerPrefs.GetInt("levelIndex", 1);
        ManageLevel(_index);
        //UIManager.Instance.UpdateIntroLevelTexts();
    }
    
    public void RestartLevel()
    {
        UIManager.Instance.retryPanel.SetActive(false);
        StartCoroutine(UIManager.Instance.LevelLoadRoutine(_index));
        //UIManager.Instance.UpdateIntroLevelTexts();
    }
    public void LoadNextLevel()
    {
        //UIManager.Instance._nextLevelPanel.SetActive(false);
        _index++;
        _levelIndex++;
        if (_index >= levels.Count)
        {
            _index = 4;
        }
        PlayerPrefs.SetInt("index", _index);
        PlayerPrefs.SetInt("levelIndex", _levelIndex);
        StartCoroutine(UIManager.Instance.LevelLoadRoutine(_index));
        CameraManager.Instance.ChangeToMainCam();
        //UIManager.Instance.UpdateIntroLevelTexts();
    }
    public void ManageLevel(int index)
    {
        if(_currentLevel)
        {
            _currentLevel.SetActive(false);
        }
        if(index >= levels.Count)
        {
            _index = 4;
        }
        if(levels.Count <= 0)
        {
            return;
        }
        _currentLevel = levels[index];
        _currentLevel.SetActive(true);
    }

    public void SetPaths(List<PathCreator> pathList)
    {
       PlayerMovement.Instance.pathCreators = pathList;
       PlayerMovement.Instance.SetPathCreator();
    }
}