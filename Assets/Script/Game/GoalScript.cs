using UnityEngine;

public class GoalScript : MonoBehaviour
{
    void OnTriggerEnter()
    {
        FlagManager.Instance.Goal = true;
    }
}
