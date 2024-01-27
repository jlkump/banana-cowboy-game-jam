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
        soundEffectObject.pitch = 1;
        soundEffectObject.loop = false;

        // Add variety to constant sounds
        if (effect.name.Contains("Walk") || effect.name.Contains("Run"))
        {
            soundEffectObject.pitch = Random.Range(1f, 1.5f);
        }
        if (effect.name.Contains("Spinning"))
        {
            soundEffectObject.loop = true;
        }
        soundEffectObject.Play();
    }

    public void StopSFX()
    {
        soundEffectObject.Stop();
    }
}
