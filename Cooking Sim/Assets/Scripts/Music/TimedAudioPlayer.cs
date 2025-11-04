using UnityEngine;

public class TimedAudioPlayer : MonoBehaviour
{
    public AudioClip audioClip;         // Assign your audio file in the Inspector
    public float intervalSeconds = 10f; // Time between plays (in seconds)

    private AudioSource audioSource;
    private float timer;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        timer = intervalSeconds;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            audioSource.Play();
            timer = intervalSeconds;
        }
    }
}
