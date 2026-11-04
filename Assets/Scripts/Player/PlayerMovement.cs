
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float directionThreshold = 0.01f;

    private Rigidbody2D rb;
    private Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        movement = Vector2.zero;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.wKey.isPressed)
            movement.y += 1;

        if (Keyboard.current.sKey.isPressed)
            movement.y -= 1;

        if (Keyboard.current.aKey.isPressed)
            movement.x -= 1;

        if (Keyboard.current.dKey.isPressed)
            movement.x += 1;

        movement = movement.normalized;

        if (playerSprite != null)
        {
            if (movement.x < -directionThreshold)
                playerSprite.flipX = true;
            else if (movement.x > directionThreshold)
                playerSprite.flipX = false;
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetFloat("Speed", movement.magnitude);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(
            rb.position + movement * moveSpeed * Time.fixedDeltaTime
        );
    }

    public void IncreaseMoveSpeed(float percentage)
    {
        moveSpeed *= 1f + percentage;

        Debug.Log("Velocidad de movimiento actual: " + moveSpeed);
    }
}
