using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private AudioClip slashSound;
    [SerializeField] private AudioClip colorChangeSound;

    public void CollectSound()
    {
        audioSource.PlayOneShot(collectSound);
    }
    public void SlashSound()
    {
        audioSource.PlayOneShot(slashSound);
    }
    public void ColorChangeSound()
    {
        audioSource.PlayOneShot(colorChangeSound);
    }
}
