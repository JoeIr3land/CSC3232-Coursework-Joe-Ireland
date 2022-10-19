using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerChar : MonoBehaviour
{
    [SerializeField]
    private float groundSpeed;

    enum playerState
    {
        idle, running, crouching
    }
    [SerializeField]
    playerState currentState;

    private Vector2 moveInput;
    private Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        setCurrentState(playerState.idle);
    }

    void FixedUpdate()
    {
        switch (currentState) {
            case playerState.idle:
                //Change animation
                GetComponent<Animator>().SetBool("isRunning", false);
                GetComponent<Animator>().SetBool("isCrouching", false);
                break;
            case playerState.running:
                //Change animation
                GetComponent<Animator>().SetBool("isRunning", true);

                //Change player orientation
                Vector3 lookDirection = new Vector3(moveInput.x, 0.0f, moveInput.y);
                transform.rotation = Quaternion.LookRotation(lookDirection);

                //Move player
                Vector3 velocity = body.velocity;
                velocity.x = moveInput.x * groundSpeed;
                velocity.z = moveInput.y * groundSpeed;
                body.velocity = velocity;
                break;
            case playerState.crouching:
                //Change animation
                GetComponent<Animator>().SetBool("isRunning", false);
                GetComponent<Animator>().SetBool("isCrouching", true);
                break;
        }
    }

    void setCurrentState(playerState state)
    {
        currentState = state;
    }

    void OnMove(InputValue value) //TODO: Figure out why OnMove stops being called when the input stays the same
    {
        Rigidbody body = GetComponent<Rigidbody>();
        moveInput = value.Get<Vector2>();

        if((moveInput.x != 0.0f && moveInput.y != 0.0f) && (currentState == playerState.idle || currentState == playerState.running))
        {
            setCurrentState(playerState.running);
        }
        else
        {
            if (currentState == playerState.running) { setCurrentState(playerState.idle); }
        }
    }

    void OnCrouch()
    {
        Debug.Log("Crouch!");
        setCurrentState(playerState.crouching);

    }

    void OnJump()
    {
        Debug.Log("Jump!");
        Rigidbody body = GetComponent<Rigidbody>();
    }

    void OnLightAttack()
    {
        Debug.Log("Neutral Light Attack!");
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


}
