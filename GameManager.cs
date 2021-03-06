﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Transform underworldSpawnPoint;                  // Spawn point upon death
    [SerializeField] private Transform respawnPoint;                          // Spawn point upon revival

    [SerializeField] private GameObject RespawnGate;                          // Spawn Point Gate

    [SerializeField] private PlayerHealthManager playerHealth;                // Sets the player health back to 100 upon revive

    [SerializeField] private PlayerMovement playerMovement;                   // Disables player movement accordingly

    [SerializeField] private Rigidbody2D m_player;                            // Used to move player between positions

    [SerializeField] private Animator playerAnimator;                         // Used in death animations

    [SerializeField] private GameObject normalBackground;                     // Normal backgorund
    [SerializeField] private GameObject deathBackground;                      // Background on death
    [SerializeField] private GameObject bgColour;                             // Normal Background Colour
    [SerializeField] private GameObject deathBgColour;                        // Background colour after death
    
    public Vector2 lastCheckpointPos;


    private void Awake()
    {
        Time.timeScale = 1f;
    }
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void onDeath() {

        // Set Dark Forest BG to active and normal bg to inactive
        deathBgColour.SetActive(true);
        deathBackground.SetActive(true);
        normalBackground.SetActive(false);
        bgColour.SetActive(false);
        // Wait for a few seconds
        StartCoroutine("timerBeforeRespawn");
        // Play Death animation animation
        playerMovement.enabled = false;
        // Spawn Player in the "underworld"
        StopCoroutine("timerBeforeRespawn");
        playerMovement.enabled = true;
        m_player.position = underworldSpawnPoint.position;
    }

    public void onRespawn()
    {
        // Set backgrounds back to normal
        deathBgColour.SetActive(false);
        deathBackground.SetActive(false);
        normalBackground.SetActive(true);
        bgColour.SetActive(true);
        // On reach of gate respawn player at their death point
        // Set player health back to maxHealth
        playerHealth.m_currentHealth = playerHealth.m_maxHealth;
        // Wait for a few seconds
        playerMovement.enabled = false;
        StartCoroutine("timerBeforeRespawn");
        // Play spawn animation

        // Spawn Player in the "underworld"
        StopCoroutine("timerBeforeRespawn");
        playerMovement.enabled = true;
        m_player.position = lastCheckpointPos;        
    }

    // Used to load the next scene
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Used to stop player movement
    public void StopPlayer()
    {
        m_player.velocity = new Vector2(0, 0);
        playerAnimator.SetFloat("Speed", 0);
    }


    public IEnumerator timerBeforeRespawn()
    {
        yield return new WaitForSeconds(3f);
    }
}
