using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BikeController : MonoBehaviour
{
    [SerializeField]
    [Header("Movimiento")]
    public float acceleration = 5f; 
    public float maxSpeed = 10f;
    public float sentido = 0f;
    public bool ground;
    public GameObject rayCast;

    [SerializeField]
    [Header("Inclinación")]
    public GameObject cicle;
    public float incline_Speed = 50f;
    private Vector3 direction;
    public bool inclinacion;

    [SerializeField]
    [Header("Manillar")]
    public float sentido_rueda = 0f;
    public GameObject manillar;
    public float turnSpeed = 50f;
    public float acceleration_turning;

    [SerializeField]
    [Header("Pendiente")]
    public Transform B1;
    public Transform B2;
    public float pendiente;

    private float horizontalInput;
    private float verticalInput;
    private float r2Value;

    public Rigidbody rb;

    void Start()
    {

    }

    void FixedUpdate()
    {
        horizontalInput = Gamepad.current.leftStick.x.ReadValue();
        verticalInput = Gamepad.current.leftStick.y.ReadValue();
        r2Value = Gamepad.current.rightTrigger.ReadValue();

        if (rb.velocity.magnitude <= 20)
        {
            inclinacion = false;
        }
        else if(!Input.GetKey(KeyCode.Space) || r2Value <= 0)
            inclinacion = true;

        if (Input.GetKey(KeyCode.Space) || r2Value > 0)
        {
            inclinacion = false;
        }


        if (inclinacion) 
        {
            if(ground)
            {
                if (Gamepad.current.buttonEast.isPressed || Input.GetKey(KeyCode.W))
                {
                    if (Mathf.Abs(sentido) > 5)
                        acceleration = acceleration_turning;
                    else
                        acceleration = 20f;

                    // Crear dirección girando el forward de la bici con un pequeño ángulo proporcional a "sentido"
                    Quaternion giro = Quaternion.Euler(0f, sentido * 3f, 0f); // 0.5f es sensibilidad de giro
                    direction = giro * transform.forward;

                    rb.AddForce(direction * acceleration, ForceMode.Acceleration);
                }

                //Visuals
                if ((Input.GetKey(KeyCode.A) || horizontalInput < 0) && sentido > -10)
                {
                    cicle.transform.Rotate(new Vector3(0, 0, 1), incline_Speed * Time.fixedDeltaTime);
                    sentido += -1f;

                }
                else if ((Input.GetKey(KeyCode.D) || horizontalInput > 0) && sentido < 10)
                {
                    cicle.transform.Rotate(new Vector3(0, 0, 1), -incline_Speed * Time.fixedDeltaTime);
                    sentido += 1f;

                }
                else if ((!Input.GetKey(KeyCode.A) && horizontalInput >= 0) && !Input.GetKey(KeyCode.D) && horizontalInput <= 0)
                {
                    if (sentido > 0)
                    {
                        cicle.transform.Rotate(new Vector3(0, 0, 1), +incline_Speed * Time.fixedDeltaTime);
                        transform.Rotate(0, -1 * Time.fixedDeltaTime, 0);
                        sentido -= 1f;
                    }
                    else if (sentido < 0)
                    {
                        cicle.transform.Rotate(new Vector3(0, 0, 1), -incline_Speed * Time.fixedDeltaTime);
                        transform.Rotate(0, 1 * Time.fixedDeltaTime, 0);
                        sentido += 1f;
                    }
                }

                //Girar la bici
                if (Input.GetKey(KeyCode.D) || horizontalInput > 0 || Input.GetKey(KeyCode.A) || horizontalInput < 0)
                {
                    cicle.transform.rotation *= Quaternion.Euler(0, sentido * 0.05f, 0);
                }

                //Volver a su sitio la rueda
                if (sentido_rueda > 0)
                {
                    manillar.transform.Rotate(new Vector3(1, 0, 0), +turnSpeed * Time.fixedDeltaTime);
                    sentido_rueda -= 1f;
                }
                else if (sentido_rueda < 0)
                {
                    manillar.transform.Rotate(new Vector3(1, 0, 0), -turnSpeed * Time.fixedDeltaTime);
                    sentido_rueda += 1f;
                }
            }
            else //En el aire-----------------------------------
            {
                if (sentido_rueda > 1 || sentido_rueda < -1)
                {
                    cicle.transform.rotation *= Quaternion.Euler(0, sentido_rueda * 0.1f, 0);
                    acceleration = acceleration_turning;
                }

                equilibrarAire();

                //Visuals
                rotarVolante();
            }

            
            

        }
        else
        {
            if(ground)
            {
                if (Input.GetKey(KeyCode.W) || Gamepad.current.buttonEast.isPressed)
                {


                        if (sentido_rueda > 5 || sentido_rueda < -5)
                        {
                            cicle.transform.rotation *= Quaternion.Euler(0, sentido_rueda * 0.2f, 0);
                            acceleration = acceleration_turning;
                        }


                        rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);
                    
                }
            }


            //Visuals
            rotarVolante();

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

            sentido = Mathf.Clamp(sentido, -10, 10);
            sentido_rueda = Mathf.Clamp(sentido_rueda, -10, 10);
            }


        //Rozamiento
        //Pendiente
        pendiente = CalcularPendiente();
        rb.AddForce(transform.forward * pendiente * 10, ForceMode.Acceleration);


        //Ground
        ground = false;

        if (Physics.Raycast(rayCast.transform.position, Vector3.down, out RaycastHit hit, 1.5f))
        {
            if (hit.collider.CompareTag("ground"))
            {
                ground = true;
            }
        }
        
        //Corregir cuando se tuerce
        if (sentido == 0 && Mathf.Abs(cicle.transform.localEulerAngles.z) > 0.1f)
        {
            Quaternion targetRotation = Quaternion.Euler(
                cicle.transform.localEulerAngles.x,
                cicle.transform.localEulerAngles.y,
                0f
            );

            cicle.transform.localRotation = Quaternion.RotateTowards(
                cicle.transform.localRotation,
                targetRotation,
                incline_Speed * Time.fixedDeltaTime
            );
        }

    }

    void equilibrarAire()
    {
        if (Input.GetKey(KeyCode.W) || verticalInput > 0)
        {
            cicle.transform.Rotate(new Vector3(1, 0, 0), incline_Speed * Time.fixedDeltaTime);
        }
        if (Input.GetKey(KeyCode.S) || verticalInput < 0)
        {
            cicle.transform.Rotate(new Vector3(1, 0, 0), -incline_Speed * Time.fixedDeltaTime);
        }
    }

    void rotarVolante()
    {
        if ((Input.GetKey(KeyCode.A) || horizontalInput < 0) && sentido_rueda > -10)
        {
            manillar.transform.Rotate(new Vector3(1, 0, 0), turnSpeed * Time.fixedDeltaTime);
            sentido_rueda += -1f;

        }
        else if ((Input.GetKey(KeyCode.D) || horizontalInput > 0) && sentido_rueda < 10)
        {
            manillar.transform.Rotate(new Vector3(1, 0, 0), -turnSpeed * Time.fixedDeltaTime);
            sentido_rueda += 1f;
        }
        else if ((!Input.GetKey(KeyCode.A) && horizontalInput >= 0) && !Input.GetKey(KeyCode.D) && horizontalInput <= 0)
        {
            if (sentido_rueda > 0)
            {
                manillar.transform.Rotate(new Vector3(1, 0, 0), +turnSpeed * Time.fixedDeltaTime);
                sentido_rueda -= 1f;
            }
            else if (sentido_rueda < 0)
            {
                manillar.transform.Rotate(new Vector3(1, 0, 0), -turnSpeed * Time.fixedDeltaTime);
                sentido_rueda += 1f;
            }
        }


            
        
    }

   

    void inclinarse()
    {

    }

    float CalcularPendiente()
    {
        return B1.transform.position.y - B2.transform.position.y;
    }
}


