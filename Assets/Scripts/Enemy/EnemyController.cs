﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

public class EnemyController : MonoBehaviour, IPool, IDamagable
{
    [SerializeField] int maxHealthPoints;
    [SerializeField] int scorePoints;
    [SerializeField] float timeDead;
    [SerializeField] ParticleSystem damagedSmoke;
    [SerializeField, Range(0, 1)] float stayOnLevelProbability;

    public static event Action<int> Mission = delegate { };
    public static event Action Kill = delegate { };
    public static event Action KillStrong = delegate { };
    public static event Action KillNormal = delegate { };

    [SerializeField] public bool isStrong;

    [SerializeField] GameObject minimapSignal;

    #region Sound

    public Action OnGettingHurt;

    #endregion

    Vector3 inicialPosition;

    int healthPoints;
    bool isDead;
    Animator enemyAnimator;
    PoolVfxs particleDamage, particleExplo;

    public static event EnemyEvent OnDie;
    public delegate void EnemyEvent(Vector3 pos);

    public int MaxHealthPoints => maxHealthPoints;
    public int HealthPoints => healthPoints;


    AIPath aIPath;
    EnemyStateMachine stateMachine;

    void Awake()
    {
        stateMachine = GetComponent<EnemyStateMachine>();
        aIPath = GetComponent<AIPath>();
        enemyAnimator = GetComponentInChildren<Animator>();
        particleDamage = GameObject.Find("VFXsChispas(Pool)").GetComponent<PoolVfxs>();
        particleExplo = GameObject.Find("VFXsExplosiones(Pool)").GetComponent<PoolVfxs>();
    }

    // Se llama cuando se instancia el objeto
    public void Instantiate(Pool parentPool)
    {
        if (aIPath)
        {
            aIPath.usingGravity = false;
            aIPath.canSearch = false;
            aIPath.canMove = false;
        }

        stateMachine.Alive = false;
        inicialPosition = transform.position;

        float prob = UnityEngine.Random.Range(0f, 1f);
        StayOnScene = prob <= stayOnLevelProbability;

        ParentPool = parentPool;
    }

    // Se llama cuando el pool devuelve el objeto
    public void Begin(Vector3 position, string tag, Vector3 _)
    {
        if (aIPath)
        {
            aIPath.usingGravity = true;
            aIPath.canSearch = true;
            aIPath.canMove = true;
        }

        enemyAnimator.SetTrigger("Init");

        stateMachine.Alive = true;
        healthPoints = maxHealthPoints;
        isDead = false;
        transform.position = position;

        foreach (Renderer material in GetComponentsInChildren<Renderer>())
        {
            material.material.SetFloat("damageChanger", (float)healthPoints / maxHealthPoints);
        }
    }

    // Se llama cuando el objeto se devuelve al pool
    public void End()
    {
        if (aIPath)
        {
            aIPath.usingGravity = false;
        }

        if (!StayOnScene)
        {
            transform.position = inicialPosition;
            healthPoints = maxHealthPoints;
            enemyAnimator.SetTrigger("Init");
        }

        if (!StayOnScene)
            ParentPool.PushItem(gameObject);
    }

    // Método para hacer que el enemigo tome daño
    public void TakeDamage()
    {

        if (isDead)
            return;

        if (damagedSmoke)
        {
            damagedSmoke.Play();
        }

        OnGettingHurt?.Invoke();

        ParticleSystem damage = particleDamage.GetItem(transform.position, tag);
        healthPoints--;

        foreach (Renderer material in GetComponentsInChildren<Renderer>())
        {
            material.material.SetFloat("damageChanger", (float)healthPoints / maxHealthPoints);
        }

        isDead = (healthPoints <= 0) ? true : false;

        if (isDead)
            Dead();
    }

    void Dead()
    {
        foreach (Renderer material in GetComponentsInChildren<Renderer>())
        {
            material.material.SetFloat("damageChanger", 0f);
        }
        stateMachine.Alive = false;

        if (damagedSmoke)
        {
            damagedSmoke.Stop();
        }

        if (StayOnScene)
        {
            enemyAnimator.SetTrigger($"Dead{UnityEngine.Random.Range(3, 5)}");
            Destroy(minimapSignal);
        }
        else
            enemyAnimator.SetTrigger($"Dead{UnityEngine.Random.Range(1, 5)}");


        ParticleSystem Explos = particleExplo.GetItem(transform.position, tag);

        OnDie?.Invoke(transform.position);

        if (ScoreManager.Instance)
        {
            ScoreManager.Instance.Addscore(scorePoints);
        }
        if (aIPath)
        {
            aIPath.canSearch = false;
            aIPath.canMove = false;
        }
        Invoke("End", timeDead);

        if (isStrong == false) KillNormal?.Invoke();
        else { KillStrong?.Invoke(); }

        Kill(); //Misiones
    }


    public bool StayOnScene { get; set; }
    public Pool ParentPool { get; set; }
}
