using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private PlayerStatsManager statsManager;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    private Vector2 moveInput;
    private bool isDashing = false;
    private Vector2 dashDirection;
    private Vector2 currentVelocity;

    private float dashTimer;
    private int dashes;
    private int MaxDashes => Mathf.FloorToInt(statsManager.GetStat(StatType.Dashes));


    private void Start()
    {
        dashes = MaxDashes;
        dashTimer = 0f;
        ResetDashes();
    }

    private void Update()
    {
        if (!GameState.inPauseMenu && !GameState.inShop && !GameState.inPauseMenu)
        {
            HandleInput();
            UpdateDashTimer();
            UpdateAnimator();
        }

    }
    private void FixedUpdate()
    {
        if (!GameState.inPauseMenu && !GameState.inShop)
        {
            if (!isDashing)
                Move();
        }
    }

    public void SetToZero()
    {
        //Debug.Log("Ist richtung null");
        //rb.MovePosition(Vector3.zero);
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.transform.position = Vector3.zero;
    }

    private void Move()
    {
        float speed = statsManager.GetStat(StatType.MoveSpeed) + statsManager.GetStatMulti(StatMulti.MovePercent);
        Vector2 targetVelocity = moveInput * speed;

        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocity, 0.075f);
    }

    private void StartDash()
    {
        isDashing = true;
        dashDirection = moveInput != Vector2.zero ? moveInput : Vector2.up;

        animator.SetFloat("MoveX", dashDirection.x);
        animator.SetFloat("MoveY", dashDirection.y);
        animator.SetTrigger("Dash");

        float dashDistance = statsManager.GetStat(StatType.DashDistance) * (Mathf.Max((statsManager.GetStat(StatType.MoveSpeed) / 4 ), 1f));
        rb.linearVelocity = dashDirection.normalized * dashDistance / 0.1f;

        float dashDuration = 0.1f;
        Invoke(nameof(EndDash), dashDuration);
    }
    private void EndDash()
    {
        isDashing = false;
    }
    private void UpdateDashTimer()
    {
        if (dashes < MaxDashes)
        {
            dashTimer -= Time.deltaTime;

            int possibleDashes = Mathf.FloorToInt(dashTimer / statsManager.GetStat(StatType.DashCooldown));
            int newDashes = MaxDashes - possibleDashes;

            dashes = Mathf.Clamp(newDashes, 0, MaxDashes);

            if (dashTimer < 0f)
            {
                dashTimer = 0f;
            }
        }

    }

    private void HandleInput()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (Input.GetKeyDown(KeyCode.LeftShift)){

            if (dashes > 0)
            {
                dashes--;
                dashTimer += statsManager.GetStat(StatType.DashCooldown);
                StartDash();
            }
        }
    }

    private void UpdateAnimator() 
    {
        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.y);
        animator.SetBool("isMoving", isMoving);
    }

    public void ResetDashes()
    {
        dashes = 0;
        dashTimer = MaxDashes * statsManager.GetStat(StatType.DashCooldown);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), $"Dashes: {dashes} | Timer: {dashTimer:F2}");
    }
}
