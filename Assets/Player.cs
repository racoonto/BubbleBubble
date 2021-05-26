using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 0.1f;
    public Animator animator;
    new public Rigidbody2D rigidbody2D;
    new public Collider2D collider2D;
    public float jumpForce = 1000f;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider2D = GetComponent<Collider2D>();

        //옛날 게임이라 Time.deltatime은 안쓰고 프레임레이트를 정해주었음.
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        FireBubble();

        // WASD, A왼쪽,, D오른쪽
        Move();

        // 점프
        Jump();

        // 아래로 점프
        DownJump();
    }

    public LayerMask wallLayer;
    public float downWallCheckY = -2.1f; //바닥까지의 거리

    private void DownJump()
    {
        //s키 누르면 아래로 점프
        if (Input.GetKeyDown(KeyCode.S))
        {
            //점프 가능한 상황인지 확인
            // 아래로 광선을 쏴서 벽이 있다면 아래로 점프
            var hit = Physics2D.Raycast(transform.position + new Vector3(0, downWallCheckY, 0), new Vector2(0, -1), 100, wallLayer); // 아래로 광선 쏘기
            if (hit.transform) //위치값을 반환한다면
            {
                ingDownJump = true;
                collider2D.isTrigger = true;
                //Debug.Log($"{hit.point}, {hit.transform.name}");

                //StartCoroutine(DownJumco());
            }
        }
    }

    private bool ingDownJump = false;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (ingDownJump)
        {
            ingDownJump = false;
            collider2D.isTrigger = false;
        }
    }

    //시간으로 처리 안하고 트리거로 아래로 내려가기.

    //public float downJumpTime = 0.4f;

    //private IEnumerator DownJumco()
    //{
    //    ingDownJump = true;
    //    collider2D.isTrigger = true;
    //    yield return new WaitForSeconds(downJumpTime);
    //    collider2D.isTrigger = false;
    //    ingDownJump = false;
    //}

    private void Jump()
    {
        if (ingDownJump == false)
        {
            //낙하할 때는 지면과 충돌하도록 isTrigger을 꺼주자.
            if (rigidbody2D.velocity.y < 0)
                collider2D.isTrigger = false;
        }

        //if (rigidbody2D.velocity.y == 0) //공중에서 점프를 막고 싶다.
        //{
        // 방향키 위 혹은 W키 누르면 점프 하자.
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            //바닥에 붙어 있으면 점프 할 수 있게
            //광선으로
            bool isGround = IsGround();
            if (isGround)
            {
                rigidbody2D.AddForce(new Vector2(0, jumpForce));
                collider2D.isTrigger = true; //점프할 때 벽을 뚫고 싶다.
            }
        }
        //}
    }

    public float groundCheckOffsetX = 0.4f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, new Vector2(0, -1) * 1.1f);
    }

    private bool IsGround()
    {
        //자기 위치, 좌우로 0.4f만큼 바닥이 있으면 바닥에 있음

        bool result = false;

        //밑으로 광선을 쏘아서 부딪히는게 있으면 바닥임
        if (IsGroundCheckRay(transform.position))
            return true;

        //좌
        if (IsGroundCheckRay(transform.position + new Vector3(-groundCheckOffsetX, 0, 0)))
            return true;

        //우
        if (IsGroundCheckRay(transform.position + new Vector3(groundCheckOffsetX, 0, 0)))
            return true;

        return result;
    }

    private bool IsGroundCheckRay(Vector3 pos)
    {
        var hit = Physics2D.Raycast(pos, new Vector2(0, -1), 1.1f, wallLayer);
        if (hit.transform)
            return true;

        return false;
    }

    public GameObject bubble;
    public Transform bubbleSpawnPos;

    private void FireBubble()
    {
        // 스페이스 누르면 버블 날리기.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(bubble, bubbleSpawnPos.position, transform.rotation);
        }
    }

    public float minX, maxX;

    private void Move()
    {
        float moveX = 0;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveX = -1;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveX = 1;
        Vector3 position = transform.position;
        position.x = position.x + moveX * speed;
        position.x = Mathf.Max(minX, position.x);
        position.x = Mathf.Min(maxX, position.x);
        transform.position = position;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("attack") == false)
        {
            if (moveX != 0)
            {
                //moveX 양수이면 180 로테이션 아니면 0도 로테이션 적용.
                float rotateY = 0;
                if (moveX < 0)
                    rotateY = 180;

                var rotation = transform.rotation;
                rotation.y = rotateY;
                transform.rotation = rotation;

                animator.Play("run");
            }
            else
                animator.Play("idle");
        }
    }
}