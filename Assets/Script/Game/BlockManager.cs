using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : SingletonMonoBehaviour<BlockManager>
{
    //---------- Queue関連 ----------//
    private Queue<CoroutineActionPair> coroutineQueue = new Queue<CoroutineActionPair>();
    private bool isCoroutineRunning = false;

    private class CoroutineActionPair
    {
        public List<IEnumerator> coroutineList;
        public Action onStart;
        public Action onCompleted;
    }

    private void EnqueueCoroutineList(List<IEnumerator> coroutineList, Action onStart = null, Action onCompleted = null)
    {
        CoroutineActionPair pair = new CoroutineActionPair
        {
            coroutineList = coroutineList,
            onStart = onStart,
            onCompleted = onCompleted
        };

        coroutineQueue.Enqueue(pair);

        if (!isCoroutineRunning)
        {
            StartNextCoroutineList();
        }
    }

    private void StartNextCoroutineList()
    {
        if (coroutineQueue.Count > 0)
        {
            CoroutineActionPair nextPair = coroutineQueue.Dequeue();
            StartCoroutine(ExecuteCoroutineList(nextPair));
        }
    }

    private IEnumerator ExecuteCoroutineList(CoroutineActionPair pair)
    {
        isCoroutineRunning = true;

        pair.onStart?.Invoke();

        List<IEnumerator> coroutineList = pair.coroutineList;

        List<bool> coroutineStatus = new List<bool>();
        foreach (var coroutine in coroutineList)
        {
            StartCoroutine(coroutine);
            coroutineStatus.Add(true);
        }

        while (coroutineStatus.Contains(true))
        {
            for (int i = 0; i < coroutineList.Count; i++)
            {
                if (coroutineStatus[i])
                {
                    if (!coroutineList[i].MoveNext())
                    {
                        coroutineStatus[i] = false;
                    }
                }
            }
            yield return null;
        }

        isCoroutineRunning = false;
        pair.onCompleted?.Invoke();
        StartNextCoroutineList();
    }

    //---------- ブロック関連 ----------//
    // ブロックを設定するクラス
    private class BlockClass
    {
        // GameObjectをKeyとして各種データにアクセスするためのDictionary
        private Dictionary<GameObject, BlockElement> elements = new Dictionary<GameObject, BlockElement>();

        public void Add(GameObject key, bool boolValue, List<GameObject> rayPositionList, List<GameObject> movedBlockList)
        {
            if (!elements.ContainsKey(key))
            {
                BlockElement element = new BlockElement();
                element.BoolValue = boolValue;
                element.RayPositionList = rayPositionList;
                element.MovedBlockList = movedBlockList;

                elements[key] = element;
            }
            else
            {
                Debug.LogError("指定されたGameObjectは既に追加されています");
            }
        }

        public BlockElement this[GameObject key]
        {
            get
            {
                if (elements.ContainsKey(key))
                {
                    return elements[key];
                }
                else
                {
                    Debug.LogError("キーが存在しません");
                    return null;
                }
            }
        }

        public IEnumerable<GameObject> GetKeys()
        {
            return elements.Keys;
        }

        public class BlockElement
        {
            public bool BoolValue { get; set; }
            public List<GameObject> RayPositionList { get; set; }
            public List<GameObject> MovedBlockList { get; set; }
        }
    }
    
    BlockClass blockClass = new BlockClass();
    List<GameObject> CollidedBlockList = new List<GameObject>(); // プレイヤーが触れた順番にブロックを格納するリスト

    // Dictionary<GameObject, bool> blockDictionary = new Dictionary<GameObject, bool>(); // フィールド上のブロックを管理
    // List<GameObject> CollidedBlockList = new List<GameObject>(); // プレイヤーが触れた順番にブロックを格納するリスト
    // List<List<GameObject>> movedBlockList = new List<List<GameObject>>(); // 動いた順番にブロックを格納するリスト

    // ブロックのStart時に呼ぶ
    public void AddBlock(GameObject block, bool boolValue, List<GameObject> rayPositionList)
    {
        List<GameObject> movedBlockList = new List<GameObject>();

        blockClass.Add(block, boolValue, rayPositionList, movedBlockList);
    }

    // ブロックが移動済みかどうか
    public bool getMoved(GameObject block)
    {
        return blockClass[block].BoolValue;
    }

    // Playerがブロックに触れたときに呼ぶ
    public void MoveUpBlock(GameObject obj)
    {
        if (blockClass[obj].BoolValue) return;
        blockClass[obj].BoolValue = true;

        List<IEnumerator> coroutineList = new List<IEnumerator>(); // コルーチンを一時的に保持するリスト
    
        foreach (var block in blockClass.GetKeys())
        {
            if (block.CompareTag(obj.tag))
            {
                coroutineList.Add(MoveUpCoroutine(block, obj)); // コルーチンをリストに追加
            }
        }

        CollidedBlockList.Add(obj);

        if (coroutineList.Count > 0)
        {
            EnqueueCoroutineList(coroutineList, onStart: () => 
            {
                // List<GameObject> innerList = new List<GameObject>();
                // movedBlockList.Add(innerList);
            });
        }
    }

    // ブロックを戻すときに呼ぶ
    public void MoveDownBlock()
    {
        if (CollidedBlockList.Count < 1) return;
        GameObject LastObj = CollidedBlockList[CollidedBlockList.Count - 1];

        List<IEnumerator> coroutineList = new List<IEnumerator>(); // コルーチンを一時的に保持するリスト
        
        foreach (var block in blockClass.GetKeys())
        {
            coroutineList.Add(MoveDownCoroutine(block, LastObj));
        }
        
        CollidedBlockList.RemoveAt(CollidedBlockList.Count - 1);

        if (coroutineList.Count > 0)
        {
            EnqueueCoroutineList(coroutineList, onCompleted: () =>
            {
                //if (CollidedBlockList.Count > 0) CollidedBlockList.RemoveAt(CollidedBlockList.Count - 1);
                blockClass[LastObj].MovedBlockList.Clear();
                blockClass[LastObj].BoolValue = false;
            });
        }
    }

    // ブロックを上方向に移動するコルーチン
    private IEnumerator MoveUpCoroutine(GameObject block, GameObject obj)
    {
        for (int i = 0; i < blockClass[block].RayPositionList.Count; i++)
        {
            // ブロックの現在位置と方向からRayを作成
            Ray ray = new Ray(blockClass[block].RayPositionList[i].transform.position, Vector3.up);
            float rayDistance = 0.3f; // Rayの飛距離

            // Rayの衝突情報を格納する変数
            RaycastHit hit;

            // Rayを発射して衝突判定
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                // 衝突したオブジェクトがPlayerタグを持っていない場合は移動しない
                if (!hit.collider.CompareTag("Player"))
                {
                    yield break;
                }
            }
        }

        blockClass[obj].MovedBlockList.Add(block);

        Vector3 initialPosition = block.transform.position;
        Vector3 targetPosition = new Vector3(RoundToNearest(initialPosition.x), RoundToNearest(initialPosition.y), RoundToNearest(initialPosition.z))
                                + Vector3.up * 1.0f;
        float duration = 2.5f; // 移動にかかる時間

        Rigidbody rb = block.GetComponent<Rigidbody>();
        if (rb == null)
        {
            // Rigidbodyがアタッチされていない場合、警告を出力して終了
            Debug.LogWarning("Rigidbodyが見つかりません。");
            yield break;
        }

        float elapsedTime = 0;
        Vector3 startingPosition = block.transform.position;
        while (elapsedTime < duration)
        {
            float t = Mathf.SmoothStep(0, 1, elapsedTime / duration);
            rb.MovePosition(Vector3.Lerp(initialPosition, targetPosition, t));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(targetPosition);
    }

    // ブロックを下方向に移動するコルーチン
    private IEnumerator MoveDownCoroutine(GameObject block, GameObject obj)
    {
        int Count = 0;
        foreach (var a in blockClass[obj].MovedBlockList)
        {
            if (a == block) Count++;
        }
        if (Count < 1) yield break;
        
        Vector3 initialPosition = block.transform.position;
        Vector3 targetPosition = new Vector3(RoundToNearest(initialPosition.x), RoundToNearest(initialPosition.y), RoundToNearest(initialPosition.z))
                                - Vector3.up * 1.0f;
        float duration = 2.5f; // 移動にかかる時間

        Rigidbody rb = block.GetComponent<Rigidbody>();
        if (rb == null)
        {
            // Rigidbodyがアタッチされていない場合、警告を出力して終了
            Debug.LogWarning("Rigidbodyが見つかりません。");
            yield break;
        }

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            float t = Mathf.SmoothStep(0, 1, elapsedTime / duration);
            rb.MovePosition(Vector3.Lerp(initialPosition, targetPosition, t));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(targetPosition);
    }

    // 0.5ごとに丸める関数
    private float RoundToNearest(float value)
    {
        return Mathf.Round(value / 0.5f) * 0.5f;
    }

    // リセット
    public void ResetBlockManager()
    {
        // キュー内のコルーチンを強制終了
         if (isCoroutineRunning)
        {
            StopAllCoroutines();
            isCoroutineRunning = false;
        }

        // キューのクリア
        coroutineQueue.Clear();

        // BlockClassの初期化
        blockClass = new BlockClass();

        // CollidedBlockListのクリア
        CollidedBlockList.Clear();
    }
}
