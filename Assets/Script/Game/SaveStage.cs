using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SaveStage : MonoBehaviour
{
    [SerializeField] private GameObject parentObject; // 対象とする親オブジェクト
    [SerializeField] private string fileName = "GameObjectInfo.json"; // JSONファイル名
    private string folderName = "StageData"; // 保存先フォルダ名

    void Start()
    {
        if (parentObject == null)
        {
            Debug.LogWarning("親オブジェクトが指定されていません。");
            return;
        }

        foreach (Transform child in parentObject.transform)
        {
            // 子オブジェクトの名前から_以降の文字を削除する
            string[] nameParts = child.gameObject.name.Split('_');
            child.gameObject.name = nameParts[0];
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SaveObjectsInfo();
        }
    }

    void SaveObjectsInfo()
    {
        if (parentObject == null) return;

        List<GameObjectInfo> objectsInfo = new List<GameObjectInfo>();

        // 親オブジェクトの直下にある子オブジェクトを取得
        foreach (Transform child in parentObject.transform)
        {
            // ゲームオブジェクトの情報を取得してリストに追加
            GameObjectInfo objInfo = new GameObjectInfo();
            objInfo.name = child.gameObject.name;
            objInfo.position = child.position;
            objInfo.rotation = child.rotation.eulerAngles;
            objInfo.scale = child.localScale;

            objectsInfo.Add(objInfo);
        }

        // JSON形式に変換
        string json = JsonUtility.ToJson(new SerializableObjectList<GameObjectInfo>(objectsInfo), true);

        // ファイルに書き込み
        string folderPath = Path.Combine(Application.dataPath, folderName);
        string filePath = Path.Combine(folderPath, fileName);

        // フォルダが存在しない場合は作成
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        File.WriteAllText(filePath, json);

        Debug.Log("ゲームオブジェクト情報を保存しました: " + filePath);
    }
}

// ゲームオブジェクト情報を保持するクラス
[System.Serializable]
public class GameObjectInfo
{
    public string name;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
}

// JsonUtilityでシリアル化するためのラッパークラス
public class SerializableObjectList<T>
{
    public List<T> list;

    public SerializableObjectList(List<T> list)
    {
        this.list = list;
    }
}
