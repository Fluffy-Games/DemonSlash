using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoSingleton<PlayerController>
{
    [SerializeField] private Animator playerAnim;
    
    private static readonly int Run = Animator.StringToHash("run");
    private static readonly int AttackIn = Animator.StringToHash("attackIn");
    private static readonly int AttackOut = Animator.StringToHash("attackOut");

    public void StartLevel()
    {
        playerAnim.SetBool(Run, true);
    }

    public void StartAttack()
    {
        StartCoroutine(Attack());
    }

    // ReSharper disable once FunctionRecursiveOnAllPaths
    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.75f);
        playerAnim.SetTrigger(AttackIn);
        StartCoroutine(Attack());
    }
}
