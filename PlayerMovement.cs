using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float runSpeed = 40f;
    [SerializeField] private float swimSpeed = 30f;
    [SerializeField] private float attackRate = 2f;
    [SerializeField] private float nextAttackTime = 0f;
    [SerializeField] private float cooldownTime = 2f;
    [SerializeField] private float nextDashTime = 0f;
    
    [SerializeField] private int  knockbackDamage = 10;

    [SerializeField] private PlayerHealthManager playerHealth;

    [SerializeField] private CharacterController2D controller;

    [SerializeField] private Rigidbody2D player;

    [SerializeField] private Animator animator;

    [SerializeField] private Transform m_WaterCheck;
    [SerializeField] private Transform m_HeadPosition;

    //[SerializeField] private AudioSource runSound;
    //[SerializeField] private AudioSource swimSound;

    [SerializeField] private float m_WaterCheckRadius;
    [SerializeField] private float m_HeadSize;

    [SerializeField] private LayerMask m_WaterLayer;

    [SerializeField] public bool isSwimming = false;

    private float horizontalMove = 0f;
    private float fallSpeed;

    private float swimMove = 0f;
    private float swimVertical = 0f;

    private bool bodySubmerged;
    public bool headSubmerged;

    bool jump = false;
    bool crouch = false;
    bool dash = false;
    bool attack = false;

    void Start()
    {
        player = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void Update()
    {
        switch (!isSwimming) {
            case true:

                // Horizontal Movement Detection
                horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

                // Animation Handlers
                // Fall Animation Handler
                fallSpeed = player.velocity.y;
                animator.SetFloat("FallSpeed", fallSpeed);
                //switch (horizontalMove != 0)
                //{
                //    case true:
                //        runSound.Play();
                //        break;
                //    case false:
                //        runSound.Stop();
                //        break;
                //}

                if (fallSpeed <= -2f)
                {
                    animator.SetBool("isFalling", true);
                }
                else
                {
                    animator.SetBool("isFalling", false);
                }
                // Run Animation Handler
                animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
                // Wall Slide animation Handler 

                // Detect Jump
                if (Input.GetButtonDown("Jump"))
                {
                    jump = true;
                    animator.SetBool("isJumping", true);
                }
                // Detect Crouch
                if (Input.GetButtonDown("Crouch"))
                {
                    crouch = true;
                }
                else if (Input.GetButtonUp("Crouch"))
                {
                    crouch = false;
                }
                // Dash Ability
                if (Input.GetButtonDown("Dash"))
                {
                    // Dash Ability cooldown 
                    if (Time.time > nextDashTime)
                    {
                        nextDashTime = Time.time + cooldownTime;
                        dash = true;
                    }
                }

                if (Time.time > nextAttackTime)
                {
                    // Attack Cooldown 
                    if (Input.GetButtonDown("Attack"))
                    {
                        animator.SetTrigger("Attack");
                        attack = true;
                        nextAttackTime = Time.time + 1f / attackRate;
                    }
                }
                break;
            case false:
                // Fall Animation Handler
                fallSpeed = player.velocity.y;
                animator.SetFloat("FallSpeed", fallSpeed);
                if (fallSpeed <= -0.08f)
                {
                    animator.SetBool("isFalling", true);
                }
                else
                {
                    animator.SetBool("isFalling", false);
                }

                // Swim Movement Horizontal
                swimMove = Input.GetAxis("Horizontal") * swimSpeed;
                animator.SetFloat("Speed", Mathf.Abs(swimMove));
                //switch (swimMove != 0 || swimVertical !=0)
                //{
                //    case true:
                //        swimSound.Play();
                //        break;
                //    case false:
                //        swimSound.Stop();
                //        break;
                //}
                // Swim Movement Vertical 
                swimVertical = Input.GetAxis("Vertical") * swimSpeed;
                animator.SetFloat("VerticalSpeed", swimVertical);
                
                // Swim Dash
                if (Input.GetButtonDown("Dash"))
                {
                    // Dash Ability cooldown 
                    if (Time.time > nextDashTime)
                    {
                        nextDashTime = Time.time + cooldownTime;
                        dash = true;
                    }
                }
                break;
        }
    }

    public void onLanding()
    {
        animator.SetBool("isFalling", false);
        animator.SetBool("isJumping", false);
    }

    public void onCrouching(bool isCrouching)
    {
        animator.SetBool("isCrouching", isCrouching);
    }
    void FixedUpdate()
    {
        headSubmerged = Physics2D.OverlapCircle(m_HeadPosition.position, m_HeadSize, m_WaterLayer);
        bodySubmerged = Physics2D.OverlapCircle(m_WaterCheck.position, m_WaterCheckRadius, m_WaterLayer);

        switch (bodySubmerged && headSubmerged)
        {
            case true:
                isSwimming = true;
                animator.SetBool("isSwimming", true);
                animator.SetBool("HeadSubmerged", true);
                controller.Swim(swimMove, swimVertical, dash);
                dash = false;
                break;
            case false:
                break;
        }

        switch (bodySubmerged && !headSubmerged)
        {
            case true:

                isSwimming = true;
                animator.SetBool("isSwimming", true);
                animator.SetBool("HeadSubmerged", false);
                controller.Swim(swimMove, swimVertical, dash);
                dash = false;
                break;
            case false:
                break;
        }

        switch (!bodySubmerged && !headSubmerged)
        {
            case true:
                isSwimming = false;
                animator.SetBool("isSwimming", false);
                animator.SetBool("HeadSubmerged", false);
                controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, dash, attack);
                jump = false;
                dash = false;
                attack = false;
                break;
            case false:
                break;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.transform.CompareTag("Enemy"))
        {
            case true:
                switch (playerHealth.m_invunerability == 0) {
                    case true:
                        
                        playerHealth.TakeDamage(knockbackDamage);
                        
                        if (collision.transform.position.x > player.transform.position.x)
                        {
                            player.AddForce(transform.up * playerHealth.m_knockbackForceY + transform.right * -(playerHealth.m_knockbackForceX));
                        }
                        else if (collision.transform.position.x < player.transform.position.x)
                        {
                            player.AddForce(transform.up * playerHealth.m_knockbackForceY + transform.right * playerHealth.m_knockbackForceX);
                        }
                        break;
                    case false:
                        break;
                }
                break;
            case false:
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(m_WaterCheck.position, m_WaterCheckRadius);
        Gizmos.DrawWireSphere(m_HeadPosition.position, m_HeadSize);
    }
}
