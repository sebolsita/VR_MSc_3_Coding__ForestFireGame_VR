using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AiLocomotion : MonoBehaviour
{
    public Transform playerTransform;
    NavMeshAgent agent;
    Animator animator;
    bool isHit = false; // Variable to track if the enemy has been hit
    bool isAnimatingHit = false; // Variable to track if hit animation is playing
    float hitAnimationLength; // Length of the hit animation
    float hitAnimationTimer; // Timer to control hit animation duration

    private float updateInterval = 3.0f; // Update player position 
    private float timeSinceLastUpdate = 0.0f;

    public string[] hitAnimations; // Array to store the hit animation state names
    public string fallDownAnimation; // Name of the "FallDown" animation state

    // Variable to track the number of hits
    private int hitCount = 0;


    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip hitSound1; // First hit sound for pain
    public AudioClip hitSound2; // Second hit sound for pain
    public float painSoundVolume; // Adjust the volume here (0.0f to 1.0f)
    public AudioClip fallDownSound1;
    public AudioClip fallDownSound2;
    public AudioClip fallDownSound3;
    private float sound2Delay = 2.0f; // 2-second delay for the second sound

    public GameObject particleObject; // Reference to the GameObject you want to activate/deactivate.

    public PlayerHealthController playerHealthController;
    private bool isTakingFireDamage = false;
    private float fireDamageInterval = 0.1f;
    private float fireDamageTimer = 0f;
    private float timeSinceLastFireDamage = 0f;
    private float fireDamageDelay = 0.33f;

    public ScoreSystem scoreSystem; // Reference to the ScoreSystem script

    float timeElapsed = 0.0f;


    void Start()
    {
        BulletCollision.OnHitRegistered += HandleHitRegistered;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        BulletCollision.OnHitRegistered -= HandleHitRegistered;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        // Check if the agent is within its stopping distance
        if (agent.remainingDistance <= agent.stoppingDistance && agent.isStopped != true)
        {
            // Calculate the direction from the agent to the player
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            directionToPlayer.y = 0f; // Ensure it's in the horizontal plane

            if (directionToPlayer != Vector3.zero)
            {
                // Make the agent face the player by setting its rotation
                transform.rotation = Quaternion.LookRotation(directionToPlayer);
            }

            /*            // Activate the particle object when the stopping distance is reached
                        particleObject.SetActive(true);*/
            if (timeElapsed >= 3.0f)
            {
                // Start applying fire damage after a delay
                if (timeSinceLastFireDamage >= fireDamageDelay)
                {
                    isTakingFireDamage = true;
                    // Activate the particle object when the stopping distance is reached
                    particleObject.SetActive(true);
                }
                else
                {
                    timeSinceLastFireDamage += Time.deltaTime;
                }

                // Apply fire damage if within stopping distance and the timer is up
                if (isTakingFireDamage)
                {
                    fireDamageTimer += Time.deltaTime;
                    if (fireDamageTimer >= fireDamageInterval)
                    {
                        playerHealthController.ApplyFireDamage();
                        fireDamageTimer = 0f; // Reset the fire damage timer
                    }
                }
            }
        }
        else
        {
            // Deactivate the particle object when the agent is not within stopping distance
            particleObject.SetActive(false);
            isTakingFireDamage = false;
            timeSinceLastFireDamage = 0f; // Reset the timer when not in stopping distance
        }


        // Check if the enemy has been hit
        if (isHit && !isAnimatingHit)
        {
            hitCount++;
            agent.isStopped = true; // Stop the enemy's movement

            if (hitCount >= 10)
            {
                Debug.Log("Enemy fall down!");
                scoreSystem.AddScoreOnFall();
                // Play the "FallDown" animation
                animator.SetTrigger("FallDownAnimation");
                animator.Play(fallDownAnimation);

                audioSource.PlayOneShot(fallDownSound1);
                StartCoroutine(PlaySecondFallDownSound());
                audioSource.PlayOneShot(fallDownSound3);

                isAnimatingHit = true; // Mark that hit animation is playing
                hitAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length +7f; // Get the length of the current hit animation
                hitAnimationTimer = 0f;

                // Stop the agent's movement
                agent.isStopped = true;

                // Reset the hit count
                hitCount = 0;
                isHit = false; // Reset the hit flag
            }
            else
            {
                Debug.Log("Enemy registered the hit! > " + hitCount);
                // Choose a random hit animation from the array
                string randomHitAnimation = hitAnimations[Random.Range(0, hitAnimations.Length)];
                animator.SetTrigger("HitAnimation"); // Trigger the generic "HitAnimation" trigger
                animator.Play(randomHitAnimation); // Play the chosen hit animation
                isAnimatingHit = true; // Mark that hit animation is playing
                hitAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length; // Get the length of the current hit animation
                hitAnimationTimer = 0f; // Initialize the timer

                isHit = false; // Reset the hit flag

                            // Play one of the two hit sounds randomly for pain with adjusted volume
                int randomSound = Random.Range(0, 2);
                     if (randomSound == 0)
                     {
                       audioSource.PlayOneShot(hitSound1, painSoundVolume);
                     }
                     else
                     {
                        audioSource.PlayOneShot(hitSound2, painSoundVolume);
                     }
            }
        }

        // Check if the hit animation is playing
        if (isAnimatingHit)
        {
            hitAnimationTimer += Time.deltaTime;
            if (hitAnimationTimer >= hitAnimationLength)
            {
                isAnimatingHit = false; // The hit animation has finished playing
                agent.isStopped = false; // Re-enable the enemy's movement
            }
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);

        // Update the time since the last position update
        timeSinceLastUpdate += Time.deltaTime;

        // Check if it's time to update the player's position
        if (timeSinceLastUpdate >= updateInterval)
        {
            // Set the destination to the player's position
            agent.destination = playerTransform.position;

            // Reset the timeSinceLastUpdate
            timeSinceLastUpdate = 0.0f;
        }
    }

    // Add a method to handle getting hit
    public void GetHit()
    {
        isHit = true;
    }

    // Handle hit event
    private void HandleHitRegistered()
    {
        GetHit(); // You can call your existing GetHit method here
        scoreSystem.AddScoreOnHit();
    }

    private IEnumerator PlaySecondFallDownSound()
    {
        yield return new WaitForSeconds(sound2Delay);
        audioSource.PlayOneShot(fallDownSound2);
    }
}