using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightAttackControl : MonoBehaviour
{
    //Components
    //Access player state and stats from PlayerChar component
    PlayerChar player;
    Rigidbody body;
    Animator animator;
    VariableGravity gravity;
    JumpControl jump;

    //Controls State
    public bool AttackInput_Held;

    void OnEnable()
    {
        player = GetComponent<PlayerChar>();
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        gravity = GetComponent<VariableGravity>();
        jump = GetComponent<JumpControl>();
        AttackInput_Held = false;
    }

    void FixedUpdate()
    {
        // Check if animation has finished, then make player actionable again
        AnimatorTransitionInfo currentTransition = animator.GetAnimatorTransitionInfo(0);
        switch (player.currentState)
        {
            //Check attack has finished - each possible transition must be checked individually because IsName returns boolean value
            //Grounded Attack Transitions
            case PlayerChar.playerState.grounded_attack:
                if (currentTransition.IsName("Grounded Neutral Attack -> Idle"))
                {
                    player.currentState = PlayerChar.playerState.grounded_idle;
                }
                else if (currentTransition.IsName("Grounded Forward Attack -> Idle"))
                {
                    player.currentState = PlayerChar.playerState.grounded_idle;
                }
                else if (currentTransition.IsName("Grounded Down Attack -> Crouch"))
                {
                    player.currentState = PlayerChar.playerState.crouching;
                }
                else if (currentTransition.IsName("Grounded Up Attack -> Idle"))
                {
                    player.currentState = PlayerChar.playerState.grounded_idle;
                }
                break;
            //Aerial Attack Transitions
            case PlayerChar.playerState.aerial_attack:
                if (currentTransition.IsName("Aerial Neutral Attack -> Airborne"))
                {
                    player.currentState = PlayerChar.playerState.airborne;
                }
                else if (currentTransition.IsName("Aerial Forward Attack -> Airborne"))
                {
                    player.currentState = PlayerChar.playerState.airborne;
                }
                else if (currentTransition.IsName("Aerial Down Attack -> Airborne"))
                {
                    player.currentState = PlayerChar.playerState.airborne;
                }
                // State doesn't change for up aerial attack until player reaches the ground - they are not actionable after using this move
                break;

        }
    }

    public void AttackInput(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //Set attack type
            case InputActionPhase.Started:

                AttackInput_Held = true;

                switch (player.currentState)
                {
                    //Grounded Neutral Attack
                    case PlayerChar.playerState.grounded_idle:
                        GroundedNeutralAttack();
                        break;
                    //Grounded Forward Attack
                    case PlayerChar.playerState.running:
                        GroundedForwardAttack();
                        break;
                    //Grounded Down Attack
                    case PlayerChar.playerState.crouching:
                        GroundedDownAttack();
                        break;
                    //Grounded Up Attack
                    case PlayerChar.playerState.grounded_jumpsquat:
                        GroundedUpAttack();
                        break;

                    //Aerial Attacks - check player input for aerial attacks (priority: UP>DOWN>FORWARD>NEUTRAL)
                    case PlayerChar.playerState.airborne:
                        player.currentState = PlayerChar.playerState.aerial_attack;
                        //Aerial Up
                        if (player.GetComponent<JumpControl>().JumpInput_Held)
                        {
                            AerialUpAttack();
                        }
                        //Aerial Down
                        else if (player.GetComponent<CrouchControl>().CrouchInput_Held)
                        {
                            AerialDownAttack();
                        }
                        //Aerial Forward
                        else if (player.GetComponent<MoveControl>().MoveInput_Held)
                        {
                            AerialForwardAttack();
                        }
                        //Aerial Neutral
                        else
                        {
                            AerialNeutralAttack();
                        }
                        break;
                }
                break;

            case InputActionPhase.Canceled:
                AttackInput_Held = false;
                break;

        }
    }

    private void GroundedNeutralAttack()
    {
        Debug.Log("Grounded Neutral Attack");
        player.currentState = PlayerChar.playerState.grounded_attack;

        animator.SetTrigger("Grounded_Neutral_Attack");

    }

    private void GroundedForwardAttack()
    {
        Debug.Log("Grounded Forward Attack");
        player.currentState = PlayerChar.playerState.grounded_attack;

        //Add upwards force to prevent player from falling to the ground during attack + some forward force to boose player forward during this attack
        Vector3 attackDirection = body.velocity.normalized;
        Vector3 forceToAdd = new Vector3(3f*attackDirection.x, 4.2f, 3f*attackDirection.z);
        body.AddForce(forceToAdd, ForceMode.Impulse);
        
        animator.SetTrigger("Grounded_Forward_Attack");
    }

    private void GroundedDownAttack()
    {
        Debug.Log("Grounded Down Attack");
        player.currentState = PlayerChar.playerState.grounded_attack;

        animator.SetTrigger("Grounded_Down_Attack");
    }

    private void GroundedUpAttack()
    {
        Debug.Log("Grounded Up Attack");
        player.currentState = PlayerChar.playerState.grounded_attack;

        Vector3 forceToAdd = new Vector3(0f, 4.2f, 0f);
        body.AddForce(forceToAdd, ForceMode.Impulse);

        animator.SetTrigger("Grounded_Up_Attack");
    }

    private void AerialNeutralAttack()
    {
        Debug.Log("Aerial Neutral Attack");
        player.currentState = PlayerChar.playerState.aerial_attack;

        animator.SetTrigger("Aerial_Neutral_Attack");
    }

    private void AerialForwardAttack()
    {
        Debug.Log("Aerial Forward Attack");
        player.currentState = PlayerChar.playerState.aerial_attack;

        animator.SetTrigger("Aerial_Forward_Attack");
    }

    private void AerialDownAttack()
    {
        Debug.Log("Aerial Down Attack");
        player.currentState = PlayerChar.playerState.aerial_attack;

        //So that you can use the attack without fastfalling
        gravity.SetGravity(player.fallAcceleration);

        animator.SetTrigger("Aerial_Down_Attack");
    }

    private void AerialUpAttack()
    {
        Debug.Log("Aerial Up Attack");
        player.currentState = PlayerChar.playerState.aerial_attack;

        //Player gains height when using this attack - like an extra double jump
        jump.PerformJump(10f);

        animator.SetTrigger("Aerial_Up_Attack");
    }
}