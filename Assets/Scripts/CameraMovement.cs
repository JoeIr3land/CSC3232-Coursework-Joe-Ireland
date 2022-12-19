using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private GameObject[] staticTargets;
    [SerializeField]
    private Transform rotationTarget;
    [SerializeField]
    private Transform dynamicTarget_A;
    [SerializeField]
    private Transform dynamicTarget_B;

    [SerializeField]
    private PlayerChar playerA;
    [SerializeField]
    private PlayerChar playerB;

    [SerializeField]
    private float moveSpeed;

    private Vector3 currentTarget;


    // Update is called once per frame
    void OnEnable()
    {
        staticTargets = GameObject.FindGameObjectsWithTag("StaticCameraTarget");
    }

    void Update()
    {
        //Move camera gradually towards closest target position
        currentTarget = ChangeTarget();
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, moveSpeed);

        //Rotate camera towards centre
        transform.LookAt(rotationTarget);
    }

    Vector3 ChangeTarget()
    {
        //If both players are alive, move to the nearest dynamic point
        if (playerA.IsAlive() && playerB.IsAlive())
        {
            Vector3 distanceToA = dynamicTarget_A.position - transform.position;
            Vector3 distanceToB = dynamicTarget_B.position - transform.position;
            if (distanceToA.magnitude <= distanceToB.magnitude)
            {
                return dynamicTarget_A.position;
            }
            else
            {
                return dynamicTarget_B.position;
            }
        }
        //If one or more players are dead, move to the nearest static point
        else
        {
            Vector3 closestPosition = Vector3.positiveInfinity;
            foreach(GameObject target in staticTargets)
            {
                Vector3 distanceToClosestTarget = closestPosition - transform.position;
                Vector3 distanceToThisTarget = target.transform.position - transform.position;
                if (distanceToThisTarget.magnitude <= distanceToClosestTarget.magnitude)
                {
                    closestPosition = target.transform.position;
                }

            }
            return closestPosition;
        }
    }

}
