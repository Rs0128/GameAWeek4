using UnityEngine;

/// <summary>
/// Bombを受け入れるBasket（左右）
/// Bombの色に応じて判定、正しい色なら中に保持
/// </summary>
public class GoalBasket : MonoBehaviour
{
    public Bomb.BombColor basketColor; // このかごの色（Red/Blue）
    public Transform bombContainer;    // Bombを収納する子オブジェクト

    private void OnTriggerEnter2D(Collider2D other)
    {
        Bomb bomb = other.GetComponent<Bomb>();
        if (bomb == null) return;

        // 正しい色なら受け入れ
        if (bomb.color == basketColor)
        {
            bomb.EnterBasket(bombContainer);
            GameManager.Instance.AddScore(100);
        }
        else
        {
            // 間違ったかごに入れたらGameOver
            GameManager.Instance.GameOver();
        }
    }
}
