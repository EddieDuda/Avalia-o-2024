using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class GameCrontroller : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float jumpYVelocity = 8f;
    [SerializeField] float runXVelocity = 4f;
    [SerializeField] float raycastDistance = 0.7f;
    [SerializeField] LayerMask collisionMask;
    [SerializeField] float attackDuration = 1f;
    [SerializeField] float dashSpeed = 8f;
    [SerializeField] float startDashTime = 0.1f;

    Animator animator;
    Rigidbody2D physics;
    SpriteRenderer sprite;

    enum State { Idle, Walk, Climb, ClimbDown, Crounch, CrounchRun, Swim, Jump, Fall, Attack, Dash }

    State state = State.Idle;
    bool isGrounded = false;
    bool isClimb = false;
    bool isCrounch = false;
    bool isJump = false;
    bool isAttack = false;
    bool UpAttack = false;
    bool isDash = false;

    float horizontalInput = 0f;
    float verticalInput = 0f;
    float CurrentAttack;
    float dashTime;
    int direction = 0;

    void FixedUpdate()
    {
        // get player input
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, collisionMask).collider != null;
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        isClimb = Input.GetKey(KeyCode.UpArrow);
        isCrounch = Input.GetKey(KeyCode.LeftShift);
        isJump = Input.GetKey(KeyCode.Z);
        isAttack = Input.GetKey(KeyCode.X);
        UpAttack = Input.GetKey(KeyCode.UpArrow);
        isDash = Input.GetKey(KeyCode.C);

        
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

        if (UpAttack == true)
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
            case State.Jump: JumpState(); break;
            case State.Fall: FallState(); break;
            case State.Attack: AttackState(); break;
            case State.Dash: DashState(); break;
        }
    }
    
    void IdleState()
    {
        // actions
        animator.Play("idle");

        // transitions
        if (horizontalInput != 0f)
        {
            state = State.Walk;
        }
        else if (isCrounch)
        {
            state = State.Crounch;
        }
        else if (isClimb && Parede == true)
        {
            state = State.Climb;
        }
        else if (isJump)
        {
            state = State.Jump;
        }
        else if (isAttack)
        {
            state = State.Attack;
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
            state = State.CrounchRun;
        }
        else if (isJump)
        {
            state = State.Jump;
        }
        else if (isAttack)
        {
            state = State.Attack;
        }
        else if (isDash)
        {
            state = State.Dash;
        }
    }

    void JumpState()
    {
        animator.Play("Jump");
        physics.velocity = runXVelocity * horizontalInput * Vector2.right + jumpYVelocity * Vector2.up;

        // transitions
        state = State.Fall;
    }

    void FallState()
    {
        if(physics.velocity.y < 0f && isGrounded == false)
        {
            animator.Play("Fall");
        }
        physics.velocity = physics.velocity.y * Vector2.up + runXVelocity * horizontalInput * Vector2.right;
        // transitions
        if (isGrounded && physics.velocity.y < 0f)
        {
            if (horizontalInput != 0f)
            {
                state = State.Walk;
            }
            if (Parede)
            {
                state = State.Climb;
            }
            else
            {
                state = State.Idle;
            }
        }
        else if (isAttack)
        {
            state = State.Attack;
        }

    }

    void DashState()
    {
     if (direction == 0)   
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                direction = 1;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                direction = 2;
            }
        }
        else
        {
            if(dashTime <= 0)
            {
                direction = 0;
                dashTime = startDashTime;
                //physics.velocity = Vector2.zero;
                physics.velocity = runXVelocity * horizontalInput * Vector2.right;
                if(isCrounch)
                {
                    state = State.CrounchRun;
                }
                else
                { 
                    state = State.Walk;     
                }           
            }
            else
            {
                dashTime -= Time.deltaTime;
                if (direction == 1)
                {
                    physics.velocity = Vector2.left * dashSpeed;
                    animator.Play("Dash");
                    if(isCrounch)
                    {
                        animator.Play("Roll");
                    }
                }
                else if(direction == 2)
                {
                    physics.velocity = Vector2.right * dashSpeed;
                    animator.Play("Dash");
                    if (isCrounch)
                    {
                        animator.Play("Roll");
                    }
                }
            }
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

    void AttackState()
    {
        if(isGrounded && UpAttack == false)
        { 
            animator.Play("attack1");
            CurrentAttack += Time.fixedDeltaTime;
        }
        else if (physics.velocity.y < 0f && isGrounded == false)
        {
            animator.Play("attack2");
            CurrentAttack += Time.fixedDeltaTime;

        }
        else if (UpAttack)
        {
            animator.Play("attack3");
            CurrentAttack += Time.fixedDeltaTime;
            UpAttack = false;
        }
        if (CurrentAttack > attackDuration)
        {
            state = State.Idle;
            CurrentAttack = 0f;
        }
    }

    void CrounchRunState()
    {
        // actions
        animator.Play("run3");
        physics.velocity = runXVelocity * horizontalInput * Vector2.right;

        // transitions
        if (horizontalInput == 0f && isCrounch)
        {
            state = State.Crounch;
        }
        else if (isCrounch == false)
        {
            state = State.Walk;
        }
        else if (isDash)
        {
            state = State.Dash;
        }
    }

    void ClimbState()
    {
        if (isClimb && Parede == true)
        {
            physics.velocity = runXVelocity * verticalInput * Vector2.up;
            animator.Play("climb");
            sprite.flipX = true;
            if (isJump)
            {
                state = State.Jump;
            }
        }
        else if (isCrounch)
        {
            physics.velocity = runXVelocity * verticalInput * Vector2.zero;
            animator.Play("Idle2");
            sprite.flipX = true;
            Parede = false;
            if (isJump)
            {
                state = State.Jump;
            }
        }
        
        else if (isClimb == false && isGrounded == false) // Se o jogador estiver descendo a parede
        {
            physics.velocity = runXVelocity * Vector2.down;
            animator.Play("climpDowm"); // Toca a animação de descida da parede
            sprite.flipX = true;
            Parede = false;
            Debug.Log("Bbbbbb");
            if (isJump)
            {
                state = State.Jump;
            }
        }
        else if (isGrounded && isClimb == false)
        {
            state = State.Idle; 
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
        if (col.CompareTag("Water"))
        {
            Agua = false;
        }
    }

    void SwimState()
    {
        // actions
        if(Agua)
        {
            animator.Play("swing");
            physics.velocity = runXVelocity * horizontalInput * Vector2.right;
        }
        else if (Agua == false && horizontalInput != 0f && isCrounch == false)
        {
            state = State.Walk;
        }
        else if (Agua == false && horizontalInput != 0f && isCrounch)
        {
            state = State.CrounchRun;
        }
    }


    void Awake()
    {
        animator = GetComponent<Animator>();
        physics = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        dashTime = startDashTime;
    }



}
