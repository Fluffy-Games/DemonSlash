using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform target;
    [SerializeField] private ParticleSystem smoke;
    
    private void OnDisable()
    {
        gameObject.GetComponent<Animator>().SetTrigger("idle");
        transform.GetChild(2).localRotation = Quaternion.Euler(Vector3.zero);
        transform.GetChild(3).localRotation = Quaternion.Euler(Vector3.zero);
    }

    public void Smoke()
    {
        smoke.Play();
    }
}
