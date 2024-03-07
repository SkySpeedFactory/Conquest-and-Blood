using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance { get { return _instance; } }

    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource sfx;

    [SerializeField] AudioClip[] audioClips; 

    [SerializeField] AudioClip audioClip;
    AudioSource audioSource;

    // Private Constructor to prevent creating instance
    private SoundManager() { }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlaySound(0);
        }
    }
    public void PlaySound(int index) => audioSource.PlayOneShot(audioClips[index]);

    public void PlaySoundInOtherSource(AudioSource source) => source.PlayOneShot(source.clip);
    public void PlaySoundInOtherSource(AudioSource source, AudioClip clip) => source.PlayOneShot(clip);
    public void PlaySoundInOtherSource(AudioSource source, AudioClip clip, float volume, float pitch) => source.pitch = pitch;
    public void PlaySoundInOtherSource(AudioSource source, AudioClip clip, float volume)
    {
        source.volume = volume;
        source.PlayOneShot(clip, volume);
    }
}
