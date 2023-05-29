using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Transform target;
    public float movementSpeed = 2f;
    public float attackDistance = 0.6f;
    public float approachDistance = 3f;
    public float attackCooldown = 0.3f;

    private Animator animator;
    private bool canAttack = true;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        animator.SetBool("isRunning", false);
    }
    private void Update()
    {
        if (!Enemy.isEnemy)
        {
            // Karakterin yönünü belirlemek için hedefi takip et
            if (transform.position.x < target.position.x)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f); // Saða bak
            }
            else if (transform.position.x > target.position.x)
            {
                transform.localScale = new Vector3(1f, 1f, 1f); // Sola bak
            }

            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (distanceToTarget <= attackDistance && canAttack)
            {
                StartCoroutine(AttackCooldown());
                Invoke("Attack", 0.3f);
                // Attack animasyonunu tetikle
                animator.SetTrigger("Attack1");
                animator.SetBool("isRunning", false);

            }
            else if (distanceToTarget > attackDistance && distanceToTarget <= approachDistance)
            {
                MoveTowardsTarget();
                animator.SetBool("isRunning", true);
            }
        }
    }

    private void MoveTowardsTarget()
    {
        float direction = Mathf.Sign(target.position.x - transform.position.x);
        transform.Translate(Vector2.right * direction * movementSpeed * Time.deltaTime);
    }

    private void Attack()
    {
        
        // Burada saldýrý mantýðý ve kodunu ekleyebilirsiniz
        Vector2 forward = transform.right;
        if (GetComponent<SpriteRenderer>().flipX)
            forward = -forward;

        Vector2 origin = transform.position;
        float raycastDistance = 0.6f;

        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, forward, raycastDistance);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player attacked!");
                hit.collider.gameObject.GetComponent<Enemy>().Hit();
                break;
                // Düþmaný vurduðunuzda yapmak istediðiniz iþlemleri buraya ekleyin
            }
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
