using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public int playerMaxHealth;
    public int playerHealth;
    private bool isDamaged;
    private float damageInterval = 0.1f;
    public AudioSource fireDamageSource;
    public AudioClip fireDamage;

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

    public void ApplyFireDamage()
    {
        playerHealth -= 1;
        playerHealth = Mathf.Max(playerHealth, 0);

        Debug.Log("Player's Current Health: " + playerHealth);
        fireDamageSource.PlayOneShot(fireDamage, 4f);

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