using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public PlayerHealthController playerHealthController; // Reference to the PlayerHealthController script
    public CellStateCounter cellStateCounter; // Reference to the CellStateCounter script

    // Update is called once per second
    private void Update()
    {
        if (playerHealthController != null && cellStateCounter != null)
        {
            float playerHealth = playerHealthController.GetPlayerHealth();
            float percentBurned = cellStateCounter.PercentageBurntRock;

            // Calculate the score based on the time elapsed since the game started
            float timeElapsed = Time.time;

            // Calculate the score based on your formula
            float score = (timeElapsed * playerHealth) / (percentBurned > 0 ? percentBurned : 1); // Avoid division by zero

            // Pass the score and time to the CellStateInfoDisplay script
            CellStateInfoDisplay.UpdateScoreTime(score, timeElapsed);
        }
    }
}