using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableGravity : MonoBehaviour
{
    public float gravityScale;
    private float globalGravity = -9.81f;
    PlayerChar player;
    Rigidbody body;

    void OnEnable()
    {
        player = GetComponent<PlayerChar>();
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
    }

    void FixedUpdate()
    {
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        body.AddForce(gravity);
    }

    public void SetGravity(float value)
    {
        gravityScale = value;
    }

}
