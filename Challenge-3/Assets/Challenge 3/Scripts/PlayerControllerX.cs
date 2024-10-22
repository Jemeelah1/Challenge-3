using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    public bool gameOver;

    public float floatForce = 10f;
    private float gravityModifier = 2f;
    private Rigidbody playerRb;

    public ParticleSystem explosionParticle;
    public ParticleSystem fireworksParticle;

    private AudioSource playerAudio;
    public AudioClip moneySound;
    public AudioClip explodeSound;
    public AudioClip bounceSound; // Sound when bouncing off the ground
    private float maxHeight = 15.0f;
    private float groundY = 0.5f; // The Y position of the ground

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        Physics.gravity *= gravityModifier;
        playerAudio = GetComponent<AudioSource>();

        // Apply a small upward force at the start of the game
        playerRb.AddForce(Vector3.up * 5, ForceMode.Impulse);
    }

    void Update()
    {
        // While space is pressed and player is low enough, float up
        if (Input.GetButton("Jump") && !gameOver)
        {
            if (transform.position.y < maxHeight)
            {
                playerRb.AddForce(Vector3.up * floatForce, ForceMode.Force); // Use ForceMode.Force for smoother control
            }
        }

        // Ensure balloon gently comes back down when space is not pressed
        if (!Input.GetButton("Jump"))
        {
            playerRb.AddForce(Vector3.down * (gravityModifier / 2), ForceMode.Force); // Adjust the downward force as needed
        }

        // Prevent the balloon from going too low and make it bounce
        if (transform.position.y < groundY && !gameOver)
        {
            // Play bounce sound effect
            playerAudio.PlayOneShot(bounceSound, 1.0f);
            playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z); // Reset vertical velocity
            transform.position = new Vector3(transform.position.x, groundY, transform.position.z); // Clamp position to ground level
        }

        // Clamp the balloon's height to maxHeight
        if (transform.position.y > maxHeight)
        {
            playerRb.velocity = Vector3.zero; // Stop upward movement
            transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z); // Clamp position to maxHeight
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // Handle collision logic
        if (other.gameObject.CompareTag("Bomb"))
        {
            explosionParticle.Play();
            playerAudio.PlayOneShot(explodeSound, 1.0f);
            gameOver = true;
            Debug.Log("Game Over!");
            Destroy(other.gameObject);
        } 
        else if (other.gameObject.CompareTag("Money"))
        {
            fireworksParticle.transform.position = transform.position;
            fireworksParticle.Play();
            playerAudio.PlayOneShot(moneySound, 1.0f);
            Destroy(other.gameObject);
        }
    }
}
