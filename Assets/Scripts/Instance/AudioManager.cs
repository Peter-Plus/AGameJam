using UnityEngine;

/// <summary>
/// 全局音频管理器 - DontDestroyOnLoad单例
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;  // 背景音乐
    public AudioSource sfxSource;    // 音效

    [Header("Audio Clips")]
    public AudioClip mainMenuMusic; //主界面音乐
    public AudioClip battleMusic;
    public AudioClip bossMusic;

    [Header("Attack SFX")]
    public AudioClip j1Sfx;
    public AudioClip j2Sfx;
    public AudioClip j3Sfx;
    public AudioClip a4Sfx;

    #region 场景音乐切换API
    /// <summary>
    /// 切换到主菜单音乐
    /// </summary>
    public void PlayMainMenuMusic()
    {
        PlayMusic(mainMenuMusic);
    }

    /// <summary>
    /// 切换到战斗音乐
    /// </summary>
    public void PlayBattleMusic()
    {
        PlayMusic(battleMusic);
    }

    /// <summary>
    /// 切换到Boss音乐
    /// </summary>
    public void PlayBossMusic()
    {
        PlayMusic(bossMusic);
    }
    #endregion

    #region 外部音乐API
    public void PlayerMainMusic()
    {
        musicSource = GetComponent<AudioSource>();
        PlayMusic(mainMenuMusic);
    }

    //播放背景音乐API
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        //Q下面这句是什么意思？
        //A 如果当前播放的音乐就是要播放的音乐，并且正在播放中，则不做任何操作，避免重复播放同一音乐。
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    /// <summary>
    /// 停止背景音乐API
    /// </summary>
    public void StopMusic()
    {
        musicSource.Stop();
    }

    /// <summary>
    /// 暂停背景音乐API
    /// </summary>
    public void PauseMusic()
    {
        musicSource.Pause();
    }

    /// <summary>
    /// 恢复背景音乐API
    /// </summary>
    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    /// <summary>
    /// 设置音乐音量API
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);// 将音量限制在0到1之间
    }
    #endregion

    #region 音效控制API
    /// <summary>
    /// 播放单次音效
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);// 使用默认音量播放音效
    }

    /// <summary>
    /// 播放音效（指定音量）
    /// </summary>
    public void PlaySFX(AudioClip clip, float volumeScale)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volumeScale);
    }

    /// <summary>
    /// 设置音效音量
    /// </summary>
    public void SetSfxVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
    #endregion

    #region 音效API
    public void PlaySound(string animName)
    {
        AudioClip clip = null;
        switch (animName)
        {
            case "J1":
                clip = j1Sfx;
                break;
            case "J2":
                clip = j2Sfx;
                break;
            case "J3":
                clip = j3Sfx;
            clip = j3Sfx;
                break;
            case "A4":
                clip = a4Sfx;
                break;
        }
        PlaySFX(clip);
    }
    #endregion

    #region else

    private void Awake()
    {
        // 单例模式
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 从DataManager加载音量设置
        LoadVolumeSettings();
    }

    /// <summary>
    /// 从DataManager加载音量设置
    /// </summary>
    private void LoadVolumeSettings()
    {
        musicSource.volume = DataManager.Instance.GetMusicVolume();
        sfxSource.volume = DataManager.Instance.GetSfxVolume();
    }
    #endregion


}
