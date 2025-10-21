using System;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private PlayerStatsManager statsManager;
    [SerializeField] private Rigidbody2D rb;

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
    }

    private void Update()
    {
        HandleInput();
        UpdateDashTimer();
    }
    private void FixedUpdate()
    {
        if (!isDashing)
            Move();
    }

    private void Move()
    {
        float speed = statsManager.GetStat(StatType.MoveSpeed);
        Vector2 targetVelocity = moveInput * speed;

        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocity, 0.075f);
    }

    private void StartDash()
    {
        isDashing = true;
        dashDirection = moveInput != Vector2.zero ? moveInput : Vector2.up;
 

        float dashDistance = statsManager.GetStat(StatType.DashDistance);
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

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), $"Dashes: {dashes} | Timer: {dashTimer:F2}");
    }
}
