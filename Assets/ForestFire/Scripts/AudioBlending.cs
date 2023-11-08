using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioBlending : MonoBehaviour
{
    public AudioSource audioSource1; // Reference to the first AudioSource
    public AudioSource audioSource2; // Reference to the second AudioSource

    private bool audioStarted = false;

    // Call this function to set audio volumes based on the new burned percentage
    public void SetAudioVolumes(float burnedPercentage)
    {
        if (!audioStarted)
        {
            audioSource1.Play();
            audioSource2.Play();
            audioStarted = true;
        }

        float maxVolume1 = 0.55f;  // Set the maximum volume limit for sound 1
        float maxVolume2 = 0.4f;  // Set the maximum volume limit for sound 2

        // Determine the threshold at which sound 1 goes silent (e.g., 70% burned)
        float sound1SilentThreshold = 0.70f;

        // Calculate volume1 to decrease more quickly and go silent as it approaches the threshold
        float volume1;
        if (burnedPercentage >= sound1SilentThreshold * 100)
        {
            volume1 = 0.0f;  // Sound 1 goes silent
        }
        else
        {
            volume1 = (1.0f - (burnedPercentage / (sound1SilentThreshold * 100))) * maxVolume1;
        }

        // Calculate volume2
        float volume2 = (burnedPercentage / 100.0f) * maxVolume2;

        // Set the audio volumes
        audioSource1.volume = volume1;
        audioSource2.volume = volume2;
    }
}