using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // 플레이어가 장애물에 닿았을 때 실드 여부에 따라 방어 또는 페널티를 처리합니다.
    // 실드가 있으면 장애물을 끄고 끝내며, 실드가 없으면 GameManager에 시간 감소와 리스폰을 맡깁니다.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMove player = collision.GetComponent<PlayerMove>();

        if (player == null)
        {
            player = collision.GetComponentInParent<PlayerMove>();
        }

        if (player == null || GameManager.Instance == null)
        {
            return;
        }

        if (player.UseShield())
        {
            PlayShieldBlockSound();
            gameObject.SetActive(false);
            return;
        }

        PlayObstacleHitSound();
        GameManager.Instance.HitHazard(player);
    }

    // 장애물 충돌 결과에 맞는 효과음을 SoundManager에 요청합니다.
    // 실드 방어음과 일반 충돌음을 나눠서 플레이어가 상황을 소리로 구분할 수 있게 합니다.
    private void PlayShieldBlockSound()
    {
        SoundManager.Instance.PlayShieldBlock();
    }

    private void PlayObstacleHitSound()
    {
        SoundManager.Instance.PlayObstacleHit();
    }
}


