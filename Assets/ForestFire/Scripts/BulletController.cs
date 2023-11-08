using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    public delegate void HitRegistered();
    public static event HitRegistered OnHitRegistered;
    public AudioSource hitMarkerAudioSource; // Reference to the AudioSource for the hit marker sound

    public float delayBeforeDestroy = 0.05f; // Adjust the delay as needed

    private void Update()
    {
        // Define the bullet's velocity and direction
        Vector3 bulletVelocity = transform.forward; // Assuming the bullet moves forward along its local Z-axis.

        // Define a ray based on the bullet's current position and velocity
        Ray ray = new Ray(transform.position, bulletVelocity);

        // Define a maximum distance for the raycast based on the bullet's speed and frame rate
        float maxRaycastDistance = bulletVelocity.magnitude * Time.deltaTime;

        // Perform the raycast and check for collisions
        if (Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance))
        {
            // Check if the hit object is tagged as an "enemy"
            if (hit.collider.CompareTag("enemy"))
            {
                // Handle the collision with the enemy here (e.g., apply damage)
                Debug.Log("Bullet hit enemy!");

                // Trigger the event to indicate that a hit was registered
                if (OnHitRegistered != null)
                {
                    OnHitRegistered();
                }

                // Play the hit marker sound using the AudioSource
                if (hitMarkerAudioSource != null)
                {
                    hitMarkerAudioSource.Play();
                }

                // Destroy the bullet GameObject with a delay
                Destroy(gameObject, delayBeforeDestroy);
            }
        }
    }
}