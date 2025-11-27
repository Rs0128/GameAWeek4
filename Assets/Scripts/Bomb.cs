using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    public enum BombColor { Red, Blue }
    public BombColor color;
    public float fallSpeed = 2f;

    private Rigidbody2D rb;
    private bool isInBasket = false; // かご内に入ったか判定

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Rigidbody2Dがない場合のチェック（必須ではないが安全のため）
        if (rb == null)
        {
            Debug.LogError("Bomb requires a Rigidbody2D component.");
        }
        // 落下処理に影響が出る可能性があるため、MoveToBasketの前にgravityScaleを0にしない
        rb.gravityScale = 0f; // 物理エンジンではなく、Transformで制御するため重力は無効化
    }

    /// <summary>
    /// Bomb初期化（色と落下速度設定）
    /// </summary>
    public void Init(BombColor c, float speed)
    {
        color = c;
        fallSpeed = speed;
        // SpriteRendererのチェックを追加（エディタ上で付いていれば不要）
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = (color == BombColor.Red) ? Color.red : Color.blue;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer not found on Bomb object.");
        }
    }

    void Update()
    {
        if (isInBasket) return;

        // 下方向に落下 (Transformによる移動)
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // 落下しすぎたらGameOver
        if (transform.position.y < -5f)
        {
            // GameManagerが次のBombを生成できるようにcurrentBombをクリア
            if (GameManager.Instance.currentBomb == gameObject)
            {
                GameManager.Instance.currentBomb = null;
            }
            GameManager.Instance.GameOver();
            Destroy(gameObject);
        }

        // ★ キー入力処理はGameManagerに移譲したため、ここから削除
    }

    /// <summary>
    /// 正しい方向のBasketへ飛ばす (public化し、GameManagerから呼ばれるように変更)
    /// </summary>
    public void MoveToBasket(string basketName)
    {
        // 仕分け処理を開始したら、落下処理を止める
        isInBasket = true;

        GameObject basket = GameObject.Find(basketName);
        if (basket)
        {
            // 物理挙動を使うため、rbが必要
            if (rb == null) return;

            // 落下処理を止めた後は物理エンジンに切り替える
            rb.gravityScale = 0f; // 重力を無効にして、飛ばす力のみに集中
            rb.linearVelocity = Vector2.zero; // 直前のTransform移動をリセット

            // Basketへ向かうベクトルを計算し、Impulseで飛ばす
            Vector2 dir = (basket.transform.position - transform.position).normalized;
            rb.AddForce(dir * 5f, ForceMode2D.Impulse);

            // GameManagerに現在のBomb処理が完了したことを通知
            GameManager.Instance.currentBomb = null;
        }
        else
        {
            Debug.LogError($"Basket not found: {basketName}");
        }
    }

    /// <summary>
    /// Basket内に入った際の処理
    /// </summary>
    public void EnterBasket(Transform container)
    {
        isInBasket = true;
        transform.SetParent(container);

        // Basket内のランダム位置に配置 (localPositionで親からの相対座標を設定)
        Vector2 randomOffset = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 0.5f));
        transform.localPosition = randomOffset; // ローカル座標で設定

        // Basket内ではゆらゆらランダム運動
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f; // 回転も止める

            // ★ 剛体の抵抗を最大にし、動きを抑制 (前回の提案から変更なし)
            rb.linearDamping = 10f;       // 線形抵抗 (動きにくくする)
            rb.angularDamping = 10f; // 角抵抗 (回転しにくくする)

            StartCoroutine(RandomMotion());
        }
    }

    /// <summary>
    /// Basket内でランダムに小さく動く
    /// </summary>
    private IEnumerator RandomMotion()
    {
        while (isInBasket)
        {
            // ★ 加える力をさらに大幅に小さくする (例: 0.01f〜0.05f)
            // わずかな振動を与える程度にする
            Vector2 randomForce = new Vector2(Random.Range(-0.01f, 0.01f), Random.Range(0.005f, 0.015f));
            rb.AddForce(randomForce, ForceMode2D.Impulse);

            // 力を加える間隔を長くすることも検討
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }

    /// <summary>
    /// 間違い仕分け → GameOver (public化)
    /// </summary>
    public void Wrong()
    {
        // GameManagerが次のBombを生成できるようにcurrentBombをクリア
        if (GameManager.Instance.currentBomb == gameObject)
        {
            GameManager.Instance.currentBomb = null;
        }
        GameManager.Instance.GameOver();
        Destroy(gameObject);
    }
}