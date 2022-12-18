using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrouchControl : MonoBehaviour
{
    //Components
    //Access player state and stats from PlayerChar component
    PlayerChar player;
    Rigidbody body;
    Animator animator;
    VariableGravity gravity;

    //Controls State
    public bool CrouchInput_Held;

    void OnEnable()
    {
        player = GetComponent<PlayerChar>();
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        gravity = GetComponent<VariableGravity>();
        CrouchInput_Held = false;
    }

    void FixedUpdate()
    {

        if(player.currentState == PlayerChar.playerState.crouching)
        {
            animator.SetTrigger("StartCrouching");
            animator.ResetTrigger("StartRunning");
        }


        //Crouch
        if (CrouchInput_Held)
        {
            switch (player.currentState)
            {
                case PlayerChar.playerState.grounded_idle:
                    player.setCurrentState(PlayerChar.playerState.crouching);
                    animator.SetTrigger("StartCrouching");
                    break;
            }
            /*
             *TODO: behaviour while holding crouch (down SDI,TDI and DDI, maybe other mechanics)
             */
        }
        else
        {
            if (GetComponent<EnemyBehaviour>() == null)
            {
                //Ensure crouch animation doesn't play when not crouching
                animator.ResetTrigger("StartCrouching");
                if (player.currentState == PlayerChar.playerState.crouching)
                {
                    player.setCurrentState(PlayerChar.playerState.grounded_idle);
                    animator.SetTrigger("StandStill");
                }
            }

        }
    }

    //Called when crouch is inputted
    public void CrouchInput(InputAction.CallbackContext context)
    {

        switch (context.phase)
        {
            case InputActionPhase.Started:

                CrouchInput_Held = true;

                switch (player.currentState)
                {
                    // If player is grounded/actionable, begin crouching
                    case PlayerChar.playerState.grounded_idle:
                    case PlayerChar.playerState.running:
                        player.setCurrentState(PlayerChar.playerState.crouching);
                         animator.SetTrigger("StartCrouching");
                        break;
                    // If player is airborne, fastfall if past apex of jump
                    case PlayerChar.playerState.airborne:
                    case PlayerChar.playerState.aerial_attack:
                        if(body.velocity.y <= 0)
                        {
                            gravity.SetGravity(player.fastFallAcceleration);
                        }
                        // If player holds crouch while landing, they will crouch on the ground
                        animator.SetTrigger("StartCrouching");
                        break;
                }
                break;
            /*TODO - different actions depending on context of input
             * 
             * sprinting, jumpsquat or deep into an attack: ignore input initially but store for TDI
             * within a few frames of/simultaneously with inputting an attack: down attack
             * hitlag: store for downwards TDI
             * knockback: downwards drift DI
             */

            case InputActionPhase.Canceled:
                //Update control state
                CrouchInput_Held = false;
                //Cancel crouch input depending on player state
                switch (player.currentState)
                {
                    //If player was crouching, transition to idle pose
                    case PlayerChar.playerState.crouching:
                        player.setCurrentState(PlayerChar.playerState.grounded_idle);
                        animator.SetTrigger("StandStill");
                        break;
                }
                break;
        }
    }
}

