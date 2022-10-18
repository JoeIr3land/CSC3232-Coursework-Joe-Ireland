using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerChar : MonoBehaviour
{
    [SerializeField]
    private float groundSpeed;

    private Vector2 moveInput;
    private Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 velocity = body.velocity;
        velocity.x = moveInput.x * groundSpeed;
        velocity.z = moveInput.y * groundSpeed;
        body.velocity = velocity;
    }

    void OnMove(InputValue value) //TODO: Figure out why OnMove stops being called when the input stays the same
    {
        Rigidbody body = GetComponent<Rigidbody>();
        moveInput = value.Get<Vector2>();

        if(moveInput.x != 0.0f && moveInput.y != 0.0f)
        {
            Vector3 lookDirection = new Vector3(moveInput.x, 0.0f, moveInput.y);
            transform.rotation = Quaternion.LookRotation(lookDirection);
            GetComponent<Animator>().SetBool("isRunning", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("isRunning", false);
        }
    }

    bool OnJump()
    {
        Debug.Log("Jump!");
        return true;
    }

    void OnLightAttack()
    {
        if (OnJump()) { Debug.Log("Up-Light"); }
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

}
