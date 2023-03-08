using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public ParticleSystem explosionParticle; // public to get it from UI
    public ParticleSystem dirtParticle;
    public AudioClip jumpSound; // pass jump sound from inspector
    public AudioClip crashSound;

    private GameObject mainCamera; // Contains "background" music inside AudioSource comp.
    private AudioSource mainCameraAudio;

    private Rigidbody playerRigidbody;
    private Animator playerAnimator;
    private AudioSource playerAudio; // the audio that plays from the player (component needs to be added!)

    private bool isOnGround = true;

    public float jumpForce = 10;
    public float gravityModifier = 1;
    public bool gameOver = false;

    public bool doubleJumpUsed = false;
    public float doubleJumpForce;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        Physics.gravity *= gravityModifier;
        playerAudio = GetComponent<AudioSource>();

        mainCameraAudio = GameObject.Find("Main Camera").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround && !gameOver)
        {
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;

            playerAnimator.SetTrigger("Jump_trig");
            dirtParticle.Stop();
            playerAudio.PlayOneShot(jumpSound, 1.0f); // play the jump sound

            doubleJumpUsed = false; // because now on the ground

        } 
        else if (Input.GetKeyDown(KeyCode.Space) && !isOnGround && !doubleJumpUsed)
        {
            doubleJumpUsed = true;
            playerRigidbody.AddForce(Vector3.up * doubleJumpForce, ForceMode.Impulse);
            playerAnimator.Play("Running_Jump", 3, 0f);
            playerAudio.PlayOneShot(jumpSound, 1f);

        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            dirtParticle.Play();
        } 
        else if(collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Game Over");
            gameOver = true;

            mainCameraAudio.Stop();

            // Meeting the conditions of the "death_01" animation 
            playerAnimator.SetBool("Death_b", true); 
            playerAnimator.SetInteger("DeathType_int", 1);

            isOnGround = false;
            
            explosionParticle.Play(); // Plays the ParticleSystem given
            dirtParticle.Stop(); // No more dirt particle when dead!

            playerAudio.PlayOneShot(crashSound, 1.0f);
        }
        
    }

}
