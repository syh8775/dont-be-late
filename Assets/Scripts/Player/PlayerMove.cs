using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMove : MonoBehaviour
{
    // Inspector에서 조절하는 이동, 점프, 아이템 효과, 시작 위치 설정값입니다.
    // 코드 수정 없이 Inspector 값만 바꿔도 속도, 점프력, 시작 위치, 낙하 리스폰 기준을 조절할 수 있습니다.
    [Header("Movement")]
    [SerializeField] private float _runSpeed = 4.5f;
    [SerializeField] private float _enterGoalSpeed = 3f;

    [Header("Jump")]
    [SerializeField] private float _jumpPower = 13f;
    [SerializeField] private int _maxJumpCount = 2;

    [Header("Item Effects")]
    [SerializeField] private float _speedBoostMultiplier = 1.25f;
    [SerializeField] private float _speedBoostSeconds = 4f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask _groundLayerMask = ~0;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.55f, 0.12f);
    [SerializeField] private float _groundCheckDistance = 0.68f;

    [Header("Stage Position")]
    [SerializeField] private Vector2 _startPosition = new Vector2(-6.5f, -2.4f);
    [SerializeField] private float _fallLimitY = -7f;

    // 플레이 중 계속 바뀌는 물리, 점프, 아이템, 골 진입 상태입니다.
    // _jumpCount는 더블 점프 제한, _speedBoostTimeLeft는 음료 지속 시간, _isEnteringGoal은 골 연출 중인지 판단합니다.
    private Rigidbody2D _rigidbody2D;
    private int _jumpCount;
    private float _speedBoostTimeLeft;
    private Transform _goalpoint;
    private bool _isEnteringGoal;

    // 다른 스크립트가 플레이어 상태를 읽을 수 있게 공개하는 값들입니다.
    public bool IsGroundedNow { get; private set; }
    public bool IsDoubleJumping { get; private set; }
    public bool IsSpeedBoosting { get; private set; }
    public bool HasShield { get; private set; }
    public bool IsRunning { get; private set; }

    // 시작할 때 Rigidbody2D를 찾아서 이동 처리에 사용합니다.
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // 매 프레임 게임 상태를 확인하고 이동, 점프, 낙하, 골 진입을 처리합니다.
    // 골 진입 중에는 일반 이동/점프를 막고 MoveToGoal만 실행해서 도착 연출이 끊기지 않게 합니다.
    private void Update()
    {
        CheckGameState();

        if (_isEnteringGoal)
        {
            MoveToGoal();
            return;
        }
        CheckGround();
        CheckBoost();

        if (!IsRunning)
        {
            _rigidbody2D.linearVelocity = new Vector2(0f, _rigidbody2D.linearVelocity.y);
            return;
        }

        MoveRight();
        CheckJump();
        CheckFall();
    }

    // 음료 아이템을 먹었을 때 일정 시간 속도 증가 상태로 만듭니다.
    // 실제 속도 증가는 MoveRight에서 IsSpeedBoosting 값을 보고 적용합니다.
    public void ActivateSpeedBoost()
    {
        _speedBoostTimeLeft = _speedBoostSeconds;
        IsSpeedBoosting = true;
    }

    // 실드 아이템을 먹었을 때 장애물을 한 번 막을 수 있는 상태로 만듭니다.
    // 이미 실드가 있는 상태에서 또 먹어도 HasShield가 true로 유지될 뿐, 중첩 횟수는 쌓이지 않습니다.
    public void ActivateShield()
    {
        HasShield = true;
    }

    // 장애물 충돌 시 실드가 있으면 실드를 소모하고 true를 반환합니다.
    public bool UseShield()
    {
        if (!HasShield)
        {
            return false;
        }

        HasShield = false;
        return true;
    }

    // 장애물 충돌, 낙하, 재시작 때 플레이어를 시작 위치로 되돌립니다.
    public void Respawn()
    {
        gameObject.SetActive(true);
        _isEnteringGoal = false;
        _goalpoint = null;

        transform.position = new Vector3(_startPosition.x, _startPosition.y, 0f);
        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.angularVelocity = 0f;
        _jumpCount = 0;
        IsDoubleJumping = false;
    }

    // 플레이어 아래쪽에 Ground Layer가 있는지 확인해서 착지 여부를 판단합니다.
    // OverlapBox 범위 안에 바닥이 있으면 착지로 보고, 점프 횟수를 다시 초기화할 수 있습니다.
    public bool IsGrounded()
    {
        Vector2 center = (Vector2)transform.position + Vector2.down * _groundCheckDistance;
        return Physics2D.OverlapBox(center, _groundCheckSize, 0f, _groundLayerMask) != null;
    }

    // GameManager 상태를 보고 지금 달릴 수 있는지 결정합니다.
    private void CheckGameState()
    {
        if (GameManager.Instance == null)
        {
            IsRunning = true;
            return;
        }

        IsRunning = GameManager.Instance.IsPlaying;
    }

    // 착지 상태를 확인하고 점프 횟수와 더블 점프 상태를 정리합니다.
    private void CheckGround()
    {
        IsGroundedNow = IsGrounded();

        if (IsGroundedNow && _rigidbody2D.linearVelocity.y <= 0.01f)
        {
            _jumpCount = 0;
            IsDoubleJumping = false;
        }
        else
        {
            IsDoubleJumping = _jumpCount >= 2;
        }
    }

    // 속도 증가 아이템의 남은 시간을 줄이고 상태를 갱신합니다.
    private void CheckBoost()
    {
        if (_speedBoostTimeLeft > 0f)
        {
            _speedBoostTimeLeft -= Time.deltaTime;
        }

        IsSpeedBoosting = _speedBoostTimeLeft > 0f;
    }

    // 플레이 중일 때 오른쪽으로 자동 이동합니다.
    private void MoveRight()
    {
        float currentRunSpeed = _runSpeed;

        if (IsSpeedBoosting)
        {
            currentRunSpeed = _runSpeed * _speedBoostMultiplier;
        }

        _rigidbody2D.linearVelocity = new Vector2(currentRunSpeed, _rigidbody2D.linearVelocity.y);
    }

    // 점프 입력을 확인하고 최대 점프 횟수 안에서 점프를 실행합니다.
    // 점프할 때마다 _jumpCount를 올리고, 두 번째 점프부터는 더블 점프 상태로 애니메이션이 바뀝니다.
    private void CheckJump()
    {
        if (!CheckJumpButton())
        {
            return;
        }

        if (_jumpCount >= _maxJumpCount)
        {
            return;
        }

        _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, _jumpPower);
        _jumpCount++;
        PlayJumpSound();
        IsGroundedNow = false;
        IsDoubleJumping = _jumpCount >= 2;
    }

    // 일반 점프와 더블 점프에 맞는 사운드를 재생합니다.
    private void PlayJumpSound()
    {
        if (_jumpCount >= 2)
        {
            SoundManager.Instance.PlayDoubleJump();
            return;
        }

        SoundManager.Instance.PlayJump();
    }

    // 현재 입력 방식에 맞게 점프 버튼 입력을 확인합니다.
    private bool CheckJumpButton()
    {

        if (Keyboard.current == null)
        {
            return false;
        }

        return Keyboard.current.spaceKey.wasPressedThisFrame;
    }

    // 플레이어가 맵 아래로 떨어지면 시작 위치로 되돌립니다.
    private void CheckFall()
    {
        if (transform.position.y < _fallLimitY)
        {
            Respawn();
            return;
        }
    }

    // 골에 닿았을 때 자동 이동을 멈추고 목표 지점으로 들어가는 상태로 바꿉니다.
    public void EnterGoal(Transform goalPoint)
    {
        _goalpoint = goalPoint;
        _isEnteringGoal = true;
        _rigidbody2D.linearVelocity = Vector2.zero;
    }

    // 골 지점까지 천천히 이동한 뒤 스테이지 클리어를 GameManager에 알립니다.
    // 목표 위치에 가까워지면 플레이어를 숨기고 ClearStage를 호출해서 GameUI가 다음 흐름을 처리하게 합니다.
    private void MoveToGoal()
    {
        if (_goalpoint == null)
        {
            return;
        }

        Vector3 targetPosition = new Vector3(_goalpoint.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _enterGoalSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            gameObject.SetActive(false);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ClearStage();
            }
        }
    }
}
