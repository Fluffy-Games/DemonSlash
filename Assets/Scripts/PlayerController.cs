using System;
using System.Collections;
using DG.Tweening;
using JetBrains.Annotations;
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
    [SerializeField] private ParticleSystem getsugaEffect;
    private Vector3 _getsugaPos;
    
    private static readonly int Run = Animator.StringToHash("run");
    private static readonly int AttackIn = Animator.StringToHash("attackIn");
    private static readonly int AttackOut = Animator.StringToHash("attackOut");
    
    private int _swordSlashCount;

    public ColorType colorType;
    private static readonly int Fall = Animator.StringToHash("fall");
    private static readonly int Getsuga = Animator.StringToHash("getsuga");

    private void Start()
    {
        _getsugaPos = getsugaEffect.transform.localPosition;
    }

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
        Door door = other.GetComponent<Door>();
        
        if(other.gameObject.CompareTag("Goal") && GameManager.Instance.CurrentGameState == GameManager.GameState.MainGame)
        {
            GameManager.Instance.CurrentGameState = GameManager.GameState.Idle;
            playerAnim.SetBool(Run,false);
        }

        if (door)
        {
            door.GetComponent<Animator>().SetTrigger(Getsuga);
        }

        if (obstacle )
        {
            GameManager.Instance.CurrentGameState = GameManager.GameState.Lose;
            playerAnim.SetTrigger(Fall);
            MMVibrationManager.Haptic(HapticTypes.Failure);
            UIManager.Instance.RetryPanel();
            StartCoroutine(FallRout());
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
                MMVibrationManager.Haptic(HapticTypes.Failure);
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

    private IEnumerator FallRout()
    {
        StartCoroutine(CameraManager.Instance.CameraShake(4f));
        yield return new WaitForSeconds(.75f);
        StartCoroutine(CameraManager.Instance.CameraShake(4f));
    }

    private void GetsugaRout(Transform target, Animator animator)
    {
        getsugaEffect.Play();
        getsugaEffect.transform.DOLocalMove(target.position, 1f).OnComplete(() => DoorAnimStart(animator));
    }

    private void DoorAnimStart(Animator animator)
    {
        animator.SetTrigger(Getsuga);
        getsugaEffect.transform.localPosition = _getsugaPos;
        getsugaEffect.Stop();
    }
    
}
