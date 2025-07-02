using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BikeController : MonoBehaviour
{
    public float acceleration = 5f; 
    public float maxSpeed = 10f;   
    public float turnSpeed = 50f;

    private Vector3 direction;
    public float sentido = 0f;

    public GameObject manillar;
    public GameObject cicle;

    public Rigidbody rb;

    void Start()
    {

    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("Arrancamos");
            if (rb.velocity.magnitude < maxSpeed)
            {
                Quaternion giro = Quaternion.Euler(0f, sentido * 0.8f, 0f);
                Vector3 direccionGiro = giro * transform.forward;

                rb.AddForce(direccionGiro.normalized * acceleration, ForceMode.Acceleration);
            }
        }


        //Visuals
        if (Input.GetKey(KeyCode.A) && sentido > -60)
        {
            manillar.transform.Rotate(new Vector3(1, 0 ,0), turnSpeed * Time.fixedDeltaTime);
            sentido += -1f;

        }
        else if (Input.GetKey(KeyCode.D) && sentido < 60)
        {
            manillar.transform.Rotate(new Vector3(1, 0, 0), -turnSpeed * Time.fixedDeltaTime);
            sentido += 1f;

        }
            

        sentido = Mathf.Clamp(sentido, -60, 60);
    }
}
