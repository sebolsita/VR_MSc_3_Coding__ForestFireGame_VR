using System.Collections; // Import the System.Collections namespace for working with coroutines.
using System.Collections.Generic; // Import the System.Collections.Generic namespace for working with lists.
using UnityEngine; // Import the UnityEngine namespace for Unity functionality.

public class AudioController : MonoBehaviour
{
    public CellStateCounter cellStateCounter; // Reference to the CellStateCounter script.
    public AudioBlending audioBlending; // Reference to the AudioBlending script.

    private float checkInterval = 1f; // Interval in seconds to check the percentage values.
    private float timer = 0f; // Timer to keep track of the time.

    private float previousBurntRockPercentage = -1f; // Initialize to a value that's not possible in percentage.

    // Update is called once per frame
    void Update()
    {
        // Update the timer.
        timer += Time.deltaTime;

        // Check percentage values and control audio blending each second.
        if (timer >= checkInterval)
        {
            if (cellStateCounter != null)
            {
                float burntRockPercentage = cellStateCounter.PercentageBurntRock;

                if (burntRockPercentage != previousBurntRockPercentage)
                {
                    audioBlending.SetAudioVolumes(burntRockPercentage); // Control audio blending based on the burnt rock percentage.
                    previousBurntRockPercentage = burntRockPercentage;
                }
            }

            // Reset the timer.
            timer = 0f;
        }
    }
}