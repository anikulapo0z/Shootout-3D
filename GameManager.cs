using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private float originalTimeScale;

    public VictoryUI victoryUI; 


    public RaycastPlayer raycastPlayer;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        originalTimeScale = Time.timeScale;
        
        //DontDestroyOnLoad(gameObject);

        DG.Tweening.DOTween.Init();
    }

    private void Update()
    {
        
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SlowTime(float slowTime, float duration)
    {
        Time.timeScale = slowTime;
        StartCoroutine(ReturnToNormalTimeAfterDelay(duration));
    }
    
    IEnumerator ReturnToNormalTimeAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = originalTimeScale;
        RaycastPlayer player = FindObjectOfType<RaycastPlayer>();
        
        if (player != null)
        {
            player.ResetTimeSlow();
        }

    }

    public void OnBossDefeated()
    {
        victoryUI.ShowVictoryUI();
    }

    public void TogglePlayerMovement(bool canMove)
    {
        RaycastPlayer player = FindObjectOfType<RaycastPlayer>();
        if (player != null)
        {
            player.canMove = canMove;
        }
    }

}
