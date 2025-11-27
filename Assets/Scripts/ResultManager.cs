using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// リザルト画面でのシーン遷移を管理するクラス。
/// ボタンに紐づけて使用します。
/// </summary>
public class ResultManager : MonoBehaviour
{
    /// <summary>
    /// タイトル画面に戻る
    /// </summary>
    public void OnTitleButton()
    {
        SceneManager.LoadScene("Title");
    }

    /// <summary>
    /// ゲームを再スタートする
    /// </summary>
    public void OnRetryButton()
    {
        SceneManager.LoadScene("Game");
    }
}
