using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform target;
    [SerializeField] private ParticleSystem smoke;

    public void Smoke()
    {
        smoke.Play();
    }
}