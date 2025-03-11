using UnityEngine;

public class Ram : MonoBehaviour
{
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float chargeSpeed = 15f;
    [SerializeField] private float stunDuration = 3f;
    [SerializeField] private LayerMask obstacleLayers;

    [SerializeField] private LineRenderer targetingLaser;
    [SerializeField] private Color stunColor = Color.blue;

    private Transform player;
    private Rigidbody2D rb;
    private float stateTimer;
    private Vector2 chargeDirection;
    private Color originalColor;

    private bool isIdle = true;
    private bool isTargeting = false;
    private bool isCharging = false;
    private bool isStunned = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalColor = GetComponent<SpriteRenderer>().color;
        targetingLaser.enabled = false;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SetIdleState();
    }

    private void Update()
    {
        if (isIdle)
        {
            UpdateIdle();
        }
        else if (isTargeting)
        {
            UpdateTargeting();
        }
        else if (isCharging)
        {
            UpdateCharging();
        }
        else if (isStunned)
        {
            UpdateStunned();
        }
    }

    private void SetIdleState()
    {
        isIdle = true;
        isTargeting = false;
        isCharging = false;
        isStunned = false;

        rb.linearVelocity = Vector2.zero;
        GetComponent<SpriteRenderer>().color = originalColor;
        targetingLaser.enabled = false;
    }

    private void SetTargetingState()
    {
        isIdle = false;
        isTargeting = true;
        isCharging = false;
        isStunned = false;

        stateTimer = 2f;
        targetingLaser.enabled = true;
    }

    private void SetChargingState()
    {
        isIdle = false;
        isTargeting = false;
        isCharging = true;
        isStunned = false;

        chargeDirection = transform.right;
        rb.linearVelocity = chargeDirection * chargeSpeed;
        stateTimer = 1f;
    }

    private void SetStunnedState()
    {
        isIdle = false;
        isTargeting = false;
        isCharging = false;
        isStunned = true;

        stateTimer = stunDuration;
        rb.linearVelocity = Vector2.zero;
        GetComponent<SpriteRenderer>().color = stunColor;
        targetingLaser.enabled = false;
    }

    private void UpdateIdle()
    {
        if (PlayerInRange() && HasLineOfSight())
        {
            SetTargetingState();
        }
    }

    private void UpdateTargeting()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,targetRotation,Time.deltaTime * 200f);

        targetingLaser.SetPosition(0, transform.position);
        targetingLaser.SetPosition(1, (Vector2)transform.position + direction * detectionRange);

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            SetChargingState();
        }
    }

    private void UpdateCharging()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            SetStunnedState();
        }
    }

    private void UpdateStunned()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            SetIdleState();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCharging && collision.gameObject.CompareTag("Wall"))
        {
            SetStunnedState();
        }
    }

    private bool PlayerInRange()
    {
        if (player == null)
        {
            return false;
        }
        else
        {
            return Vector2.Distance(transform.position, player.position) <= detectionRange;
        }
    }

    private bool HasLineOfSight()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction,distance,obstacleLayers);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }
}