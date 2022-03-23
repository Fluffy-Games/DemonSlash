using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplierTrigger : MonoBehaviour
{
    private MultiplierPlatform _mMultiplierPlatform;

    private void Start()
    {
        _mMultiplierPlatform = transform.parent.GetComponent<MultiplierPlatform>();
    }

    private void OnTriggerEnter(Collider iOther)
    {
        if (iOther.CompareTag($"Enemy")) _mMultiplierPlatform.ChangeMaterial();
    }
}
