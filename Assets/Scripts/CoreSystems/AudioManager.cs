using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("SFX")]
    public AudioSource sfxSource;
    public AudioClip shootClip;
    public AudioClip collectClip;
    public AudioClip enemyDeathClip;
    public AudioClip levelUpClip;
    public AudioClip playerDeathClip;
    public AudioClip hitClip;
    public AudioClip clickClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        PlayMusic(menuMusic);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    
    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicSource.clip != null && !musicSource.isPlaying)
            musicSource.Play();
    }

}