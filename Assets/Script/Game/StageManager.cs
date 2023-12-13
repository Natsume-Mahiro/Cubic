using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] LoadStage loadStage;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject parentObject;

    GameObject PlayerObject;
    GameObject SpawnPosition;

    void Awake()
    {
        loadStage._Start();
        
        // プレイヤーの生成
        SpawnPosition = GameObject.FindWithTag("Spawn");
        if (SpawnPosition != null) Spawn();
    }

    public void ReStart()
    {
        if (parentObject != null)
        {
            if (parentObject.transform.childCount == 0) return;
            // 子オブジェクトの削除
            foreach (Transform child in parentObject.transform)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("Objects Parentが設定されていません。");
            return;
        }

        BlockManager.Instance.ResetBlockManager();
        loadStage._Start();
        SpawnPosition = GameObject.FindWithTag("Spawn");
        Spawn();
    }

    void Spawn()
    {
        if (PlayerObject != null)
        {
            Destroy(PlayerObject);
        }

        PlayerObject = Instantiate(Player, SpawnPosition.transform.position + new Vector3(0f, 1.0f, 0f), Quaternion.identity);
    }
}
