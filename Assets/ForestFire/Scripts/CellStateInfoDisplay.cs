using UnityEngine; // Import the UnityEngine namespace for Unity functionality.
using TMPro; // Import the TextMeshPro namespace for TextMeshPro functionality.
using System; // Import the System namespace for TimeSpan.

public class CellStateInfoDisplay : MonoBehaviour
{
    public CellStateCounter cellStateCounter; // Reference to the CellStateCounter script.
    public PlayerHealthController playerHealthController; // Reference to the PlayerHealthController script.
    public TextMeshPro burntRockText; // Reference to TextMeshPro for displaying burnt and rock percentages.
    public TextMeshPro treeGrassText; // Reference to TextMeshPro for displaying tree and grass percentages.
    public TextMeshPro alightText; // Reference to TextMeshPro for displaying alight percentage.
    public TextMeshPro scoreText; // Reference to TextMeshPro for displaying the score.
    public TextMeshPro timeText; // Reference to TextMeshPro for displaying the elapsed time.
    public TextMeshPro playerHpText; // Reference to TextMeshPro for displaying the player's health.

    private static float score; // Store the score.
    private static float timeElapsed; // Store the elapsed time.

    // Start the repeating update every 0.5 seconds
    private void Start()
    {
        InvokeRepeating("UpdateDisplay", 0f, 0.5f);
    }

    // Update the display
    void UpdateDisplay()
    {
        float playerHealth = playerHealthController.GetPlayerHealth();
        if (cellStateCounter != null)
        {
            burntRockText.text = $"{cellStateCounter.PercentageBurntRock:F0}%"; // Display the percentage of burnt and rock cells.
            treeGrassText.text = $"{cellStateCounter.PercentageTreeGrass:F0}%"; // Display the percentage of tree and grass cells.
            alightText.text = $"{cellStateCounter.PercentageAlight:F0}%"; // Display the percentage of alight cells.
        }

        // Display the score, time, and player health
        scoreText.text = $"{score:F0}"; // Display the score.

        TimeSpan timeSpan = TimeSpan.FromSeconds(timeElapsed); // Create a TimeSpan from the elapsed time in seconds.
        timeText.text = $"{timeSpan.ToString("mm':'ss")}"; // Display the time in "mm:ss" format.

        playerHpText.text = $"{playerHealth}HP"; // Display the player's health.
    }

    // Update the score and time
    public static void UpdateScoreTime(float newScore, float newTime)
    {
        score = newScore; // Update the score.
        timeElapsed = newTime; // Update the elapsed time.
    }
}
