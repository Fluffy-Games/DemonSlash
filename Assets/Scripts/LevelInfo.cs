using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    [SerializeField] public List<PathCreator> levelPaths;
    
    private void OnEnable()
    {
        LevelManager.Instance.SetPaths(levelPaths);
    }
}
