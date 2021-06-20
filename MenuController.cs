using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static bool isPaused = false;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject player;

    [SerializeField] private AudioSource music;

    [SerializeField] private PlayerHealthManager playerHealth;
    [SerializeField] private Rigidbody2D bodyPosition;
    [SerializeField] private GameManager manager;



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
                Pause();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Time.timeScale = 1f;
    }
   
    public void QuitGame()
    {
        //If we are running in a standalone build of the game
#if UNITY_STANDALONE
        //Quit the application
        Application.Quit();
#endif

        //If we are running in the editor
#if UNITY_EDITOR
        //Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        player.SetActive(true);
        music.Play();
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        player.SetActive(false);
        music.Pause();
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        isPaused = false;
    }

    public void Save()
    {
        // To do Later, customised save system

        PlayerPrefs.SetInt("Level", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.SetInt("Health", playerHealth.m_currentHealth);
        PlayerPrefs.SetFloat("Last Checkpoint Positon X" , manager.lastCheckpointPos.x);
        PlayerPrefs.SetFloat("Last Checkpoint Positon Y", manager.lastCheckpointPos.y);
    }

    public void Load()
    {
        // To do Later, customised load system

        SceneManager.LoadScene(PlayerPrefs.GetInt("Level"));
        playerHealth.m_currentHealth = PlayerPrefs.GetInt("Health");
        bodyPosition.position = new Vector2(PlayerPrefs.GetFloat("Last Checkpoint Positon X"), PlayerPrefs.GetFloat("Last Checkpoint Positon Y"));
    }
}
