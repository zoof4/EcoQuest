using System.Collections;
using UnityEngine;

public class ObstacleBlock : MonoBehaviour
{
    [Header("Layer Settings")]
    public LayerMask obstacleLayer; // 다른 기물
    public LayerMask wallLayer;     // 벽

    [Header("Move Settings")]
    public float moveDuration = 0.15f; // 한 칸 이동 시간
    public Vector2 boxSize = new Vector2(0.8f, 0.8f); // 충돌 검사 박스 크기 (타일보다 살짝 작게)

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 이동 가능 여부 검사 (앞 칸이 비었는지 확인)
    /// </summary>
    public bool CanMove(Vector2 direction)
    {
        // 이동할 목표 위치 (한 칸 앞으로)
        Vector2 targetPos = (Vector2)transform.position + direction;

        // 목표 위치에 충돌체가 있는지 BoxCast로 검사
        RaycastHit2D hit = Physics2D.BoxCast(
            targetPos,         // 검사 중심점
            boxSize,           // 박스 크기
            0f,                // 회전 없음
            Vector2.zero,      // 방향 없음 (고정 검사)
            0f,                // 거리 없음
            obstacleLayer | wallLayer // 검사할 레이어
        );

        return hit.collider == null; // 아무것도 없으면 이동 가능
    }

    /// <summary>
    /// 실제로 한 칸 이동하는 코루틴 (부드럽게 이동)
    /// </summary>
    public IEnumerator MoveRoutine(Vector2 direction)
    {
        Vector2 startPosition = transform.position;
        Vector2 endPosition = startPosition + direction;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            rb.MovePosition(Vector2.Lerp(startPosition, endPosition, elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 부동소수점 오차 제거
        rb.MovePosition(endPosition);
    }
}
