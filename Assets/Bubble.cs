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

    private void FixedUpdate()
    {
        if (currentFrame++ < moveForwardFrame)
        {
            var pos = rigidbody2D.position;
            pos.x += (speed * transform.forward.z);
            rigidbody2D.position = pos;
        }
        else
        {
            rigidbody2D.gravityScale = gravityScale;
            enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //버블이 터질만큼 붙어 있다면 터트리자.
    }
}