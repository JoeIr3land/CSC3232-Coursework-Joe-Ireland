using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerChar : MonoBehaviour
{

    //Character Properties
    [SerializeField]
    public float groundSpeed;
    [SerializeField]
    public float airAcceleration;
    [SerializeField]
    public float maxAirSpeed;
    [SerializeField]
    public float jumpStrengthMin;
    [SerializeField]
    public float jumpStrengthMax;
    [SerializeField]
    public float midairJumpStrength;
    [SerializeField]
    public int maxNumMidairJumps;
    [SerializeField]
    public float fallAcceleration;
    [SerializeField]
    public float fastFallAcceleration;

    //Character Stats
    private bool isGrounded;
    private float distToGround;
    private int jumpsRemaining;

    //Components
    Rigidbody body;
    Animator animator;
    VariableGravity gravity;

    //Player State
    public enum playerState { grounded_idle, running, crouching, airborne }
    [SerializeField]
    public playerState currentState;
    private List<playerState> groundedStates;


    void OnEnable()
    {

        //Set initial player state
        setCurrentState(playerState.grounded_idle);
        jumpsRemaining = maxNumMidairJumps;

        //Get components
        body = GetComponent<Rigidbody>();
        gravity = GetComponent<VariableGravity>();
        gravity.SetGravity(fallAcceleration);
        animator = GetComponent<Animator>();

        //Setup for checking if grounded
        distToGround = GetComponent<Collider>().bounds.extents.y;
        isGrounded = CheckIfGrounded();
        groundedStates = new List<playerState>();
        groundedStates.Add(playerState.grounded_idle);
        groundedStates.Add(playerState.running);
        groundedStates.Add(playerState.crouching);
        //ADD NEW GROUNDED STATES HERE
    }


    void FixedUpdate()
    {
        //Update grounded state
        isGrounded = CheckIfGrounded();
        if (!isGrounded && groundedStates.Contains(currentState))
        {
            setCurrentState(playerState.airborne);
            animator.SetTrigger("GoAirborne");
        }
        if (isGrounded && currentState == playerState.airborne)
        {
            setCurrentState(playerState.grounded_idle);
            animator.SetTrigger("BeginLanding");
        }

        if (isGrounded)
        {
            jumpsRemaining = maxNumMidairJumps;
            gravity.SetGravity(fallAcceleration);

        }
        
        //Ensure running animation doesn't play when not running
        if(currentState == playerState.grounded_idle)
        {
            animator.ResetTrigger("StartRunning");
        }

    }


    //Change player state
    public void setCurrentState(playerState state)
    {
        currentState = state;
    }


    //Check if grounded
    public bool CheckIfGrounded()
    {
        return Physics.Raycast(GetComponent<Collider>().bounds.center, -Vector3.up, distToGround + 0.1f);
    }

    public int GetJumpsRemaining()
    {
        return jumpsRemaining;
    }

    public void DecrementJumpsRemaining()
    {
        jumpsRemaining = Mathf.Max(0, jumpsRemaining - 1);
    }

}