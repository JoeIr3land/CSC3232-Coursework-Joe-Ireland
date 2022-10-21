using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerChar : MonoBehaviour
{

    /*
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * TODO: move crouch to its own script
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     */
    

    //Character Stats
    [SerializeField]
    public float groundSpeed;

    //Player State
    public enum playerState { grounded_idle, running, crouching }
    [SerializeField]
    public playerState currentState;

    //Controls state - is a button being held?

    bool CrouchInput_Held;

    //Components
    Rigidbody body;
    Animator animator;


    void Start()
    {

        //Get components
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        //Set initial player state
        setCurrentState(playerState.grounded_idle);

        //Set initial controls state
        CrouchInput_Held = false;

    }


    void FixedUpdate()
    {
        //Crouch
        if (CrouchInput_Held)
        {
            /*
             *TODO: behaviour while holding crouch (down SDI,TDI and DDI, maybe other mechanics)
             */
        }
    }

    // Change player state
    public void setCurrentState(playerState state)
    {
        currentState = state;
    }


    //Called when crouch is inputted
    public void CrouchInput(InputAction.CallbackContext context)
    {
        Debug.Log(context.phase);

        switch (context.phase)
        {
            case InputActionPhase.Started:
                setCurrentState(playerState.crouching);
                CrouchInput_Held = true;
                animator.SetTrigger("StartCrouching");
                break;
                /*TODO - different actions depending on context of input
                 * 
                 * idle_grounded, walking or running: crouch
                 * sprinting, jumpsquat or deep into an attack: ignore input initially but store for TDI
                 * within a few frames of/simultaneously with inputting an attack: down attack
                 * idle_air: fastfall
                 * hitlag: store for downwards TDI
                 * knockback: downwards drift DI
                 */

            case InputActionPhase.Canceled:
                //Update control state
                CrouchInput_Held = false;
                //Cancel crouch input depending on player state
                switch (currentState)
                {
                    //If player was crouching, transition to idle pose
                    case playerState.crouching:
                        setCurrentState(playerState.grounded_idle);
                        animator.SetTrigger("StandStill");
                        break;
                }
                break;
        }
    }
}