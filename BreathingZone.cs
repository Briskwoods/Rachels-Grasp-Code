using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingZone : MonoBehaviour
{
    [SerializeField] private PlayerHealthManager playerHealth;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag == "Player")
        {
            case true:
                playerHealth.m_holdBreath = 30f;
                break;
            case false:
                break;
        }
    }
}
