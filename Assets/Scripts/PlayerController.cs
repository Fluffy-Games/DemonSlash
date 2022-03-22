using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorType
{
    Green,
    Red,
    Yellow
}
public class PlayerController : MonoSingleton<PlayerController>
{
    [SerializeField] private Animator playerAnim;
    [SerializeField] private SkinnedMeshRenderer capeMesh;
    [SerializeField] private Material capeRed;
    [SerializeField] private Material capeGreen;
    [SerializeField] private Material capeYellow;
    
    private static readonly int Run = Animator.StringToHash("run");
    private static readonly int AttackIn = Animator.StringToHash("attackIn");
    private static readonly int AttackOut = Animator.StringToHash("attackOut");
    
    private int _swordSlashCount;

    public ColorType colorType;

    public void StartLevel()
    {
        _swordSlashCount = 0;
        playerAnim.SetBool(Run, true);
    }
    

    private void OnTriggerEnter(Collider other)
    {
        Obstacle obstacle = other.GetComponentInParent<Obstacle>();
        Collectable collectable = other.GetComponentInParent<Collectable>();
        Gate gate = other.GetComponent<Gate>();
        
        if (obstacle && !obstacle.oneSlash)
        {
            _swordSlashCount++;
            print(_swordSlashCount.ToString());
            playerAnim.SetTrigger(_swordSlashCount %2 == 0 ? AttackIn: AttackOut);
            obstacle.oneSlash = true;
        }

        if (gate)
        {
            colorType = gate.colorType;
            UpdatePlayerColor();
        }
    }

    private void UpdatePlayerColor()
    {
        switch (colorType)
        {
            case ColorType.Green:
                capeMesh.material = capeGreen;
                break;
            case ColorType.Red:
                capeMesh.material = capeRed;
                break;
            case ColorType.Yellow:
                capeMesh.material = capeYellow;
                break;
        }
    }
}
