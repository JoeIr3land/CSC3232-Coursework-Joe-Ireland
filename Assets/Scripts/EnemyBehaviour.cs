using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    //Components
    PlayerChar playerChar;
    Rigidbody body;
    Animator animator;
    MoveControl moveControl;
    CrouchControl crouchControl;
    JumpControl jumpControl;
    LightAttackControl attackControl;
    VariableGravity gravity;

    [SerializeField]
    float distanceThresholdToAttack;
    [SerializeField]
    int framesBetweenDecisions = 5;
    private int frameCounter;

    [SerializeField]
    GameObject targetPlayer;

    private enum behaviourState { aggressive }
    private behaviourState currentBehaviourState;

    void OnEnable()
    {
        //Get components
        playerChar = GetComponent<PlayerChar>();
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        moveControl = GetComponent<MoveControl>();
        crouchControl = GetComponent<CrouchControl>();
        jumpControl = GetComponent<JumpControl>();
        attackControl = GetComponent<LightAttackControl>();
        gravity = GetComponent<VariableGravity>();

        frameCounter = 0;
    }

    void FixedUpdate()
    {
        if(frameCounter >= framesBetweenDecisions)
        {
            frameCounter = 0;

            Vector3 targetPos = targetPlayer.transform.position;
            float distanceToTarget = (targetPos - transform.position).magnitude;

            // If player is dead, enemy heads to middle of stage
            if (!(targetPlayer.GetComponent<PlayerChar>().IsAlive()))
            {
                targetPos = Vector3.zero;
            }

            Vector2 runDir;

            switch (playerChar.currentState)
            {
                //If AI is grounded/actionable, walk up to player
                case PlayerChar.playerState.grounded_idle:
                case PlayerChar.playerState.running:
                case PlayerChar.playerState.crouching:
                    //If within attack range, do random thing
                    if (distanceToTarget <= distanceThresholdToAttack)
                    {
                        //If player is attacking while enemy is in range, enemy tries to block by crouching
                        if(targetPlayer.GetComponent<PlayerChar>().currentState == PlayerChar.playerState.grounded_attack || targetPlayer.GetComponent<PlayerChar>().currentState == PlayerChar.playerState.aerial_attack)
                        {
                            Debug.Log("Attempting to block");
                            playerChar.setCurrentState(PlayerChar.playerState.crouching);
                            animator.SetTrigger("StartCrouching");
                            break;
                        }
                        //Else throw attacks out
                        else
                        {
                            int randomDecision = Random.Range(1, 6);
                            switch (randomDecision)
                            {
                                case 1:// 1-4 Random grounded attack
                                    attackControl.GroundedNeutralAttack();
                                    break;
                                case 2:
                                    attackControl.GroundedForwardAttack();
                                    break;
                                case 3:
                                    attackControl.GroundedUpAttack();
                                    break;
                                case 4:
                                    attackControl.GroundedDownAttack();
                                    break;
                                case 5: //Run away from player
                                    runDir = new Vector2(-(targetPos.x - transform.position.x), -(targetPos.z - transform.position.z));
                                    runDir.Normalize();
                                    moveControl.EnemyRun(runDir);
                                    playerChar.setCurrentState(PlayerChar.playerState.running);
                                    animator.SetTrigger("StartRunning");
                                    break;
                            }
                        }
                        
                    }
                    //If not in range, do random movement
                    else
                    {
                        int randomDecision = Random.Range(1, 21);
                        switch (randomDecision)
                        {
                            case 1: //Run towards the player 90% of the time
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                            case 9:
                            case 10:
                            case 11:
                            case 12:
                            case 13:
                            case 14:
                            case 15:
                            case 16:
                            case 17:
                            case 18:
                                runDir = new Vector2((targetPos.x - transform.position.x), (targetPos.z - transform.position.z));
                                runDir.Normalize();
                                moveControl.EnemyRun(runDir);
                                playerChar.setCurrentState(PlayerChar.playerState.running);
                                animator.SetTrigger("StartRunning");
                                break;
                            case 19: //Crouch
                                playerChar.setCurrentState(PlayerChar.playerState.crouching);
                                animator.SetTrigger("StartCrouching");
                                break;
                            case 20: //Jump
                                float randomJumpHeight = Random.Range(playerChar.jumpStrengthMin, playerChar.jumpStrengthMax);
                                jumpControl.PerformJump(randomJumpHeight);
                                break;
                        }
                    }
                    break;
                case PlayerChar.playerState.airborne:
                    if (distanceToTarget <= distanceThresholdToAttack)
                    {
                        int randomDecision = Random.Range(1, 5);
                        switch (randomDecision)
                        {
                            case 1:// Random aerial attack
                                attackControl.AerialNeutralAttack();
                                break;
                            case 2:
                                attackControl.AerialForwardAttack();
                                break;
                            case 3:
                                attackControl.AerialUpAttack();
                                break;
                            case 4:
                                attackControl.AerialDownAttack();
                                break;
                        }
                    }
                    else
                    {
                        int randomDecision = Random.Range(1, 9);
                        switch (randomDecision)
                        {
                            case 1: //Move towards player 3/4 of the time
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                                runDir = new Vector2((targetPos.x - transform.position.x), (targetPos.z - transform.position.z));
                                runDir.Normalize();
                                moveControl.EnemyAerialDrift(runDir);
                                break;
                            case 7: //Fastfall
                                gravity.SetGravity(playerChar.fastFallAcceleration);
                                break;
                            case 8: //Midair jump if below player (Preserves them for recovery)
                                if(targetPlayer.transform.position.y > transform.position.y)
                                {
                                    if (playerChar.GetJumpsRemaining() > 0)
                                    {
                                        jumpControl.PerformJump(playerChar.midairJumpStrength);
                                        playerChar.DecrementJumpsRemaining();
                                    }
                                    else
                                    {
                                        attackControl.AerialUpAttack();
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
        
        }
        frameCounter += 1;
    }
}
