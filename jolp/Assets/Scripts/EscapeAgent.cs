using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class EscapeAgent : Agent
{
    public float moveSpeed = 1;
    public float turnSpeed = 100;
    Rigidbody rigidbody;
    Vector3 initlocation;
    public override void Initialize()
    {
        rigidbody = GetComponent<Rigidbody>();
        initlocation = this.transform.localPosition;
    }
    
    public override void OnEpisodeBegin()
    {
        this.rigidbody.velocity = Vector3.zero;
        this.rigidbody.angularVelocity = Vector3.zero;
        this.transform.localPosition = initlocation;
        this.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.InverseTransformDirection(rigidbody.velocity));
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        AddReward(-0.005f);
        MoveAgent(vectorAction);
    }

    public void MoveAgent(float[] act)
    {
        // Vector3 moveDir = Vector3.zero;
        // Vector3 rotateDir = Vector3.zero;
        
        // moveDir.x = act[0];
        // moveDir.z = act[1];
        // rotateDir.y = act[2];

        Vector3 moveDir = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        int forwardAxis = (int)act[0];
        int sideAxis = (int)act[1];
        int rotateAxis = (int)act[2];

        switch (forwardAxis)
        {
            case 0:
                break;
            case 1:
                moveDir = transform.forward;
                break;
        }

        switch (sideAxis)
        {
            case 0:
                break;
            case 1:
                moveDir = transform.right;
                break;
            case 2:
                moveDir = -transform.right;
                break;
        }

        switch (rotateAxis)
        {
            case 0:
                break;
            case 1:
                rotateDir = transform.up;
                break;
            case 2:
                rotateDir = -transform.up;
                break;
        }
        

        transform.Rotate(rotateDir, Time.deltaTime * turnSpeed);
        rigidbody.AddForce(moveDir * moveSpeed, ForceMode.VelocityChange);

        if (rigidbody.velocity.sqrMagnitude > 25f)
        {
            rigidbody.velocity *= 0.95f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Escape
        if (collision.gameObject.CompareTag("Exit"))
        {
            AddReward(10f);
            EndEpisode();
        }
        // Agent Collision
        else if (collision.gameObject.CompareTag("Agent"))
        {
            AddReward(-5f);
        }
        // Wall Collision
        else if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-1f);
        }
    }
}