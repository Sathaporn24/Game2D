using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int score = 0;
    public float speed = 1f;
    public float jumpSpeed = 9f;
    public float maxSpeed = 10f;
    public float JumpPower = 20f;
    public bool grounded;
    public float jumpRate = 1f;
    public float nextJumpPress = 0.0f;
    public float fireRate = 0.2f;
    public float nextFireRate = 0.0f;
    private int comboStep = 0; // ขั้นตอนในคอมโบ
    public float comboResetTime = 0.05f; // เวลาสำหรับรีเซ็ตคอมโบ
    private float lastComboTime = 0.0f; // เวลาโจมตีครั้งล่าสุด

    private Rigidbody2D ridigBody2D;
    private Physics2D physic2D;
    Animator animator;
    public int healthbar = 100;
    public Text healthText;
    public GameObject hitArea;
    public Slider sliderHp;

    private float originalSpeed;
    private float originalJumpPower;
    public bool isInDamageZone = false;

    // สำหรับจัดการการเกิดใหม่ของไอเทม
    private Dictionary<Vector3, bool> healthItemRespawned = new Dictionary<Vector3, bool>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ridigBody2D = this.gameObject.GetComponent<Rigidbody2D>();
        animator = this.gameObject.GetComponent<Animator>();
        sliderHp.maxValue = healthbar;
        sliderHp.value = healthbar;

        originalSpeed = speed;
        originalJumpPower = JumpPower;

        // เก็บตำแหน่งเริ่มต้นของไอเทม health ทั้งหมดในเกม
        GameObject[] healthItems = GameObject.FindGameObjectsWithTag("health");
        foreach (GameObject healthItem in healthItems)
        {
            healthItemRespawned.Add(healthItem.transform.position, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = "HEALTH: " + healthbar + "  SCORE: " + score;

        if (healthbar <= 0)
        {
            healthbar = 0;
            animator.SetTrigger("Death");
        }

        sliderHp.value = healthbar;

        animator.SetBool("Grounded", true);
        animator.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));

        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput < -0.1f)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            transform.eulerAngles = new Vector2(0, 180);
        }
        else if (horizontalInput > 0.1f)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            transform.eulerAngles = new Vector2(0, 0);
        }
        else
        {
            // หยุดการเคลื่อนไหวเมื่อไม่มีการกดปุ่ม
            transform.Translate(Vector2.zero);
        }

        if (Input.GetButtonDown("Jump") && Time.time > nextJumpPress)
        {
            animator.SetBool("Jump", true);
            nextJumpPress = Time.time + jumpRate;
            ridigBody2D.AddForce(jumpSpeed * (Vector2.up * JumpPower));
        }
        else
        {
            animator.SetBool("Jump", false);
        }

        if (!isInDamageZone && Input.GetKeyDown(KeyCode.X) && Time.time > nextFireRate)
        {
            nextFireRate = Time.time + fireRate;

            if (Time.time - lastComboTime > comboResetTime)
            {
                // รีเซ็ตคอมโบหากเกินเวลาที่กำหนด
                comboStep = 0;
            }

            comboStep++; // เพิ่มขั้นของคอมโบ
            lastComboTime = Time.time; // บันทึกเวลาที่โจมตีล่าสุด

            if (comboStep > 3) // กำหนดจำนวนคอมโบสูงสุด (ปรับได้)
            {
                comboStep = 0; // รีเซ็ตไปเริ่มต้นที่คอมโบแรก
            }

            animator.SetInteger("ComboStep", comboStep); // ส่งค่าขั้นตอนคอมโบไปยัง Animator
            animator.SetTrigger("Attack"); // เล่นแอนิเมชันโจมตี
        }
    }

    void TakeDamage(int damage)
    {
        healthbar = healthbar - damage;
    }

    public void Attack()
    {
        StartCoroutine(DelaySlash());
    }

    IEnumerator DelaySlash()
    {
        yield return new WaitForSeconds(0.3f);
        Instantiate(hitArea, transform.position, transform.rotation);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "health")
        {
            healthbar += 13;
            if (healthbar > 100) healthbar = 100;
            score += 7;
            
            Vector3 healthPosition = other.transform.position;
            if (healthItemRespawned.ContainsKey(healthPosition) && healthItemRespawned[healthPosition])
            {
                healthItemRespawned[healthPosition] = false;
                StartCoroutine(RespawnHealth(healthPosition, 5f));
                Destroy(other.gameObject);
            }
        }
        if (other.gameObject.tag == "deathzone")
        {
            healthbar = 0;
        }
        if (other.gameObject.tag == "enemy")
        {
            TakeDamage(10);
        }
        if (other.gameObject.tag == "damagezone")
        {
            isInDamageZone = true;
            speed = originalSpeed / 2; // ลดความเร็วลงครึ่งหนึ่ง
            JumpPower = originalJumpPower / 2; // ลดความสูงในการกระโดดลงครึ่งหนึ่ง
            StartCoroutine(DamageOverTime());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "damagezone")
        {
            isInDamageZone = false;
            speed = originalSpeed; // คืนค่าความเร็วเดิม
            JumpPower = originalJumpPower; // คืนค่าความสูงในการกระโดดเดิม
            StopCoroutine(DamageOverTime());
        }
    }

    IEnumerator DamageOverTime()
    {
        while (isInDamageZone)
        {
            TakeDamage(5);
            yield return new WaitForSeconds(1f); // โดนความเสียหายทุก ๆ 1 วินาที
        }
    }

    IEnumerator RespawnHealth(Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject newHealthItem = Instantiate(Resources.Load("HealthPrefab") as GameObject, position, Quaternion.identity);
        healthItemRespawned[position] = true;
    }
}
