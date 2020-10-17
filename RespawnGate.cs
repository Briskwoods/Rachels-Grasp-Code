using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnGate : MonoBehaviour
{
    [SerializeField] public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D gate)
    {
        if (gate.CompareTag("Player"))
        {
            gameManager.onRespawn();
        }
    }
}
