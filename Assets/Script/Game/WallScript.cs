using UnityEngine;

public class WallScript : MonoBehaviour
{
    [SerializeField] Vector4 newClipNormalValue; // 新しい ClipNormal の値をインスペクターから設定

    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            Material mat = rend.material;
            if (mat != null)
            {
                mat.SetVector("_ClipNormal", newClipNormalValue); // マテリアルの _ClipNormal を変更
            }
        }
    }
}
