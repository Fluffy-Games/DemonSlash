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
    [SerializeField] private ParticleSystem colorChangeEffectEnd;
    [SerializeField] private GameObject confetti;
    [SerializeField] private GameObject getsugaEffect;
    [SerializeField] private GameObject swordEnergy;
    [SerializeField] private GameObject finalSwordEnergy;
    [SerializeField] private GameObject endGetsugaEffect;

    private int _diamondCount;
    private int _demonSlashCount;
    private int _diamond;
    
    private int _powerMultiplier;
    
    public int Diamond => _diamond;
    
    private Vector3 _getsugaPos;
    private Vector3 _endGetsugaPos;
    
    private static readonly int Run = Animator.StringToHash("run");
    private static readonly int AttackIn = Animator.StringToHash("attackIn");
    private static readonly int AttackOut = Animator.StringToHash("attackOut");
    
    private int _swordSlashCount;

    private ColorType _colorType;
    private static readonly int Fall = Animator.StringToHash("fall");
    private static readonly int Getsuga = Animator.StringToHash("getsuga");
    private static readonly int Spin = Animator.StringToHash("spin");
    private static readonly int JumpAttack = Animator.StringToHash("jumpAttack");

    private float _lavaTimer;

    private void Start()
    {
        _diamond = PlayerPrefs.GetInt("diamond", 0);
        _upgradeCost = PlayerPrefs.GetInt("upgradeCost", 40);
        _powerIndex = PlayerPrefs.GetInt("powerIndex", 1);
        _powerMultiplier = PlayerPrefs.GetInt("powerMultiplier", 50);
        _getsugaPos = getsugaEffect.transform.localPosition;
        _endGetsugaPos = endGetsugaEffect.transform.localPosition;
        UIManager.Instance.TotalDiamond(_diamond);
        SetColorType();
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Final)
        {
            FinalAttack();
        }
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
        _powerMultiplier += 5;
        _diamond -= _upgradeCost;
        _upgradeCost += 20;
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
                _colorType = ColorType.Yellow;
                break;
            case 1:
                _colorType = ColorType.Red;
                break;
            case 2:
                _colorType = ColorType.Green;
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
            EndJumpAttack();
            ShopManager.Instance.CheckPreUnlock();
        }

        if (lavaObstacle)
        {
            _lavaTimer = 0f;
            _demonSlashCount-= 2;
            if (_demonSlashCount < 0)
            {
                GameManager.Instance.CurrentGameState = GameManager.GameState.Lose;
                playerAnim.SetTrigger(Fall);
                MMVibrationManager.Haptic(HapticTypes.Failure);
                UIManager.Instance.RetryPanel();
                StartCoroutine(FallRout());
            }
            UIManager.Instance.DemonSlashCountUpdate(_demonSlashCount);
            UIManager.Instance.PowerBarUpdate(-0.1f);
            StartCoroutine(UIManager.Instance.CollectFeedbackText(1));
            MMVibrationManager.Haptic(HapticTypes.Failure);
            audioManager.WrongSound();
        }
        if (door)
        {
            StartCoroutine(GetsugaRout(door.target, door.GetComponent<Animator>()));
            playerAnim.SetTrigger(Spin);
            swordEnergy.SetActive(true);
            UIManager.Instance.PowerBarUpdate(-.1f);
            UIManager.Instance.EnergyAnimate(false);
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
            _colorType = gate.colorType;
            colorChangeEffect.Play();
            UpdatePlayerColor();
            audioManager.ColorChangeSound();
        }

        if (slashable && !slashable.oneSlash)
        {
            slashable.oneSlash = true;
            
            if (slashable.colorType == _colorType)
            {
                _demonSlashCount++;
                UIManager.Instance.DemonSlashCountUpdate(_demonSlashCount);
                UIManager.Instance.PowerBarUpdate(0.05f);
                StartCoroutine(UIManager.Instance.CollectFeedbackText(0));
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
                _demonSlashCount-= 2;
                if (_demonSlashCount < 0)
                {
                    GameManager.Instance.CurrentGameState = GameManager.GameState.Lose;
                    playerAnim.SetTrigger(Fall);
                    MMVibrationManager.Haptic(HapticTypes.Failure);
                    UIManager.Instance.RetryPanel();
                    StartCoroutine(FallRout());
                }
                UIManager.Instance.DemonSlashCountUpdate(_demonSlashCount);
                UIManager.Instance.PowerBarUpdate(-0.1f);
                StartCoroutine(UIManager.Instance.CollectFeedbackText(1));
                MMVibrationManager.Haptic(HapticTypes.Failure);
                audioManager.WrongSound();
            }
        }

        if (collectable)
        {
            _diamondCount++;
            UIManager.Instance.DiamondCountUpdate(_diamondCount);
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
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
                _demonSlashCount-= 2;
                if (_demonSlashCount < 0)
                {
                    GameManager.Instance.CurrentGameState = GameManager.GameState.Lose;
                    playerAnim.SetTrigger(Fall);
                    MMVibrationManager.Haptic(HapticTypes.Failure);
                    UIManager.Instance.RetryPanel();
                    StartCoroutine(FallRout());
                }
                UIManager.Instance.DemonSlashCountUpdate(_demonSlashCount);
                UIManager.Instance.PowerBarUpdate(-0.1f);
                StartCoroutine(UIManager.Instance.CollectFeedbackText(1));
                MMVibrationManager.Haptic(HapticTypes.Failure);
                audioManager.WrongSound();
            }
        }
    }
    public void SumDiamond()
    {
        _diamond += _diamondCount;
        PlayerPrefs.SetInt("diamond", _diamond);
        UIManager.Instance.TotalDiamond(_diamond);
    }
    public void ResetModelPos()
    {
        modelRoot.SetActive(false);
        modelRoot.transform.localPosition = Vector3.zero;
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;
        playerRoot.transform.localPosition = Vector3.zero;
        modelRoot.SetActive(true);
        _diamondCount = 0;
        _demonSlashCount = 0;
        UIManager.Instance.DemonSlashCountUpdate(_demonSlashCount);
        UIManager.Instance.DiamondCountUpdate(_diamondCount);
        SetColorType();
    }

    private void UpdatePlayerColor()
    {
        switch (_colorType)
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
        yield return new WaitForSeconds(1f);
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
        transform.DOLocalMove(playerTarget, 1f).OnComplete((StartFinal));
        playerRoot.transform.DOLocalMove(Vector3.zero, 1f);
        modelRoot.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void StartFinal()
    {
        GameManager.Instance.CurrentGameState = GameManager.GameState.Final;
        playerAnim.SetBool(Run, false);
        UIManager.Instance.TapPanel(true);
    }

    private void FinalAttack()
    {
        if (Input.GetMouseButtonDown(0) && UIManager.Instance.PowerBarValue > 0f)
        {
            UIManager.Instance.PowerBarUpdate(-.1f);
            UIManager.Instance.EnergyAnimate(true);
            if (UIManager.Instance.PowerBarValue <= 0f)
            {
                GameManager.Instance.CurrentGameState = GameManager.GameState.Victory;
                StartCoroutine(EndGetsugaAttack());
                UIManager.Instance.TapPanel(false);
            }
        }
    }

    public void FinalScaleUp()
    {
        transform.localScale += Vector3.one * .05f;
        finalSwordEnergy.SetActive(true);
        colorChangeEffectEnd.Play();
    }
    private IEnumerator EndGetsugaAttack()
    {
        yield return new WaitForSeconds(1.5f);
        playerAnim.SetTrigger(JumpAttack);
        yield return new WaitForSeconds(0.5f);
        Vector3 playerTarget = transform.localPosition + Vector3.forward * 5f;
        transform.DOLocalMove(playerTarget, 1f);
        audioManager.FinalGetsugaSound();
        yield return new WaitForSeconds(1f);
        finalSwordEnergy.SetActive(false);
        Vector3 getsugaTarget = modelRoot.transform.localPosition + Vector3.forward * _powerMultiplier;
        endGetsugaEffect.SetActive(true);
        endGetsugaEffect.GetComponentInChildren<VisualEffect>().Play();
        endGetsugaEffect.transform.DOLocalMove(getsugaTarget, 3f);
        CameraManager.Instance.ChangeToSlash();
        yield return new WaitForSeconds(4f);
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
