using System.Collections;
using System.Collections.Generic;
using Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelScript : MonoBehaviour
{
    // active bricks inside last level
    private Brick[] bricks;
    
    // if zero bricks, show winning screen
    void Update()
    {
        bricks = gameObject.GetComponentsInChildren<Brick>();
        if (bricks.Length == 0)
        {
            SceneManager.LoadScene("03 Winning Scene");
        }
    }
}
