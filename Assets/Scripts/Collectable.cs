using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private ParticleSystem impact;

    public void ImpactEffect()
    {
        impact.Play();
    }

    private void OnDisable()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
}
