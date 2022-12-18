using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastZone : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        PlayerChar player = other.GetComponentInParent<PlayerChar>();

        if (player != null)
        {
            Debug.Log("Killing:" + other.name);
            player.KillPlayer();
        }
    }
}
