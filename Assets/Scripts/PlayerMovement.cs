using System;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoSingleton<PlayerMovement>
{
    [HideInInspector] public List<PathCreator> pathCreators;
    [SerializeField] private EndOfPathInstruction endOfPathInstruction;
    [SerializeField] private SwerveMovement swerveMovement;
    [SerializeField] private float moveSpeed;

    private PathCreator _pathCreator;

    private float _distanceTravelled;
    private float _targetRotY;
    
    private Quaternion _targetRot;
    
    private void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.MainGame)
        {
            Move();
        }
    }
    private void Move()
    {
        swerveMovement.SwerveMove();

        if (_pathCreator == null) return;
        
        _distanceTravelled += moveSpeed * Time.deltaTime;
        
        transform.position = _pathCreator.path.GetPointAtDistance(_distanceTravelled, endOfPathInstruction);
        
        _targetRotY = _pathCreator.path.GetRotationAtDistance(_distanceTravelled, endOfPathInstruction).eulerAngles.y;
        _targetRot = Quaternion.Euler(0, _targetRotY, 0);
        transform.rotation = _targetRot;
            
        UIManager.Instance.LevelProgress(_distanceTravelled / _pathCreator.path.length);
    }

    public void StartLevel()
    {
        _distanceTravelled = 0;
        GameManager.Instance.CurrentGameState = GameManager.GameState.MainGame;
        UIManager.Instance.StartLevel();
        CameraManager.Instance.ChangeToMainCam();
        PlayerController.Instance.StartLevel();
    }
    
    public void SetPathCreator()
    {
        _pathCreator = pathCreators[0];
        _distanceTravelled = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        Ramp ramp = other.gameObject.GetComponent<Ramp>();

        if (ramp)
        {
            if (ramp.toUp)
            {
                StartCoroutine(swerveMovement.RampMove(5.5f));
                swerveMovement.UpdateMaxSwerve(-1.7f, -4);
            }
            else
            {
                StartCoroutine(swerveMovement.RampMove(-5.5f));
                StartCoroutine(swerveMovement.EndRampMove());
            }
        }

        if (other.gameObject.CompareTag("RoadEntry"))
        {
            swerveMovement.UpdateMaxSwerve(4, -.5f);
        }

        if (other.gameObject.CompareTag("RoadExit"))
        {
            swerveMovement.UpdateMaxSwerve(4, -4);
        }
    }
}
