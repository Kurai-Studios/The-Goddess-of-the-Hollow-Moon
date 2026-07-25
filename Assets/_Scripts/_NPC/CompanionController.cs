using UnityEngine;

public class CompanionController : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float followSpeed = 3f;
    [SerializeField] private float stopDistance = 1f;

    private Rigidbody2D rb;
    private Collider2D interactionCollider;
    private bool isFollowing;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        interactionCollider = GetComponent<Collider2D>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (!isFollowing || player == null || PauseController.IsGamePaused)
            return;

        Vector2 toPlayer = player.position - transform.position;
        if (toPlayer.magnitude > stopDistance)
        {
            Vector2 direction = toPlayer.normalized;
            rb.MovePosition(rb.position + direction * followSpeed * Time.fixedDeltaTime);
        }
    }

    public void StartFollowing()
    {
        isFollowing = true;

        if (interactionCollider != null)
            interactionCollider.enabled = false;
    }

    public void StopFollowing()
    {
        isFollowing = false;
    }
}
