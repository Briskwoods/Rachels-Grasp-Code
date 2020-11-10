using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [SerializeField] private Transform underworldSpawnPoint;
    [SerializeField] private Transform respawnPoint;

    [SerializeField] private GameObject RespawnGate;

    [SerializeField] private PlayerHealthManager playerHealth;

    [SerializeField] private Rigidbody2D m_player;

    [SerializeField] private GameObject normalBackground;
    [SerializeField] private GameObject deathBackground;
    [SerializeField] private GameObject bgColour;
    [SerializeField] private GameObject deathBgColour;
    
    public Vector2 lastCheckpointPos;

    private void Awake()
    {
        switch (instance == null)
        {
            case true:
                instance = this;
                DontDestroyOnLoad(instance);
                break;
            case false:
                Destroy(gameObject);
                break;
        }    
    }

    // Start is called before the first frame update
    void Start()
    {
       
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
        
        // Spawn Player in the "underworld"
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
        StartCoroutine("timerBeforeRespawn");
        // Play spawn animation

        // Spawn Player in the "underworld"
        m_player.position = lastCheckpointPos;
    }

    public IEnumerator timerBeforeRespawn()
    {
        yield return new WaitForSeconds(3f);
    }
}
