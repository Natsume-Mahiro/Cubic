using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] GameObject targetTransform; // StageのTransformコンポーネント
    float rotationSpeed = 20.0f; // 回転速度調整
    float zoomSpeed = 5.0f; // ズーム速度調整
    float minZoomDistance = 7.0f; // 最小ズーム距離
    float maxZoomDistance = 20.0f; // 最大ズーム距離

    private bool isRotating = false;
    private Vector3 lastMousePosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isRotating = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isRotating = false;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            // ズームインとズームアウト
            ZoomCamera(scroll);
        }
    }

    void LateUpdate()
    {
        if (isRotating)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            transform.RotateAround(targetTransform.transform.position, Vector3.up, mouseDelta.x * rotationSpeed * Time.deltaTime);
            lastMousePosition = Input.mousePosition;
        }
    }

    void ZoomCamera(float scrollInput)
    {
        // カメラの現在の位置からターゲットまでの距離を計算
        float currentDistance = Vector3.Distance(transform.position, targetTransform.transform.position);

        // マウスホイールのスクロールに基づいてズームインまたはズームアウト
        float newDistance = currentDistance - scrollInput * zoomSpeed;
        newDistance = Mathf.Clamp(newDistance, minZoomDistance, maxZoomDistance);

        // カメラを新しい位置に設定
        Vector3 dir = (transform.position - targetTransform.transform.position).normalized;
        transform.position = targetTransform.transform.position + dir * newDistance;
    }
}
