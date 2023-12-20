using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    [SerializeField] Color emissionColor = Color.red; // 変更したいEmissionの色
    [SerializeField] MeshRenderer[] targetRenderers; // 変更したいRenderer

    [SerializeField] List<GameObject> GlassYUp = new List<GameObject>();

    bool blockBool = false;

    void Start()
    {
        BlockManager.Instance.AddBlock(gameObject, false, GlassYUp);

        foreach (MeshRenderer targetRenderer in targetRenderers)
        {
            if (targetRenderer != null)
            {
                // Emissionを変更する
                targetRenderer.material.SetColor("_EmissionColor", emissionColor);
            }
        }
    }

    void Update()
    {
        if (!BlockManager.Instance.getMoved(gameObject))
        {
            // ブロックの上にプレイヤーが乗っている場合
            if (blockBool)
            {
                BlockManager.Instance.MoveUpBlock(gameObject);
            }
            
            LightDown();
        }
        else
        {
            LightUp();
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            blockBool = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            blockBool = false;
        }
    }

    void LightUp()
    {
        foreach (MeshRenderer targetRenderer in targetRenderers)
        {
            if (targetRenderer != null)
            {
                // Emissionを有効にする
                targetRenderer.material.EnableKeyword("_EMISSION");
            }
        }
    }

    void LightDown()
    {
        foreach (MeshRenderer targetRenderer in targetRenderers)
        {
            if (targetRenderer != null)
            {
                // Emissionを無効にする
                targetRenderer.material.DisableKeyword("_EMISSION");
            }
        }
    }
}
