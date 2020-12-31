using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    [Range(0, 100)][SerializeField] public int m_maxHealth = 100;       // Player max health variable.
    [SerializeField] public int m_currentHealth = 0;                    // Player current Health variable
    [SerializeField] public int m_drownDamage = 5;                      // Damage to the player when drowning


    [SerializeField] public float m_knockbackForceX = 0;                // Amount of Force th player is knocked back with on the X axis
    [SerializeField] public float m_knockbackForceY = 0;                // Amount of force the player is knocked back wih on the Y axis
    [Range(0, 30)][SerializeField] public float m_holdBreath = 30f;    // How long the player can hold their breath.

    [SerializeField] private GameManager gameManager;

    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private Transform m_respawnPoint;                  // "Underworld" respawn point

    [SerializeField] private Animator m_animator;                       // Player animation controller   

    [SerializeField] private Rigidbody2D m_player;                      // Player Rigidbody

    [SerializeField] public int m_invunerability = 0;                   // Sets player vunerability state, used to prevent damage whenn vunerable

    private Renderer rend;                                              // Sprite Renderer, used in changing player trancparency on damage
    private Color C;                                                    // Sets player character to red upon damage

    private bool m_canBreath;                                           // Detects if player is in a position to breathe or not

    [SerializeField] private Image healthBar;
    [SerializeField] private Image breathBar;


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

    private void FixedUpdate()
    {
        m_canBreath = !playerMovement.headSubmerged;

        switch (playerMovement.isSwimming && !m_canBreath)
        {
            case true:
                m_holdBreath -= Time.deltaTime;
                switch (m_holdBreath < 0 ) {
                    case true:
                        switch (m_invunerability == 0)
                        {
                            case true:
                                TakeDamage(m_drownDamage);
                                break;
                            case false:
                                break;
                            }
                        break;
                    case false:
                        break;

                }
                break;
            case false:
                m_holdBreath = 30f;
                break;
        }

        healthBar.fillAmount = (float)m_currentHealth /(float) m_maxHealth;
        breathBar.fillAmount = m_holdBreath / 30;

    }

    public void TakeDamage(int damage)
    {
        switch (m_invunerability == 0)
        {
            case true:               
                StartCoroutine("GetInvunerable");               
                m_currentHealth -= damage;
                m_animator.SetTrigger("Hurt");
                StopCoroutine("GetInvunerable");
                break;
            case false:
                break;

        }
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
        m_invunerability = 0;
        Physics2D.IgnoreLayerCollision(10, 8, false);
    }

    // Allows Player to run past enemy after taking damage
    public IEnumerator GetInvunerable()
    {
        m_invunerability = 1;
        Physics2D.IgnoreLayerCollision(10, 8, true);
        yield return new WaitForSeconds(3f);
        Physics2D.IgnoreLayerCollision(10, 8, false);
    }
}

