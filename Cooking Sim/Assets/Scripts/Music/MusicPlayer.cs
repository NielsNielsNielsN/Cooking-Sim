using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] musicClips; // Assign in Inspector
    private AudioSource audioSource;
    private int currentTrackIndex = 0;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        PlayCurrentTrack();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayNextTrack();
        }
    }

    void PlayCurrentTrack()
    {
        if (musicClips.Length == 0) return;
        audioSource.clip = musicClips[currentTrackIndex];
        audioSource.Play();
    }

    void PlayNextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % musicClips.Length;
        PlayCurrentTrack();
    }
}