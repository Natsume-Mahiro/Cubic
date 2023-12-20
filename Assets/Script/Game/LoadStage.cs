using UnityEngine;
using System.Collections.Generic;

public class LoadStage : MonoBehaviour
{
    [SerializeField] private string fileName = "Stage1.json"; // JSONファイル名
    [SerializeField] TextAsset[] StageData;
    [SerializeField] private List<GameObject> prefabList; // プレハブのリスト
    [SerializeField] private GameObject parentObject; // 親オブジェクト

    [System.Serializable]
    public class LoadedObjectInfo
    {
        public string name;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }

    public class SerializableObjectList
    {
        public List<LoadedObjectInfo> list;
    }

    public void _Start()
    {
        LoadObjectsInfo();
    }

    void LoadObjectsInfo()
    {
        if (parentObject == null)
        {
            Debug.LogWarning("親オブジェクトが設定されていません。");
            return;
        }

        // 指定したファイル名に一致するTextAssetを探す
        TextAsset targetFile = null;
        foreach (TextAsset stageFile in StageData)
        {
            if (stageFile.name == fileName)
            {
                targetFile = stageFile;
                break;
            }
        }

        if (targetFile != null)
        {
            string json = targetFile.text;

            SerializableObjectList objectsInfo = JsonUtility.FromJson<SerializableObjectList>(json);

            foreach (LoadedObjectInfo objInfo in objectsInfo.list)
            {
                // nameに一致するプレハブを探す
                GameObject prefab = prefabList.Find(x => x.name == objInfo.name);

                if (prefab != null)
                {
                    // プレハブを生成し、位置、回転、スケールを設定してオブジェクトを生成し、親オブジェクトの子オブジェクトとして配置
                    GameObject obj = Instantiate(prefab, objInfo.position, Quaternion.Euler(objInfo.rotation), parentObject.transform);
                    obj.transform.localScale = objInfo.scale;
                }
                else
                {
                    Debug.LogWarning("Prefabが見つかりません: " + objInfo.name);
                }
            }
        }
        else
        {
            Debug.LogWarning("ファイルが見つかりません: " + fileName);
        }
    }
}
