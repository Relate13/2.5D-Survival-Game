using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    private static SoundManager instance;
    public static SoundManager GetInstance()
    {
        return instance;
    }
    SoundManager()
    {
        instance = this;
    }
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
