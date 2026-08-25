using UnityEngine;

public class SoundManager : MonoBehaviour
{

    // 효과음과 배경음악을 재생할 AudioSource 참조입니다.
    // 효과음은 짧게 여러 번 겹칠 수 있고, 배경음악은 하나만 길게 재생되므로 AudioSource를 분리했습니다.
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioSource _musicSource;

    // 효과음과 배경음악의 기본 볼륨입니다.
    [Header("Volume")]
    [SerializeField] private float _sfxVolume = 0.8f;
    [SerializeField] private float _musicVolume = 0.55f;

    // 다른 스크립트가 SoundManager에 쉽게 접근하기 위한 전역 참조입니다.
    private static SoundManager _instance;

    // 게임 안에서 짧게 재생되는 효과음 클립들입니다.
    [Header("SFX Clips")]
    [SerializeField] private AudioClip _uiClickClip;
    [SerializeField] private AudioClip _jumpClip;
    [SerializeField] private AudioClip _doubleJumpClip;
    [SerializeField] private AudioClip _itemClockClip;
    [SerializeField] private AudioClip _itemDrinkClip;
    [SerializeField] private AudioClip _itemShieldClip;
    [SerializeField] private AudioClip _shieldBlockClip;
    [SerializeField] private AudioClip _obstacleHitClip;

    // 반복 배경음악과 결과 화면 음악 클립들입니다.
    [Header("Music Clips")]
    [SerializeField] private AudioClip _gameBGMClip;
    [SerializeField] private AudioClip _stageClearClip;
    [SerializeField] private AudioClip _gameFailClip;

    // 일반적으로는 씬에 있는 SoundManager가 Awake에서 등록되고, 다른 스크립트는 Instance로 재생 함수를 호출합니다.
    public static SoundManager Instance
    {
        get
        {
            return _instance;
        }
    }

    // 중복 SoundManager를 막고 AudioSource를 준비합니다.
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        PrepareAudioSource();
    }

    // 효과음 재생 함수들입니다. 실제 재생은 PlaySound에서 처리합니다.
    public void PlayUIClick()
    {
        PlaySound(_uiClickClip);
    }

    public void PlayJump()
    {
        PlaySound(_jumpClip);
    }

    public void PlayDoubleJump()
    {
        PlaySound(_doubleJumpClip);
    }

    public void PlayItemClock()
    {
        PlaySound(_itemClockClip);
    }

    public void PlayItemDrink()
    {
        PlaySound(_itemDrinkClip);
    }

    public void PlayItemShield()
    {
        PlaySound(_itemShieldClip);
    }

    public void PlayShieldBlock()
    {
        PlaySound(_shieldBlockClip);
    }

    public void PlayObstacleHit()
    {
        PlaySound(_obstacleHitClip);
    }

    // 배경음악과 결과 음악 재생 함수들입니다. 실제 재생은 PlayMusic에서 처리합니다.
    public void PlayGameBGM()
    {
        PlayMusic(_gameBGMClip, true);
    }

    public void PlayStageClearMusic()
    {
        PlayMusic(_stageClearClip, false);
    }

    public void PlayGameFailMusic()
    {
        PlayMusic(_gameFailClip, false);
    }

    // 현재 재생 중인 배경음악을 멈춥니다.
    public void StopMusic()
    {
        if (_musicSource == null)
        {
            return;
        }

        _musicSource.Stop();
        _musicSource.clip = null;
    }

    
    private void PrepareAudioSource()
    {
        if (_audioSource != null)
        {
            _audioSource.playOnAwake = false;
            _audioSource.volume = _sfxVolume;
        }

        if (_musicSource != null)
        {
            _musicSource.playOnAwake = false;
            _musicSource.volume = _musicVolume;
        }

    }

    // 효과음을 한 번만 짧게 재생합니다.
    private void PlaySound(AudioClip clip)
    {
        if (_audioSource == null || clip == null)
        {
            return;
        }

        _audioSource.PlayOneShot(clip, _sfxVolume);
    }

    // 배경음악을 교체하고 반복 여부를 설정해서 재생합니다.
    // 이미 같은 음악이 재생 중이면 다시 시작하지 않고, 다른 음악이면 기존 음악을 멈춘 뒤 새 음악을 재생합니다.
    private void PlayMusic(AudioClip clip, bool isLoop)
    {
        if (_musicSource == null)
        {
            return;
        }

        if (clip == null)
        {
            StopMusic();
            return;
        }

        if (_musicSource.clip == clip && _musicSource.isPlaying)
        {
            return;
        }

        _musicSource.Stop();
        _musicSource.clip = clip;
        _musicSource.loop = isLoop;
        _musicSource.volume = _musicVolume;
        _musicSource.Play();
    }
}
