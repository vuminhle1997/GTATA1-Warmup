using Scripts;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectContoller : MonoBehaviour
{
    [SerializeField] private RunCharacterController runCharacterController;

    [SerializeField] private Button previous;

    [SerializeField] private Button next;

    private Color[] colors = 
    {
        new Color(255, 255, 255),
        new Color(255, 0, 0),
        new Color(0, 255, 255)
    };

    private int pos = 0;

    public void Start()
    {
        Init();
    }

    void Init()
    {
        previous.onClick.AddListener(delegate
        {
            if (!Input.GetKeyDown(KeyCode.Space))
            {
                ShiftLeft();
                ChangeAppearance();
            }
        });
        next.onClick.AddListener(delegate
        {
            if (!Input.GetKeyDown(KeyCode.Space))
            {
                ShiftRight();
                ChangeAppearance();
            }
        });
    }

    private void ShiftLeft()
    {
        if (pos - 1 < 0)
        {
            pos = colors.Length-1;
        }
        else
        {
            pos--;
        }
    }

    private void ShiftRight()
    {
        if (pos + 1 >= colors.Length)
        {
            pos = 0;
        }
        else
        {
            pos++;
        }
    }

    private void ChangeAppearance()
    {
        runCharacterController.CharacterSprite.color = colors[pos];
    }
}
