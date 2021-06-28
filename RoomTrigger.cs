using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] private GameObject m_mainCamera;
    [SerializeField] private GameObject m_roomCamera;

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch(other.tag == "Player")
        {
            case true:
                m_mainCamera.SetActive(false);
                m_roomCamera.SetActive(true);
                break;
            case false:
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        switch (other.tag == "Player")
        {
            case true:
                m_mainCamera.SetActive(true);
                m_roomCamera.SetActive(false);
                break;
            case false:
                break;
        }
    }
}
