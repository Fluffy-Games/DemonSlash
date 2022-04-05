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
        transform.GetChild(6).gameObject.SetActive(true);
        transform.GetChild(7).gameObject.SetActive(true);
    }
    
    public void Smoke()
    {
        smoke.Play();
    }
}
