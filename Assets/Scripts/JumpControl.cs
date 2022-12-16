using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpControl : MonoBehaviour
{
    //Components
    //Access player state and stats from PlayerChar component
    PlayerChar player;
    Rigidbody body;
    Animator animator;
    MoveControl moveInput;
    VariableGravity gravity;

    //Controls state
    bool JumpInput_Held;
    float jumpCharge;
    [SerializeField]
    float chargeTime;

    void OnEnable()
    {
        player = GetComponent<PlayerChar>();
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        moveInput = GetComponent<MoveControl>();
        gravity = GetComponent<VariableGravity>();
        JumpInput_Held = false;
        jumpCharge = 0f;
    }

    void FixedUpdate()
    {
        if(JumpInput_Held)
        {
            //Hold for 1/2 second to charge jump
            jumpCharge = Mathf.Min(jumpCharge + 1, chargeTime);
            //Releases jump automatically when charged
            if(jumpCharge == chargeTime)
            {
                JumpInput_Held = false;
                float magnitude = Mathf.Lerp(player.jumpStrengthMin, player.jumpStrengthMax, jumpCharge / chargeTime);
                PerformJump(magnitude);
            }
        }
        else
        {
            jumpCharge = 0f;
        }
    }

    public void JumpInput(InputAction.CallbackContext context)
    {

        switch (context.phase)
        {
            //When button is first held
            case InputActionPhase.Started: 
                //Check player state and choose action accordingly
                switch (player.currentState)
                {
                    //If grounded/actionable, start a crouch and charge jump
                    case PlayerChar.playerState.grounded_idle:
                    case PlayerChar.playerState.running:
                    case PlayerChar.playerState.crouching:
                        JumpInput_Held = true;
                        animator.SetTrigger("BeginJumpSquat");
                        break;
                    //If airborne, perform midair jump
                    case PlayerChar.playerState.airborne:
                        if(player.GetJumpsRemaining() > 0)
                        {
                            PerformJump(player.midairJumpStrength);
                            player.DecrementJumpsRemaining();
                        }

                        break;
                }
                break;

            //When player releases button, perform jump if grounded
            case InputActionPhase.Canceled:
                if (JumpInput_Held && player.CheckIfGrounded())
                {
                    float magnitude = Mathf.Lerp(player.jumpStrengthMin, player.jumpStrengthMax, jumpCharge / 30);
                    PerformJump(magnitude);
                    JumpInput_Held = false;
                    animator.ResetTrigger("BeginJumpSquat");
                }
                break;
                

        }
    }

    private void PerformJump(float jumpStrength)
    {

        //Reset fall acceleration in case player was fast-falling
        gravity.SetGravity(player.fallAcceleration);

        Vector3 velocity = body.velocity;
        velocity.y = jumpStrength;

        //For midair jumps - jump in a different direction to existing momentum
        if (!player.CheckIfGrounded() && moveInput.MoveInput_Held == true)
        {
            velocity.x = 0.2f * moveInput.MoveInput_Value.x * player.maxAirSpeed;
            velocity.z = 0.2f * moveInput.MoveInput_Value.y * player.maxAirSpeed;
        }

        body.velocity = velocity;

    }
}
