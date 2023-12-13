using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] Sprite[] frames; // アニメーションのスプライトフレーム配列
    [SerializeField] float frameRate; // フレームの更新レート

    private Image image;
    private int currentFrameIndex;
    private float timer;
    private int playCount; // 再生した回数
    private int desiredPlayCount; // 再生する回数 (-1 で無限再生)

    public void _Start()
    {
        image = GetComponent<Image>();
        frameRate /= frames.Length;
        currentFrameIndex = 0;
        timer = 0;
        playCount = 0;
        desiredPlayCount = 0;
    }

    void Update()
    {
        if (desiredPlayCount != 0)
        {
            timer += Time.deltaTime;

            if (timer >= frameRate)
            {
                // 次のフレームを表示
                image.sprite = frames[currentFrameIndex];
                currentFrameIndex = (currentFrameIndex + 1) % frames.Length;
                timer = 0;

                if (currentFrameIndex == 0) // 1ループが終了したら再生回数を更新
                {
                    playCount++;

                    if (desiredPlayCount > 0 && playCount >= desiredPlayCount)
                    {
                        StopAnimation();
                    }
                }
            }
        }
    }

    // アニメーションを再生する関数
    public void PlayAnimation(int playTimes = -1)
    {
        if (frames != null && frames.Length > 0)
        {
            desiredPlayCount = playTimes;
            playCount = 0;
            currentFrameIndex = 0;
            timer = 0;
        }
    }

    // アニメーションを停止する関数
    public void StopAnimation()
    {
        desiredPlayCount = 0;
        playCount = 0;
    }
}
