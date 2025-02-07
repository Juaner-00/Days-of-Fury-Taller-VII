﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : Menu
{
    public static DeathScreen Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
            Destroy(gameObject);
        Instance = this;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
            LoseGame();
#endif

        if (IsDead == true && HasWon == false)
            Navigate();
    }

    // Pausa el juego al perder el juego
    public void LoseGame()
    {
        Pause();
        IsDead = true;
    }

    // Maneja los botones
    public override void Action()
    {
        if (Option.gameObject.name == "Home")
        {
            GoHome();
            MisionManager.Instance.Resetear();
        }
        else if (Option.gameObject.name == "Restart")
        {
            RestartLevel();
            MisionManager.Instance.Resetear();
        }
    }
}
