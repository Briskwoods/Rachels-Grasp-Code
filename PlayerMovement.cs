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

    [SerializeField] private PlayerHealthManager playerHealth;

    [SerializeField] private CharacterController2D controller;

    [SerializeField] private Rigidbody2D player;

    [SerializeField] private Animator animator;

    [SerializeField] private Transform m_WaterCheck;
    [SerializeField] private float m_WaterCheckRadius;
    [SerializeField] private LayerMask m_WaterLayer;

    [SerializeField] private bool isSwimming = false;

    private float horizontalMove = 0f;
    private float fallSpeed;

    private float swimMove = 0f;
    private float swimVertical = 0f;

    private bool waterDetected;

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
        waterDetected = Physics2D.OverlapCircle(m_WaterCheck.position, m_WaterCheckRadius, m_WaterLayer);

        switch (!waterDetected)
        {
            // if not true then the character is grounded and uses the ground movement functionality
            case true:
                isSwimming = false;
                animator.SetBool("isSwimming", false);
                controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, dash, attack);
                jump = false;
                dash = false;
                attack = false;
                break;
            // if true then the player swims
            case false:
                isSwimming = true;
                animator.SetBool("isSwimming", true);
                controller.Swim(swimMove, swimVertical, dash);
                dash = false;
                break;
        }

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.transform.CompareTag("Enemy"))
        {
            case true:
                playerHealth.StartCoroutine("GetInvunerable");
                // Deal damage to player.
                playerHealth.TakeDamage(10);
                // Animate taking damage
                animator.SetTrigger("Hurt");

                // Start invincibility Coroutine
                // Player Knockback
                if (collision.transform.position.x > player.transform.position.x)
                {
                    player.AddForce(transform.up * playerHealth.m_knockbackForceY + transform.right * -(playerHealth.m_knockbackForceX));
                }
                else if (collision.transform.position.x < player.transform.position.x)
                {
                    player.AddForce(transform.up * playerHealth.m_knockbackForceY + transform.right * playerHealth.m_knockbackForceX);
                }
                // Reset invincibility
                playerHealth.Invoke("resetInvulnerability", 2);

                // Check if player health is < 0 to invoke death.
                switch (playerHealth.m_currentHealth <= 0) {
                    case true:
                        playerHealth.Die();
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
    }
}
