using UnityEngine;
public class SwerveMovement : MonoBehaviour
{
    [SerializeField] private SwerveInput swerveInput;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform modelTransform;
    
    private readonly float _swerveSpeed = 0.15f;
    private readonly float _maxSwerveAmount = 4f;

    private Quaternion _firstRot;
    private Quaternion _secondRot;
    
    public void SwerveMove()
    {
        float swerveAmount = Time.deltaTime * _swerveSpeed * swerveInput.moveX;
        playerRoot.Translate(swerveAmount, 0, 0);
        
        _firstRot = modelTransform.localRotation;
        _secondRot = Quaternion.Euler(0, Mathf.Clamp((swerveInput.moveX * 5f), -30f, 30f), 0);
        modelTransform.localRotation = Quaternion.Slerp(_firstRot, _secondRot, Time.deltaTime * 3f);

        if (playerRoot.localPosition.x < -_maxSwerveAmount)
        {
            playerRoot.localPosition = new Vector3(-_maxSwerveAmount, 0, 0);
        }
        else if (playerRoot.localPosition.x > _maxSwerveAmount)
        {
            playerRoot.localPosition = new Vector3(_maxSwerveAmount, 0, 0);
        }
    }
}
