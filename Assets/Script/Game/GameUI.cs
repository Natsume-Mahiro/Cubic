using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] SpriteAnimation clearAnimation;
    [SerializeField] GameObject Clear;

    bool GameClear;

    void Start()
    {
        clearAnimation._Start();
        Clear.SetActive(false);
        GameClear = false;
    }

    void Update()
    {
        if (FlagManager.Instance.Goal && !GameClear)
        {
            Clear.SetActive(true);
            clearAnimation.PlayAnimation(1);
            GameClear = true;
        }
    }
}
