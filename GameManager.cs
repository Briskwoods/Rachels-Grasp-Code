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

        // Wait for a few seconds
        timerBeforeRespawn();
        // Spawn Player in the "underworld"
        m_player.position = underworldSpawnPoint.position;
        // Play spawn animation

    }

    public void onRespawn()
    {
        // On reach of gate respawn player at their death point

        // Set player health back to maxHealth
        playerHealth.m_currentHealth = playerHealth.m_maxHealth;

        // Wait for a few seconds
        timerBeforeRespawn();
        // Spawn Player in the "underworld"
        m_player.position = lastCheckpointPos;
        // Play spawn animation

    }

    public IEnumerator timerBeforeRespawn()
    {
        yield return new WaitForSeconds(3);
    }
}
