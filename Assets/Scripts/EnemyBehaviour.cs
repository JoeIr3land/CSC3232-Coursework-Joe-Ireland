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
                        int randomDecision = Random.Range(1, 5);
                        switch (randomDecision)
                        {
                            case 1:// Random grounded attack
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
                        }
                    }
                    //If not in range, do random movement
                    else
                    {
                        int randomDecision = Random.Range(1, 6);
                        switch (randomDecision)
                        {
                            case 1: //1-4 : Run in random cardinal direction
                                runDir = new Vector2(0, -1);
                                moveControl.Run(runDir);
                                playerChar.setCurrentState(PlayerChar.playerState.running);
                                animator.SetTrigger("StartRunning");
                                break;
                            case 2: 
                                runDir = new Vector2(0, 1);
                                moveControl.Run(runDir);
                                playerChar.setCurrentState(PlayerChar.playerState.running);
                                animator.SetTrigger("StartRunning");
                                break;
                            case 3: 
                                runDir = new Vector2(-1, 0);
                                moveControl.Run(runDir);
                                playerChar.setCurrentState(PlayerChar.playerState.running);
                                animator.SetTrigger("StartRunning");
                                break;
                            case 4: 
                                runDir = new Vector2(1, 0);
                                moveControl.Run(runDir);
                                playerChar.setCurrentState(PlayerChar.playerState.running);
                                animator.SetTrigger("StartRunning");
                                break;
                            case 5: //Crouch
                                playerChar.setCurrentState(PlayerChar.playerState.crouching);
                                animator.SetTrigger("StartCrouching");
                                break;
                            case 6: //Jump
                                jumpControl.PerformJump(playerChar.jumpStrengthMin);
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
                        int randomDecision = Random.Range(1, 6);
                        switch (randomDecision)
                        {
                            case 1: //1-4 : Move in random cardinal direction
                                runDir = new Vector2(0, -1);
                                moveControl.Run(runDir);
                                break;
                            case 2:
                                runDir = new Vector2(0, 1);
                                moveControl.Run(runDir);
                                break;
                            case 3: 
                                runDir = new Vector2(-1, 0);
                                moveControl.Run(runDir);
                                break;
                            case 4:
                                runDir = new Vector2(1, 0);
                                moveControl.Run(runDir);
                                break;
                            case 5: //Fastfall
                                gravity.SetGravity(playerChar.fastFallAcceleration);
                                break;
                            case 6: //Midair jump
                                if (playerChar.GetJumpsRemaining() > 0)
                                {
                                    jumpControl.PerformJump(playerChar.midairJumpStrength);
                                    playerChar.DecrementJumpsRemaining();
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
