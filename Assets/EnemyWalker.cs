using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalker : MonoBehaviour
{
    public float walkSpeed = 0.25f;
    public float walkDistance = 0.3f; // ระยะทางที่ Enemy เดินไปซ้ายหรือขวาจากจุดเริ่มต้น
    public float walkDirection = 1.0f;
    public GameObject Explode;
    Vector3 walkAmount;
    private Vector3 startPoint;

    void Start()
    {
        startPoint = transform.position; // เก็บตำแหน่งเริ่มต้นของ Enemy
    }

    void Update()
    {
        walkAmount.x = (walkDirection * walkSpeed) * Time.deltaTime;
        if (walkDirection > 0.0f && transform.position.x >= startPoint.x + walkDistance)
        {
            walkDirection = -1.0f; // เปลี่ยนทิศทางไปทางซ้าย
            Flip(); // ฟลิปตัวละคร
        }
        else if (walkDirection < 0.0f && transform.position.x <= startPoint.x - walkDistance)
        {
            walkDirection = 1.0f; // เปลี่ยนทิศทางไปทางขวา
            Flip(); // ฟลิปตัวละคร
        }
        transform.Translate(walkAmount);
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "weaponA")
        {
            Destroy(other.gameObject);
            StartCoroutine(secondDeath(0.2f));
        }
    }

    IEnumerator secondDeath(float sec)
    {
        yield return new WaitForSeconds(sec);
        Instantiate(Explode, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}