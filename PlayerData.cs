using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData : MonoBehaviour
{
    public int level;
    public int health;
    public float[] position;

    public PlayerData(PlayerHealthManager player, GameManager game, Rigidbody2D bodyPosition)
    {
        level = SceneManager.GetActiveScene().buildIndex;
        health = player.m_currentHealth;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

    }

}
