using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class GameCrontroller : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float runXVelocity = 4f;
    [SerializeField] float raycastDistance = 0.7f;
    [SerializeField] LayerMask collisionMask;

    Animator animator;
    Rigidbody2D physics;
    SpriteRenderer sprite;

    enum State { Idle, Walk, Climb, ClimbDown, Crounch, CrounchRun, Swim }

    State state = State.Idle;
    bool isGrounded = false;
    bool isClimb = false;
    bool isClimbDown = false;
    bool isCrounch = false;
    

    float horizontalInput = 0f;
    float verticalInput = 0f;


    void FixedUpdate()
    {
        // get player input
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, collisionMask).collider != null;
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        isClimb = Input.GetKey(KeyCode.UpArrow);
        isClimbDown = Input.GetKey(KeyCode.DownArrow);
        isCrounch = Input.GetKey(KeyCode.Z);
        
        // flip sprite based on horizontal input
        if (horizontalInput < 0f)
        {
            sprite.flipX = false;
        }
        else if (horizontalInput > 0f)
        {
            sprite.flipX = true;
        }

        if (Parede == true)
        {

            state = State.Climb;

        }

        if (Parede == false)
        {

            Debug.Log("Ccccc");

        }

        if (Agua == true)
        {

            state = State.Swim;

        }

        if (Agua == false)
        {

            Debug.Log("Fffff");

        }

        // run current state
        switch (state)
        {
            case State.Idle: IdleState(); break;
            case State.Walk: WalkState(); break;
            case State.Climb: ClimbState(); break;
            case State.Crounch:CrounchState(); break;
            case State.CrounchRun:CrounchRunState(); break;
            case State.Swim: SwimState(); break;
        }
    }

    void IdleState()
    {
        // actions
        animator.Play("idle");

        // transitions
        if (isGrounded)
        {
            if (horizontalInput != 0f)
            {
                state = State.Walk;
            }
            else if (isCrounch)
            {
                state = State.Crounch;
            }
        }
    }

    void WalkState()
    {
        // actions
        animator.Play("walk");
        physics.velocity = runXVelocity * horizontalInput * Vector2.right;

        // transitions
        if (horizontalInput == 0f)
        {
            state = State.Idle;
        }
        else if (isCrounch)
        {
            state = State.Crounch;
        }
    }

    void CrounchState()
    {
        // actions
        animator.Play("idle3");

        // transitions
        if (!isCrounch)
        {
            state = State.Idle;
        }
        else if (horizontalInput != 0f && isGrounded)
        {
            state = State.CrounchRun;
        }
    }

    void CrounchRunState()
    {
        // actions
        animator.Play("run3");
        physics.velocity = runXVelocity * horizontalInput * Vector2.right;

        // transitions
        if (horizontalInput == 0f || !isCrounch)
        {
            animator.Play("idle3");
            state = State.Idle;
        }
    }

    void ClimbState()
    {
        if (isClimb)
        {
            physics.velocity = runXVelocity * Vector2.up;
            animator.Play("climb");
            Debug.Log("Bbbbbb");
        }
        else if (verticalInput < 0f && Parede) // Se o jogador estiver descendo a parede
        {
            physics.velocity = runXVelocity * Vector2.down;
            animator.Play("climpDowm"); // Toca a animação de descida da parede

        }
        else if (horizontalInput != 0f)
        {
            state = State.Walk;
        }
        else
        {
            state = State.Idle; Parede = false;
        }
    }


    bool Parede;
    bool Agua;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Wall"))
        {

            Parede = true;
            Debug.Log("Aaaaaa");

        }

        if (col.CompareTag("Water"))
        {

            Agua = true;
            Debug.Log("Dddddd");

        }

    }

    
    void OnTriggerExit2D(Collider2D col)
    {
        // Resetar a variável Parede quando sair da colisão com uma parede
        if (col.CompareTag("Wall"))
        {
            Parede = false;
        }
    }

    void SwimState()
    {
        // actions
        animator.Play("swing");
        physics.velocity = runXVelocity * horizontalInput * Vector2.right;

        if (horizontalInput != 0f)
        {
            state = State.Walk;
        }
        else
        {
            state = State.Idle; Agua = false;
        }
    }


    void Awake()
    {
        animator = GetComponent<Animator>();
        physics = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }
}
