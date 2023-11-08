using System.Collections;
using UnityEngine;

public class FogController : MonoBehaviour
{
    public CellStateCounter cellStateCounter; // Reference to the CellStateCounter script
    private float startPercentage = 10f;
    private float endPercentage = 75f;
    private float startFogDensity = 0.0001f;
    private float endFogDensity = 0.04f;
    private float updateInterval = 0.25f; // Update every 0.25 seconds

    private void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogDensity = 0.0001f; // Initial fog density
        StartCoroutine(UpdateFogDensityCoroutine());
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
                    // Calculate the fog density based on the percentageBurntRock
                    float fogDensity = Mathf.Lerp(startFogDensity, endFogDensity, (percentageBurntRock - startPercentage) / (endPercentage - startPercentage));
                    RenderSettings.fogDensity = fogDensity;
                }
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }
}