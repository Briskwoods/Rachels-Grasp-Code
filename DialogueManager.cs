using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;                                       // Holds the NPC of the character speaking
    public Text dialogueText;                                   // Where the dialogue shows up
        
    [SerializeField] private Animator m_animator;               // Controls the dialogue animation

    private Queue<string> sentences;                            // These are the dialogue sentences that will be shown
    private Queue<string> names;                                // In the event the dialogue has more than one conversant

    private InteractibleDialogue[] interactibles;               // Returns a list of all interactibles in the scene and disables interactions during a dialogue and re-enables after
 
    // Start is called before the first frame update
    void Start()
    {
        names = new Queue<string>();
        sentences = new Queue<string>();
        interactibles = FindObjectsOfType<InteractibleDialogue>();

    }

    public void StartDialogue(Dialogue dialogue)
    {
        // When we start a dialogue, we want the player to stop moving
        GameObject.Find("Player").GetComponent<PlayerMovement>().enabled = false;
        // We prevent the interact button from being pressed again by disabling the interact script during dialogue
        foreach(InteractibleDialogue interactible in interactibles)
        {
            interactible.enabled = false;
        }
        // Hide other UI elements

        // Show the dialogue box
        m_animator.SetBool("isTalking", true);
        // Start the conversation with the entity
        // Set the name for the character in the dialogue box
        nameText.text = dialogue.name[0];
        names.Clear();
        foreach (string name in dialogue.name) 
        {
            names.Enqueue(name); 
        }
        // This line clears any dialogue lines done previously.
        sentences.Clear();
        // We add our sentences into the queue
        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        // This line displayes the first dialogue sentence.
        DisplayNextSentence();
        Cursor.lockState = CursorLockMode.None;
    }


    // When alled outside the start dialogue function, this function displays the Next Sentence
    public void DisplayNextSentence()
    {
        // It checks  for ones position in the queue and if the number of sentences in the queue is 0 then we end the conversation
        if (sentences.Count == 0)
        {
                EndDialogue();
                return;
        }
        // After displaying the sentence it dequeues the sentence from the Queue
        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
        string name = names.Dequeue();
        nameText.text = name;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
        Cursor.lockState = CursorLockMode.None;
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    public void EndDialogue()
    {
        // On the end of the dialogue we hide the dialogue box on the UI
        m_animator.SetBool("isTalking", false);
        // Unhide other UI elements

        // Player is able to move
        GameObject.Find("Player").GetComponent<PlayerMovement>().enabled = true;
        // Interactibles can now function normally
        // We prevent the interact button from being pressed again by disabling the interact script during dialogue
        foreach (InteractibleDialogue interactible in interactibles)
        {
            interactible.enabled = true;
        }
        Cursor.lockState = CursorLockMode.Locked;
    }
}
