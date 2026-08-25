using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Inspector에서 연결하는 플레이어와 시간 설정값입니다.
    // _player는 시작/재시작 때 위치를 되돌릴 대상이고, 시간 값들은 게임 규칙을 조절합니다.
    [SerializeField] private PlayerMove _player;
    [SerializeField] private float _startTime = 25f;
    [SerializeField] private float _hazardPenalty = 3f;

    // 게임 중에 계속 바뀌는 시간과 상태값입니다.
    // _timeLeft는 실패 판정에 쓰이고, _elapsedTime은 결과 화면의 걸린 시간 표시용입니다.
    private float _timeLeft;
    private float _elapsedTime;
    private bool _isStarted;
    private bool _isCleared;

    // 다른 스크립트가 GameManager에 쉽게 접근하기 위한 전역 참조입니다.
    public static GameManager Instance { get; private set; }

    // UI 타이머에 표시할 남은 시간입니다.
    public int TimeLeft
    {
        get
        {
            return Mathf.CeilToInt(Mathf.Max(0f, _timeLeft));
        }
    }

    // 결과 화면에 표시할 실제 플레이 시간입니다.
    public float ElapsedTime
    {
        get
        {
            return Mathf.FloorToInt(_elapsedTime);
        }
    }

    // 현재 게임이 진행 중인지 알려주는 상태입니다.
    // 시작했고, 아직 클리어하지 않았고, 시간이 남아 있을 때만 true가 됩니다.
    public bool IsPlaying
    {
        get
        {
            return _isStarted && !_isCleared && _timeLeft > 0f;
        }
    }

    // 스테이지 클리어 여부를 GameUI가 확인할 때 사용합니다.
    public bool IsCleared
    {
        get
        {
            return _isCleared;
        }
    }

    // 시간이 0이 되었는지 GameUI가 확인할 때 사용합니다.
    public bool IsFailed
    {
        get
        {
            return _isStarted && !_isCleared && _timeLeft <= 0f;
        }
    }

    // 시작할 때 자기 자신을 Instance로 등록하고 기본 시간을 준비합니다.
    private void Awake()
    {
        Instance = this;
        _timeLeft = _startTime;
    }

    // 플레이 중일 때 남은 시간은 줄이고, 걸린 시간은 늘립니다.
    // IsPlaying이 false면 타이머가 멈추기 때문에 시작 화면이나 결과 화면에서는 시간이 흐르지 않습니다.
    private void Update()
    {
        if (!IsPlaying)
        {
            return;
        }

        _timeLeft -= Time.deltaTime;
        _elapsedTime += Time.deltaTime;
    }

    // 시계 아이템을 먹었을 때 남은 시간을 추가합니다.
    public void AddTime(float bonusTime)
    {
        if (!IsPlaying)
        {
            return;
        }

        _timeLeft += bonusTime;
    }

    // 장애물에 부딪혔을 때 시간을 줄이고 플레이어를 시작 위치로 돌립니다.
    public void HitHazard(PlayerMove player)
    {
        if (!IsPlaying)
        {
            return;
        }

        _timeLeft = Mathf.Max(0f, _timeLeft - _hazardPenalty);
        player.Respawn();
    }

    // 목표 지점에 도착했을 때 클리어 상태로 바꿉니다.
    public void ClearStage()
    {
        if (!IsPlaying)
        {
            return;
        }

        _isCleared = true;
    }

    // 새 게임 또는 새 스테이지를 시작할 때 상태와 시간을 초기화합니다.
    // 각 스테이지 시작마다 남은 시간과 걸린 시간이 다시 0 기준으로 정리됩니다.
    public void StartGame()
    {
        _isStarted = true;
        _isCleared = false;
        _timeLeft = _startTime;
        _elapsedTime = 0f;

        if (_player != null)
        {
            _player.Respawn();
        }
    }

    public void ResetState()
    {
        _isStarted = false;
        _isCleared = false;
        _timeLeft = _startTime;
        _elapsedTime = 0f;

        if (_player != null)
        {
            _player.Respawn();
        }
    }

}


