﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class FirstScreen : Menu
{
    bool firstClick = false;

    public static Action OnFirstClick;

    private void Start()
    {
        IsPaused = true;
    }

    private void LateUpdate()
    {
        if ((Input.anyKeyDown || Input.GetAxisRaw("Horizontal") > 0.1 || Input.GetAxisRaw("Vertical") > 0.1) && firstClick == false)
        {
            firstClick = true;
            Resume();
            Invoke("CallEvent", 0.2f);
        }
    }

    void CallEvent()
    {
        OnFirstClick?.Invoke();
    }

    // Maneja los botones
    public override void Action()
    {

    }
}
