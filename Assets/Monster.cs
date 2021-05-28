using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Monster : MonoBehaviour
{
    public static List<Monster> monsters = new List<Monster>();

    // 앞으로 움직이자.
    //절벽 만나면 방향 전환

    private new Rigidbody2D rigidbody2D;
    public string monsterName = "MonsterA";

    private void Awake()
    {
        monsters.Add(this);
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        monsters.Remove(this);
        if (monsters.Count == 0)
        {
            SceneManager.LoadScene("Stage2");
        }
    }

    public float speed = 0.1f;

    private void FixedUpdate()
    {
        var pos = rigidbody2D.position;
        pos.x += speed * transform.forward.z;
        rigidbody2D.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.name);
        if (collision.transform.CompareTag("TurnTrigger"))
        {
            var rotation = transform.rotation;
            if (rotation.y == 0)
                rotation.y = 180;
            else
                rotation.y = 0;

            transform.rotation = rotation;
        }
    }
}