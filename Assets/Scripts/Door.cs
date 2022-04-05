using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform target;
    [SerializeField] private ParticleSystem smoke;

    private void OnEnable()
    {
        transform.GetChild(2).localRotation = Quaternion.identity;
        transform.GetChild(3).localRotation = Quaternion.identity;
    }

    public void Smoke()
    {
        smoke.Play();
    }
}
