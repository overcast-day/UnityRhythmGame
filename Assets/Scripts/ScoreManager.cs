using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public AudioSource hitSFX;
    public AudioSource missSFX;
    public AudioSource beatSFX;
    public AudioSource perfectSFX;
    public TMPro.TextMeshPro scoreText;

    void Start()
    {
        Instance = this;
        SharedData.score = 0;
        SharedData.maxScore = 0;
        SharedData.combo = 0;
        SharedData.maxCombo = 0;
        SetSfxVolume();
    }
    public static void Hit()
    {
        SharedData.score += 1;
        SharedData.combo += 1;
        Instance.hitSFX.Play();
    }
    public static void PerfectHit()
    {
        SharedData.score += 2;
        SharedData.combo += 1;
        Instance.beatSFX.Play();
    }

    public static void MisHit()
    {
        SetMaxCombo(SharedData.combo);
        SharedData.combo = 0;
        Instance.missSFX.Play();
    }
    public static void Miss()
    {
        SetMaxCombo(SharedData.combo);
        SharedData.combo = 0;
    }
    public static void Beat()
    {
        Instance.beatSFX.Play();
    }

    public static void SetMaxCombo(int currentCombo)
    {
        if (currentCombo > SharedData.maxCombo) 
        {
            SharedData.maxCombo = currentCombo;
        }
    }
    
    private void Update()
    {
        scoreText.text = (SharedData.score).ToString();
    }
    public void SetSfxVolume()
    {
        hitSFX.volume = SharedData.sfxVolume;
        missSFX.volume = SharedData.sfxVolume;
        beatSFX.volume = SharedData.sfxVolume;
        perfectSFX.volume = SharedData.sfxVolume;
    }
}