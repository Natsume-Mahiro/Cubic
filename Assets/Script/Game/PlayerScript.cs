using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1.4f; // 移動速度
    [SerializeField] GameObject Dead;
    
    Transform cameraTransform; // カメラのTransformコンポーネント
    Rigidbody rb; // プレイヤーのTransformコンポーネント

    float horizontalInput;
    float verticalInput;

    bool isGrounded; // 地面に接地しているかどうかを示すフラグ

    Vector3 ResetPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;

        ResetPoint = transform.position;
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            BlockManager.Instance.MoveDownBlock();
        }
    }

    void FixedUpdate()
    {
        // カメラの向いている方向を前方として移動ベクトルを計算
        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 movement = (cameraForward * verticalInput + cameraTransform.right * horizontalInput) * moveSpeed;

        // プレイヤーのTransformを使用して移動
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

        // プレイヤーの正面方向を進行方向に向ける
        if (movement.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(movement.x, 0.002f, movement.z));
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.deltaTime * 5f));
        }

        // 上下方向の接触判定を行う
        RaycastHit hit;
        float distance = 0.35f; // 上下方向の判定距離

        // 下方向の判定
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distance))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        // 上方向の判定
        if (Physics.Raycast(transform.position, Vector3.up, out hit, distance))
        {
            if (isGrounded)
            {
                BlowObjectsAway();
            }
        }
    }

    void BlowObjectsAway()
    {
        if (Dead == null) return;

        // 生成するGameObjectを[SerializeField]で指定したPrefabなどに置き換える必要があります
        GameObject prefabToInstantiate = Dead;

        for (int i = 0; i < 10; i++)
        {
            // ランダムな位置にPrefabを生成する
            Vector3 randomPosition = transform.position + Random.insideUnitSphere * 1f; // プレイヤーの周囲5ユニット以内にランダムな位置を生成
            GameObject obj = Instantiate(prefabToInstantiate, randomPosition, Quaternion.identity);

            // ランダムな方向に力を加えて吹き飛ばす
            Rigidbody objRb = obj.GetComponent<Rigidbody>();
            if (objRb != null)
            {
                Vector3 randomDirection = Random.onUnitSphere; // ランダムな方向を生成
                float force = Random.Range(2f, 10f); // 力の大きさをランダムに設定（10～20の範囲）

                objRb.AddForce(randomDirection * force, ForceMode.Impulse);
            }

            // 一定時間後に生成したオブジェクトを削除する
            Destroy(obj, 2.5f);
        }
        
        transform.position = ResetPoint;
    }
}
