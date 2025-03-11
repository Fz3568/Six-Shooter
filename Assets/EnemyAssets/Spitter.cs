using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spitter : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private float attackRange = 8f;
    [SerializeField] private float shootCooldown = 3f;
    [SerializeField] private GameObject acidBlobPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private LayerMask obstacleLayers;

    private Transform player;
    private float lastShootTime;
    private Rigidbody2D rb;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        
        rb = GetComponent<Rigidbody2D>();
        lastShootTime = Time.time;

    }

    private void Update()
    {

        if (PlayerInRange() && HasLineOfSight())
        {
            FacePlayer();
            TryShoot();

            if (Vector2.Distance(transform.position, player.position) > stoppingDistance)
            {
                MoveTowardsPlayer();
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (shootPoint != null)
        {

            Debug.DrawRay(shootPoint.position, shootPoint.right * 2, Color.green);
        }

    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 newPosition = rb.position + direction * movementSpeed * Time.deltaTime;
        rb.MovePosition(newPosition);
    }

    private bool PlayerInRange()
    {
        return Vector3.Distance(transform.position, player.position) <= attackRange;
    }

    private bool HasLineOfSight()
    {
        Vector2 origin = shootPoint.position;
        Vector2 target = player.position;
        Vector2 direction = (target - origin).normalized;

        float maxDistance = Vector2.Distance(origin, target);

        int playerLayer = LayerMask.NameToLayer("Player");
        LayerMask finalMask = obstacleLayers | (1 << playerLayer);

        RaycastHit2D hit = Physics2D.Raycast( origin,direction,maxDistance,finalMask);

        Debug.DrawRay(origin, direction * maxDistance, Color.magenta, 0.1f);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    private void FacePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }


    private void TryShoot()
    {
        if (Time.time >= lastShootTime + shootCooldown)
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }

    private void Shoot()
    {
        GameObject blob = Instantiate(acidBlobPrefab, shootPoint.position, shootPoint.rotation);
        blob.GetComponent<Rigidbody2D>().linearVelocity = shootPoint.right * 10f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }

}
