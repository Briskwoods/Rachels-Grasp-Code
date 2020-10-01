using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    private Rigidbody2D player;

    public float runSpeed = 40f;
    float horizontalMove = 0f;
    float fallSpeed;

    public float cooldownTime = 2f;
    private float nextDashTime = 0f;
    
    bool jump = false;
    bool crouch = false;
    bool dash = false;

    void Start()
    {
        player = GetComponent<Rigidbody2D>();    
    }


    // Update is called once per frame
    void Update()
    {
        //Horizontal Movement Detection
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        //Animation Handlers
        //Fall Animation Handler
        fallSpeed = player.velocity.y;
        animator.SetFloat("FallSpeed", fallSpeed);
        if (fallSpeed <= -2f)
        {
            animator.SetBool("isFalling", true);
        }
        else
        {
            animator.SetBool("isFalling", false);
        }
        //Run Animation Handler
        animator.SetFloat("Speed",Mathf.Abs(horizontalMove));
        //Wall Slide animation Handler 
        
        //Detect Jump
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("isJumping",true);
        }
        //Detect Crouch
        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }
        //Dash Ability
        if (Input.GetButtonDown("Dash"))
        {
            if (Time.time > nextDashTime) //Dash Ability cooldown 
            {
                nextDashTime = Time.time + cooldownTime;
                dash = true;
            }
        }
    }

    public void onLanding()
    {
        animator.SetBool("isFalling", false);
        animator.SetBool("isJumping",false);
    }

    public void onCrouching(bool isCrouching)
    {
        animator.SetBool("isCrouching", isCrouching);
    }
    void FixedUpdate()
    {
        //Move Character
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, dash);
        jump = false;
        dash = false;
    }

}
