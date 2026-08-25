using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Video;

public class GameUI : MonoBehaviour
{
    // Inspector에서 연결하는 UI, 버튼, 스테이지, 영상 참조입니다.
    // 화면 오브젝트는 켜고 끄는 용도이고, Button은 Start()에서 클릭 이벤트를 연결합니다.
    [Header("Start UI")]
    [SerializeField] private GameObject _startPanel;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _stageSelectButton;
    [SerializeField] private Button _quitButton;

    [Header("Result UI")]
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TMP_Text _resultTitleText;
    [SerializeField] private TMP_Text _resultTimeText;
    [SerializeField] private Button _retryButton;

    [Header("Timer UI")]
    [SerializeField] private GameObject _timerUI;
    [SerializeField] private TMP_Text _timeText;

    [Header("Stage Select UI")]
    [SerializeField] private GameObject _stageSelectPanel;
    [SerializeField] private Button _stageBackButton;
    [SerializeField] private Button _homeStageButton;
    [SerializeField] private Button _alleyStageButton;
    [SerializeField] private Button _schoolGateStageButton;

    [Header("Pause UI")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _pauseBackButton;

    [Header("Stage")]
    [SerializeField] private StageController _stageController;

    [Header("Video")]
    [SerializeField] private GameObject _videoPanel;
    [SerializeField] private VideoPlayer _introVideoPlayer;
    [SerializeField] private VideoClip _clearVideoClip;

    // 코드 안에서 현재 UI/스테이지 흐름을 기억하는 상태값입니다.
    // _isResultShown은 결과 화면 중복 호출 방지, _playStageVideos는 메인 시작과 스테이지 선택 흐름 구분에 사용합니다.
    private bool _isResultShown;
    private bool _playStageVideos = true;

    // 초기 설정: 화면 상태를 맞추고 버튼 클릭 이벤트를 연결합니다.
    // 씬이 켜질 때 시작 화면만 보이게 만들고, 각 버튼이 어떤 함수를 실행할지 등록합니다.
    private void Start()
    {
        
        Time.timeScale = 1f;

        if (_startPanel != null)
        {
            _startPanel.SetActive(true);
        }

        if (_resultPanel != null)
        {
            _resultPanel.SetActive(false);
        }

        if (_timerUI != null)
        {
            _timerUI.SetActive(false);
        }

        if (_stageSelectPanel != null)
        {
            _stageSelectPanel.SetActive(false);
        }

        if (_pausePanel != null)
        {
            _pausePanel.SetActive(false);
        }

        if (_pauseButton != null)
        {
            _pauseButton.gameObject.SetActive(false);
        }

        if (_startButton != null)
        {
            _startButton.onClick.AddListener(StartHomeStage);
        }

        if (_retryButton != null)
        {
            _retryButton.onClick.AddListener(RetryGame);
        }

        if (_stageSelectButton != null)
        {
            _stageSelectButton.onClick.AddListener(ShowStageSelect);
        }

        if (_stageBackButton != null)
        {
            _stageBackButton.onClick.AddListener(ShowMainPanel);
        }

        if (_homeStageButton != null)
        {
            _homeStageButton.onClick.AddListener(StartHomeStageFromStageSelect);
        }

        if (_alleyStageButton != null)
        {
            _alleyStageButton.onClick.AddListener(StartAlleyStage);
        }

        if (_schoolGateStageButton != null)
        {
            _schoolGateStageButton.onClick.AddListener(StartSchoolGateStage);
        }

        if (_pauseButton != null)
        {
            _pauseButton.onClick.AddListener(PauseGame);
        }

        if (_resumeButton != null)
        {
            _resumeButton.onClick.AddListener(ResumeGame);
        }

        if (_pauseBackButton != null)
        {
            _pauseBackButton.onClick.AddListener(BackToMain);
        }

        if (_quitButton != null)
        {
            _quitButton.onClick.AddListener(QuitGame);
        }

        if (_videoPanel != null)
        {
            _videoPanel.SetActive(false);
        }

        StartCoroutine(PlayGameBGMDelay());
    }

    // 매 프레임 확인: 시간 표시와 클리어/실패 상태를 감지합니다.
    // GameManager의 상태를 읽어서 CLEAR면 다음 흐름으로, FAIL이면 결과 화면으로 보냅니다.
    private void Update()
    {
        UpdateTimer();

        if (_isResultShown || GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.IsCleared)
        {
            GoNextStageOrShowClear();
        }

        if (GameManager.Instance.IsFailed)
        {
            ShowResult("FAIL");
        }
    }

    // 타이머 UI에 남은 시간을 표시합니다.
    private void UpdateTimer()
    {
        if (_timeText == null || GameManager.Instance == null)
        {
            return;
        }

        _timeText.text = GameManager.Instance.TimeLeft.ToString();
    }

    // 게임 시작 전 배경음악이 바로 겹치지 않도록 잠깐 기다렸다가 재생합니다.
    private IEnumerator PlayGameBGMDelay()
    {
        yield return new WaitForSeconds(0.5f);
        SoundManager.Instance.PlayGameBGM();
    }

    // 메인 시작 버튼 흐름입니다. 인트로 영상까지 포함해서 1스테이지를 시작합니다.
    private void StartHomeStage()
    {
        StartSelectedStage(1, true);
    }

    // 스테이지 선택 화면을 여는 메뉴 처리입니다.
    private void ShowStageSelect()
    {
        PlayUIClick();

        if (_startPanel != null)
        {
            _startPanel.SetActive(false);
        }

        if (_stageSelectPanel != null)
        {
            _stageSelectPanel.SetActive(true);
        }
    }

    // 스테이지 선택 화면에서 다시 메인 화면으로 돌아갑니다.
    private void ShowMainPanel()
    {
        PlayUIClick();

        if (_stageSelectPanel != null)
        {
            _stageSelectPanel.SetActive(false);
        }

        if (_startPanel != null)
        {
            _startPanel.SetActive(true);
        }
    }

    // 스테이지 선택 버튼들입니다. 선택 시작은 인트로와 결과 영상을 생략합니다.
    private void StartHomeStageFromStageSelect()
    {
        StartSelectedStage(1, false);
    }

    private void StartAlleyStage()
    {
        StartSelectedStage(2, false);
    }

    private void StartSchoolGateStage()
    {
        StartSelectedStage(3, false);
    }

    // 스테이지 시작 공통 입구입니다. 어떤 스테이지를 켤지, 영상을 재생할지 결정합니다.
    // 메인 시작은 playStageVideos=true, 스테이지 선택은 false로 들어와 이후 클리어 처리 방식도 달라집니다.
    private void StartSelectedStage(int stageIndex, bool playStageVideos)
    {
        _playStageVideos = playStageVideos;

        PlayUIClick();

        StartCoroutine(StartSelectedStageRoutine(stageIndex, _playStageVideos));
    }

    // 실제 스테이지 시작 순서입니다. 필요하면 인트로 영상을 끝까지 재생한 뒤 맵을 켭니다.
    // 코루틴을 쓰는 이유는 영상 준비와 재생이 끝날 때까지 다음 코드 실행을 기다리기 위해서입니다.
    private IEnumerator StartSelectedStageRoutine(int stageIndex, bool playStageVideos)
    {
        SoundManager.Instance.StopMusic();

        if (_startPanel != null)
        {
            _startPanel.SetActive(false);
        }

        if (_stageSelectPanel != null)
        {
            _stageSelectPanel.SetActive(false);
        }

        if (playStageVideos)
        {
            if (_videoPanel != null)
            {
                _videoPanel.SetActive(true);
            }

            if (_introVideoPlayer != null)
            {
                _introVideoPlayer.Stop();
                _introVideoPlayer.Prepare();

                while (!_introVideoPlayer.isPrepared)
                {
                    yield return null;
                }

                _introVideoPlayer.Play();

                yield return null;

                while (_introVideoPlayer.isPlaying)
                {
                    yield return null;
                }

                _introVideoPlayer.Stop();
            }

            if (_videoPanel != null)
            {
                _videoPanel.SetActive(false);
            }
        }

        if (_stageController != null)
        {
            _stageController.ShowStage(stageIndex);
        }

        StartPlay();
    }

    // 실제 플레이 시작 처리입니다. 게임 UI를 켜고 GameManager의 타이머를 시작합니다.
    // 이 함수가 실행되면 시작/결과 화면은 꺼지고, 타이머와 일시정지 버튼이 보이며 플레이어가 리스폰됩니다.
    private void StartPlay()
    {
        Time.timeScale = 1f;
        _isResultShown = false;

        if (_startPanel != null)
        {
            _startPanel.SetActive(false);
        }

        if (_stageSelectPanel != null)
        {
            _stageSelectPanel.SetActive(false);
        }

        if (_resultPanel != null)
        {
            _resultPanel.SetActive(false);
        }

        if (_timerUI != null)
        {
            _timerUI.SetActive(true);
        }

        if (_pausePanel != null)
        {
            _pausePanel.SetActive(false);
        }

        if (_pauseButton != null)
        {
            _pauseButton.gameObject.SetActive(true);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }

        SoundManager.Instance.PlayGameBGM();
    }

    // 일시정지 버튼 처리입니다. 시간을 멈추고 PausePanel을 켭니다.
    private void PauseGame()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
        {
            return;
        }

        PlayUIClick();

        Time.timeScale = 0f;

        if (_pauseButton != null)
        {
            _pauseButton.gameObject.SetActive(false);
        }

        if (_pausePanel != null)
        {
            _pausePanel.SetActive(true);
        }
    }

    // Resume 버튼 처리입니다. 멈춘 시간을 다시 흐르게 하고 게임 화면으로 돌아갑니다.
    private void ResumeGame()
    {
        PlayUIClick();

        Time.timeScale = 1f;

        if (_pausePanel != null)
        {
            _pausePanel.SetActive(false);
        }

        if (_pauseButton != null)
        {
            _pauseButton.gameObject.SetActive(true);
        }
    }

    // Back 버튼 처리입니다. 현재 게임을 멈추고 시작 화면 상태로 되돌립니다.
    private void BackToMain()
    {
        PlayUIClick();

        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetState();
        }

        SoundManager.Instance.StopMusic();

        if (_stageController != null)
        {
            _stageController.ShowStage(1);
        }

        if (_timerUI != null)
        {
            _timerUI.SetActive(false);
        }

        if (_pausePanel != null)
        {
            _pausePanel.SetActive(false);
        }

        if (_pauseButton != null)
        {
            _pauseButton.gameObject.SetActive(false);
        }

        if (_resultPanel != null)
        {
            _resultPanel.SetActive(false);
        }

        if (_stageSelectPanel != null)
        {
            _stageSelectPanel.SetActive(false);
        }

        if (_startPanel != null)
        {
            _startPanel.SetActive(true);
        }
    }

    // 종료 버튼 처리입니다. 빌드된 게임에서는 프로그램 종료를 요청합니다.
    private void QuitGame()
    {
        Debug.Log("Quit Game");
        PlayUIClick();
        SoundManager.Instance.StopMusic();
        Application.Quit();

    }

    // 클리어 후 흐름을 결정합니다. 다음 스테이지로 갈지, 결과 화면을 보여줄지 나눕니다.
    // 스테이지 선택으로 시작한 경우에는 선택한 스테이지만 끝내고, 메인 시작 흐름은 1->2->3 순서로 진행합니다.
    private void GoNextStageOrShowClear()
    {
        if (!_playStageVideos)
        {
            _isResultShown = true;
            ShowResult("CLEAR");
            return;
        }

        if (_stageController == null || _stageController.IsLastStage)
        {
            _isResultShown = true;
            StartCoroutine(ShowClearVideoAndResult());
            return;
        }

        Time.timeScale = 1f;
        _stageController.ShowNextStage();
        StartPlay();
    }

    // 최종 클리어 영상 처리입니다. 영상을 끝까지 보여준 뒤 CLEAR 결과를 띄웁니다.
    // 같은 VideoPlayer를 재사용하기 때문에 기존 인트로 클립을 기억해 두고, 클리어 영상이 끝나면 다시 돌려놓습니다.
    private IEnumerator ShowClearVideoAndResult()
    {
        Time.timeScale = 1f;
        SoundManager.Instance.StopMusic();

        if (_timerUI != null)
        {
            _timerUI.SetActive(false);
        }

        if (_pausePanel != null)
        {
            _pausePanel.SetActive(false);
        }

        if (_pauseButton != null)
        {
            _pauseButton.gameObject.SetActive(false);
        }

        if (_videoPanel != null)
        {
            _videoPanel.SetActive(true);
        }

        if (_introVideoPlayer != null && _clearVideoClip != null)
        {
            VideoClip previousClip = _introVideoPlayer.clip;


            _introVideoPlayer.Stop();
            _introVideoPlayer.clip = _clearVideoClip;
            _introVideoPlayer.Prepare();

            while (!_introVideoPlayer.isPrepared)
            {
                yield return null;
            }

            _introVideoPlayer.Play();

            yield return null;

            while (_introVideoPlayer.isPlaying)
            {
                yield return null;
            }
            _introVideoPlayer.Stop();
            _introVideoPlayer.clip = previousClip;
        }

        if (_videoPanel != null)
        {
            _videoPanel.SetActive(false);
        }

        ShowResult("CLEAR");
    }

    // Retry 버튼 처리입니다. 현재 스테이지를 다시 켜고 게임을 재시작합니다.
    private void RetryGame()
    {
        PlayUIClick();

        Time.timeScale = 1f;

        if (_stageController != null)
        {
            _stageController.ShowStage(_stageController.CurrentStageIndex);
        }

        StartPlay();
    }

    // 결과 화면 처리입니다. CLEAR/FAIL 제목과 걸린 시간을 표시합니다.
    private void ShowResult(string title)
    {
        _isResultShown = true;
        Time.timeScale = 1f;

        if (_timerUI != null)
        {
            _timerUI.SetActive(false);
        }

        if (_pausePanel != null)
        {
            _pausePanel.SetActive(false);
        }

        if (_pauseButton != null)
        {
            _pauseButton.gameObject.SetActive(false);
        }

        if (_resultTitleText != null)
        {
            _resultTitleText.text = title;
        }

        if (_resultTimeText != null && GameManager.Instance != null)
        {
            _resultTimeText.text = "Time " + GameManager.Instance.ElapsedTime.ToString();
        }

        if (_resultPanel != null)
        {
            _resultPanel.SetActive(true);
        }

        PlayResultMusic(title);
    }

    // 사운드 보조 함수입니다. UI 클릭음과 결과 음악 재생을 SoundManager에 맡깁니다.
    private void PlayUIClick()
    {
        SoundManager.Instance.PlayUIClick();
    }

    private void PlayResultMusic(string title)
    {
        if (title == "CLEAR")
        {
            SoundManager.Instance.PlayStageClearMusic();
            return;
        }

        if (title == "FAIL")
        {
            SoundManager.Instance.PlayGameFailMusic();
        }
    }

}
