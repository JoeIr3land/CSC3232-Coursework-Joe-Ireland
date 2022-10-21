using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerChar : MonoBehaviour
{
    
    //Character Stats
    [SerializeField]
    private float groundSpeed;

    //Player State
    enum playerState { grounded_idle, running, crouching }
    [SerializeField]
    playerState currentState;

    //Components
    Rigidbody body;
    Animator animator;


    public void Start()
    {

        //Get components
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        setCurrentState(playerState.grounded_idle);

    }

    // Change player state
    void setCurrentState(playerState state)
    {
        currentState = state;
    }


    //Called when movement input is registered
    public void MoveInput(InputAction.CallbackContext context)
    {

        //Change player state and animation
        switch (context.phase)
        {

            //When movement starts, change player state and animation accordingly
            case InputActionPhase.Started:
                setCurrentState(playerState.running);
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

            //While movement is held, move player in the corresponding way
            case InputActionPhase.Performed:

                switch (currentState)
                {
                    // If player is running, continue running
                    case playerState.running:
                        Run(context.ReadValue<Vector2>());
                        break;
                }
                break;
                /*TODO: run behaviour for sprint, sprint->run, walk, walk->run, run->walk, aerial drift, crawling,
                 * updating stored position for jump/aerial move/TDI, drift DI.
                 * 
                 * May need to add behaviour for other transitions, depending on if something happens to the player
                 * but they don't change their input (input would now apply to new context)
                 */

            case InputActionPhase.Canceled:

                // Cancel movement input depending on player state
                switch (currentState)
                {
                    //If player is running when they let go of stick, they transition to idle pose
                    case playerState.running:
                        setCurrentState(playerState.grounded_idle);
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
        velocity.x = moveDir.x * groundSpeed;
        velocity.z = moveDir.y * groundSpeed;
        body.velocity = velocity;
    }


    //Called when crouch is inputted
    public void CrouchInput(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                setCurrentState(playerState.crouching);
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
                 * 
                 * ALSO: behaviour while holding crouch, but not started (maybe TDI and drift DI go here?)
                 * 
                 */

            case InputActionPhase.Canceled:

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