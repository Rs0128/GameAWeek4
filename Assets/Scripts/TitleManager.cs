using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトル画面のシーン遷移を管理するクラス。
/// ボタンに紐づけて使用します。
/// </summary>
public class TitleManager : MonoBehaviour
{
    /// <summary>
    /// ゲームシーンをロードします（MainSceneなど）
    /// </summary>
    public void OnStartButton()
    {
        SceneManager.LoadScene("Game");
    }
}