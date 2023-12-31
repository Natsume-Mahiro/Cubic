using UnityEngine;

public class WallScript : MonoBehaviour
{
    [SerializeField] Vector4 newClipNormalValue; // 新しい ClipNormal の値をインスペクターから設定
    [SerializeField] Texture2D newTexture; // 新しいテクスチャをインスペクターから設定

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

                if (newTexture != null)
                {
                    mat.mainTexture = newTexture; // テクスチャを変更

                    // オブジェクトのスケールに合わせてテクスチャのタイリングを変更
                    Vector2 newTiling = new Vector2(0, 0);
                    if (newClipNormalValue.x != 0) newTiling = new Vector2(transform.localScale.z, transform.localScale.y);
                    if (newClipNormalValue.y != 0) newTiling = new Vector2(transform.localScale.x, transform.localScale.z);
                    if (newClipNormalValue.z != 0) newTiling = new Vector2(transform.localScale.x, transform.localScale.y);
                    mat.mainTextureScale = newTiling;
                }
            }
        }
    }
}
