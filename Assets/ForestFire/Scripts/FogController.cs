using System.Collections; // Import the System.Collections namespace for working with coroutines.
using UnityEngine; // Import the UnityEngine namespace for Unity functionality.

public class FogController : MonoBehaviour
{
    public CellStateCounter cellStateCounter; // Reference to the CellStateCounter script.
    private float startPercentage = 10f; // The starting percentage for fog density adjustment.
    private float endPercentage = 75f; // The ending percentage for fog density adjustment.
    private float startFogDensity = 0.0001f; // The initial fog density.
    private float endFogDensity = 0.04f; // The target fog density.
    private float updateInterval = 0.25f; // Update fog density every 0.25 seconds.

    private void Start()
    {
        RenderSettings.fog = true; // Enable fog rendering in the scene.
        RenderSettings.fogDensity = 0.0001f; // Set the initial fog density.
        StartCoroutine(UpdateFogDensityCoroutine()); // Start a coroutine to update fog density over time.
    }

    private IEnumerator UpdateFogDensityCoroutine()
    {
        while (true)
        {
            if (cellStateCounter != null)
            {
                float percentageBurntRock = cellStateCounter.PercentageBurntRock;

                // Check if the percentageBurntRock is within the range to adjust fog density
                if (percentageBurntRock >= startPercentage && percentageBurntRock <= endPercentage)
                {
                    // Calculate the fog density based on the percentageBurntRock using linear interpolation.
                    float fogDensity = Mathf.Lerp(
                        startFogDensity,
                        endFogDensity,
                        (percentageBurntRock - startPercentage) / (endPercentage - startPercentage)
                    );
                    RenderSettings.fogDensity = fogDensity; // Update the fog density in the scene.
                }
            }

            yield return new WaitForSeconds(updateInterval); // Wait for the specified update interval before checking again.
        }
    }
}
