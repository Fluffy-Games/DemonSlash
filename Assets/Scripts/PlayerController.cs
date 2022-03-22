using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoSingleton<PlayerController>
{
    [SerializeField] private Animator playerAnim;
    
    private static readonly int Run = Animator.StringToHash("run");
    private static readonly int AttackIn = Animator.StringToHash("attackIn");
    private static readonly int AttackOut = Animator.StringToHash("attackOut");
    private int swordSlashCount;
    private bool oneSlash;

    public void StartLevel()
    {
        swordSlashCount = 0;
        playerAnim.SetBool(Run, true);
    }

    public void StartAttack()
    {
        // StartCoroutine(Attack());
    }

    private void OnTriggerEnter(Collider other)
    {
        Obstacle obstacle = other.GetComponentInParent<Obstacle>();
        Collectable collectable = other.GetComponentInParent<Collectable>();
        
        if (obstacle && !obstacle.oneSlash)
        {
            swordSlashCount++;
            print(swordSlashCount.ToString());
            playerAnim.SetTrigger(swordSlashCount %2 == 0 ? AttackIn: AttackOut);
            obstacle.oneSlash = true;
        }
    }

    // ReSharper disable once FunctionRecursiveOnAllPaths
    // private IEnumerator Attack()
    // {
    //     yield return new WaitForSeconds(1.5f);
    //     playerAnim.SetTrigger(AttackIn);
    //     yield return new WaitForSeconds(1.5f);
    //     playerAnim.SetTrigger(AttackOut);
    //     StartCoroutine(Attack());
    // }
}
