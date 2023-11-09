using System.Collections; // Import the System.Collections namespace for working with coroutines.
using UnityEngine; // Import the UnityEngine namespace for Unity functionality.
using UnityEngine.AI; // Import the UnityEngine.AI namespace for navigation.
using Random = UnityEngine.Random; // Import the UnityEngine.Random namespace and alias it as "Random."

public class AiLocomotion : MonoBehaviour
{
    public Transform playerTransform; // Reference to the player's Transform component.
    NavMeshAgent agent; // Reference to the NavMeshAgent component for navigation.
    Animator animator; // Reference to the Animator component for animations.
    bool isHit = false; // Variable to track if the enemy has been hit.
    bool isAnimatingHit = false; // Variable to track if hit animation is playing.
    float hitAnimationLength; // Length of the hit animation.
    float hitAnimationTimer; // Timer to control hit animation duration.

    private float updateInterval = 3.0f; // Time interval to update the player's position.
    private float timeSinceLastUpdate = 0.0f;

    public string[] hitAnimations; // Array to store different hit animation state names.
    public string fallDownAnimation; // The animation when the enemy falls down.

    private int hitCount = 0; // Count of how many times the enemy has been hit.

    public AudioSource audioSource; // Reference to the AudioSource component for playing audio.
    public AudioClip hitSound1; // First hit sound for pain.
    public AudioClip hitSound2; // Another sound for when the enemy is hit.
    public float painSoundVolume; // Adjust the sound volume (0.0f to 1.0f).
    public AudioClip fallDownSound1; // Sound when the enemy falls down.
    public AudioClip fallDownSound2;
    public AudioClip fallDownSound3;
    private float sound2Delay = 2.0f; // 2-second delay for the second sound.

    public GameObject particleObject; // The object to activate/deactivate for special effects.

    public PlayerHealthController playerHealthController; // Reference to the player's health controller.
    private bool isTakingFireDamage = false; // Tracks if the enemy is taking fire damage.
    private float fireDamageInterval = 0.1f; // Time interval for applying fire damage.
    private float fireDamageTimer = 0f;
    private float timeSinceLastFireDamage = 0f;
    private float fireDamageDelay = 0.33f; // Delay for fire damage.

    public ScoreSystem scoreSystem; // Reference to the ScoreSystem script.

    float timeElapsed = 0.0f;

    void Start()
    {
        BulletCollision.OnHitRegistered += HandleHitRegistered; // Subscribe to the hit registration event.
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component.
        animator = GetComponent<Animator>(); // Get the Animator component.
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component.
    }

    private void OnDisable()
    {
        BulletCollision.OnHitRegistered -= HandleHitRegistered; // Unsubscribe from the hit registration event when the script is disabled.
    }

    void Update()
    {
        timeElapsed += Time.deltaTime; // Track the elapsed time.
        if (agent.remainingDistance <= agent.stoppingDistance && agent.isStopped != true)
        {
            // Calculate the direction from the agent to the player
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            directionToPlayer.y = 0f; // Ensure it's in the horizontal plane.

            if (directionToPlayer != Vector3.zero)
            {
                // Make the agent face the player by setting its rotation.
                transform.rotation = Quaternion.LookRotation(directionToPlayer);
            }

            if (timeElapsed >= 3.0f)
            {
                // Start applying fire damage after a delay.
                if (timeSinceLastFireDamage >= fireDamageDelay)
                {
                    isTakingFireDamage = true;
                    particleObject.SetActive(true); // Activate the particle object when the stopping distance is reached.
                }
                else
                {
                    timeSinceLastFireDamage += Time.deltaTime;
                }

                if (isTakingFireDamage)
                {
                    fireDamageTimer += Time.deltaTime;
                    if (fireDamageTimer >= fireDamageInterval)
                    {
                        playerHealthController.ApplyFireDamage(); // Apply fire damage to the player.
                        fireDamageTimer = 0f; // Reset the fire damage timer.
                    }
                }
            }
        }
        else
        {
            particleObject.SetActive(false); // Deactivate the particle object when the agent is not within stopping distance.
            isTakingFireDamage = false;
            timeSinceLastFireDamage = 0f; // Reset the timer when not in stopping distance.
        }

        if (isHit && !isAnimatingHit)
        {
            hitCount++;
            agent.isStopped = true; // Stop the enemy's movement.

            if (hitCount >= 10)
            {
                Debug.Log("Enemy fall down!");
                scoreSystem.AddScoreOnFall(); // Add score for the enemy falling down.
                animator.SetTrigger("FallDownAnimation"); // Play the "FallDown" animation.
                animator.Play(fallDownAnimation);

                audioSource.PlayOneShot(fallDownSound1);
                StartCoroutine(PlaySecondFallDownSound());
                audioSource.PlayOneShot(fallDownSound3);

                isAnimatingHit = true; // Mark that hit animation is playing.
                hitAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length + 7f; // Get the length of the current hit animation.
                hitAnimationTimer = 0f;

                agent.isStopped = true; // Stop the agent's movement.

                hitCount = 0; // Reset the hit count.
                isHit = false; // Reset the hit flag.
            }
            else
            {
                Debug.Log("Enemy registered the hit! > " + hitCount);
                string randomHitAnimation = hitAnimations[Random.Range(0, hitAnimations.Length)]; // Choose a random hit animation from the array.
                animator.SetTrigger("HitAnimation"); // Trigger the generic "HitAnimation" trigger.
                animator.Play(randomHitAnimation); // Play the chosen hit animation.
                isAnimatingHit = true; // Mark that hit animation is playing.
                hitAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length; // Get the length of the current hit animation.
                hitAnimationTimer = 0f; // Initialize the timer.

                isHit = false; // Reset the hit flag.

                int randomSound = Random.Range(0, 2); // Play one of the two hit sounds randomly for pain with adjusted volume.
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

        if (isAnimatingHit)
        {
            hitAnimationTimer += Time.deltaTime;
            if (hitAnimationTimer >= hitAnimationLength)
            {
                isAnimatingHit = false; // The hit animation has finished playing.
                agent.isStopped = false; // Re-enable the enemy's movement.
            }
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);

        timeSinceLastUpdate += Time.deltaTime; // Update the time since the last position update.
        if (timeSinceLastUpdate >= updateInterval)
        {
            agent.destination = playerTransform.position; // Set the destination to the player's position.
            timeSinceLastUpdate = 0.0f; // Reset the timeSinceLastUpdate.
        }
    }

    public void GetHit()
    {
        isHit = true; // Mark the enemy as hit.
    }

    private void HandleHitRegistered()
    {
        GetHit(); // Call your existing GetHit method when a hit is registered.
        scoreSystem.AddScoreOnHit(); // Add a score when the enemy is hit.
    }

    private IEnumerator PlaySecondFallDownSound()
    {
        yield return new WaitForSeconds(sound2Delay); // Delay before playing the second fall-down sound.
        audioSource.PlayOneShot(fallDownSound2); // Play the second fall-down sound.
    }
}