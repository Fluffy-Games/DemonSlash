using System;
using System.Collections;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public enum ColorType
{
    Green,
    Red,
    Yellow
}
public class PlayerController : MonoSingleton<PlayerController>
{
    [SerializeField] private SwerveMovement swerveMovement;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Animator playerAnim;
    [SerializeField] private GameObject modelRoot;
    [SerializeField] private GameObject playerRoot;
    [SerializeField] private SkinnedMeshRenderer capeMesh;
    [SerializeField] private Material capeRed;
    [SerializeField] private Material capeGreen;
    [SerializeField] private Material capeYellow;
    [SerializeField] private GameObject capeEffectRed;
    [SerializeField] private GameObject capeEffectYellow;
    [SerializeField] private GameObject capeEffectGreen;
    
    private int _upgradeCost;
    [SerializeField] private TextMeshProUGUI upgradeText;
    [SerializeField] private TextMeshProUGUI upgradeLevelText;
    private int _powerIndex;
    
    [SerializeField] private ParticleSystem slashEffect1;
    [SerializeField] private ParticleSystem slashEffect2;
    [SerializeField] private ParticleSystem colorChangeEffect;
    [SerializeField] private GameObject confetti;
    [SerializeField] private GameObject getsugaEffect;
    [SerializeField] private GameObject swordEnergy;
    [SerializeField] private GameObject endGetsugaEffect;

    private int diamondCount;
    private int demonSlashCount;
    private int _diamond;
    
    [SerializeField] private int _powerMultiplier;
    
    public int Diamond => _diamond;
    
    private Vector3 _getsugaPos;
    private Vector3 _endGetsugaPos;
    
    private static readonly int Run = Animator.StringToHash("run");
    private static readonly int AttackIn = Animator.StringToHash("attackIn");
    private static readonly int AttackOut = Animator.StringToHash("attackOut");
    
    private int _swordSlashCount;

    private ColorType colorType;
    private static readonly int Fall = Animator.StringToHash("fall");
    private static readonly int Getsuga = Animator.StringToHash("getsuga");
    private static readonly int Spin = Animator.StringToHash("spin");
    private static readonly int JumpAttack = Animator.StringToHash("jumpAttack");

    private float _lavaTimer;
    private int _upgradeValue;

    private void Start()
    {
        _diamond = PlayerPrefs.GetInt("diamond", 20000);
        _upgradeCost = PlayerPrefs.GetInt("upgradeCost", 100);
        _powerIndex = PlayerPrefs.GetInt("powerIndex", 1);
        _powerMultiplier = PlayerPrefs.GetInt("powerMultiplier", 30);
        _getsugaPos = getsugaEffect.transform.localPosition;
        _endGetsugaPos = endGetsugaEffect.transform.localPosition;
        UIManager.Instance.TotalDiamond(_diamond);
        SetColorType();
    }

    public void StartLevel()
    {
        _swordSlashCount = 0;
        playerAnim.SetBool(Run, true);
        GameManager.Instance.CurrentGameState = GameManager.GameState.MainGame;
        UIManager.Instance.TotalDiamond(_diamond);
    }

    public void UpdateTotalDiamond(int value)
    {
        _diamond = value;
        PlayerPrefs.SetInt("diamond", _diamond);
        UIManager.Instance.TotalDiamond(_diamond);
    }
    public void UpgradePower()
    {
        if (_diamond < _upgradeCost) return;
        _powerMultiplier += 2;
        _diamond -= _upgradeCost;
        _upgradeCost += 10;
        _powerIndex += 1;
        PlayerPrefs.SetInt("powerMultiplier", _powerMultiplier);
        PlayerPrefs.SetInt("upgradeCost", _upgradeCost);
        PlayerPrefs.SetInt("diamond", _diamond);
        PlayerPrefs.SetInt("upgradeCost",_upgradeCost);
        PlayerPrefs.SetInt("powerIndex",_powerIndex);
        UIManager.Instance.TotalDiamond(_diamond);
        upgradeText.text = _upgradeCost.ToString();
        upgradeLevelText.text = $"{"LEVEL " + _powerIndex}";
    }
    private void SetColorType()
    {
        switch (LevelManager.Instance.Index % 3)
        {
            case 0:
                colorType = ColorType.Yellow;
                break;
            case 1:
                colorType = ColorType.Red;
                break;
            case 2:
                colorType = ColorType.Green;
                break;
        }
        UpdatePlayerColor();
    }

    private void OnTriggerEnter(Collider other)
    {
        Obstacle obstacle = other.GetComponentInParent<Obstacle>();
        Collectable collectable = other.GetComponentInParent<Collectable>();
        Gate gate = other.GetComponent<Gate>();
        Slashable slashable = other.GetComponent<Slashable>();
        Door door = other.GetComponent<Door>();
        LavaObstacle lavaObstacle = other.GetComponent<LavaObstacle>();
        
        if(other.gameObject.CompareTag("Goal") && GameManager.Instance.CurrentGameState == GameManager.GameState.MainGame)
        {
            GameManager.Instance.CurrentGameState = GameManager.GameState.Idle;
            playerAnim.SetTrigger(JumpAttack);
            EndJumpAttack();
            ShopManager.Instance.CheckPreUnlock();
        }

        if (lavaObstacle)
        {
            _lavaTimer = 0f;
            demonSlashCount-= 2;
            if (demonSlashCount < 0)
            {
                GameManager.Instance.CurrentGameState = GameManager.GameState.Lose;
                playerAnim.SetTrigger(Fall);
                MMVibrationManager.Haptic(HapticTypes.Failure);
                UIManager.Instance.RetryPanel();
                StartCoroutine(FallRout());
            }
            UIManager.Instance.DemonSlashCountUpdate(demonSlashCount);
            UIManager.Instance.PowerBarUpdate(-0.1f);
            MMVibrationManager.Haptic(HapticTypes.Failure);
            audioManager.WrongSound();
        }
        if (door)
        {
            StartCoroutine(GetsugaRout(door.target, door.GetComponent<Animator>()));
            playerAnim.SetTrigger(Spin);
            swordEnergy.SetActive(true);
            StartCoroutine(CloseDoor(door.transform.GetChild(6).gameObject, door.transform.GetChild(7).gameObject));
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
            audioManager.ColorChangeSound();
        }

        if (slashable && !slashable.oneSlash)
        {
            slashable.oneSlash = true;
            
            if (slashable.colorType == colorType)
            {
                demonSlashCount++;
                UIManager.Instance.DemonSlashCountUpdate(demonSlashCount);
                UIManager.Instance.PowerBarUpdate(0.05f);
                _swordSlashCount++;
                StartCoroutine(CameraManager.Instance.CameraShake(1.5f));
                MMVibrationManager.Haptic(HapticTypes.LightImpact);
                audioManager.SlashSound();
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
                demonSlashCount-= 2;
                if (demonSlashCount < 0)
                {
                    GameManager.Instance.CurrentGameState = GameManager.GameState.Lose;
                    playerAnim.SetTrigger(Fall);
                    MMVibrationManager.Haptic(HapticTypes.Failure);
                    UIManager.Instance.RetryPanel();
                    StartCoroutine(FallRout());
                }
                UIManager.Instance.DemonSlashCountUpdate(demonSlashCount);
                UIManager.Instance.PowerBarUpdate(-0.1f);
                MMVibrationManager.Haptic(HapticTypes.Failure);
                audioManager.WrongSound();
            }
        }

        if (collectable)
        {
            diamondCount++;
            UIManager.Instance.DiamondCountUpdate(diamondCount);
            other.gameObject.GetComponent<MeshRenderer>().enabled = false;
            collectable.ImpactEffect();
            audioManager.CollectSound();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        LavaObstacle lavaObstacle = other.gameObject.GetComponent<LavaObstacle>();
        if (lavaObstacle)
        {
            _lavaTimer += Time.deltaTime;
            if (_lavaTimer >= 1f)
            {
                _lavaTimer = 0f;
                demonSlashCount-= 2;
                if (demonSlashCount < 0)
                {
                    GameManager.Instance.CurrentGameState = GameManager.GameState.Lose;
                    playerAnim.SetTrigger(Fall);
                    MMVibrationManager.Haptic(HapticTypes.Failure);
                    UIManager.Instance.RetryPanel();
                    StartCoroutine(FallRout());
                }
                UIManager.Instance.DemonSlashCountUpdate(demonSlashCount);
                UIManager.Instance.PowerBarUpdate(-0.1f);
                MMVibrationManager.Haptic(HapticTypes.Failure);
                audioManager.WrongSound();
            }
        }
    }
    public void SumDiamond()
    {
        _diamond += diamondCount;
        PlayerPrefs.SetInt("diamond", _diamond);
        UIManager.Instance.TotalDiamond(_diamond);
    }
    public void ResetModelPos()
    {
        modelRoot.SetActive(false);
        modelRoot.transform.localPosition = Vector3.zero;
        transform.position = Vector3.zero;
        playerRoot.transform.localPosition = Vector3.zero;
        modelRoot.SetActive(true);
        diamondCount = 0;
        demonSlashCount = 0;
        UIManager.Instance.DemonSlashCountUpdate(demonSlashCount);
        UIManager.Instance.DiamondCountUpdate(diamondCount);
        SetColorType();
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

    private IEnumerator CloseDoor(GameObject gameObject1,GameObject gameObject2)
    {
        yield return new WaitForSeconds(2);
        gameObject1.SetActive(false);
        gameObject2.SetActive(false);
        
    }
    private IEnumerator GetsugaRout(Transform target, Animator animator)
    {
        yield return new WaitForSeconds(.75f);
        getsugaEffect.SetActive(true);
        audioManager.GetsugaSound();
        swordEnergy.SetActive(false);
        getsugaEffect.transform.DOLocalMove(target.position, 1.5f).OnComplete(GetsugaReset);
        StartCoroutine(DoorAnimStart(animator));
        animator.gameObject.GetComponent<Door>().Smoke();
    }

    private IEnumerator DoorAnimStart(Animator animator)
    {
        yield return new WaitForSeconds(.25f);
        animator.SetTrigger(Getsuga);
        yield return new WaitForSeconds(2f);
        StartCoroutine(DoorCamera());
    }

    private void GetsugaReset()
    {
        getsugaEffect.transform.localPosition = _getsugaPos;
        getsugaEffect.SetActive(false);
    }

    private IEnumerator DoorCamera()
    {
        StartCoroutine(swerveMovement.DoorMove(true));
        yield return new WaitForSeconds(2f);
        StartCoroutine(swerveMovement.DoorMove(false));
    }

    private void EndJumpAttack()
    {
        Vector3 playerTarget = transform.localPosition + Vector3.forward * 10f;
        transform.DOLocalMove(playerTarget, 1f);
        playerRoot.transform.DOLocalMove(Vector3.zero, 1f);
        StartCoroutine(EndGetsugaAttack());
    }

    private IEnumerator EndGetsugaAttack()
    {
        audioManager.FinalGetsugaSound();
        yield return new WaitForSeconds(1.3f);
        Vector3 getsugaTarget = modelRoot.transform.localPosition + Vector3.forward * _powerMultiplier;
        endGetsugaEffect.SetActive(true);
        endGetsugaEffect.GetComponentInChildren<VisualEffect>().Play();
        endGetsugaEffect.transform.DOLocalMove(getsugaTarget, 2f);
        CameraManager.Instance.ChangeToSlash();
        yield return new WaitForSeconds(3f);
        endGetsugaEffect.GetComponentInChildren<VisualEffect>().Stop();
        confetti.SetActive(true);
        UIManager.Instance.WinPanel();
        ShopManager.Instance.IncreasePreUnlock();
        endGetsugaEffect.SetActive(false);
        endGetsugaEffect.transform.localPosition = _endGetsugaPos;
    }

    public void CloseConfetti()
    {
        confetti.SetActive(false);
    }
}
