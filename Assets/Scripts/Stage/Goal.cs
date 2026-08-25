using UnityEngine;

public class Goal : MonoBehaviour
{
    // 플레이어가 목표 지점에 닿으면 골 진입 연출을 PlayerMove에 맡깁니다.
    // Goal은 직접 클리어 처리까지 하지 않고, PlayerMove가 목표 지점으로 들어간 뒤 GameManager.ClearStage를 호출합니다.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMove player = collision.GetComponent<PlayerMove>();

        if (player == null)
        {
            player = collision.GetComponentInParent<PlayerMove>();
        }

        if (player == null)
        {
            return;
        }

        player.EnterGoal(transform);
    }
}


