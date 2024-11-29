using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA; // จุดเริ่มต้น
    public Transform pointB; // จุดปลายทาง
    public float speed = 2f; // ความเร็วในการเคลื่อนที่

    private Vector3 target; // จุดหมายปลายทางปัจจุบัน

    void Start()
    {
        // กำหนดจุดหมายปลายทางเริ่มต้นเป็นจุด A
        target = pointB.position;
    }

    void Update()
    {
        // เคลื่อนที่แพลตฟอร์มไปยังจุดหมายปลายทางด้วยความเร็วที่กำหนด
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // เมื่อแพลตฟอร์มไปถึงจุดหมายปลายทาง
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            // สลับจุดหมายปลายทาง
            if (target == pointA.position)
            {
                target = pointB.position;
            }
            else
            {
                target = pointA.position;
            }
        }
    }

    // ฟังก์ชันให้ Player ยืนบนแพลตฟอร์มและเคลื่อนที่ไปพร้อมแพลตฟอร์ม
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
