using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{

    // We use this script to trigger the beginning of a dialogue.
    public Dialogue dialogue;

    public void TriggerDialogue() {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    public void EndDialogue() {
        FindObjectOfType<DialogueManager>().DisplayNextSentence();
    }

    public void DisplayNextLine() {
        FindObjectOfType<DialogueManager>().EndDialogue();
    }
}
