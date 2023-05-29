using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    Animator animator;
    public int healt = 100;
    public HealtBar healthBar;
    public static bool isDeath = false;
    public static bool isEnemy = false;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Hit()
    {
        animator.SetTrigger("Hit");
        healt -= 20;
        healthBar.SetHealt(healt);
        if (healt == 0 || healt < 0)
        {
            if (this.gameObject.tag == "Player")
            {
                this.gameObject.GetComponent<HeroKnight>().m_body2d.velocity = Vector2.zero;
                this.gameObject.GetComponent<HeroKnight>().enabled = false;
            }
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
        Destroy(this.gameObject);
        //SceneManager.LoadScene(0);
    }
}
