using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsUI : MonoBehaviour
{
    [SerializeField] List<AudioClip> uiEffectSoundsList = new List<AudioClip>();
    AudioSource source;

    private void Start()
    {
        source = gameObject.GetComponent<AudioSource>();
    }

    public void PlayClip(int index)
    {
        AudioClip clip = uiEffectSoundsList[index];
        source.PlayOneShot(clip);
    }
}
