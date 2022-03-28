using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private AudioClip slashSound;

    public void CollectSound()
    {
        audioSource.PlayOneShot(collectSound);
    }
    public void SlashSound()
    {
        audioSource.PlayOneShot(slashSound);
    }
}
