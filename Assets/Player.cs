using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 0.1f;
    public Animator animator;
    new public CircleCollider2D collider2D;
    new public Rigidbody2D rigidbody2D;
    public float jumpForce = 100f;
    public float wallOffset = 0.02f;

    public enum StateType
    {
        Ground,
        Jump,
        DownJump,
        DownJumpExitCollider,
    }

    // public이 아니지만 인스펙터에 노출
    [SerializeField] private StateType state = StateType.Ground;

    private StateType State
    {
        get { return state; }
        set
        {
            Log.Print($"{state} -> {value}", OptionType.Player상태변화로그);
            state = value;
        }
    }

    //bool ingDownJump = false;
    //bool ingJump = false;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider2D = GetComponent<CircleCollider2D>();
        Application.targetFrameRate = 60;

        //minX, maxX값을  인스펙터에서 넣으니깐 제대로 넣지 않을 경우 좌우 벽에 붙어 있는 문제 발생 -> trigger Stage상태 유지.
        // -> 프로그래밍적으로 할당하자.
        // 좌우로 레이를 쏘아서 마지막 벽을 min, max로 할당.

        RaycastHit2D rightmostHit = new RaycastHit2D();
        Vector2 checkPosition = transform.position;

        //오른쪽 벽 구하기
        checkPosition.x += 100f; //오른쪽 벽을 많이 넘어서 왼쪽으로 측정.

        int count = 0;
        rightmostHit = Physics2D.Raycast(checkPosition, Vector2.left, 100f, wallLayer); //가장 오른쪽에 있는 벽.
        //16
        while (count++ < 10)
        {
            checkPosition.x = rightmostHit.point.x - 1.01f; //오른쪽 끝벽에서 안쪽으로 들어온 1.01f 값을 뺌. 안쪽벽의 위치를 구하기 위해서.벽위 두께 고려
            var hit = Physics2D.Raycast(checkPosition, Vector2.left, 100f, wallLayer);

            Debug.Log(rightmostHit.point.x); //15, 14
            if (hit.transform == null) //hit의 위치값이 없으면 break
            {
                break;
            }
            rightmostHit = hit; //오른쪽 끝 벽은 hit이다.

            //12.99
            //rightmostHit 13.99
        }
        Debug.Log("최종 벽 위치" + rightmostHit.point.x);
        float wallWidth = 1f;
        maxX = rightmostHit.point.x - wallWidth - collider2D.radius; //maxX는 오른쪽 끝벽 - 벽 두께 - 캐릭터 반지름

        //왼쪽 벽 구하기
        checkPosition = transform.position;
        checkPosition.x -= 100;
        var leftmostHit = Physics2D.Raycast(checkPosition, Vector2.right, 100f, wallLayer); //가장 오른쪽에 있는 벽.
        // -16
        while (count++ < 10)
        {
            checkPosition.x = leftmostHit.point.x + 1.01f; //오른쪽 끝벽에서 안쪽으로 들어온 1.01f 값을 뺌. 안쪽벽의 위치를 구하기 위해서.벽위 두께 고려
            var hit = Physics2D.Raycast(checkPosition, Vector2.right, 100f, wallLayer);

            Debug.Log(leftmostHit.point.x); // -15, -14
            if (hit.transform == null) //hit의 위치값이 없으면 break
            {
                break;
            }
            leftmostHit = hit; //오른쪽 끝 벽은 hit이다.

            //12.99
            //rightmostHit 13.99
        }

        //leftmostHit 오른쪽으로 1 이동한 다음에 왼쪽으로 레이를 다시 쏜다. 벽의 두께가 1로 딱 떨어지지 않아서.
        checkPosition.x += 1;

        var hit1 = Physics2D.Raycast(checkPosition, Vector2.left, 2f, wallLayer);
        leftmostHit = hit1;
        Debug.Log("최종 왼쪽 벽 위치" + leftmostHit.point.x);
        minX = leftmostHit.point.x + collider2D.radius + wallOffset; //maxX는 오른쪽 끝벽 + 캐릭터 반지름 + 약간 띄어줌

        //RaycastHit2D Leftmost = new RaycastHit2D();
        //RaycastHit2D hit;

        //int count = 0;
        //List<RaycastHit2D> hits = new List<RaycastHit2D>();
        //float wallWidth = 1.01f; // 벽의 두께. 다음 벽을 체크
        //while (count++ < 10) // 최대 10회만 검사.
        //{
        //    //Raycast의 형식에 주의
        //    hit = Physics2D.Raycast(checkPosition, Vector2.right, 100f, wallLayer); //반환된 값이 들어온다.  Raycase(자기위치,오른쪽방향 , 100길이만큼, 벽레이어만)
        //    if (hit.transform == null) //hit 된게 없으면 반복 종료
        //        break;
        //    hits.Add(hit); //hit를 모아놓음
        //    rightmostHit = hit; // 마지막 벽이 2중 벽일때 바로 앞에 벽을 마지막 오른쪽 벽으로 하자.
        //    checkPosition.x = hit.point.x + wallWidth; //
        //};

        //if (hits.Count >= 2)
        //{
        //    var previousHit = hits[hits.Count - 2];
        //    if (rightmostHit.point.x < previousHit.point.x + wallWidth + 0.01f)
        //    {
        //        rightmostHit = previousHit;
        //    }
        //}

        //count = 0;
        //hits.Clear();
        //checkPosition = transform.position;
        //while (count++ < 10) // 최대 10회만 검사.
        //{
        //    hit = Physics2D.Raycast(checkPosition, Vector2.left, 100f, wallLayer);
        //    if (hit.transform == null)
        //        break;
        //    hits.Add(hit);
        //    Leftmost = hit;
        //    checkPosition.x = hit.point.x - 1.01f; // 1.01은 벽 두께
        //};

        //if (hits.Count >= 2)
        //{
        //    var previousHit = hits[hits.Count - 2];
        //    if (Leftmost.point.x > previousHit.point.x - wallWidth - 0.01f)
        //    {
        //        Leftmost = previousHit;

        //        if (hits.Count >= 3) // 왼쪽은 3중벽이어서 추가 확인
        //        {
        //            previousHit = hits[hits.Count - 3];
        //            if (Leftmost.point.x > previousHit.point.x - wallWidth - 0.01f)
        //            {
        //                Leftmost = previousHit;
        //            }
        //        }
        //    }
        //}

        //float halfSize = collider2D.bounds.size.x * 0.5f + wallOffset;
        //maxX = rightmostHit.point.x - halfSize;
        //minX = Leftmost.point.x + halfSize;
    }

    private void Update()
    {
        FireBubble();

        // WASD, A왼쪽,, D오른쪽
        Move();

        // 점프
        Jump();

        // 아래로점프
        DownJump();
    }

    public LayerMask wallLayer;
    public LayerMask groundLayer;
    public float downWallCheckY = -2.1f;

    private void DownJump()
    {
        // s키 누르면 아래로 점프
        if (State == StateType.Ground)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (IsGround() == false)
                    return;

                // 점프 가능한 상황인지 확인
                //  아래로 광선을 쏘아서 벽이 있다면 아래로 점프를 하자
                var hit = Physics2D.Raycast(
                    transform.position + new Vector3(0, downWallCheckY, 0)
                    , new Vector2(0, -1), 100, groundLayer);
                if (hit.transform)
                {
                    State = StateType.DownJump;
                    collider2D.isTrigger = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Log.Print("T Enter " + collision.transform.name, OptionType.ShowCollideLog);

        if (State == StateType.DownJumpExitCollider)
        {
            // 밑에 벽이 있다면 그라운드.(옆에 벽에 부딪힌것일 수도 있다. -> 옆에 벽에 부딪힌후 밑에 벽에 부 딪힌다면 총돌 신호가 1번만 온다)
            // 벽과 바닥을 다른 타일로 만들어서 구분 해야한다.
            if (IsGround())
                State = StateType.Ground;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Log.Print("T Exit  " + collision.transform.name, OptionType.ShowCollideLog);
        if (State == StateType.DownJump)
        {
            State = StateType.DownJumpExitCollider; // Ground혹은 밑으로 점프중.
            collider2D.isTrigger = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Log.Print("C Enter " + collision.transform.name, OptionType.ShowCollideLog);
        if (State == StateType.DownJumpExitCollider) // 아래 벽과 (옆+천장)벽을 분리 해야함
        {
            if (IsGround())
                State = StateType.Ground;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Log.Print("C Exit  " + collision.transform.name, OptionType.ShowCollideLog);
    }

    private void Jump()
    {
        // 낙하할때는 지면과 충돌하도록 isTrigger를 꺼주자.
        if (State == StateType.Jump)
        {
            if (rigidbody2D.velocity.y < 0)
            {
                if (IsGround())
                {
                    State = StateType.Ground;
                    collider2D.isTrigger = false; // 점프하고 나서 뚫은 벽에 서고싶다.
                }
            }
        }
        // 방향위혹은 W키 누르면 점프 하자.
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            // 우리가 바닥에 붙어 있으면 점프 할 수 있게 하자.
            //bool isGround = IsGround();
            if (State == StateType.Ground)
            {
                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.AddForce(new Vector2(0, jumpForce));
                collider2D.isTrigger = true; // 점프할때 벽을 뚫고 싶다.
                State = StateType.Jump;
            }
        }
    }

    public float groundCheckOffsetX = 0.4f;

    private bool IsGround()
    {
        //return rigidbody2D.velocity.y == 0;

        // 밑으로 광선을 쏘아서 부딪히는게 있으면 우리는 바닥에 있다.
        // 자기위치, 그리고 좌우로 0.4f만큼 바닥이 있으면 나는 바닥에 있다.

        if (IsGroundCheckRay(transform.position))
            return true;

        // 좌.
        if (IsGroundCheckRay(transform.position + new Vector3(-groundCheckOffsetX, 0, 0)))
            return true;

        // 우.
        if (IsGroundCheckRay(transform.position + new Vector3(groundCheckOffsetX, 0, 0)))
            return true;

        return false;
    }

    private void OnDrawGizmos()
    {
        DrawRay(transform.position);

        // 좌.
        DrawRay(transform.position + new Vector3(-groundCheckOffsetX, 0, 0));

        // 우.
        DrawRay(transform.position + new Vector3(groundCheckOffsetX, 0, 0));
    }

    private void DrawRay(Vector3 position)
    {
        Gizmos.DrawRay(position + new Vector3(0, downWallCheckY, 0), new Vector2(0, -1) * 1.1f);
    }

    private bool IsGroundCheckRay(Vector3 pos)
    {
        var hit = Physics2D.Raycast(pos, new Vector2(0, -1), 1.1f, groundLayer);
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

    public float minX = -12.3f, maxX = 12.3f;

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