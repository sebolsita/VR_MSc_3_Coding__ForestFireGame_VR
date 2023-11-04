using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiLocomotion : MonoBehaviour
{
    public Transform playerTransform;
    NavMeshAgent agent;
    Animator animator;

    private float updateInterval = 1.0f; // Update player position every 1 second
    private float timeSinceLastUpdate = 0.0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent <Animator>();
    }

    void Update()
    {
        // Update the time since the last position update
        timeSinceLastUpdate += Time.deltaTime;

        // Check if it's time to update the player's position
        if (timeSinceLastUpdate >= updateInterval)
        {
            // Set the destination to the player's position
            agent.destination = playerTransform.position;
            animator.SetFloat("Speed", agent.velocity.magnitude);

            // Reset the timeSinceLastUpdate
            timeSinceLastUpdate = 0.0f;
        }
    }
}
