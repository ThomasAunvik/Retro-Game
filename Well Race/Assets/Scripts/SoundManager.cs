using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager instance;
    public AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    public static void PlayAudio(AudioClip clip, float volume = 1)
    {
        instance.audioSource.volume = volume;
        instance.audioSource.clip = clip;
        instance.audioSource.Play();
    }

}
