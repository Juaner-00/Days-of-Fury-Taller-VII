﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class ReticleController : MonoBehaviour
{
    [Header("Turret Properties")]
    [SerializeField] Transform turretTransform;
    [SerializeField] TurretTank turretTank;

    Quaternion target;
    Vector3 reticlePosition;

    public Action OnShooting;

    private void Update()
    {
        HandleInputs();

        if (Input.GetButtonDown("Fire1") && !Menu.IsPaused)
        {
            if (turretTank == true && turretTank.Available == true)
            {
                turretTank.Shot();
                OnShooting?.Invoke();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!Menu.IsPaused)
            HandleTurret();
    }

    void HandleTurret()
    {
        if (Input.GetJoystickNames().Length == 0 || string.IsNullOrEmpty(Input.GetJoystickNames()[0]))
        {
            if (turretTransform)
            {
                Vector3 turretLookDir = reticlePosition - turretTransform.position;
                Vector3 newDir = Vector3.RotateTowards(turretTransform.forward, turretLookDir, 1, 0.0F);
                target = Quaternion.LookRotation(newDir);
                turretTransform.rotation = Quaternion.Euler(-90, target.eulerAngles.y, 0);
            }
        }
        else
        {
            float horizontal = Input.GetAxis("AimHorizontal");
            float vertical = -Input.GetAxis("AimVertical");

            float dir = Mathf.Atan2(horizontal, vertical)*Mathf.Rad2Deg;
            turretTransform.rotation = Quaternion.Euler(-90, dir, 0);
        }
    }

    void HandleInputs()
    {
        Ray screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(screenRay, out hit))
        {
            reticlePosition = hit.point;
        }
    }
}
