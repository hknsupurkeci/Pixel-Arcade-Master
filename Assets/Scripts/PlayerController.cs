using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerController : MonoBehaviour {

    [SerializeField] float      m_speed = 3.0f;
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

    //Damage and Combo
    int damage = 20;
    float blinkPower = 1;
    float repeatRate = 0.5f;
    float rightClickTime = 0;
    float combo = 0, combotime = 1;
    [SerializeField] TextMeshProUGUI comboTxt;
    [SerializeField] TextMeshProUGUI comboTxtStr;

    //Blink
    public Vector3 vec;
    public float speed = 3;
    [SerializeField] GameObject blink;
    GameObject cameraObject;
    bool rightClickFlag = false;

    //Shield Buf
    public bool isShield = false;
    private float shieldTimer = 0;
    public float ActiveShieldTime = 7;
    [SerializeField] GameObject shieldParticalObj;

    //Attack Buf
    public bool isAttackBuff = false;
    private float attackBuffTimer = 0;
    public float ActiveAttackBuffTime = 7;
    [SerializeField] GameObject attackParticelObj;

    //Speed Buf
    public bool isSpeedBuff = false;
    private float speedBuffTimer = 0;
    public float ActiveSpeedBuffTime = 7;
    public float SpeedBuffSpeed = 7;
    [SerializeField] GameObject speedParticelObj;

    // Use this for initialization
    void Start ()
    {
        cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (!GetHit.isDeath)
        {
            // Increase timer that controls attack combo
            m_timeSinceAttack += Time.deltaTime;

            //Jump controller
            JumpAndFallController();

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
            m_body2d.velocity = new Vector2(inputX * (isSpeedBuff ? SpeedBuffSpeed:m_speed), m_body2d.velocity.y); //Speed buff aktifse

            //Set AirSpeed in animator
            m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

            //Attack
            if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f)
            {
                AttackAnim();
            }

            // Blink
            else if (Input.GetMouseButtonDown(1) && !rightClickFlag)
            {
                rightClickFlag = true;
            }
            if (Input.GetMouseButtonUp(1))
            {
                RightClickUp();
            }
            //Blink özellikleri
            BlinkProperties();
            //Jump
            if (Input.GetKeyDown("space") && m_grounded)
                Jump();
            //Run
            else if (Mathf.Abs(inputX) > Mathf.Epsilon)
                Run();
            //Idle
            else
                Idle();
            //Combo özelliği
            ComboTimer();

            //Shield
            if (isShield)
            {
                //7 saniye sonra kalkanı kaldırıyoruz
                shieldTimer += Time.deltaTime;
                if(shieldTimer > ActiveShieldTime)
                {
                    shieldParticalObj.SetActive(false);
                    isShield = false;
                    shieldTimer = 0;
                }
            }
            //Attack
            if (isAttackBuff)
            {
                attackBuffTimer += Time.deltaTime;
                if (attackBuffTimer > ActiveShieldTime)
                {
                    attackParticelObj.SetActive(false);
                    isAttackBuff = false;
                    attackBuffTimer = 0;
                }
            }
            //Speed
            if (isSpeedBuff)
            {
                speedBuffTimer += Time.deltaTime;
                if (speedBuffTimer > ActiveShieldTime)
                {
                    speedParticelObj.SetActive(false);
                    isSpeedBuff = false;
                    speedBuffTimer = 0;
                }
            }
        }
    }

    void JumpAndFallController()
    {
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
    }
    void RightClickUp()
    {
        rightClickFlag = false;
        // Fare sağ tıkı basıldığında yapılacak işlemler buraya yazılır
        AttackAnim();
        Instantiate(blink, transform.position, Quaternion.identity);
        Invoke("BlinkPozision", 0.3f);
    }
    void BlinkProperties()
    {
        float maxBlinkPower = 3;
        if (rightClickFlag)
        {
            rightClickTime += Time.deltaTime;

            if (rightClickTime >= repeatRate)
            {
                Debug.Log(damage);
                if (blinkPower < maxBlinkPower)
                {
                    blinkPower += 0.2f;
                    if (damage < 100)
                        damage += 20;
                }

                // Burada her 0.5 saniyede bir çalışacak kodlarınızı yazabilirsiniz
                StartCoroutine(RightClickAnim());
                rightClickTime = 0f; // Timer'ı sıfırla
            }
        }
    }
    void ComboTimer()
    {
        //comboTime eğer 5 saniye boyunca atak yapmazsa combo sıfırlanıyor
        combotime += Time.deltaTime;
        if (combotime > 3f)
        {
            comboTxt.text = "0";
            combo = 0;
        }
    }
    void Idle()
    {
        // Prevents flickering transitions to idle
        m_delayToIdle -= Time.deltaTime;
        if (m_delayToIdle < 0)
            m_animator.SetInteger("AnimState", 0);
    }
    void Run()
    {
        // Reset timer
        m_delayToIdle = 0.05f;
        m_animator.SetInteger("AnimState", 1);
    }
    void Jump()
    {
        m_animator.SetTrigger("Jump");
        m_grounded = false;
        m_animator.SetBool("Grounded", m_grounded);
        m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
        m_groundSensor.Disable(0.2f);
    }

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
                CameraFollow cameraShake = cameraObject.GetComponent<CameraFollow>();
                cameraShake.ShakeCamera();
                IncreaseComboCount();
                //burada her atak yaptığında combotime 1 oluyor
                combotime = 1;
                combo++;
                comboTxt.text = combo.ToString();
                Debug.Log("Enemy attacked!");
                hit.collider.gameObject.GetComponent<GetHit>().Hit(isAttackBuff ? damage + 30 : damage); //Eğer atak bufi varsa ekstra 30 vuracak
                //break;
                // Düşmanı vurduğunuzda yapmak istediğiniz işlemleri buraya ekleyin
            }
        }
        damage = 20;
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
    private void BlinkPozision()
    {
        float blinkDistance = speed * Time.fixedDeltaTime * blinkPower;

        if (GetComponent<SpriteRenderer>().flipX)
        {
            transform.position += -vec.normalized * blinkDistance;
        }
        else
        {
            transform.position += vec.normalized * blinkDistance;
        }
        blinkPower = 1;
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
        float shakeMagnitude = 3f;

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

    private IEnumerator RightClickAnim()
    {
        StartCoroutine(ShakeComboText());
        Instantiate(blink, transform.position, Quaternion.identity);
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Healt"))
        {
            Destroy(collision.gameObject);
            this.gameObject.GetComponent<GetHit>().healthBar.SetHealt(100);
            this.gameObject.GetComponent<GetHit>().healt = 100;
        }
        else if (collision.CompareTag("Shield"))
        {
            isShield = true;
            shieldParticalObj.SetActive(true);
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("AttackBuff"))
        {
            isAttackBuff = true;
            attackParticelObj.SetActive(true);
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("SpeedBuff"))
        {
            isSpeedBuff = true;
            speedParticelObj.SetActive(true);
            Destroy(collision.gameObject);
        }
    }
}