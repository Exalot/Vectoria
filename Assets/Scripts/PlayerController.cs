using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour // TODO: Fix Dashing error
{
    // Movement:

    // Horizontal
    private Rigidbody2D body;
    private float horizontalRaw;
    public float horizontalSpeed = 5.0f;

    // Jump
    public float jumpSpeed = 5.0f;
    public float jumpAcceleration = 2.0f;
    private float jumpVelocity = 0.0f;
    private bool isJumping = false;
    private bool canJump = true;

    // Dash
    float dashSpeed = 10.0f;
    float dashAcceleration = 10.0f;
    float dashTime = 0.75f;
    float dashTimer = 0.0f;
    float dashCancelTime = 0.15f;
    float dashEnableGravityTime = 0.5f;
    float dashCooldownTime = 5.0f;
    float dashCooldownTimer = 0.0f;
    bool canDash = true;
    bool isDashing = false; 
    bool wasDashing = false;
    bool canCancelDash = false;
    bool isFallingFromDash = false;
    bool isOnDashCooldown = false;
    Vector2 dashDirection;

    // Sprint
    [SerializeField] float sprintFactor = 1.75f;
    private float currentSprintFactor = 1.0f;
    float sprintTime = 2.0f;
    float sprintTimer = 0.0f;
    float sprintCooldownTime = 5.0f;
    float sprintCooldownTimer = 0.0f;
    bool canSprint = true;
    bool isSprinting = false;
    bool isOnSprintcooldown = false;

    // Animation:
    private SpriteRenderer _spriteRenderer;
    Animator anim;
    float fallDelay = 0.15f;
    float timeSinceFall = 0.0f;
    float blinkTime = 5.0f;
    float idleTimer;
    bool fall = false;
    private Vector2 movement;
    [SerializeField] private GameObject dashPrefab;
    [SerializeField] private float dashTailDelayDistance = 0.5f;
    private float DashTailDelayDistanceTracker = 0.0f;
    [SerializeField] private ParticleSystem dashParticles;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        isJumping = false;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && canJump)
        {
            jump();
        }

        if (Input.GetKeyDown(KeyCode.X) && canDash)
        {
            dash();
        }
        
        /*
        if (Input.GetKeyDown(KeyCode.Z) && canSprint)
        {
            sprint();
        }
        */

        if (fall)
        {
            timeSinceFall += Time.deltaTime;
            if(timeSinceFall > fallDelay)
            {
                fallOff();
            }
        }

        if (checkDashCancelled() && isDashing && canCancelDash)
            unDash();

        if (isDashing)
        {
            dashTimer += Time.deltaTime;
            DashTailDelayDistanceTracker += (body.velocity * Time.deltaTime).magnitude;

            if (dashTimer > dashCancelTime)
                canCancelDash = true;

            if (dashTimer > dashEnableGravityTime)
                body.gravityScale = 1.0f;

            if (dashTimer > dashTime)
                unDash();

            if (DashTailDelayDistanceTracker > dashTailDelayDistance)
            {
                DashTailDelayDistanceTracker = 0.0f;
                makeDashTail();
            }
        }

        else
        {
            anim.SetFloat("HorizontalRaw", horizontalRaw);
            anim.SetFloat("Speed", Mathf.Abs(horizontalRaw));

            if (Mathf.Abs(horizontalRaw) < 0.1f)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer > blinkTime)
                {
                    idleTimer = 0.0f;
                    anim.SetTrigger("Blink");
                }
            }

            else
                idleTimer = 0.0f;
        }

        anim.SetFloat("VerticalRaw", body.velocity.y);

        if (isOnDashCooldown)
        {
            dashCooldownTimer += Time.deltaTime;
            if (dashCooldownTimer > dashCooldownTime)
            {
                dashCooldownTimer = 0;
                isOnDashCooldown = false;
                canDash = true;
            }
        }

        if (isSprinting)
        {
            sprintTimer += Time.deltaTime;
            if (sprintTimer > sprintTime)
                unSprint();
        }

        else if (isOnSprintcooldown)
        {
            sprintCooldownTimer += Time.deltaTime;
            if (sprintCooldownTimer > sprintCooldownTime)
            {
                sprintCooldownTimer = 0;
                isOnSprintcooldown = false;
                canSprint = true;
            }
        }
    }


    private void FixedUpdate()
    {
        if (isDashing)
        {
            body.velocity -= dashDirection * (dashAcceleration * Time.fixedDeltaTime);
            return;
        }

        else
        {
            horizontalRaw = Input.GetAxis("Horizontal");

            if (isJumping || isFallingFromDash)
                jumpVelocity -= Time.fixedDeltaTime * jumpAcceleration;

            body.velocity = new Vector2(horizontalRaw * horizontalSpeed * currentSprintFactor, jumpVelocity);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (wasDashing)
        {
            canDash = false;
            isOnDashCooldown = true;
        }

        isJumping = false;
        canJump = true;
        isDashing = false;
        wasDashing = false;
        fall = false;
        isFallingFromDash = false;
        anim.SetBool("isJumping", false);
        jumpVelocity = 0.0f;
        timeSinceFall = 0.0f;
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        fall = true;
        canJump = false;
    }


    void fallOff()
    {
        isJumping = true;
        anim.SetBool("isJumping", true);
    }


    private void jump()
    {
        jumpVelocity = jumpSpeed;
        canJump = false;
        isJumping = true;
        anim.SetBool("isJumping", true);
    }


    private void dash()
    {
        dashDirection = getDashDirection();
        if (dashDirection != Vector2.zero)
        {
            body.velocity = dashDirection * dashSpeed;
            DashTailDelayDistanceTracker = dashTailDelayDistance;
            body.gravityScale = 0.0f;
            canJump = false;
            isJumping = false;
            isDashing = true;
            canDash = false;
            anim.SetFloat("HorizontalRaw", dashDirection.x);
            dashParticles.Play();
        }
    }


    private void unDash() 
    {
        dashTimer = 0.0f;
        jumpVelocity = 0.0f;
        body.gravityScale = 1.0f;
        DashTailDelayDistanceTracker = 0.0f;
        canJump = false;
        isDashing = false;
        wasDashing = true;
        isFallingFromDash = dashDirection.y > 0.0f;
        canDash = false;
        canCancelDash = false;
        anim.SetFloat("VerticalRaw", body.velocity.y);
        dashParticles.Stop();
    }
    
    private void makeDashTail()
    {
        GameObject dashTail = Instantiate(dashPrefab, body.position, Quaternion.identity);
        PlayerDashController dashController = dashTail.GetComponent<PlayerDashController>();
        dashController.begin(_spriteRenderer.sprite, body.velocity.x < 0);
    }


    private Vector2 getDashDirection()
    {
        float dashX = 0.0f;
        float dashY = 0.0f;

        if (Input.GetKey(KeyCode.RightArrow))
            dashX = 1.0f;

        else if (Input.GetKey(KeyCode.LeftArrow))
            dashX = -1.0f;

        if (Input.GetKey(KeyCode.UpArrow))
            dashY = 1.0f;

        else if (Input.GetKey(KeyCode.DownArrow))
            dashY = -1.0f;

        return new Vector2(dashX, dashY).normalized;
    }


    private bool checkDashCancelled()
    {
        Vector2 dashMovement = body.velocity;

        if (Input.GetKeyDown(KeyCode.DownArrow) && dashMovement.y > 0.0f)
            return true;

        else if (Input.GetKeyDown(KeyCode.UpArrow) && dashMovement.y < 0.0f)
            return true;

        if (Input.GetKeyDown(KeyCode.LeftArrow) && dashMovement.x > 0.0f)
            return true;

        else if (Input.GetKeyDown(KeyCode.RightArrow) && dashMovement.x < 0.0f)
            return true;

        return false;
    }


    private void sprint()
    {
        if (!isJumping)
        {
            currentSprintFactor = sprintFactor;
            isSprinting = true;
            canSprint = false;
            anim.SetBool("isSprinting", true);
        }
    }


    private void unSprint()
    {
        currentSprintFactor = 1.0f;
        sprintTimer = 0.0f;
        isSprinting = false;
        isOnSprintcooldown = true;
        anim.SetBool("isSprinting", false);
    }

}
