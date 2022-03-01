using Cinemachine;
using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager>
{
    [SerializeField] private CinemachineVirtualCamera introCam;
    [SerializeField] private CinemachineVirtualCamera mainCam;
    
    public void ChangeToIntroCam()
    {
        introCam.Priority = 1;
        mainCam.Priority = 0;
    }
    
    public void ChangeToMainCam()
    {
        introCam.Priority = 0;
        mainCam.Priority = 1;
    }
}
