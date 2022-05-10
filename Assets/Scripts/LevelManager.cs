using System;
using System.Collections.Generic;
using GameAnalyticsSDK;
using PathCreation;
using UnityEngine;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField] private List<GameObject> levels;

    private int _index;
    private GameObject _currentLevel;
    private int _levelIndex;

    public int Index
    {
        get => _levelIndex;
    }
    
    private void OnEnable()
    {
        _index = PlayerPrefs.GetInt("index", 0);
        _levelIndex = PlayerPrefs.GetInt("levelIndex", 1);
        ManageLevel(_index);
        UIManager.Instance.UpdateIntroLevelTexts();
    }

    private void Start()
    {
        GameAnalytics.Initialize();
    }

    public void RestartLevel()
    {
        UIManager.Instance.retryPanel.SetActive(false);
        StartCoroutine(UIManager.Instance.LevelLoadRoutine(_index, false));
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
        StartCoroutine(UIManager.Instance.LevelLoadRoutine(_index, true));
    }

    public void ManageLevel(int index)
    {
        if (_currentLevel)
        {
            _currentLevel.SetActive(false);
        }

        if (index >= levels.Count)
        {
            _index = 4;
        }

        if (levels.Count <= 0)
        {
            return;
        }

        _currentLevel = levels[index];
        _currentLevel.SetActive(true);
        CameraManager.Instance.ChangeToIntroCam();
        PlayerController.Instance.ResetModelPos();
        UIManager.Instance.UpdateIntroLevelTexts();
    }

    public void SetPaths(List<PathCreator> pathList)
    {
        PlayerMovement.Instance.pathCreators = pathList;
        PlayerMovement.Instance.SetPathCreator();
    }
}