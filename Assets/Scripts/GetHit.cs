using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetHit : MonoBehaviour
{
    [SerializeField] private GameObject healtPref;
    [SerializeField] private GameObject shieldPref;
    [SerializeField] private GameObject attackBuffPref;
    [SerializeField] private GameObject speedBuffPref;


    Animator animator;
    public int healt = 100;
    public HealtBar healthBar;
    public static bool isDeath = false;
    public static bool isEnemy = false;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Hit(int damage=20)
    {
        //
        if(this.gameObject.tag == "Player" && this.gameObject.GetComponent<PlayerController>().isShield)
        {
            //block
        }
        else
        {
            animator.SetTrigger("Hit");
            healt -= damage;
            healthBar.SetHealt(healt);
        }
        if (healt == 0 || healt < 0)
        {
            //player death
            if (this.gameObject.tag == "Player")
            {
                this.gameObject.GetComponent<PlayerController>().m_body2d.velocity = Vector2.zero;
                this.gameObject.GetComponent<PlayerController>().enabled = false;
            }
            //Enemy death
            else if (this.gameObject.tag == "Enemy")
                this.gameObject.GetComponent<EnemyController>().enabled = false;
            animator.SetBool("isDeath", true);
            var colliders = GetComponents<Collider2D>();
            foreach (var item in colliders)
            {
                item.enabled = false;
            }
            this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            this.enabled = false;
            Invoke("Death", 2.5f);
        }
    }

    void Death()
    {
        //%50 ihtimalle özellik verecek
        if(Random.Range(0,2) != 0)
        {
            int selectBuf = Random.Range(0, 5);
            //Eðer 0 ise can, 1 ise atak, 2 ise speed, 3 ise shield
            if (selectBuf == 0)
                Instantiate(healtPref, this.gameObject.transform.position, Quaternion.identity);
            else if(selectBuf == 1)
                Instantiate(attackBuffPref, this.gameObject.transform.position, Quaternion.identity);
            else if (selectBuf == 2)
                Instantiate(speedBuffPref, this.gameObject.transform.position, Quaternion.identity);
            else
                Instantiate(shieldPref, this.gameObject.transform.position, Quaternion.identity);
        }
        Destroy(this.gameObject);
        //SceneManager.LoadScene(0);
    }
}
