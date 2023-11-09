using System.Collections; // Import the System.Collections namespace for working with coroutines.
using System.Collections.Generic; // Import the System.Collections.Generic namespace for working with lists.
using UnityEngine; // Import the UnityEngine namespace for Unity functionality.

public class PlayerHealthController : MonoBehaviour
{
    public int playerMaxHealth; // Maximum health points for the player.
    public int playerHealth; // Current health points for the player.
    private bool isDamaged; // Flag to track if the player is currently taking damage.
    private float damageInterval = 0.1f; // Time interval for applying fire damage.
    public AudioSource fireDamageSource; // Reference to the AudioSource for playing fire damage sounds.
    public AudioClip fireDamage; // Audio clip for fire damage.

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Fire"))
        {
            isDamaged = true; // The player is damaged when entering a fire area.
            InvokeRepeating("ApplyFireDamage", 0, damageInterval); // Start applying fire damage at regular intervals.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Fire"))
        {
            isDamaged = false; // The player is no longer damaged when leaving the fire area.
            CancelInvoke("ApplyFireDamage"); // Stop applying fire damage.
        }
    }

    public void ApplyFireDamage()
    {
        playerHealth -= 1; // Decrease player health by 1.
        playerHealth = Mathf.Max(playerHealth, 0); // Ensure player health doesn't go below zero.

        Debug.Log("Player's Current Health: " + playerHealth); // Log the player's current health.
        fireDamageSource.PlayOneShot(fireDamage, 4f); // Play a fire damage sound with a volume of 4.

        if (playerHealth <= 0)
        {
            GameOver(); // Call the game over function when player health reaches 0.
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over! Player's health reached 0."); // Log a game over message.
        // Implement your game over logic here (e.g., displaying a game over screen).
    }

    public int GetPlayerHealth()
    {
        return playerHealth; // Return the current player health.
    }
}