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
    [SerializeField]
    public int maxLifeCount = 4;

    //Components and Children
    Rigidbody body;
    Animator animator;
    VariableGravity gravity;
    Component[] hurtboxes;
    private Transform leftFoot;
    private Transform rightFoot;
    private Transform model;
    private Transform bones;

    //Player State
    public enum playerState { grounded_idle, running, crouching, grounded_jumpsquat, airborne, grounded_attack, aerial_attack, hitstun}
    public playerState currentState;
    private bool isGrounded;
    private float leftDistToGround;
    private float rightDistToGround;
    private int jumpsRemaining;
    [SerializeField]//For viewing in editor
    public float damageStat;
    private float hitstunFramesRemaining;
    private bool alive;
    public int currentLives;
    private int timeUntilRespawn;

    //Spawn point
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;


    void OnEnable()
    {
        //Get components
        body = GetComponent<Rigidbody>();
        gravity = GetComponent<VariableGravity>();
        gravity.SetGravity(fallAcceleration);
        animator = GetComponent<Animator>();
        hurtboxes = GetComponentsInChildren<Collider>();
        model = FindChildByName(this.transform, "Model");
        bones = FindChildByName(this.transform, "mixamorig:Hips");

        //Setup for checking if grounded
        leftFoot = FindChildByName(this.transform, "mixamorig:LeftLeg");
        rightFoot = FindChildByName(this.transform, "mixamorig:RightLeg");
        leftDistToGround = leftFoot.GetComponent<Collider>().bounds.extents.y;
        rightDistToGround = rightFoot.GetComponent<Collider>().bounds.extents.y;
        isGrounded = CheckIfGrounded();

        //Set spawn position 
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        //Set initial player state
        setCurrentState(playerState.grounded_idle);
        jumpsRemaining = maxNumMidairJumps;
        damageStat = 0f;
        hitstunFramesRemaining = 0f;
        alive = true;
        currentLives = maxLifeCount;
        timeUntilRespawn = 0; //Counted in frames (1/60 of a second)
    }


    void FixedUpdate()
    {
        //When dead wait a few seconds then respawn
        if (!alive && currentLives > 0)
        {
            if (timeUntilRespawn <= 0)
            {
                damageStat = 0f;
                alive = true;
                hitstunFramesRemaining = 0f;
                setCurrentState(playerState.grounded_idle);
                jumpsRemaining = maxNumMidairJumps;

                body.velocity = Vector3.zero;
                transform.position = spawnPosition;
                transform.rotation = spawnRotation;
                model.gameObject.SetActive(true);
                bones.gameObject.SetActive(true);
            }
            timeUntilRespawn -= 1;

        }

        if (alive)
        {
            //Update grounded state
            isGrounded = CheckIfGrounded();

            if (isGrounded)
            {
                bool landingThisFrame = false; //Attempt at preventing landing animation from playing twice, or being ignored

                //Stops player getting stuck in falling animation
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Airborne"))
                {
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
                    case playerState.aerial_attack:
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
                    case playerState.grounded_jumpsquat:
                        setCurrentState(playerState.airborne);
                        animator.SetTrigger("GoAirborne");
                        animator.ResetTrigger("BeginLanding");
                        break;
                }
            }

            //Ensure running animation doesn't play when not running
            if (currentState == playerState.grounded_idle)
            {
                animator.ResetTrigger("StartRunning");
            }

            //Hitstun state
            if (currentState == playerState.hitstun)
            {
                //Exit hitstun when hitstun time ends
                if (hitstunFramesRemaining <= 0f)
                {
                    hitstunFramesRemaining = 0f; //Reset in case of being slightly below 0

                    if (isGrounded)
                    {
                        setCurrentState(playerState.grounded_idle);
                        animator.SetTrigger("StandStill");
                    }
                    else
                    {
                        setCurrentState(playerState.airborne);
                        animator.SetTrigger("GoAirborne");
                    }
                }

                //Knockdown animation when player hits the ground in hitstun
                if (isGrounded && body.velocity.y == 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("Tumble"))
                {
                    animator.SetTrigger("KnockDown");
                }
                hitstunFramesRemaining -= 1f;
            }

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


    public Transform FindChildByName(Transform transform, string name)
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

    public Component[] GetHurtboxes()
    {
        return hurtboxes;
    }

    public float GetDamageStat()
    {
        return damageStat;
    }

    public void ApplyDamage(float damage, float knockbackBase, float knockbackScale, Vector3 knockbackAngle)
    {
        float effectScale = 1f;

        //Apply reductions due to crouching - Crouching acts like a block - it halves damage/knockback and prevents going into hitstun
        if(currentState == playerState.crouching)
        {
            effectScale = 0.5f;
            knockbackAngle.y = 0f;
        }

        damageStat += (damage*effectScale);

        // Knockback calculation formula (based on SSB knockback formula)
        float knockbackMagnitude = ((((((damageStat / 10f) + ((damageStat * damage) / 20f)) * (200f / (body.mass + 100f)) * 1.4f) + 18f) * (knockbackScale / 100f)) + knockbackBase) * effectScale;
        hitstunFramesRemaining = 4f * knockbackMagnitude * effectScale;

        // Reset player velocity before launching
        body.velocity = Vector3.zero;

        // Launch player - if crouching, player is knocked back but not up
        body.AddForce(knockbackAngle.normalized * knockbackMagnitude, ForceMode.Impulse);

        // Put player in hitstun state and enable ragdoll
        if(currentState != playerState.crouching)
        {
            currentState = playerState.hitstun;
            animator.SetTrigger("KnockBack");
        }

    }

    public void KillPlayer()
    {
        alive = false;
        currentLives -= 1;

        model.gameObject.SetActive(false);
        bones.gameObject.SetActive(false);
        timeUntilRespawn = 120;

        Debug.Log("Player Killed");
        Debug.Log("Lives remaining: " + currentLives);
    }

    public int GetLivesRemaining()
    {
        return currentLives;
    }

    public bool IsAlive()
    {
        return alive;
    }
}