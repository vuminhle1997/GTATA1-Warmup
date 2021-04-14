using System;
using System.Collections;
using System.Collections.Generic;
using Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level01Script : MonoBehaviour
{
    // active bricks inside level 01
    private Brick[] bricks;
    
    // if zero bricks, load 2nd level
    void Update()
    {
        bricks = gameObject.GetComponentsInChildren<Brick>();
        if (bricks.Length == 0)
        {
            SceneManager.LoadScene("03 Breakout/Scenes/Breakout");
        }
    }
}
