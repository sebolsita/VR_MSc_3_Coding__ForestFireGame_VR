using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public ForestFire3D forestFire3D; // Reference to the ForestFire3D script
    public MiniMap miniMap;
    public PlayerHealthController playerHealthController; // Reference to the PlayerHealthController script
    private bool isPaused = false;
    private float gameStartTime;
    private float lastUpdateTime;

    private void Start()
    {
        gameStartTime = Time.time;
        lastUpdateTime = gameStartTime;
        StartCoroutine(UpdateScoreEverySecond());
    }
    IEnumerator UpdateScoreEverySecond()
    {
        while (!isPaused)
        {
            float timeElapsed = Time.time - gameStartTime;
            int minutes = Mathf.FloorToInt(timeElapsed / 60);
            int seconds = Mathf.FloorToInt(timeElapsed % 60);
            string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);

            int playerHealth = playerHealthController.GetPlayerHealth();
            float notBurningPercentage = miniMap.NotBurningPercentage;
            float onFirePercentage = miniMap.OnFirePercentage;
            float burnedPercentage = miniMap.BurnedPercentage;
            float percentCellsBurnedAndOnFire = burnedPercentage + onFirePercentage;

            float score = CalculateScore(timeElapsed, playerHealth, percentCellsBurnedAndOnFire);

            // Update the scoreText component in the MiniMap with the calculated score
            miniMap.scoreLabel.text = score.ToString("F0"); // "F0" formats the float to two decimal places
            miniMap.timeLabel.text = FormatTime(timeElapsed); // Format the time and set it to the TextMeshPro component

            // Wait for one second before updating the score again
            yield return new WaitForSeconds(1f);
        }
    }

    void Update()
    {

    }

    float CalculateScore(float timeInSeconds, int playerHealth, float percentCellsBurnedAndOnFire)
    {
        if (timeInSeconds <= 0 || percentCellsBurnedAndOnFire == 0)
            return 0; // Prevent division by zero

        return (timeInSeconds * playerHealth) / percentCellsBurnedAndOnFire;
    }
    string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void Pause()
    {
        isPaused = true;
        // Handle pausing the timer and score calculation
    }

    public void Unpause()
    {
        isPaused = false;
        // Handle unpausing the timer and score calculation
    }
}