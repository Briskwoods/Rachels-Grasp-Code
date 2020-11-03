using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] public int m_maxHealth = 100;                      // Player max health variable.
    [SerializeField] public int m_currentHealth = 0;                    // Player current Health variable

    [SerializeField] public float m_knockbackForceX = 0;                // Amount of Force th player is knocked back with on the X axis
    [SerializeField] public float m_knockbackForceY = 0;                // Amount of force the player is knocked back wih on the Y axis

    [SerializeField] private GameManager gameManager;

    [SerializeField] private Transform m_respawnPoint;                  // "Underworld" respawn point

    [SerializeField] private Animator m_animator;                       // Player animation controller   

    [SerializeField] private Rigidbody2D m_player;                      // Player Rigidbody

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

        //m_enemy = GameObject.FindGameObjectsWithTag("Enemy");
    }

    public void TakeDamage(int damage)
    {


        StartCoroutine("GetInvunerable");
        m_animator.SetTrigger("Hurt");
        m_currentHealth -= damage;
        Invoke("resetInvulnerability", 2);
        if (m_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // Play the Death animation
        
        // On death timer before respawn
        gameManager.timerBeforeRespawn();
        
        // Do onDeath function
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
        yield return new WaitForSeconds(5f);
        Physics2D.IgnoreLayerCollision(10, 8, false);
        C.a = 1.0f;
        rend.material.color = C;
    }
}

