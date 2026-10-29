using DG.Tweening;
using System;
using UnityEngine;
public class MenuPanel : MonoBehaviour
{
    [SerializeField] GameMenuObj gameMenuObj;
    [SerializeReference] UIAnim btnEnter;
    [Range(0, 2)][SerializeField] float delay;
    [SerializeField] UIButton startBtn;
    [SerializeField] UIButton loadBtn;
    [SerializeField] UIButton quitBtn;
    public void Init(GameMenuObj obj)
    {
        gameMenuObj = obj;
        startBtn.Init(LoadingEnter).SetActive(false);
        loadBtn.Init(LoadGame).SetActive(false);
        quitBtn.Init(QuitGame).SetActive(false);
    }
    public void Enter()
    {
        loadBtn.interactable = GameSerialize.Loadable();
        EnterAnime();
    }
    void EnterAnime()
    {
        var sequeue = GameAnim.AutoPlay();
        sequeue.Insert(delay, btnEnter.Tween(startBtn.uiGroup));
        sequeue.Insert(delay * 2, btnEnter.Tween(loadBtn.uiGroup));
        sequeue.Insert(delay * 3, btnEnter.Tween(quitBtn.uiGroup));
        sequeue.onComplete += SetRayTrue;
    }
    private void SetRayTrue()
    {
        startBtn.Ray(true);
        loadBtn.Ray(true);
        quitBtn.Ray(true);
    }
    void LoadingEnter()
    {
        var tween = GameLoading.GetObj().Enter();
        tween.onComplete += StartGame;
    }
    void StartGame()
    {
        gameMenuObj.Exit();
        GameStory.Enter();
    }
    void LoadGame()
    {
        GameSerialize.LoadGame();
        gameMenuObj.Exit();
    }
    void QuitGame()
    {
        Application.Quit();
    }
}