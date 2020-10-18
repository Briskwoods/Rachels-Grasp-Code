using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] public int m_maxHealth = 100;                      // Player max health variable.
    [SerializeField] public int m_currentHealth = 0;                    // Player current Health variable

    [SerializeField] public float m_knockbackForceX = 0;
    [SerializeField] public float m_knockbackForceY = 0;

    [SerializeField] private GameManager gameManager;

    
    [SerializeField] private Transform m_respawnPoint;                  // "Underworld" respawn point

    [SerializeField] private Animator m_animator;                       // Player animation controller   

    [SerializeField] private Rigidbody2D m_player;                      // Player Rigidbody

    [SerializeField] public GameObject[] m_enemy;                        // Used in determining enemy position

    private Renderer rend;                                              // Sprite Renderer, used in changing player trancparency on damage
    private Color C;                                                    // Sets player character to red upon damage

    // Start is called before the first frame update
    void Start()
    {
        // Set current health to max health
        m_currentHealth = m_maxHealth;
        gameManager.lastCheckpointPos = m_player.position;
        // Initialise Specified components
        m_player = GetComponent<Rigidbody2D>();
        rend = GetComponent<Renderer>();
        C = rend.material.color;

    }

    public void TakeDamage(int damage)
    {
        m_currentHealth -= damage;

        StartCoroutine("GetInvunerable");
        m_animator.SetTrigger("Hurt");

        foreach(GameObject enemy in m_enemy)
        {
            if (enemy.transform.position.x > m_player.transform.position.x)
            {
                m_player.AddForce(transform.up * m_knockbackForceY + transform.right * -m_knockbackForceX);
            }
            else if (enemy.transform.position.x < m_player.transform.position.x)
            {
                m_player.AddForce(transform.up * m_knockbackForceY + transform.right * m_knockbackForceX);
            }
        }
        Invoke("resetInvulnerability", 2);


        if (m_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Dead");
        // Rise at respawn point
        
        // Play the Death animation
        gameManager.timerBeforeRespawn();
        

        gameManager.onDeath();           
    }

    public void Heal(int healAmount)
    {
        // Heal Player
        m_currentHealth += healAmount;
        // Play heal effect on player

    }

    // Resets player invunerability
    public void resetInvulnerability()
    {
        Physics2D.IgnoreLayerCollision(10, 8, false);
    }

    // Allows Player to run past enemy after taking damage
    public IEnumerator GetInvunerable()
    {
        Physics2D.IgnoreLayerCollision(10, 8, true);
        C.a = 0.5f;
        rend.material.color = C;
        yield return new WaitForSeconds(3f);
        Physics2D.IgnoreLayerCollision(10, 8, false);
        C.a = 1.0f;
        rend.material.color = C;
    }
}

