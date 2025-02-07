﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : Menu
{
    public static VictoryScreen Instance { get; private set; }

    [Header("Medal's Win")]
    [SerializeField] GameObject OneMedalWin; //Medalla1
    [SerializeField] GameObject TwoMedalWin; //Medalla2
    [SerializeField] GameObject ThreeMedalWin; //Medalla3
    Medals medals;

    private void Awake()
    {
        if (Instance)
            Destroy(gameObject);
        Instance = this;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.G))
            WinGame();
#endif

        if (HasWon == true && IsDead == false)
            Navigate();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ScoreManager.OnMedalObtained += AsignMedals;
    }

    // Se llama al gamar el juego
    public void WinGame()
    {
        Pause();
        HasWon = true;
        switch (medals)
        {
            case Medals.OneMedal:
                OneMedalWin.SetActive(true);
                break;
            case Medals.TwoMedal:
                OneMedalWin.SetActive(true);
                TwoMedalWin.SetActive(true);
                break;
            case Medals.ThreeMedal:
                ThreeMedalWin.SetActive(true);
                OneMedalWin.SetActive(true);
                TwoMedalWin.SetActive(true);
                break;
        }
    }

    void AsignMedals(Medals medal)
    {
        medals = medal;
    }

    // Maneja los botones
    public override void Action()
    {
        if (Option.gameObject.name == "Home")
        {
            GoHome();
            MisionManager.Instance.Resetear();
        }
        else if (Option.gameObject.name == "Next Level")
        {
            NextLevel();
            MisionManager.Instance.Resetear();
        }
        else if (Option.gameObject.name == "Restart")
        {
            RestartLevel();
            MisionManager.Instance.Resetear();
        }
    }
}
