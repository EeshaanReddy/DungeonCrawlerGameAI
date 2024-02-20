using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public static Enemies enemyManager;

    public List<Enemy> enemies = new List<Enemy>();

    private void Awake()
    {
        enemyManager = this;
    }
}
