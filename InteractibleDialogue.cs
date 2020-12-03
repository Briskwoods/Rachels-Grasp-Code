using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractibleDialogue : MonoBehaviour
{
    [SerializeField] private Transform m_detectionPoint;

    [SerializeField] private float m_detectionRadius;

    [SerializeField] private LayerMask m_player;

    [SerializeField] private Animator m_interactPrompt;

    [SerializeField] private DialogueTrigger conversation;

    private bool playerDetected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerDetected = Physics2D.OverlapCircle(m_detectionPoint.position, m_detectionRadius, m_player);
        switch (playerDetected)
        {
            case true:
                m_interactPrompt.SetBool("inRange", true);
                switch (playerDetected && Input.GetButtonDown("Interact"))
                {
                    case true:
                        m_interactPrompt.SetBool("inRange", false);
                        conversation.TriggerDialogue();
                        break;
                    case false:
                        break;
                }

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
