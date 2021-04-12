using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickEvent : MonoBehaviour
{
    public Button btn;
    // Start is called before the first frame update

    [SerializeField] private string name;
    void Start()
    {
        Button button = btn.GetComponent<Button>();
        button.onClick.AddListener(delegate
        {
            print(name);
        });
    }
}
