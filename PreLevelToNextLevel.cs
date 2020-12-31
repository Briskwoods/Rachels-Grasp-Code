using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreLevelToNextLevel : MonoBehaviour
{
    [SerializeField] public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D zone)
    {
        if (zone.CompareTag("Player"))
        {
            gameManager.LoadNextScene();
        }
    }
}
