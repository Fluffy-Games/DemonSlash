using UnityEngine;

public class SwerveInput : MonoBehaviour
{
    private float _lastPosX;
    private float _moveX;
    private Touch _touch;
    
    public float moveX => _moveX;

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.MainGame) return;
        GetInput();
    }

    private void GetInput()
    {
        if (Input.touchCount > 0)
        {
            _touch = Input.GetTouch(0);
            switch (_touch)
            {
                case {phase: TouchPhase.Began}:
                    _lastPosX = _touch.position.x;
                    break;
                case {phase: TouchPhase.Moved}:
                    _moveX = _touch.position.x - _lastPosX;
                    _lastPosX = _touch.position.x;
                    break;
                case {phase: TouchPhase.Stationary}:
                    _moveX = 0f;
                    _lastPosX = _touch.position.x;
                    break;
                case {phase: TouchPhase.Ended}:
                    _moveX = 0f;
                    _lastPosX = _touch.position.x;
                    break;
                case {phase: TouchPhase.Canceled}:
                    _moveX = 0f;
                    _lastPosX = _touch.position.x;
                    break;
            }
        }
    }
}
