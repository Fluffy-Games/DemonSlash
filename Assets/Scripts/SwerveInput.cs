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
#if UNITY_EDITOR
    private void GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _lastPosX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButton(0))
        {
            _moveX = (Input.mousePosition.x - _lastPosX) * 10;
            _lastPosX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _moveX = 0f;
        }
    }
#else
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
#endif
}
