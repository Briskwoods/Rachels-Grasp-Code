using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPhysics : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject Player;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }
}
