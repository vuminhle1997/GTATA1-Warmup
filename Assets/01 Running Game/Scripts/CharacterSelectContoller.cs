using Scripts;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectContoller : MonoBehaviour
{
    [SerializeField] private RunCharacterController runCharacterController;

    [SerializeField] private Button previous;

    [SerializeField] private Button next;
    
    [SerializeField] private Sprite[] idleSprites;

    private const int CHOICES = 3;

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

    /// <summary>
    /// Shifts the index to left
    /// </summary>
    private void ShiftLeft()
    {
        if (pos - 1 < 0)
        {
            pos = 2;
        }
        else
        {
            pos--;
        }
    }

    /// <summary>
    /// Shifts index to right
    /// </summary>
    private void ShiftRight()
    {
        if (pos + 1 >= CHOICES)
        {
            pos = 0;
        }
        else
        {
            pos++;
        }
    }

    /// <summary>
    /// Changes the idle sprite
    /// </summary>
    private void ChangeAppearance()
    {
        runCharacterController.CharacterSprite.sprite = idleSprites[pos];
    }

    /// <summary>
    /// Getter, returns the position
    /// </summary>
    /// <returns></returns>
    public int GetPos()
    {
        return pos;
    }
}
