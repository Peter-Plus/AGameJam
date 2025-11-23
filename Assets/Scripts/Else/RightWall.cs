using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightWall : MonoBehaviour
{
    public LevelManager levelManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        levelManager.LevelComplete();
        Debug.Log("Ω”¥•”“≤‡");
    }
}
