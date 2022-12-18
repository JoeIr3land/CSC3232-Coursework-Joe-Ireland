using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTarget : MonoBehaviour
{
    [SerializeField]
    private Transform centreTarget;

    [SerializeField]
    private float moveSpeed;

    //Allows camera to rotate continuously
    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, centreTarget.position, moveSpeed);
    }
}