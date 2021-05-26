using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public int moveForwardFrame = 6;
    public int currentFrame = 0;
    public float speed = 0.7f;
    new public Rigidbody2D rigidbody2D;
    public float gravityScale = -0.7f;
    //앞쪽 방향으로 이동. 6프레임 움직이고 나서 위로 이동(중력에 의해)

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0;
        //for (int i = 0; i < moveForwardFrame; i++)
        //{
        //    //transform.Translate(speed, 0, 0);
        //    yield return null;
        //}
        //rigidbody2D.gravityScale = gravityScale;
    }

    public LayerMask wallLayer;

    private void FixedUpdate()
    {
        if (currentFrame++ < moveForwardFrame)
        {
            var pos = rigidbody2D.position;
            pos.x += (speed * transform.forward.z);

            //버블이 앞으로 가고 있으면 최대 x값을 레이 쏘아서 찾기
            //뒤로가고 있으면 최소 x값을 레이 쏘아서 찾기
            if (transform.forward.z > 0) // 앞으로 가고 있다.
            {
                //최대 x 값 찾자.
                //wallLayer = LayerMask.NameToLayer("Wall");
                var hit = Physics2D.Raycast(transform.position, new Vector2(1, 0), 100f, wallLayer);
                Debug.Assert(hit.transform != null, "벽 레이어 없음. 확인");
                if (hit.transform)
                {
                    float maxX = hit.point.x;
                    pos.x = Mathf.Min(pos.x, maxX);
                }
            }
            else
            {
                var hit = Physics2D.Raycast(transform.position, new Vector2(-1, 0), 100f, wallLayer);
                float minX = hit.point.x;
                pos.x = Mathf.Max(pos.x, minX);
            }

            rigidbody2D.position = pos;
        }
        else
        {
            state = State.FreeFly;
            rigidbody2D.gravityScale = gravityScale;
            enabled = false;
        }
    }

    public enum State
    {
        FastMove,
        FreeFly,
        //Explosion
    }

    public State state = State.FastMove;

    private void OnTouchCollision(Transform tr)
    {
        if (state == State.FreeFly)
        {
            if (tr.CompareTag("Player"))
            {
                //플레이어
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //버블끼리, 벽에 닿았을 때

        Debug.Log("Collision:" + collision.transform.name);

        //버블이 플레이어에 닿았을 때
        OnTouchCollision(collision.transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //플레이어가 점프할 때
        //트리거 벽
        Debug.Log("Trigger:" + collision.transform.name);
        OnTouchCollision(collision.transform);
    }
}