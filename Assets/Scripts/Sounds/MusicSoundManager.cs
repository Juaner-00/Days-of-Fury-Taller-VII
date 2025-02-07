﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicSoundManager : SoundController
{
    [SerializeField] ScenesPerAction[] scenesPerActions;

    private static MusicSoundManager instance;

    public static MusicSoundManager _Instance => instance;

    protected override void SetUp(bool child)
    {
        if (_Instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += UploadMusic;
    }

    private void UploadMusic(Scene scene, LoadSceneMode loadSceneMode)
    {
        StopAll();

        for (int i = 0; i < actionClips.Length; i++)
        {
            if(actionClips[i].ActionName == scenesPerActions[i].ActionName)
            {
                if (scenesPerActions[i].SceneNames.Contains(scene.name))
                {
                    PlayActionByName(actionClips[i].ActionName, 0, scenesPerActions[i].PlayRandomClip, true);
                }
            }
        }
    }

    private void StopAll()
    {
        for(int i = 0; i < actionClips.Length; i++)
        {
            sources[actionClips[i].ActionName].StopAll();
        }
    }

    /*
    private void OnValidate()
    {
        if (actionClips != null) 
        { 
            if(actionClips.Length != scenesPerActions.Length)
            {
                if (actionClips.Length != ActionClipsCount)
                {
                    scenesPerActions = new ScenesPerAction[actionClips.Length];

                    for (int i = 0; i < scenesPerActions.Length; i++)
                    {
                        if(ScenesPerActions.Length > 0) scenesPerActions[i] = ScenesPerActions[i];
                    }
                }
            }
        }
        ActionClipsCount = actionClips.Length;
        ScenesPerActions = scenesPerActions;
    }

    private int ActionClipsCount { get; set; }
    private ScenesPerAction[] ScenesPerActions { get; set; }
    */
}
