using UnityEditor;
using UnityEngine;

public class ResetPlayerPrefs
{
    [MenuItem("Tools/Reset PlayerPrefs")]

    // Delete all PlayerPrefs
    public static void ResetAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("********************  RESET PLAYERPREFS  ***********************");
    }
}
