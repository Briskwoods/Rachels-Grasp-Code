using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prompt : MonoBehaviour
{

    [SerializeField] private Transform m_detectionPoint;            // Position where the detet radius begins

    [SerializeField] private float m_detectionRadius;               // Detection radius

    [SerializeField] private LayerMask m_player;                    // Identify the player to trigger prompt

    [SerializeField] EnemyController m_enemy;                       // Only show the prompt when enemy health is > 0
    
    [SerializeField] Animator m_interactPrompt;                     // Show prompt

    private bool playerDetected;

    // Update is called once per frame
    void Update()
    {
        playerDetected = Physics2D.OverlapCircle(m_detectionPoint.position, m_detectionRadius, m_player);

        switch (playerDetected && m_enemy.m_currentHealth > 0)
        {
            case true:
                m_interactPrompt.SetBool("inRange", true);
                break;
            case false:
                m_interactPrompt.SetBool("inRange", false);
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(m_detectionPoint.position, m_detectionRadius);
    }
}
