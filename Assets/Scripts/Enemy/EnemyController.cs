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

    Vector3 inicialPosition;
   
    int healthPoints;
    bool isDead;
    Animator enemyAnimator;
    PoolVfxs particleDamage, particleExplo;

    public static event EnemyEvent OnDie;

    public delegate void EnemyEvent(Vector3 pos);

    public int MaxHealthPoints => maxHealthPoints;
    public int HealthPoints => healthPoints;

    public Action OnGettingHurt;

    AIPath aIPath;
    AIDestinationSetter aIDestinationSetter;
    FieldOfView fieldOfView;
    NavMeshAgent navMeshAgent;
    NewEnemyShoot newEnemyShoot;

    void Awake()
    {
        newEnemyShoot = GetComponent<NewEnemyShoot>();
        aIDestinationSetter = GetComponent<AIDestinationSetter>();
        aIPath = GetComponent<AIPath>();
        enemyAnimator = GetComponentInChildren<Animator>();
        particleDamage = GameObject.Find("VFXsChispas(Pool)").GetComponent<PoolVfxs>();
        particleExplo = GameObject.Find("VFXsExplosiones(Pool)").GetComponent<PoolVfxs>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        fieldOfView = GetComponent<FieldOfView>();
    }

    // Se llama cuando se instancia el objeto
    public void Instantiate()
    {
        if (aIPath)
        {
            aIPath.usingGravity = false;
            aIPath.canSearch = false;
            aIPath.canMove = false; 
            newEnemyShoot.Dead = true;
        }
        if (aIDestinationSetter)
        {
            aIDestinationSetter.target = null;
        }
        if (fieldOfView)
        {
            fieldOfView.enabled = false;
        }
        inicialPosition = transform.position;
    }

    // Se llama cuando el pool devuelve el objeto
    public void Begin(Vector3 position, string tag)
    {
        if (aIPath)
        {
            aIPath.usingGravity = true;
            aIPath.canSearch = true;
            aIPath.canMove = true; 
            newEnemyShoot.Dead = false;
        }
        if (aIDestinationSetter)
        {
            aIDestinationSetter.target = GameManager.Player.transform;
        }
        healthPoints = maxHealthPoints;
        isDead = false;
        transform.position = position;
        if (fieldOfView)
        {
            navMeshAgent.enabled = true;
            fieldOfView.enabled = true;
        }
    }

    // Se llama cuando el objeto se devuelve al pool
    public void End()
    {
        if (aIPath)
        {
            aIPath.usingGravity = false;
        }
        if (aIDestinationSetter)
        {
            aIDestinationSetter.target = null;
        }
        transform.position = inicialPosition;
        healthPoints = maxHealthPoints;
    }

    // Método para hacer que el enemigo tome daño
    public void TakeDamage()
    {

        if (damagedSmoke)
        {
            damagedSmoke.Play();
        }
        
        
        
        OnGettingHurt?.Invoke();
        if (isDead)
            return;

        ParticleSystem damage = particleDamage.GetItem(transform.position, tag);
        healthPoints--;
        isDead = (healthPoints <= 0) ? true : false;

        if (isDead)
        {
            if (damagedSmoke)
            {
                damagedSmoke.Stop();
            }
            
            enemyAnimator.SetTrigger("Dead4");
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
                newEnemyShoot.Dead = true;
            }
            if (fieldOfView)
            {
                navMeshAgent.enabled = false;
                fieldOfView.enabled = false;
            }
            Invoke("End", timeDead);
        }
    }
}
