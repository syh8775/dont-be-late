using UnityEngine;

public class Item : MonoBehaviour
{
    // Inspector에서 아이템 종류와 시계 아이템의 추가 시간을 설정합니다.
    [SerializeField] private ItemType _itemType;
    [SerializeField] private float _bonusTime = 5f;

    // 하나의 Item 스크립트로 시계, 음료, 실드 아이템을 구분합니다.
    private enum ItemType
    {
        Clock,
        Drink,
        Shield
    }

    // 플레이어가 아이템에 닿았을 때 아이템 종류에 맞는 효과를 적용합니다.
    // Clock은 시간 추가, Drink는 속도 증가, Shield는 방어 상태를 켜고 마지막에 아이템을 숨깁니다.
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

        if (_itemType == ItemType.Clock && GameManager.Instance != null)
        {
            GameManager.Instance.AddTime(_bonusTime);
            PlayItemClockSound();
        }

        if (_itemType == ItemType.Drink)
        {
            player.ActivateSpeedBoost();
            PlayItemDrinkSound();
        }

        if (_itemType == ItemType.Shield)
        {
            player.ActivateShield();
            PlayItemShieldSound();
        }

        gameObject.SetActive(false);
    }

    // 아이템 종류별 획득 사운드를 SoundManager에 요청합니다.
    // 사운드 재생 코드를 따로 함수로 둬서 아이템 효과 처리와 소리 처리를 구분합니다.
    private void PlayItemClockSound()
    {
        SoundManager.Instance.PlayItemClock();
    }

    private void PlayItemDrinkSound()
    {
        SoundManager.Instance.PlayItemDrink();
    }

    private void PlayItemShieldSound()
    {
        SoundManager.Instance.PlayItemShield();
    }
}
