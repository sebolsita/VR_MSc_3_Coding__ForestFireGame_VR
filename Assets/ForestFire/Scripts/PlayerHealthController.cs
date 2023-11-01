using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public int playerMaxHealth;
    public int playerHealth;
    private bool isDamaged;
    private float damageInterval = 0.1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Fire"))
        {
            isDamaged = true;
            InvokeRepeating("ApplyFireDamage", 0, damageInterval);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Fire"))
        {
            isDamaged = false;
            CancelInvoke("ApplyFireDamage");
        }
    }

    void ApplyFireDamage()
    {
        playerHealth -= 1;
        playerHealth = Mathf.Max(playerHealth, 0);

        Debug.Log("Player's Current Health: " + playerHealth);

        if (playerHealth <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over! Player's health reached 0.");
        // Implement your game over logic here.
    }

    public int GetPlayerHealth()
    {
        return playerHealth;
    }
}