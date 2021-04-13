using System;
using System.Collections;
using System.Collections.Generic;
using Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level01Script : MonoBehaviour
{
    private Brick[] bricks;
    
    // Update is called once per frame
    void Update()
    {
        bricks = gameObject.GetComponentsInChildren<Brick>();
        if (bricks.Length == 0)
        {
            SceneManager.LoadScene("03 Breakout/Scenes/Breakout");
        }
    }
}
