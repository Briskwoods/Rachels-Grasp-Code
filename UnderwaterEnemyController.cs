using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterEnemyController : MonoBehaviour
{
    [SerializeField] private float m_patrolSpeed = 1f;                  // Patrol Speed of the enemy.
    
    [SerializeField] private float m_wallCheckRadius;                   // Radius of wall check cicrcle collider when patrolling on ground
    [SerializeField] private float m_moveSpeed = 2f;                    // Speed at which enemy moves towards the player after player detection
    [SerializeField] private float m_playerDetectRadius;                // Enemy radius to detect if player is in attacking range

    [SerializeField] public float m_knockbackForceX = 100f;
    [SerializeField] public float m_knockbackForceY = 100f;

    [SerializeField] private PlayerHealthManager playerHealth;          // For interactions with the player health manager
    
    [SerializeField] private Transform m_enemy;                       // Used in flipping the enemy sprite when player is detected
    [SerializeField] private Transform m_target;                        // Where we declare the player as the target for the enemy
    [SerializeField] private Transform m_wallDetection;                 // Detects the wall for enemy patrol.
    [SerializeField] private Transform m_playerInRange;                 // Range in which the enemy follows the player

    [SerializeField] private LayerMask m_groundLayer;                   // Declare the ground layer for wall detection on patrol.
    [SerializeField] private LayerMask m_PlayerLayer;                   // Declare the player layer for player detection.

    [SerializeField] private bool m_isPatrolling;                       // Decided if the enemy is idle or patrolling.

    private bool wallInfo;                                              // Detects the precense of a wall.
    private bool m_movingRight = true;                                  // Used in flipping the enemy sprite when the enemy is turning
    private bool inRange;                                               // Detects if player is in range.
    
    private int m_numberOfTimesPlayerIsDetected = 0;                    // Sets enemy to patrol after detecting player

    // Start is called before the first frame update
    void Start()
    {
        // Sets the number of times a player is detected to 0, this allows us to have idle enemies waiting to detect the player and move.
        m_numberOfTimesPlayerIsDetected = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //This line prevents the raycast from detecting the enemy's box colliders
        Physics2D.queriesStartInColliders = false;
        // Checks if enemy is in range to follow the player.
        inRange = Physics2D.OverlapCircle(m_playerInRange.position, m_playerDetectRadius, m_PlayerLayer);
        // If a player is in range
        switch (!inRange)
        {
            case true:
                // Checks if the enemy is patrolling
                switch (m_isPatrolling)
                {
                    case true:
                        //Move Enemy on patrol route
                        transform.Translate(Vector2.right * m_patrolSpeed * Time.deltaTime);
                        // We use a sphere collider to detect the presence of a wall and if found, the enemy can turn as they patrol.
                        wallInfo = Physics2D.OverlapCircle(m_wallDetection.position, m_wallCheckRadius, m_groundLayer);
                        // If a wall is detected then we turn the enemy around.
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
                        break;
                    case false:
                        //Sets the enemy to idle when isPatrolling is set to false
                        transform.Translate(Vector2.right * 0 * Time.deltaTime);
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
                switch (m_isPatrolling)
                {
                    case true:
                        // Sets is patrolling to false if a player is detected.
                        m_isPatrolling = false;
                        break;
                    case false:
                        // If patrolling is false then the enemy moves towards the player.
                        transform.position = Vector2.MoveTowards(transform.position, m_target.position, m_moveSpeed * Time.deltaTime);
                        switch (m_target.position.x > m_enemy.position.x)
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
                }
                break;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(m_playerInRange.position, m_playerDetectRadius);
        Gizmos.DrawWireSphere(m_wallDetection.position, m_wallCheckRadius);
    }
}
