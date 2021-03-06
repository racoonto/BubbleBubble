using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public static List<Bubble> Items = new List<Bubble>();
    public int moveForwardFrame = 6;
    public int currentFrame = 0;
    public float speed = 0.7f;
    new public Rigidbody2D rigidbody2D;
    new public CircleCollider2D collider2D;
    public float gravityScale = -0.7f;

    public enum State
    {
        FastMove,
        FreeFly,
        Capture,
        //Explosion
    }

    //앞쪽 방향으로 이동. 6프레임 움직이고 나서 위로 이동(중력에 의해)

    private void Awake()
    {
        name = "Bubble" + (Items.Count + 1);
        Items.Add(this);
    }

    private void OnDestroy()
    {
        Items.Remove(this);
    }

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<CircleCollider2D>();
        rigidbody2D.gravityScale = 0;
        //for (int i = 0; i < moveForwardFrame; i++)
        //{
        //    //transform.Translate(speed, 0, 0);
        //    yield return null;
        //}
        //rigidbody2D.gravityScale = gravityScale;
    }

    public LayerMask wallLayer;
    public LayerMask playerLayer;
    // 벽과 이미 충돌한상태로 생성되면 충돌되지 않음 <- 이 상황에선 버블이 터져야함.

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
            //플레이어와 닿아 있다면 거품을 터뜨리자
            var allCollides = Physics2D.OverlapCircleAll(transform.position, collider2D.radius, playerLayer);
            if (allCollides.Length > 0)
            {
                Explosion();
            }
            else
            {
                //공룡이 인근에 있으면 자신 버블을 터뜨리자.
                float distance = Vector3.Distance(Player.instance.transform.position, transform.position); //플레이어와 버블의 거리
                if (distance < nearPlayerCheckDistance)
                {
                    Explosion();
                }

                collider2D.isTrigger = false;//충돌 켜줌
                state = State.FreeFly;
                rigidbody2D.gravityScale = gravityScale;
                enabled = false;
            }
        }
    }

    public float nearPlayerCheckDistance = 1.9f;

    public State state = State.FastMove;
    public float JumpHeight = 0.5f; //플레이어가 버블 밟은걸로 인정되는 높이.

    private void OnTouchCollision(Transform tr)
    {
        if (state == State.FreeFly)
        {
            if (tr.CompareTag("Player"))
            {
                //플레이어가 나보다 높게 있다면
                //플레이어가 방향키를 위로 하고 있다면
                bool pressedUpkey = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
                if (pressedUpkey && (transform.position.y + JumpHeight < tr.position.y))
                {
                    //버블을 터트리지 말고 놔주다.
                    //작아졌다가 커지는 트윈효과 주자.
                    tr.GetComponent<Player>().StartJump();
                }
                else
                {
                    //플레이어면 버블 폭발
                    Explosion();
                }
            }
        }
        else if (state == State.FastMove)
        {
            if (tr.CompareTag("Enemy"))
            {
                // 적을 안보이게 하자.
                tr.gameObject.SetActive(false);
                // 버블 트랜스폼에 자식으로 달자.
                tr.parent = transform; // = tr.SetParent(transform);

                //버블의 상태를 캡쳐 상태로 변경.
                state = State.Capture;
                string monsterName = tr.GetComponent<Monster>().monsterName;
                //Debug.Log(monsterName);
                //버블 이미지를 해당 몬스터 잡은 애니메이션을 플레이하자
                grabedMonster = tr.gameObject; // 잡은 몬스터
                StartCoroutine(BubbleExplosionco(monsterName));
            }
        }
    }

    public GameObject grabedMonster;

    //버블이 몇초 후에 터짐
    //파랑 녹색 -> 빨간색
    public List<float> bubbleExplosionTime;

    private IEnumerator BubbleExplosionco(string monsterName)
    {
        //prefix + 1
        //prefix + 2
        for (int i = 0; i < bubbleExplosionTime.Count; i++)
        {
            GetComponent<Animator>().Play(monsterName + (i + 1));
            yield return new WaitForSeconds(bubbleExplosionTime[i]);
        }
        //터지는 과정 진행
        grabedMonster.transform.parent = null; //부모를 없앤다.
        grabedMonster.SetActive(true);

        Destroy(gameObject);
    }

    public float nearBubbleDistance = 2.2f;

    private void Explosion()
    {
        //인근의 버블을 모두 터뜨리자.
        //1.모든 버블에 접근
        //2.인근의 있는 버블을 모으자.

        //2.모은 버블을 터트리자.
        Vector2 pos = transform.position;
        List<Bubble> nearBubbles = new List<Bubble>();
        nearBubbles.Add(this); //자기자신 추가
        FindNearBubble(pos, nearBubbles);

        nearBubbles.ForEach(x => Destroy(x.gameObject));
        Destroy(gameObject);
    }

    private void FindNearBubble(Vector2 pos, List<Bubble> nearBubbles)
    {
        foreach (var item in Items)
        {
            if (nearBubbles.Contains(item))
                continue; //다음 아이템 선택

            //pos 가까이(2.2)에 있는 버블을 모으자
            float distance = Vector2.Distance(item.transform.position, pos);
            if (distance < nearBubbleDistance)
            {
                nearBubbles.Add(item); //거리가 가까우면 nearBubble에 넣기
                FindNearBubble(item.transform.position, nearBubbles); // 자기 자신을 호출하는 재귀함수
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //버블끼리, 벽에 닿았을 때

        //Debug.Log("Collision:" + collision.transform.name);

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