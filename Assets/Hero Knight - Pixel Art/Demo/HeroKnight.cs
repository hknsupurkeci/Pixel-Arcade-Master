using UnityEngine;
using System.Collections;
using TMPro;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] float      m_rollForce = 6.0f;
    [SerializeField] bool       m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    private Animator            m_animator;
    public Rigidbody2D          m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private Sensor_HeroKnight   m_wallSensorR1;
    private Sensor_HeroKnight   m_wallSensorR2;
    private Sensor_HeroKnight   m_wallSensorL1;
    private Sensor_HeroKnight   m_wallSensorL2;
    private bool                m_isWallSliding = false;
    private bool                m_grounded = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    public static float         m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private float               m_rollDuration = 8.0f / 14.0f;
    private float               m_rollCurrentTime;

    RaycastHit2D hit;
    Vector2 forward;
    public Vector3 vec;
    public float speed;
    [SerializeField] GameObject blink;
    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (!Enemy.isDeath)
        {
            // Increase timer that controls attack combo
            m_timeSinceAttack += Time.deltaTime;

            //// Increase timer that checks roll duration
            //if (m_rolling)
            //    m_rollCurrentTime += Time.deltaTime;

            //// Disable rolling if timer extends duration
            //if (m_rollCurrentTime > m_rollDuration)
            //    m_rolling = false;

            //Check if character just landed on the ground
            if (!m_grounded && m_groundSensor.State())
            {
                m_grounded = true;
                m_animator.SetBool("Grounded", m_grounded);
            }

            //Check if character just started falling
            if (m_grounded && !m_groundSensor.State())
            {
                m_grounded = false;
                m_animator.SetBool("Grounded", m_grounded);
            }

            // -- Handle input and movement --
            float inputX = Input.GetAxis("Horizontal");

            //Swap direction of sprite depending on walk direction
            if (inputX > 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;
                m_facingDirection = 1;
            }

            else if (inputX < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
                m_facingDirection = -1;
            }

            // Move
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

            //Set AirSpeed in animator
            m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

            // -- Handle Animations --
            //Wall Slide
            //m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
            //m_animator.SetBool("WallSlide", m_isWallSliding);

            //Death
            //if (Input.GetKeyDown("e") && !m_rolling)
            //{
            //    m_animator.SetBool("noBlood", m_noBlood);
            //    m_animator.SetTrigger("Death");
            //}

            ////Hurt
            //else if (Input.GetKeyDown("q") && !m_rolling)
            //    m_animator.SetTrigger("Hurt");

            //Attack
            if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f)
            {
                AttackAnim();
            }

            // Blink
            else if (Input.GetMouseButtonDown(1))
            {
                AttackAnim();
                Instantiate(blink, transform.position, Quaternion.identity);
                Invoke("Blink", 0.3f);
            }

            //else if (Input.GetMouseButtonUp(1))
            //    m_animator.SetBool("IdleBlock", false);


            //Jump
            if (Input.GetKeyDown("space") && m_grounded /*&& !m_rolling*/)
            {
                m_animator.SetTrigger("Jump");
                m_grounded = false;
                m_animator.SetBool("Grounded", m_grounded);
                m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
                m_groundSensor.Disable(0.2f);
            }

            //Run
            else if (Mathf.Abs(inputX) > Mathf.Epsilon)
            {
                // Reset timer
                m_delayToIdle = 0.05f;
                m_animator.SetInteger("AnimState", 1);
            }

            //Idle
            else
            {
                // Prevents flickering transitions to idle
                m_delayToIdle -= Time.deltaTime;
                if (m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
            }

            //comboTime eğer 5 saniye boyunca atak yapmazsa combo sıfırlanıyor
            combotime += Time.deltaTime;
            if (combotime > 3f)
            {
                comboTxt.text = "0";
                combo = 0;
            }
            //Debug.Log(combotime);
        }
    }

    [SerializeField] TextMeshProUGUI comboTxt;
    [SerializeField] TextMeshProUGUI comboTxtStr;

    float combo = 0, combotime = 1;
    private void Attack()
    {
        Vector2 forward = transform.right;
        if (GetComponent<SpriteRenderer>().flipX)
            forward = -forward;

        Vector2 origin = transform.position;
        float raycastDistance = 0.8f;

        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, forward, raycastDistance);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                //kamera sallama efekti
                GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
                CameraFollow cameraShake = cameraObject.GetComponent<CameraFollow>();
                cameraShake.ShakeCamera();
                IncreaseComboCount();
                //burada her atak yaptığında combotime 1 oluyor
                combotime = 1;
                combo++;
                comboTxt.text = combo.ToString();
                Debug.Log("Enemy attacked!");
                hit.collider.gameObject.GetComponent<Enemy>().Hit();
                //break;
                // Düşmanı vurduğunuzda yapmak istediğiniz işlemleri buraya ekleyin
            }
        }
    }
    private void AttackAnim()
    {
        m_currentAttack++;

        // Loop back to one after third attack
        if (m_currentAttack > 2)
            m_currentAttack = 1;

        // Reset Attack combo if time since last attack is too large
        if (m_timeSinceAttack > 1.0f)
            m_currentAttack = 1;
        Invoke("Attack", 0.3f);
        // Call one of three attack animations "Attack1", "Attack2", "Attack3"
        m_animator.SetTrigger("Attack" + m_currentAttack);

        // Reset timer
        m_timeSinceAttack = 0.0f;
    }
    private void Blink()
    {
        float blinkDistance = speed * Time.fixedDeltaTime;

        if (GetComponent<SpriteRenderer>().flipX)
        {
            transform.position += -vec.normalized * blinkDistance;
        }
        else
        {
            transform.position += vec.normalized * blinkDistance;
        }
    }

    public void IncreaseComboCount()
    {
        ApplyComboEffects();
    }

    private void ApplyComboEffects()
    {
        //// Rengi değiştirme efekti
        //comboTxt.color = Color.red;

        // Titreme efekti
        StartCoroutine(ShakeComboText());
    }

    private IEnumerator ShakeComboText()
    {
        Vector3 originalPosition = comboTxt.transform.position;
        Vector3 originalPositionStr = comboTxtStr.transform.position;

        float shakeDuration = 0.5f;
        float shakeMagnitude = 5f;

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            comboTxt.transform.position = originalPosition + new Vector3(x, y, 0f);
            comboTxtStr.transform.position = originalPositionStr + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;

            yield return null;
        }
        comboTxtStr.transform.position = originalPositionStr;
        comboTxt.transform.position = originalPosition;
    }
}



//// Animation Events
//// Called in slide animation.
//void AE_SlideDust()
//    {
//        Vector3 spawnPosition;

//        if (m_facingDirection == 1)
//            spawnPosition = m_wallSensorR2.transform.position;
//        else
//            spawnPosition = m_wallSensorL2.transform.position;

//        if (m_slideDust != null)
//        {
//            // Set correct arrow spawn position
//            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
//            // Turn arrow in correct direction
//            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
//        }
//    }
//}
