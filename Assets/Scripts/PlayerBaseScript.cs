using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerBaseScript : MonoBehaviour
{
    //float XInput;
   // float YInput;// check later
    public bool IsDodging;

    public float speed;
    public float DodgeDuration;
    public float DodgeSpeed;
    public float DodgeDelay;
    float DodgeTime;
    float DodgeStop;

    float kbf; //Kickback float

    public Camera cam;

    Vector2 Movement;
    Vector2 MousePos;

    public GameObject gunPoint;
    public AudioSource DashSound;

    Rigidbody2D rb;

    [SerializeField] PlayerInput playerInput;
    
    
    
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        IsDodging = false;
        DodgeTime = Time.time;
        kbf = 0;
    }

    // Update is called once per frame
    void Update()
    {
        MousePos = cam.ScreenToWorldPoint(Input.mousePosition);   // there's a "Look" thing that keeps track of mouse position in the new input system,
                                                                    // not sure how it works, will figure out, maybe

        if (playerInput.actions["Dash"].triggered && DodgeTime < Time.time)
        {
            IsDodging = true;
            speed = speed * DodgeSpeed;
            DodgeTime = Time.time + DodgeDelay + DodgeDuration;
            DodgeStop = Time.time + DodgeDuration;
            gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            DashSound.Play();

        }
        if (IsDodging == true && DodgeStop < Time.time)
        {
            IsDodging = false;
            speed = speed / DodgeSpeed;
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            DashSound.Stop();
        }
       
    }

    private void FixedUpdate()
    {
        //seperate keyboard and mouse input and controller input
        //tho movement are the same, so no changes there, but for aiming, there's difference
        Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
        Movement = new Vector2(input.x, input.y) * (speed * Time.deltaTime);
        rb.MovePosition(Movement + new Vector2(transform.position.x, transform.position.y) + new Vector2(transform.up.x, transform.up.y) * kbf); //movement + kickback if shot
        kbf = 0;
        
        
        if (Gamepad.current != null)
        {
            //me no understand maths works how
            //so basically when lookdirection is at 0, rotation of 0 degrees is staight up, which is fine
            //but as soon as there's input from stick, direction gets all messed up unless you rotate them -90 degrees
            //so I just made it so that when there's no input, it undose the -90 degrees adjustment
            //this way of structuring code also means it supports hot swap on input devices
            Vector2 lookDirection = playerInput.actions["Look"].ReadValue<Vector2>(); //this spits out a normalized directional vector, thanks unity
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f; // Converts to degrees
            if (lookDirection.x == 0.00f && lookDirection.y == 0.00f)    
            {
                angle += 90f;
                rb.rotation = angle;
            }
            else
            {
                rb.rotation = angle;
            }
        }
        else if (Gamepad.current == null)
        {
            Vector2 LookDir = rb.position - MousePos;
            float angle = Mathf.Atan2(LookDir.y, LookDir.x) * Mathf.Rad2Deg + 90f;
            rb.rotation = angle;
        }
        
        
    }

    public void Kickback(float kb)
    {
        kbf = -kb;
    }
    
    
}
