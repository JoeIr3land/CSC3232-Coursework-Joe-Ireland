using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastZone : MonoBehaviour
{
    AudioSource clip;

    void OnEnable()
    {
        clip = GetComponent<AudioSource>();
    }


    void OnTriggerExit(Collider other)
    {
        PlayerChar player = other.GetComponentInParent<PlayerChar>();

        if (player != null)
        {
            player.KillPlayer();
            clip.Play();

        }
    }
}
