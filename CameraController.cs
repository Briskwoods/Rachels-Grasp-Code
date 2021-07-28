using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineBrain m_brain;
    [SerializeField] private PlayerMovement m_playerInput;

    //[SerializeField] private GameObject m_camra;
    //private CinemachineVirtualCamera m_vcam;
    //private CinemachineFramingTransposer m_transposer;


    //[SerializeField] private CharacterController2D m_player;
    //[SerializeField] private PlayerMovement m_controller;

    private void Start()
    {
    //    m_vcam = m_camra.GetComponent<CinemachineVirtualCamera>();
    //    m_transposer = m_vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Update()
    {
       switch (m_brain.IsBlending /*&& !m_controller.m_isCrouching*/)
        {
            case true:
                m_playerInput.enabled = false;
                Time.timeScale = 0;
                break;
            case false:
                Time.timeScale = 1;
                m_playerInput.enabled = true;
                break;
        } 

        //switch (m_player.m_isWallSliding)
        //{
        //    case true:
        //        m_transposer.m_TrackedObjectOffset.y = -10;
        //        break;
        //    case false:
        //        m_transposer.m_TrackedObjectOffset.y = 2;
        //        break;
        //}

        //switch(m_player.m_isNearLedge && m_controller.m_isCrouching)
        //{
        //    case true:
        //        m_transposer.m_TrackedObjectOffset.y = -10;
        //        break;
        //    case false:
        //        m_transposer.m_TrackedObjectOffset.y = 2;
        //        break;
        //}
    }
}
