using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemiSolidPlatform : MonoBehaviour
{
    float platformHeight;

    GameObject[] fighters;

    void OnEnable()
    {
        platformHeight = transform.position.y;
        fighters = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(fighters[0]);
    }

    void FixedUpdate()
    {
        //If a player is below the platform, ignore collision detection between them
        foreach (GameObject fighter in fighters)
        {

            if (fighter.transform.position.y < platformHeight || fighter.GetComponent<CrouchControl>().CrouchInput_Held)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), fighter.GetComponent<Collider>(), true);
            }
            else
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), fighter.GetComponent<Collider>(), false);
            }
        }
    }
}
