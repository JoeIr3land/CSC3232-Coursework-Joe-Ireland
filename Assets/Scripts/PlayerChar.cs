using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerChar : MonoBehaviour
{
    [SerializeField]
    private float groundSpeed;
    private Rigidbody body;
    private Vector2 moveInput;

    private Vector3 lookDirection;

    void FixedUpdate()
    {
        Move();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        lookDirection.x = moveInput.x; //TODO: Figure out how this is updating with 0 input
        lookDirection.z = moveInput.y;

    }

    bool OnJump()
    {
        Debug.Log("Jump!");
        return true;
    }

    void OnLightAttack()
    {
        if (OnJump()) { Debug.Log("Down-Light"); }
        Debug.Log("Neutral Light!");
    }

    void OnStrongAttack()
    {
        if(moveInput.x == 0 && moveInput.y ==0)
        {
            Debug.Log("Neutral Strong!");
        }
    }

    void OnSpecialAttack()
    {
        Debug.Log("Neutral Special!");
    }

    void OnShield()
    {
        Debug.Log("Shield!");
    }

    void OnCrouch()
    {
        Debug.Log("Crouch!");
    }

    private void Move()
    {

        body = GetComponent<Rigidbody>();
        Vector3 velocity = body.velocity;

        velocity.x = moveInput.x * groundSpeed;
        velocity.z = moveInput.y * groundSpeed;

        transform.rotation = Quaternion.LookRotation(lookDirection);

        body.velocity = velocity;
    }

}
