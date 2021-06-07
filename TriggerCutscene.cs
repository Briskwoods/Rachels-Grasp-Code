using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TriggerCutscene : MonoBehaviour
{
    [SerializeField] private PlayableDirector m_cutsceneDirector;
    [SerializeField] private GameObject m_cutscene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_cutscene.SetActive(true);
            m_cutsceneDirector.Play();
        }
    }
}
