using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoSingleton<PlayerController>
{
    [SerializeField] private Animator playerAnim;
    
    private static readonly int Run = Animator.StringToHash("run");

    public void StartLevel()
    {
        playerAnim.SetBool(Run, true);
    }
}
