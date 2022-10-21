using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveControl : MonoBehaviour
{

    //Components
    //Access player state and stats  from PlayerChar component
    PlayerChar player;
    Rigidbody body;
    Animator animator;

    //Controls state
    bool MoveInput_Held;
    Vector2 MoveInput_Value;


    void Start()
    {
        player = GetComponent<PlayerChar>();
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        MoveInput_Held = false;
        MoveInput_Value = Vector2.zero;
    }


    void FixedUpdate()
    {
        //Movement
        if (MoveInput_Held)
        {
            //Move player according to the player's state
            switch (player.currentState)
            {
                // If player is running, continue running
                case PlayerChar.playerState.running:
                    Run(MoveInput_Value);
                    break;
                // Otherwise, player cannot move
                default:
                    break;
            }
            /*TODO: run behaviour for sprint, sprint->run, walk, walk->run, run->walk, aerial drift, crawling,
            * updating stored position for jump/aerial move/TDI, drift DI.
            * 
            * May need to add behaviour for other transitions, depending on if something happens to the player
            * but they don't change their input (input would now apply to new context)
            */
        }
    }


    //Called when movement input is registered
    public void MoveInput(InputAction.CallbackContext context)
    {
        //Change player state and animation
        switch (context.phase)
        {

            //When movement starts, change player state, control state and animation accordingly
            case InputActionPhase.Started:
                player.setCurrentState(PlayerChar.playerState.running);
                MoveInput_Held = true;
                animator.SetTrigger("StartRunning");
                break;
            /*TODO: change animation/state according to player's existing state, and magnitude of player input
             * 
             * idle_grounded: 
             *      soft input: walk
             *      hard input: sprint
             * idle_air - aerial drift
             * crouching - crawl
             * jumpsquat - store input for jump direction and aerial move direction
             * hitlag - smash DI & store input for trajectory DI
             * knockback - drift DI
             * within a few frames of/simultaneously with inputting an attack: up attack
             * 
             * Maybe an idea is to report a class-scale input direction for use with DI, aerial drift, attack inputs, inputs 
             * at ledge etc.
             * 
             */

            //Update stored movement value each time it changes
            case InputActionPhase.Performed:
                MoveInput_Value = context.ReadValue<Vector2>();
                break;

            //When movement ends, change player state, control state and animation accordingly
            case InputActionPhase.Canceled:
                //Update control state, so Update loop stops trying to move the player
                MoveInput_Held = false;
                // Cancel movement input depending on player state
                switch (player.currentState)
                {
                    //If player was running, transition to idle pose
                    case PlayerChar.playerState.running:
                        player.setCurrentState(PlayerChar.playerState.grounded_idle);
                        animator.SetTrigger("StandStill");
                        break;
                }
                break;
                /*TODO: change player state/animation depending on action and context
                 * 
                 * Structure is made, just add contexts/player states as they are created
                 * 
                 */
        }

    }


    //Called when player is in 'running' state
    void Run(Vector2 moveDir)
    {
        //Change player orientation
        Vector3 lookDirection = new Vector3(moveDir.x, 0.0f, moveDir.y);
        transform.rotation = Quaternion.LookRotation(lookDirection);

        //Move player
        Vector3 velocity = body.velocity;
        velocity.x = moveDir.x * player.groundSpeed;
        velocity.z = moveDir.y * player.groundSpeed;
        body.velocity = velocity;
    }


}
