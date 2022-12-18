using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxAttack : MonoBehaviour
{
    // Hitbox stats
    [SerializeField]
    private float damage = 1f;
    [SerializeField]
    private float knockbackBase = 1f;
    [SerializeField]
    private float knockbackScale = 1f;
    [SerializeField]
    private Vector3 knockbackAngle = new Vector3(0, 0, 0);

    [SerializeField]
    private PlayerChar player;

    //Only hit target once
    private bool canAttack;

    void OnEnable()
    {
        canAttack = true;
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerChar otherPlayer = other.GetComponentInParent<PlayerChar>();

        if (otherPlayer != null && other.GetComponent<Collider>().enabled && canAttack)
        {
            Debug.Log("Attacking:" + other.name);
            Vector3 attackDirection = other.transform.position - player.transform.position;
            Debug.Log("attackDirection=" + attackDirection + " knockbackAngle = " + knockbackAngle + "overallknockbackdirection=" + Vector3.Scale(attackDirection.normalized, knockbackAngle.normalized));
            otherPlayer.ApplyDamage(damage, knockbackBase, knockbackScale, Vector3.Scale(attackDirection.normalized, knockbackAngle.normalized));
            canAttack = false;
        }
    }


}
