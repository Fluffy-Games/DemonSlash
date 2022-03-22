using System.Collections.Generic;
using PathCreation;
using UnityEngine;

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
}
