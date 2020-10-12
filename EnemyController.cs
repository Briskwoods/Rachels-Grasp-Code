using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int m_maxHealth = 100;                     // Enemy max health variable.
    [SerializeField] private int m_currentHealth;                       // Enemy current Health variable
    [SerializeField] private float m_attackRange = 0.5f;                // Enemy attack range.
    [SerializeField] private float m_patrolSpeed;                       // Patrol Speed of the enemy.
    [SerializeField] private float m_groundCheckDistance;               // Height for ground check distance when patrolling on a platform.
    [SerializeField] private float m_wallCheckRadius;                   // Radius of wall check cicrcle collider when patrolling on ground
    [SerializeField] private float m_moveSpeed;                         // Speed at which enemy moves towards the player after player detection
    [SerializeField] private float m_playerDetectRadius;                // Enemy radius to detect if player is in attacking range

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
    private bool wallInfo;
    private bool m_movingRight = true;                                  // Used in flipping the enemy sprite when the enemy is turning
    private bool inRange;

    private int m_numberOfTimesPlayerIsDetected = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        m_currentHealth = m_maxHealth;
        m_target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //This line prevents the raycast from detecting the enemy's box colliders
        Physics2D.queriesStartInColliders = false;
        //We use a Linecast to detect the presence of a player and if found the enemy can move towards the player.
        playerSeen = Physics2D.Linecast(m_lineOfSightStart.position, m_lineOfSightEnd.position,m_PlayerLayer);       
        Debug.DrawLine(m_lineOfSightStart.position, m_lineOfSightEnd.position, new Color(1f,1f,1f));

        switch (playerSeen)
        {
            case true:
                // Used in setting the enemy to patrol after player detection
                m_numberOfTimesPlayerIsDetected = 1;
                // Animate React
                m_animator.SetTrigger("SeePlayer");
                // Sets Patrolling to false to prevent clash.
                m_isPatrolling = false;
                //Move to player
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(m_target.transform.position.x, transform.position.y), m_moveSpeed * Time.deltaTime);
                break;
            case false:
                // Sets patrolling back to true after player detection.
                switch (m_numberOfTimesPlayerIsDetected == 1) {
                    case true:
                        m_isPatrolling = true;
                        break;
                    case false:
                        break;        
                }
                // Checks if the enemy is patrolling
                switch (m_isPatrolling)
                {
                    case true:
                        //Move Enemy on patrol route
                        transform.Translate(Vector2.right * m_patrolSpeed * Time.deltaTime);
                        //Animate the movement
                        m_animator.SetFloat("WalkSpeed", Mathf.Abs(m_patrolSpeed));
                        // Checks if the enemy is on the ground or not. If they're not on the ground then they're on a floating platform.
                        switch (m_isGround)
                        {
                            case true:
                            // We use a sphere collider to detect the presence of a wall and if found, the enemy can turn as they patrol.
                            wallInfo = Physics2D.OverlapCircle(m_wallDetection.position, m_wallCheckRadius,m_groundLayer);
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
                                RaycastHit2D GroundInfo = Physics2D.Raycast(m_groundDetection.position, Vector2.down, m_groundCheckDistance);
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
                                //If there is no ground information
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
                break;
        }

        // We use a sphere collider to detect the presence of a wall and if found, the enemy can turn as they patrol.
        inRange = Physics2D.OverlapCircle(m_playerAttackRange.position, m_playerDetectRadius, m_PlayerLayer);

        switch(inRange)
        {
            case true:
                Debug.Log("Attacked");
                break;
            case false:
                break;
                
        }

    }

    private void FixedUpdate()
    {


    }

    public void TakeDamage(int damage)
    {
        m_currentHealth -= damage;
        m_animator.SetTrigger("Hurt");

        if (m_currentHealth <= 0)
        {
            Die();
        }
    }

    void Attack()
    {

    }

    void Die()
    {
        //Death animation
        m_animator.SetBool("isDead", true);
        
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
        Gizmos.DrawWireSphere(m_playerAttackRange.position, m_playerDetectRadius);
    }
}
