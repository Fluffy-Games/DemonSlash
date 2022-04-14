using System.Collections;
using UnityEngine;
public class SwerveMovement : MonoBehaviour
{
    [SerializeField] private SwerveInput swerveInput;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Transform playerCamFollow;
    [SerializeField] private Vector3 doorOffset;
    
    private float _swerveSpeed = 0.3f;
    private float _maxSwerveAmountX = 4f;
    private float _maxSwerveAmountMinusX = -4f;

    private Quaternion _firstRot;
    private Quaternion _secondRot;
    
    public void SwerveMove()
    {
        float swerveAmount = Time.deltaTime * _swerveSpeed * swerveInput.moveX;
        playerRoot.Translate(swerveAmount, 0, 0);
        
        _firstRot = modelTransform.localRotation;
        _secondRot = Quaternion.Euler(0, Mathf.Clamp((swerveInput.moveX * 5f), -20f, 20f), 0);
        modelTransform.localRotation = Quaternion.Slerp(_firstRot, _secondRot, Time.deltaTime * 3f);

        if (playerRoot.localPosition.x < _maxSwerveAmountMinusX)
        {
            playerRoot.localPosition = new Vector3(_maxSwerveAmountMinusX, playerRoot.localPosition.y, 0);
        }
        else if (playerRoot.localPosition.x > _maxSwerveAmountX)
        {
            playerRoot.localPosition = new Vector3(_maxSwerveAmountX, playerRoot.localPosition.y, 0);
        }
    }

    public IEnumerator RampMove(float y1)
    {
        float timer = 0;
        Vector3 originModel = modelTransform.localPosition;
        Vector3 targetModel = originModel + Vector3.up * y1;
        Vector3 originFollow = playerCamFollow.localPosition;
        Vector3 targetFollow = originFollow + Vector3.up * y1;
        while (true)
        {
            timer += Time.deltaTime * 1.7f;
            modelTransform.localPosition = Vector3.Lerp(originModel, targetModel, timer);
            playerCamFollow.localPosition = Vector3.Lerp(originFollow, targetFollow, timer);
            yield return null;
            if (timer >= 1f)
            {
                break;
            }
        }
    }

    public IEnumerator DoorMove(bool isEntry)
    {
        float timer = 0;
        Vector3 originFollow = playerCamFollow.localPosition;
        Vector3 targetFollow;
        if (isEntry)
        {
            targetFollow = originFollow - doorOffset;
        }
        else
        {
            targetFollow = originFollow + doorOffset;
        }
        while (true)
        {
            timer += Time.deltaTime;
            playerCamFollow.localPosition = Vector3.Lerp(originFollow, targetFollow, timer);
            yield return null;
            if (timer >= 1f)
            {
                break;
            }
        }
    }

    public IEnumerator EndRampMove()
    {
        yield return new WaitForSeconds(1f);
        UpdateMaxSwerve(4, -4);
    }

    public void UpdateMaxSwerve(float a, float b)
    {
        _maxSwerveAmountX = a;
        _maxSwerveAmountMinusX = b;
    }
}
