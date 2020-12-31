using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogue : MonoBehaviour
{
    [SerializeField] private DialogueTrigger dialogue;
    [SerializeField] private GameManager gameManager;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            gameManager.StopPlayer();
            dialogue.TriggerDialogue();
            Destroy(gameObject);
        }
    }
}
