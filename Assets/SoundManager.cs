using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource soundEffectObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySFX(AudioClip effect, float volume)
    {
        soundEffectObject.clip = effect;
        soundEffectObject.volume = volume;
        soundEffectObject.Play();
    }

    public void StopSFX()
    {
        soundEffectObject.Stop();
    }
}
