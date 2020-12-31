using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopPlayer : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch(collision.tag == "Player")
        {
            case true:
                gameManager.StopPlayer();
                break;
            case false:
                break;
        }
    }
}
