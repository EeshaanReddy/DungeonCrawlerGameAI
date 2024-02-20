using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    void Start()
    {
        Enemies.enemyManager.enemies.Add(this);
    }

    private void OnDestroy()
    {
        Enemies.enemyManager.enemies.Remove(this);
    }
}
