using System.Collections;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
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
    [SerializeField] private GameObject capeEffectRed;
    [SerializeField] private GameObject capeEffectYellow;
    [SerializeField] private GameObject capeEffectGreen;
    [SerializeField] private ParticleSystem slashEffect1;
    [SerializeField] private ParticleSystem slashEffect2;
    [SerializeField] private ParticleSystem colorChangeEffect;
    
    private static readonly int Run = Animator.StringToHash("run");
    private static readonly int AttackIn = Animator.StringToHash("attackIn");
    private static readonly int AttackOut = Animator.StringToHash("attackOut");
    
    private int _swordSlashCount;

    public ColorType colorType;

    public void StartLevel()
    {
        _swordSlashCount = 0;
        playerAnim.SetBool(Run, true);
        GameManager.Instance.CurrentGameState = GameManager.GameState.MainGame;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        Obstacle obstacle = other.GetComponentInParent<Obstacle>();
        Collectable collectable = other.GetComponentInParent<Collectable>();
        Gate gate = other.GetComponent<Gate>();
        Slashable slashable = other.GetComponent<Slashable>();
        
        if(other.gameObject.CompareTag("Goal") && GameManager.Instance.CurrentGameState == GameManager.GameState.MainGame)
        {
            GameManager.Instance.CurrentGameState = GameManager.GameState.Idle;
            playerAnim.SetBool(Run,false);
        }
        
        if (obstacle )
        {
            
            
        }

        if (gate)
        {
            colorType = gate.colorType;
            colorChangeEffect.Play();
            UpdatePlayerColor();
        }

        if (slashable && !slashable.oneSlash)
        {
            slashable.oneSlash = true;
            if (slashable.colorType == colorType)
            {
                _swordSlashCount++;
                StartCoroutine(CameraManager.Instance.CameraShake(1.5f));
                MMVibrationManager.Haptic(HapticTypes.LightImpact);
                playerAnim.SetTrigger(_swordSlashCount %2 == 0 ? AttackIn: AttackOut);
                
                if (_swordSlashCount % 2 == 0)
                {
                    slashEffect1.Stop();
                    slashEffect1.Play();
                }
                else
                {
                    slashEffect2.Stop();
                    slashEffect2.Play();
                }
                slashable.Slash();
            }
            else
            {
                
            }
        }

        if (collectable)
        {
            other.gameObject.SetActive(false);
        }
    }

    private void UpdatePlayerColor()
    {
        switch (colorType)
        {
            case ColorType.Green:
                capeMesh.material = capeGreen;
                capeEffectGreen.SetActive(true);
                capeEffectYellow.SetActive(false);
                capeEffectRed.SetActive(false);
                break;
            case ColorType.Red:
                capeMesh.material = capeRed;
                capeEffectGreen.SetActive(false);
                capeEffectYellow.SetActive(false);
                capeEffectRed.SetActive(true);
                break;
            case ColorType.Yellow:
                capeMesh.material = capeYellow;
                capeEffectGreen.SetActive(false);
                capeEffectYellow.SetActive(true);
                capeEffectRed.SetActive(false);
                break;
        }
    }
}
