using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager>
{
    [SerializeField] private CinemachineVirtualCamera introCam;
    [SerializeField] private CinemachineVirtualCamera mainCam;
    [SerializeField] private Transform slashFollow;
    [SerializeField] private Transform slashLook;
    [SerializeField] private Transform playerFollow;
    [SerializeField] private Transform playerLook;
    
    public void ChangeToIntroCam()
    {
        introCam.Priority = 1;
        mainCam.Priority = 0;
        mainCam.Follow = playerFollow;
        mainCam.LookAt = playerLook;
    }
    
    public void ChangeToMainCam()
    {
        introCam.Priority = 0;
        mainCam.Priority = 1;
        mainCam.Follow = playerFollow;
        mainCam.LookAt = playerLook;
    }

    public void ChangeToSlash()
    {
        mainCam.Follow = slashFollow;
        mainCam.LookAt = slashLook;
    }
    public IEnumerator CameraShake(float value)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            mainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        float timer = 0;

        while (true)
        {
            timer += Time.deltaTime * 5f;
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(value, 0, timer);

            if (timer >= 1f)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
