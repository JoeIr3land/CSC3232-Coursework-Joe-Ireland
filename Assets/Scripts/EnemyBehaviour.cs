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

    [SerializeField]
    GameObject targetPlayer;

    private enum behaviourState { aggressive }
    private behaviourState currentBehaviourState;

    void OnEnable()
    {
        //Get components
        playerChar = GetComponent<playerChar>();
        body = GetComponent<Rigidbody>();
        animator = GetComponent<animator>();
        moveControl = GetComponent<MoveControl>();
        crouchControl = GetComponent<CrouchControl>();
        jumpControl = GetComponent<JumpControl>();
        attackControl = GetComponent<LightAttackControl>();
    }

    void FixedUpdate()
    {

    }
}
