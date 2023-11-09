using UnityEngine; // Import the UnityEngine namespace for Unity functionality.

public class ScoreSystem : MonoBehaviour
{
    public PlayerHealthController playerHealthController; // Reference to the PlayerHealthController script.
    public CellStateCounter cellStateCounter; // Reference to the CellStateCounter script.
    public ForestFire3D forestFire; // Reference to the ForestFire3D script.
    public float score; // Variable to store the player's score.
    private float timeElapsed; // Variable to track the time elapsed since the game started.

    private void Update()
    {
        if (forestFire != null && forestFire.gameRunning)
        {
            if (playerHealthController != null && cellStateCounter != null)
            {
                // Calculate the score based on the time elapsed since the game started
                timeElapsed += Time.deltaTime;

                float playerHealth = playerHealthController.GetPlayerHealth(); // Get the player's health from the PlayerHealthController.
                float percentBurned = cellStateCounter.PercentageBurntRock; // Get the percentage of burnt rocks from CellStateCounter.

                // Calculate the score based on your formula, avoid division by zero.
                score = (timeElapsed * playerHealth) / (percentBurned > 0 ? percentBurned : 1);

                // Pass the score and time to the CellStateInfoDisplay script.
                CellStateInfoDisplay.UpdateScoreTime(score, timeElapsed);
            }
        }
    }

    public void AddScoreOnHit()
    {
        score += 50; // Add 50 points to the score when the enemy gets hit.
    }

    public void AddScoreOnFall()
    {
        score += 500; // Add 500 points to the score when the enemy falls on the ground.
    }
}