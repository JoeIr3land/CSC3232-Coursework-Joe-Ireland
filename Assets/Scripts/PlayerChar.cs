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
    private float leftDistToGround;
    private float rightDistToGround;
    private int jumpsRemaining;

    //Components and Children
    Rigidbody body;
    Animator animator;
    VariableGravity gravity;
    Component[] hitboxes;
    private Transform leftFoot;
    private Transform rightFoot;

    //Player State
    public enum playerState { grounded_idle, running, crouching, airborne }
    [SerializeField]
    public playerState currentState;


    void OnEnable()
    {

        //Get components
        body = GetComponent<Rigidbody>();
        gravity = GetComponent<VariableGravity>();
        gravity.SetGravity(fallAcceleration);
        animator = GetComponent<Animator>();
        hitboxes = GetComponentsInChildren<Collider>();

        //Setup for checking if grounded

        leftFoot = FindChildByName(this.transform, "mixamorig:LeftLeg");
        rightFoot = FindChildByName(this.transform, "mixamorig:RightLeg");
        leftDistToGround = leftFoot.GetComponent<Collider>().bounds.extents.y;
        rightDistToGround = rightFoot.GetComponent<Collider>().bounds.extents.y;
        isGrounded = CheckIfGrounded();

        //Set initial player state
        setCurrentState(playerState.grounded_idle);
        jumpsRemaining = maxNumMidairJumps;
    }


    void FixedUpdate()
    {

        //Update grounded state
        isGrounded = CheckIfGrounded();

        Debug.Log(isGrounded);
        Debug.Log(currentState);

        if (isGrounded)
        {
            bool landingThisFrame = false; //For preventing landing animation from playing twice, or being ignored

            //Stops player getting stuck in falling animation
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Airborne"))
            {
                Debug.Log("Falling on ground detected");
                landingThisFrame = true;
                animator.SetTrigger("BeginLanding");
            }

            //Workaround - so that falling through a platform without landing doesn't give you back midair jumps
            if (body.velocity.y == 0)
            {
                jumpsRemaining = maxNumMidairJumps;
                gravity.SetGravity(fallAcceleration);
            }
            
            switch (currentState)
            {
                case playerState.grounded_idle:
                case playerState.running:
                case playerState.crouching:
                    if (!landingThisFrame)
                    {
                        animator.ResetTrigger("BeginLanding");
                    }
                    landingThisFrame = false;
                    break;
                case playerState.airborne:
                    animator.SetTrigger("BeginLanding");
                    animator.ResetTrigger("GoAirborne");
                    setCurrentState(playerState.grounded_idle);
                    break;
            }

        }
        else
        {
            switch (currentState)
            {
                case playerState.grounded_idle:
                case playerState.running:
                case playerState.crouching:
                    setCurrentState(playerState.airborne);
                    animator.SetTrigger("GoAirborne");
                    animator.ResetTrigger("BeginLanding");
                    break;
            }
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
        // Always returns false if player has upwards velocity - stops player from being able to do grounded actions when jumping up through a platform
        if (body.velocity.y < 0.1)
        {
            bool leftCheck = Physics.Raycast(leftFoot.GetComponent<Collider>().bounds.center, -Vector3.up, leftDistToGround + 0.1f);
            bool rightCheck = Physics.Raycast(rightFoot.GetComponent<Collider>().bounds.center, -Vector3.up, rightDistToGround + 0.1f);
            return (leftCheck || rightCheck);
        }
        else return false;

    }


    public int GetJumpsRemaining()
    {
        return jumpsRemaining;
    }


    public void DecrementJumpsRemaining()
    {
        jumpsRemaining = Mathf.Max(0, jumpsRemaining - 1);
    }


    private Transform FindChildByName(Transform transform, string name)
    {
        foreach (Transform child in transform)
        {
            if (child.name == name)
            {
                return child;
            }
            else if (child.childCount > 0)
            {
                Transform checkChildren = FindChildByName(child, name);
                if (checkChildren != null)
                {
                    return checkChildren;
                }
            }
        }
        return null;
    }

    public Component[] GetHitboxes()
    {
        return hitboxes;
    }
}