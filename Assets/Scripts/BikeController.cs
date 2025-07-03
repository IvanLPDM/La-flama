using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BikeController : MonoBehaviour
{
    [SerializeField]
    [Header("Movimiento")]
    public float acceleration = 5f; 
    public float maxSpeed = 10f;
    private float sentido = 0f;

    [SerializeField]
    [Header("Inclinación")]
    public GameObject cicle;
    public float incline_Speed = 50f;
    private Vector3 direction;
    public bool inclinacion;

    [SerializeField]
    [Header("Manillar")]
    public GameObject manillar;
    public float turnSpeed = 50f;
    public float acceleration_turning;

    [SerializeField]
    [Header("Pendiente")]
    public Transform B1;
    public Transform B2;
    public float pendiente;

    public Rigidbody rb;

    void Start()
    {

    }

    void FixedUpdate()
    {
        

        if (inclinacion) 
        {
            Debug.Log("DERRAPA");
            if (Input.GetKey(KeyCode.W))
            {
                if (sentido < -1 || sentido > 1)
                {
                    if (sentido > 10 || sentido < -10)
                    {
                        acceleration = acceleration_turning;
                    }
                    else if (sentido > 5 || sentido < -5)
                    {
                        acceleration = acceleration_turning;
                    }
                    else
                        acceleration = 20f;

                    direction = new Vector3(sentido * 0.1f, transform.forward.y, transform.forward.z);
                }
                else
                    direction = transform.forward;

                rb.AddForce(direction * acceleration, ForceMode.Acceleration);
            }

            //Visuals
            if (Input.GetKey(KeyCode.A) && sentido > -10)
            {
                cicle.transform.Rotate(new Vector3(0, 0, 1), incline_Speed * Time.fixedDeltaTime);
                sentido += -1f;

            }
            else if (Input.GetKey(KeyCode.D) && sentido < 10)
            {
                cicle.transform.Rotate(new Vector3(0, 0, 1), -incline_Speed * Time.fixedDeltaTime);
                sentido += 1f;

            }
            else if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                if (sentido > 0)
                {
                    cicle.transform.Rotate(new Vector3(0, 0, 1), +incline_Speed * Time.fixedDeltaTime);
                    sentido -= 1f;
                }
                else if (sentido < 0)
                {
                    cicle.transform.Rotate(new Vector3(0, 0, 1), -incline_Speed * Time.fixedDeltaTime);
                    sentido += 1f;
                }
            }

        }
        else
        {
                if (Input.GetKey(KeyCode.W))
                {
                    if (rb.velocity.magnitude < maxSpeed)
                    {
                        if (sentido > 10 || sentido < -10)
                        {
                            cicle.transform.rotation *= Quaternion.Euler(0, sentido * 0.8f, 0);
                            acceleration = acceleration_turning;
                        }
                        else if (sentido > 5 || sentido < -5)
                        {
                            cicle.transform.rotation *= Quaternion.Euler(0, sentido * 0.2f, 0);
                            acceleration = acceleration_turning;
                        }
                        else
                            acceleration = 20f;

                        rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);
                    }
                }




            //Visuals
            if (Input.GetKey(KeyCode.A) && sentido > -10)
            {
                manillar.transform.Rotate(new Vector3(1, 0, 0), turnSpeed * Time.fixedDeltaTime);
                sentido += -1f;

            }
            else if (Input.GetKey(KeyCode.D) && sentido < 10)
            {
                manillar.transform.Rotate(new Vector3(1, 0, 0), -turnSpeed * Time.fixedDeltaTime);
                sentido += 1f;
            }
            else if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                if (sentido > 0)
                {
                    manillar.transform.Rotate(new Vector3(1, 0, 0), +turnSpeed * Time.fixedDeltaTime);
                    sentido -= 1f;
                }
                else if (sentido < 0)
                {
                    manillar.transform.Rotate(new Vector3(1, 0, 0), -turnSpeed * Time.fixedDeltaTime);
                    sentido += 1f;
                }
            }


            sentido = Mathf.Clamp(sentido, -10, 10);
            }


        //Rozamiento
        //Pendiente
        pendiente = CalcularPendiente();
        rb.AddForce(transform.forward * pendiente * 10, ForceMode.Acceleration);

    }

    float CalcularPendiente()
    {
        return B1.transform.position.y - B2.transform.position.y;
    }

    //void FixedUpdate()
    //{
    //    if (Input.GetKey(KeyCode.W))
    //    {
    //        if (rb.velocity.magnitude < maxSpeed)
    //        {
    //            if (sentido > 10 || sentido < -10)
    //            {
    //                cicle.transform.rotation *= Quaternion.Euler(0, sentido * 0.8f, 0);
    //                acceleration = acceleration_turning;
    //            }
    //            else if (sentido > 5 || sentido < -5)
    //            { 
    //                cicle.transform.rotation *= Quaternion.Euler(0, sentido * 0.2f, 0);
    //                acceleration = acceleration_turning;
    //            }

    //            else
    //                acceleration = 20f;

    //            rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);
    //        }
    //    }




    //    //Visuals
    //    if (Input.GetKey(KeyCode.A) && sentido > -10)
    //    {
    //        manillar.transform.Rotate(new Vector3(1, 0 ,0), turnSpeed * Time.fixedDeltaTime);
    //        sentido += -1f;

    //    }
    //    else if (Input.GetKey(KeyCode.D) && sentido < 10)
    //    {
    //        manillar.transform.Rotate(new Vector3(1, 0, 0), -turnSpeed * Time.fixedDeltaTime);
    //        sentido += 1f;

    //    }


    //    sentido = Mathf.Clamp(sentido, -10, 10);
    //}


}


