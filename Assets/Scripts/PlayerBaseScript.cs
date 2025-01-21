using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerBaseScript : MonoBehaviour
{
    float XInput;
    float YInput;
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
                                                                    // not sure how it works will figure out, maybe

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
        Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
        Movement = new Vector2(input.x, input.y) * (speed * Time.deltaTime);
        rb.MovePosition(Movement + new Vector2(transform.position.x, transform.position.y) + new Vector2(transform.up.x, transform.up.y) * kbf); //movement + kickback if shot
        kbf = 0;

        Vector2 LookDir = rb.position - MousePos;
        float angle = Mathf.Atan2(LookDir.y, LookDir.x) * Mathf.Rad2Deg + 90f;
        
        rb.rotation = angle;
    }

    public void Kickback(float kb)
    {
        kbf = -kb;
    }
}
