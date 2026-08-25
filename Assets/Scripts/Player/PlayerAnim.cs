using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    // Inspector에서 연결하는 플레이어 스프라이트와 애니메이션 이미지 배열입니다.
    // PlayerMove의 상태를 읽어서 idle, run, jump, double jump 배열 중 하나를 선택해 보여줍니다.
    [Header("References")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private PlayerMove _controller;

    [Header("Animation Sprites")]
    [SerializeField] private Sprite[] _idleSprites;
    [SerializeField] private Sprite[] _runSprites;
    [SerializeField] private Sprite[] _jumpSprites;
    [SerializeField] private Sprite[] _doubleJumpSprites;

    [Header("Animation Settings")]
    [SerializeField] private float _framesPerSecond = 8f;

    [Header("Shield Effect")]
    [SerializeField] private SpriteRenderer _shieldRenderer;
    [SerializeField] private Collider2D _shieldCollider;

    // 현재 재생 중인 스프라이트 배열과 프레임 진행 상태입니다.
    private Sprite[] _playingSprites;
    private int _spriteIndex;
    private float _timer;

    // 시작할 때 기본 대기 애니메이션을 먼저 보여줍니다.
    private void Start()
    {
        _playingSprites = _idleSprites;
        ShowSprite();
    }

    // 매 프레임 플레이어 상태에 맞는 애니메이션으로 바꿉니다.
    // 상태가 바뀌면 프레임 번호를 0으로 되돌려 새 애니메이션이 처음부터 재생되게 합니다.
    private void Update()
    {
        ShowEffectSprites();

        Sprite[] nextSprites = GetSpritesByState();

        if (_playingSprites != nextSprites)
        {
            _playingSprites = nextSprites;
            _spriteIndex = 0;
            _timer = 0f;
            ShowSprite();
        }

        _timer += Time.deltaTime;

        if (_timer < 1f / _framesPerSecond)
        {
            return;
        }

        _timer = 0f;
        _spriteIndex++;

        if (_spriteIndex >= _playingSprites.Length)
        {
            _spriteIndex = 0;
        }

        ShowSprite();
    }

    // 플레이어 상태에 따라 어떤 스프라이트 배열을 사용할지 결정합니다.
    // 우선순위는 더블 점프, 공중 점프, 달리기, 대기 순서입니다.
    private Sprite[] GetSpritesByState()
    {
        if (_controller == null)
        {
            return _idleSprites;
        }

        if (_controller.IsDoubleJumping)
        {
            return _doubleJumpSprites;
        }

        if (!(_controller.IsGroundedNow || _controller.IsGrounded()))
        {
            return _jumpSprites;
        }

        if (_controller.IsRunning)
        {
            return _runSprites;
        }

        return _idleSprites;
    }

    // 현재 프레임 번호에 맞는 이미지를 SpriteRenderer에 넣습니다.
    private void ShowSprite()
    {
        if (_spriteRenderer == null || _playingSprites == null)
        {
            return;
        }

        if (_playingSprites.Length == 0 || _playingSprites[_spriteIndex] == null)
        {
            return;
        }

        _spriteRenderer.sprite = _playingSprites[_spriteIndex];
    }

    // 플레이어 상태와 연결된 추가 표시 효과를 갱신합니다.
    private void ShowEffectSprites()
    {
        if (_controller == null)
        {
            return;
        }

        ShowShield();
    }

    // 실드 보유 상태에 따라 친구 실드 이미지와 콜라이더를 켜거나 끕니다.
    // PlayerMove.HasShield 값이 true일 때만 화면에 보이고 충돌 방어용 콜라이더도 켜집니다.
    private void ShowShield()
    {
        if (_shieldRenderer == null)
        {
            return;
        }

        _shieldRenderer.enabled = _controller.HasShield;

        if (_shieldCollider != null)
        {
            _shieldCollider.enabled = _controller.HasShield;
        }
    }
}


