using System.Collections;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    // 이동 속도 (이름 규칙에 따라 소문자로 시작하도록 변경)
    public float moveSpeed = 3.0f;
    public LayerMask obstacleLayer;  // Obstacle 레이어
    public LayerMask wallLayer;  // Wall 레이어
     // 이동에 필요한 설정값
    private float moveDuration = 0.15f; // 한 칸 이동에 걸리는 시간
    private bool isMoving = false;  // 이동 중인지 확인하는 플래그

    // 필요한 컴포넌트들을 담을 변수
    Rigidbody2D rb;
    Animator anime;
    Vector2 movement;

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
    }


    // (PlayerController.cs의 Update 함수 부분)

    void Update()
    {
        // 맵 타입이 소코반일 때만 그리드 기반 이동을 처리
        if (MapManager.Instance != null && MapManager.Instance.currentMapType == MapType.Sokoban)
        {
            HandleSokobanMovement();
        }
        else // 소코반 맵이 아닐 경우
        {
            HandleNormalMovement();
        }
    }

    private void HandleSokobanMovement()
    {
        if (isMoving) return;

        Vector2 moveDirection = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.W))
            moveDirection = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S))
            moveDirection = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A))
            moveDirection = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D))
            moveDirection = Vector2.right;

        if (moveDirection != Vector2.zero)
        {
            StartCoroutine(MoveRoutine(moveDirection));
        }
    }

    private void HandleNormalMovement()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        anime.SetBool("isMoving", movement.sqrMagnitude > 0);

        if (Mathf.Abs(movement.x) > 0.01f)
        {
            anime.SetFloat("moveX", movement.x);
        }
    }
    
    private IEnumerator MoveRoutine(Vector2 direction)  // private IEnumerator  코루틴 선언
    {
        isMoving = true;    // 움직이는 중에

        Vector2 nextPosition = (Vector2)transform.position + direction;

        // 이동하려는 방향에 있는 오브젝트를 감지(플레이어보다 살짝 작음 박스 모양으로)
        Vector2 boxSize = new Vector2(0.8f, 0.8f);
        RaycastHit2D hit = Physics2D.BoxCast(
            nextPosition,       // 검사 위치 (앞 칸)
            boxSize,            // 검사 크기
            0f,                 // 회전 없음
            Vector2.zero,       // 방향 없음
            0f,                 // 거리 없음
            obstacleLayer | wallLayer
        );
        if (hit.collider != null)
        {
            // 감지된 오브젝트가 기물인지 확인
            ObstacleBlock obstacle = hit.collider.GetComponent<ObstacleBlock>();
            if (obstacle != null)
            {
                // 장애물 로직 처리
                // 기물이 이동 가능한지 검사하고, 가능하면 이동
                if (obstacle.CanMove(direction))
                {
                    StartCoroutine(obstacle.MoveRoutine(direction));
                    yield return StartCoroutine(SmoothMovement(nextPosition));
                }
            }
        }
        else
        {
                // 앞에 아무것도 없다면 플레이어만 부드럽게 이동
            yield return StartCoroutine(SmoothMovement(nextPosition));
        }
        isMoving = false;
    }


    private IEnumerator SmoothMovement(Vector2 endPosition)
    {
        float elapsedTime = 0f;
        Vector2 startPosition = transform.position;

        while (elapsedTime < moveDuration)
        {
            rb.MovePosition(Vector2.Lerp(startPosition, endPosition, (elapsedTime / moveDuration)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 부동 소수점 오차를 피하기 위해 최종 위치에 정확히 맞춤
        rb.MovePosition(endPosition);
    }
    
    void FixedUpdate()
    {
        if (MapManager.Instance != null && MapManager.Instance.currentMapType != MapType.Sokoban)
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
        