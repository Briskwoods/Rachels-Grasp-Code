using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesController : MonoBehaviour
{
    [SerializeField] private PlayerHealthManager playerHealth;
    [SerializeField] private int spikeDamage;
    [SerializeField] private Rigidbody2D m_player;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Player")
        {
            playerHealth.TakeDamage(spikeDamage);

            // Knockback the player
            if (transform.position.x > m_player.position.x)
            {
                m_player.AddForce(transform.up * playerHealth.m_knockbackForceY + transform.right * (playerHealth.m_knockbackForceX));
            }
            else if (transform.position.x < m_player.transform.position.x)
            {
                m_player.AddForce(transform.up * playerHealth.m_knockbackForceY + transform.right * playerHealth.m_knockbackForceX);
            }
        }
    }
}
