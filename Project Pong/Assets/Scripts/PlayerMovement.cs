using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public CharacterController controller;
    public float speed = 2f;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float jumpHeight = 3f;

    [Header("Animations")]
    public Animator animator;

    private Vector3 velocity;
    private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        MovementAnimations();
    }

    void Movement()
    {
        //checks if player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //resets gravity if isGrounded
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        //this gets the horizontal and vertical inputs
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //moves character within local, not global
        Vector3 move = transform.right * x + transform.forward * z;
        //.Move is how the character controller moves the player
        controller.Move(move * speed * Time.deltaTime);

        //if player not moving, go to idle state
        if(move.magnitude == 0f)
        {
            animator.SetBool("isWalking", false);
        }

        //JUMPING
        if(Input.GetKey(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //GRAVITY
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void MovementAnimations()
    {
        //if player pressed wasd, play walking animation
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A))
        {
            animator.SetBool("isWalking", true);
        }
        
    }
}
