using UnityEngine;

public class RestrictionScript : MonoBehaviour
{
    [SerializeField] GameObject coll;
    [SerializeField] float angleThreshold = 30f; // 角度制限値（通過を制限する角度）

    void OnTriggerEnter(Collider other)
    {
        // 侵入したオブジェクトがプレイヤーでない場合は処理を終了
        if (!other.CompareTag("Player")) return;

        // 侵入した方向ベクトルを計算
        Vector3 direction = other.transform.position - transform.position;

        // 角度を計算
        float angle = Vector3.Angle(transform.forward, direction);
        Debug.Log(angle);

        // 30度以上の場合、通過を制限する
        if (angle >= angleThreshold)
        {
            coll.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        coll.SetActive(true);
    }
}
