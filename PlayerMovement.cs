using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float runSpeed = 40f;
    [SerializeField] private float attackRate = 2f;
    [SerializeField] private float nextAttackTime = 0f;
    [SerializeField] private float cooldownTime = 2f;
    [SerializeField] private float nextDashTime = 0f;

    [SerializeField] private PlayerHealthManager playerHealth;

    [SerializeField] private CharacterController2D controller;

    [SerializeField] private Rigidbody2D player;

    [SerializeField] private Animator animator;

    private float horizontalMove = 0f;
    private float fallSpeed;

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
        //Move Character
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, dash, attack);
        jump = false;
        dash = false;
        attack = false;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.transform.CompareTag("Enemy"))
        {
            case true:
                // Get enemy script for damage.
                EnemyController enemy = GetComponent<EnemyController>();

                // Deal damage to player.
                playerHealth.TakeDamage(10);
                // Animate taking damage
                animator.SetTrigger("Hurt");

                // Start invincibility Coroutine
                playerHealth.StartCoroutine("GetInvunerable");
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
}
