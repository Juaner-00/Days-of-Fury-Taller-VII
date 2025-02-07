﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SoundController : MonoBehaviour
{
    [SerializeField] protected GameObject source;
    [SerializeField] protected ActionClips[] actionClips;

    protected Dictionary<string, SoundPool> sources;

    private void Awake()
    {
        Debug.Log("Initialized");
        sources = new Dictionary<string, SoundPool>();

        for(int acI = 0; acI < actionClips.Length; acI++)
        {
            SoundPool sPool = new SoundPool(source != null? source : gameObject);
            sources.Add(actionClips[acI].ActionName, sPool);
        }

        SetUp(true);
    }

    protected abstract void SetUp(bool child);

    protected void PlayActionByName(string actionName, float spacing3D = 0f, bool random = false, bool loop = false)
    {
        for (int acI = 0; acI < actionClips.Length; acI++)
        {
            if (actionClips[acI].ActionName == actionName)
            {
                SoundPool sPool = default;
                if (sources.TryGetValue(actionName, out sPool))
                {
                    if (this == null)
                    {
                        Debug.Log($"Sound Controller es nulo = {this == null}");
                    }
                    else
                    {
                        AudioClip[] clips = actionClips[acI].Clips;
                        int clipI = random ? UnityEngine.Random.Range(0, clips.Length) : 0;
                        StartCoroutine(sPool.PlayCorroutine(clips[clipI], actionClips[acI].ActionVolume, loop, spacing3D));
                        StartCoroutine("TestCoroutine");
                    }
                }
            }
        }
    }

    protected void StopActionByName(string actionName)
    {
        for(int acI = 0; acI < actionClips.Length; acI++)
        {
            if(actionClips[acI].ActionName == actionName)
            {
                SoundPool sPool = default;
                if (sources.TryGetValue(actionName, out sPool))
                {
                    sPool.StopAll();
                }
            }
        }
    }

    IEnumerator TestCoroutine()
    {
        yield return null;
    }
}
