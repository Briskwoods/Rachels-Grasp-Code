using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int m_maxHealth = 100;                     // Enemy max health variable.
    [SerializeField] public int m_currentHealth = 0;                    // Enemy current Health variable
    [SerializeField] public int m_attackDamage = 0;                     // How much damage the enemy does to the player

    [SerializeField] private float m_attackRange = 0.5f;                // Enemy attack range.
    [SerializeField] private float m_patrolSpeed = 1f;                  // Patrol Speed of the enemy.
    [SerializeField] private float m_groundCheckDistance;               // Height for ground check distance when patrolling on a platform.
    [SerializeField] private float m_wallCheckRadius;                   // Radius of wall check cicrcle collider when patrolling on ground
    [SerializeField] private float m_moveSpeed = 2f;                    // Speed at which enemy moves towards the player after player detection
    [SerializeField] private float m_playerDetectRadius;                // Enemy radius to detect if player is in attacking range
    [SerializeField] private float m_nextAttackTime = 0f;               // Time Until Next Attack
    [SerializeField] private float m_TimeBetweenAttacks = 1.5f;
    [SerializeField] private float m_stoppingDistance =1f;              // Distance at which the enemy stops before the player.
    
    [SerializeField] public float m_knockbackForceX =100f;
    [SerializeField] public float m_knockbackForceY = 100f;

    [SerializeField] private PlayerHealthManager playerHealth;          // For interactions with the player health manager
    [SerializeField] private Rigidbody2D m_rigidbody;                   // For Knockback force
    [SerializeField] private Rigidbody2D m_player;                      // Used in knocking the player back when enemy attacks

    [SerializeField] private Transform m_lineOfSightStart;              // Where the enemies Line of sight begins
    [SerializeField] private Transform m_lineOfSightEnd;                // Where the enemies Line of sight ends
    [SerializeField] private Transform m_attackPoint;                   // Where the enemy attack lands
    [SerializeField] private Transform m_target;                        // Where we declare the player as the target for the enemy
    [SerializeField] private Transform m_groundDetection;               // Detects the ground for enemies patrolling on a floating platform.
    [SerializeField] private Transform m_wallDetection;                 // Detects the wall for enemy patrol.
    [SerializeField] private Transform m_playerAttackRange;             // Range in which the enemy stops and attacks the player

    [SerializeField] private Animator m_animator;                       // Enemy animation controller    

    [SerializeField] private LayerMask m_groundLayer;                   // Declare the ground layer for wall detection on patrol.
    [SerializeField] private LayerMask m_PlayerLayer;                   // Declare the player layer for player detection.

    [SerializeField] private bool m_isPatrolling;                       // Decided if the enemy is idle or patrolling.
    [SerializeField] private bool m_isGround;                           // Variable used to check if the enemy is patrolling on a platform or on the ground

    private bool playerSeen;                                            // Set to true or false when enemy detects player
    private bool wallInfo;                                              // Detects the precense of a wall.
    private bool m_movingRight = true;                                  // Used in flipping the enemy sprite when the enemy is turning
    private bool inRange;                                               // Detects if player is in range.
    private bool hitPlayer;                                             // Detects if the player is hit to deal damage
        
    private int m_numberOfTimesPlayerIsDetected = 0;                    // Sets enemy to patrol after detecting player
    
    // Start is called before the first frame update
    void Start()
    {
        // Set current health to max on start
        m_currentHealth = m_maxHealth;

        // Initialise specified variables
        m_target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        
        // Sets the number of times a player is detected to 0, this allows us to have idle enemies waiting to detect the player and move.
        m_numberOfTimesPlayerIsDetected = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //This line prevents the raycast from detecting the enemy's box colliders
        Physics2D.queriesStartInColliders = false;
        //We use a Linecast to detect the presence of a player and if found the enemy can move towards the player.
        playerSeen = Physics2D.Linecast(m_lineOfSightStart.position, m_lineOfSightEnd.position, m_PlayerLayer);
        Debug.DrawLine(m_lineOfSightStart.position, m_lineOfSightEnd.position, new Color(1f, 1f, 1f));
        // If the player in not Detected
        switch(!playerSeen){
            case true:
                // Checks if the enemy is patrolling
                switch (m_isPatrolling)
                {
                    case true:
                        //Move Enemy on patrol route
                        transform.Translate(Vector2.right * m_patrolSpeed * Time.deltaTime);
                        //Animate the movement
                        m_animator.SetFloat("WalkSpeed", Mathf.Abs(m_patrolSpeed));
                        // Checks if the enemy is on the ground or not. If they're not on the ground then they're on a platform.
                        switch (m_isGround)
                        {
                            case true:
                                // We use a sphere collider to detect the presence of a wall and if found, the enemy can turn as they patrol.
                                wallInfo = Physics2D.OverlapCircle(m_wallDetection.position, m_wallCheckRadius, m_groundLayer);
                                // If a wall is detected then we turn the player around.
                                switch (wallInfo)
                                {
                                    case true:
                                        switch (!m_movingRight)
                                        {
                                            case true:
                                                transform.eulerAngles = new Vector3(0, 0, 0);
                                                m_movingRight = true;
                                                break;
                                            case false:
                                                transform.eulerAngles = new Vector3(0, -180, 0);
                                                m_movingRight = false;
                                                break;
                                        }
                                        break;
                                    case false:
                                        break;
                                }
                                // Used in detecting edges to prevent enemy from falling off the map.
                                RaycastHit2D GroundInfo = Physics2D.Raycast(m_groundDetection.position, Vector2.down, m_groundCheckDistance);
                                // If ground is not detected it changes to the platform script
                                switch (!GroundInfo)
                                {
                                    case true:
                                        m_isGround = false;
                                        break;
                                    case false:
                                        break;
                                }
                                break;
                            case false:
                                // In this case, when isGround is false that means the enemy is on a platform therefore we use a downwards raycast to detect where the plaform stops to turn the enemy
                                RaycastHit2D groundInfo = Physics2D.Raycast(m_groundDetection.position, Vector2.down, m_groundCheckDistance);
                                Debug.DrawRay(m_groundDetection.position, Vector2.down * transform.localScale.x * m_groundCheckDistance, new Color(1f, 1f, 1f));
                                //If there is no ground information we can turn and patrol in the opposite direction
                                switch (!groundInfo)
                                {
                                    case true:
                                        switch (m_movingRight)
                                        {
                                            case true:
                                                transform.eulerAngles = new Vector3(0, -180, 0);
                                                m_movingRight = false;
                                                break;
                                            case false:
                                                transform.eulerAngles = new Vector3(0, 0, 0);
                                                m_movingRight = true;
                                                break;
                                        }
                                        break;
                                    case false:
                                        break;
                                }
                                // We use a sphere collider to detect the presence of a wall and if found, the enemy switches to is grounded.
                                wallInfo = Physics2D.OverlapCircle(m_wallDetection.position, m_wallCheckRadius, m_groundLayer);
                                switch (wallInfo)
                                {
                                    case true:
                                        m_isGround = true;
                                        break;
                                    case false:
                                        break;
                                }
                                break;
                        }
                        break;
                    case false:
                        //Sets the enemy to idle when isPatrolling is set to false
                        transform.Translate(Vector2.right * 0 * Time.deltaTime);
                        //Animate the movement
                        m_animator.SetFloat("WalkSpeed", Mathf.Abs(0));
                        break;
                }
                switch (m_numberOfTimesPlayerIsDetected >= 1)
                {
                    case true:
                        m_isPatrolling = true;
                        break;
                    case false:
                        break;
                }
                break;
            case false:
                // If a player is detected, then the counter increases by one. This counter sets the enemy to constantly patrol after seeing the player.
                m_numberOfTimesPlayerIsDetected += 1;
                switch (m_isPatrolling){
                    case true:
                        // Sets is patrolling to false if a player is detected.
                        m_isPatrolling = false;
                        break;
                    case false:
                        // If patrolling is false then the enemy moves towards the player.
                        // We use stopping distance to determine where the enemy stops to prevent collision with the player
                        switch(Vector2.Distance(transform.position, m_target.position)>m_stoppingDistance){
                            case true:
                                // Moves enemy towards player if the distance to player is greater than 1
                                transform.position  = Vector2.MoveTowards(transform.position, m_target.position, m_moveSpeed * Time.deltaTime);
                                // Animate the movement
                                m_animator.SetFloat("WalkSpeed", m_moveSpeed);
                                break;
                            case false:
                               //Sets the enemy to stop in front of the player
                            transform.Translate(Vector2.right * 0 * Time.deltaTime);
                            //Animate the movement
                            m_animator.SetFloat("WalkSpeed", Mathf.Abs(0));
                            break; 
                        }
                        break;   
                }
            break;
        }
        // Checks if player is in attacking range for attacking enemies.
        inRange = Physics2D.OverlapCircle(m_playerAttackRange.position, m_playerDetectRadius, m_PlayerLayer);
       

        // If a player is in range
        switch (inRange)
        {
            // The enemy attacks the player.
            case true:
                // Enemy Attack cooldown 
                switch (Time.time > m_nextAttackTime)
                {
                    case true:
                        //Set Attack Animation trigger
                        m_animator.SetTrigger("Attack");
                        // Pause between Attacks
                        m_nextAttackTime = Time.time + m_TimeBetweenAttacks;
                        break;
                    case false:
                        break;
                }
                break;
            case false:
                break;
        }
    }
     

    // Enemy takes damage after attack from player
    public void TakeDamage(int damage)
    {
        m_currentHealth -= damage;
        m_animator.SetTrigger("Hurt");

        if (m_currentHealth <= 0)
        {
            Die();
        }
    }
    // Enemy Attack Function
    void Attack()
    {
        switch (playerHealth.m_invunerability == 0)
        {
            case true:
                // We use an OverlapCircle to Detect the Player
                hitPlayer = Physics2D.OverlapCircle(m_attackPoint.position, m_attackRange, m_PlayerLayer);
                // If the player object is detected the player takes damage
                try
                {
                    switch (hitPlayer)
                    {
                        case true:
                            // Damage the player.
                            playerHealth.TakeDamage(m_attackDamage);

                            // Knockback the player
                            if (m_rigidbody.transform.position.x > m_player.position.x)
                            {
                                m_player.AddForce(transform.up * playerHealth.m_knockbackForceY + transform.right * (playerHealth.m_knockbackForceX));
                            }
                            else if (m_rigidbody.transform.position.x < m_player.transform.position.x)
                            {
                                m_player.AddForce(transform.up * playerHealth.m_knockbackForceY + transform.right * playerHealth.m_knockbackForceX);
                            }
                            break;
                        case false:
                            break;
                    }
                }catch(Exception e)
                {
                    Debug.LogException(e, this);
                }
                break;
            case false:
                break;
        }    
    }
    // Enemy Death function
    void Die()
    {
        //Death animation
        m_animator.SetBool("isDead", true);
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        //Disable the enemy collider on death and prevent the script from continuing
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;


    }

    private void OnDrawGizmosSelected()
    {
        if (m_attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(m_attackPoint.position, m_attackRange);
        Gizmos.DrawWireSphere(m_wallDetection.position, m_wallCheckRadius);
    }
}
