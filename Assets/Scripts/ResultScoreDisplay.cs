using UnityEngine;
using TMPro; // ★ 修正: TextMeshProを使用するためにインポート

/// <summary>
/// リザルト画面でスコア表示のみを担当するクラス。
/// </summary>
public class ResultScoreDisplay : MonoBehaviour
{
    [Header("UI参照")]
    public TMP_Text scoreText; // ★ 修正: TextをTMP_Textに変更

    private void Start()
    {
        // GameManagerで保存したスコアを取得して表示
        // PlayerPrefs.GetInt("LastScore", 0) でスコアキーは正しく使えています。
        int finalScore = PlayerPrefs.GetInt("LastScore", 0);

        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + finalScore;
        }
        else
        {
            // エラーチェックを追加
            Debug.LogError("Score Text (TMP_Text)がインスペクターに割り当てられていません！");
        }

        // 【重要】スコアを読み込んだら PlayerPrefs から削除しておくことを推奨します。
        // PlayerPrefs.DeleteKey("LastScore"); 
    }
}