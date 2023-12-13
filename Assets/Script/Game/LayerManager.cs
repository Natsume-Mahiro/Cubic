using UnityEngine;

public class LayerManager : MonoBehaviour
{
    [SerializeField] LayerMask Red;     // Redレイヤー
    [SerializeField] LayerMask Green;   // Greenレイヤー
    [SerializeField] LayerMask Blue;    // Blueレイヤー
    [SerializeField] LayerMask White;    // Whiteレイヤー

    private bool isRedVisible = true;     // Redレイヤーの表示状態を追跡
    private bool isGreenVisible = true;   // Greenレイヤーの表示状態を追跡
    private bool isBlueVisible = true;    // Blueレイヤーの表示状態を追跡
    private bool isWhiteVisible = true;    // Whiteレイヤーの表示状態を追跡

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) OnRedButtonPress();
        if (Input.GetKeyDown(KeyCode.Alpha2)) OnGreenButtonPress();
        if (Input.GetKeyDown(KeyCode.Alpha3)) OnBlueButtonPress();
        if (Input.GetKeyDown(KeyCode.Alpha4)) OnWhiteButtonPress();
    }

    private void ToggleLayerVisibility(LayerMask layer, ref bool isVisible)
    {
        isVisible = !isVisible; // 表示状態を切り替える
        if (isVisible)
        {
            Camera.main.cullingMask |= layer; // レイヤーを表示に設定
        }
        else
        {
            Camera.main.cullingMask &= ~layer; // レイヤーを非表示に設定
        }
    }

    public void OnRedButtonPress()
    {
        ToggleLayerVisibility(Red, ref isRedVisible); // Redボタンが押されたとき、Redレイヤーの表示を切り替える
    }

    public void OnGreenButtonPress()
    {
        ToggleLayerVisibility(Green, ref isGreenVisible); // Greenボタンが押されたとき、Greenレイヤーの表示を切り替える
    }

    public void OnBlueButtonPress()
    {
        ToggleLayerVisibility(Blue, ref isBlueVisible); // Blueボタンが押されたとき、Blueレイヤーの表示を切り替える
    }

    public void OnWhiteButtonPress()
    {
        ToggleLayerVisibility(White, ref isWhiteVisible); // Whiteボタンが押されたとき、Whiteレイヤーの表示を切り替える
    }
}
