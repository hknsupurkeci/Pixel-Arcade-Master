using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject enemySpeelObj;

    private Transform target;
    public float movementSpeed = 2f;
    public float attackDistance = 0.8f;
    public float approachDistance = 3f;
    public float attackCooldown = 0.3f;

    private Animator animator;
    private bool canAttack = true;

    private BoxCollider2D collider;
    public GameObject rectTransform;
    private float colliderOffsetXvalue;

    private float speelTime = 1;
    private void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        colliderOffsetXvalue = collider.offset.x;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        animator.SetBool("isRunning", false);
        //if (this.gameObject.tag == "Enemy2")
        //    animator.SetTrigger("Cast");
    }
    
    private void Update()
    {
        if (!GetHit.isEnemy)
        {
            // Karakterin yönünü belirlemek için hedefi takip et
            if (transform.position.x < target.position.x)
            {
                //transform.localScale = new Vector3(1f, 1f, 1f); // Saða bak
                //GetComponent<SpriteRenderer>().flipX = false;
                Turn(false);
            }
            else if (transform.position.x > target.position.x)
            {
                //transform.localScale = new Vector3(-1f, 1f, 1f); // Sola bak
                //GetComponent<SpriteRenderer>().flipX = true;
                Turn(true);
            }

            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (distanceToTarget <= attackDistance && canAttack)
            {
                StartCoroutine(AttackCooldown());
                Invoke("Attack", 0.5f);
                // Attack animasyonunu tetikle
                animator.SetTrigger("Attack1");
                animator.SetBool("isRunning", false);

            }
            else if (distanceToTarget > attackDistance && distanceToTarget <= approachDistance)
            {
                if(this.gameObject.tag == "Enemy2")
                {
                    if(speelTime > 0)
                    {
                        animator.SetTrigger("Cast");
                        Vector3 speel = new Vector3(
                            target.transform.position.x,
                            target.transform.position.y + 0.7f,
                            target.transform.position.z
                        );
                        Instantiate(enemySpeelObj, speel, Quaternion.identity);
                        speelTime = -0.5f;
                    }
                    else
                    {
                        MoveTowardsTarget();
                    }
                    speelTime += Time.deltaTime;
                }
                else
                {
                    MoveTowardsTarget();
                }
                animator.SetBool("isRunning", true);
            }
            else
            {
                animator.SetBool("isRunning", false);
            }
        }
    }

    void Turn(bool flag)
    {
        if(this.gameObject.tag == "Enemy")
            GetComponent<SpriteRenderer>().flipX = flag;
        else if (this.gameObject.tag == "Enemy2")
        {
            // true ise false,false ise true
            if (flag)
            {
                GetComponent<SpriteRenderer>().flipX = false;
                collider.offset = new Vector2(colliderOffsetXvalue, collider.offset.y);
                rectTransform.transform.localPosition = Vector3.zero;
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = true;
                collider.offset = new Vector2(-0.36f, collider.offset.y);
                rectTransform.transform.localPosition = new Vector3(-0.723f,0,0);
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
        if (GetComponent<SpriteRenderer>().flipX && this.gameObject.tag != "Enemy2")
            forward = -forward;
        else if(!GetComponent<SpriteRenderer>().flipX && this.gameObject.tag == "Enemy2")
            forward = -forward;
        Vector2 origin = transform.position;
        float raycastDistance = 0.8f;

        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, forward, raycastDistance);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player attacked!");
                hit.collider.gameObject.GetComponent<GetHit>().Hit(10);
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
