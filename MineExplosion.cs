using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineExplosion : MonoBehaviour
{
    [SerializeField] private PlayerHealthManager m_playerHealth;

    [SerializeField] private int m_damage;

    [SerializeField] private GameObject explosion;

    [SerializeField] private Rigidbody2D m_player;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            Destroy(gameObject);
            Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);
            m_playerHealth.TakeDamage(m_damage);

            // Knockback the player
            if (transform.position.x > m_player.position.x)
            {
                m_player.AddForce(transform.up * m_playerHealth.m_knockbackForceY + transform.right * (m_playerHealth.m_knockbackForceX));
            }
            else if (transform.position.x < m_player.transform.position.x)
            {
                m_player.AddForce(transform.up * m_playerHealth.m_knockbackForceY + transform.right * m_playerHealth.m_knockbackForceX);
            }
        }
        
    }
}
