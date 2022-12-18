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
    }

    void FixedUpdate()
    {
        //If a player is below the platform, ignore collision detection between them
        foreach (GameObject fighter in fighters)
        {
            Component[] collidersInFighter = fighter.GetComponent<PlayerChar>().GetHurtboxes();

            // +0.3f because the global position for the player is offset compared to the platform somehow? +0.3f stops player from falling through as long as they aren't crouching
            if (fighter.transform.position.y + 0.3f < platformHeight || fighter.GetComponent<CrouchControl>().CrouchInput_Held)
            {
                foreach (Component collider in collidersInFighter)
                {
                    Physics.IgnoreCollision(this.GetComponent<Collider>(), collider.GetComponent<Collider>(), true);
                }
            }
            else
            {
                foreach (Component collider in collidersInFighter)
                {
                    Physics.IgnoreCollision(this.GetComponent<Collider>(), collider.GetComponent<Collider>(), false);
                }
            }
        }
    }
}
