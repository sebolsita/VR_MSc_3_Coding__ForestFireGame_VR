using UnityEngine;
using TMPro;
using System;

public class CellStateInfoDisplay : MonoBehaviour
{
    public CellStateCounter cellStateCounter; // Reference to the CellStateCounter script
    public PlayerHealthController playerHealthController; // Reference to the PlayerHealthController script
    public TextMeshPro burntRockText;
    public TextMeshPro treeGrassText;
    public TextMeshPro alightText;
    public TextMeshPro scoreText;
    public TextMeshPro timeText;
    public TextMeshPro playerHpText;

    private static float score;
    private static float timeElapsed;

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
            burntRockText.text = $"{cellStateCounter.PercentageBurntRock:F0}%";
            treeGrassText.text = $"{cellStateCounter.PercentageTreeGrass:F0}%";
            alightText.text = $"{cellStateCounter.PercentageAlight:F0}%";
        }

        // Display the score, time, and player health
        scoreText.text = $"{score:F0}";

        TimeSpan timeSpan = TimeSpan.FromSeconds(timeElapsed);
        timeText.text = $"{timeSpan.ToString("mm':'ss")}";

        playerHpText.text = $"{playerHealth}HP";
    }

    // Update the score and time
    public static void UpdateScoreTime(float newScore, float newTime)
    {
        score = newScore;
        timeElapsed = newTime;
    }
}