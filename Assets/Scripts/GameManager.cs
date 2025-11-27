using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// <summary>
/// ゲーム全体を管理するクラス。
/// ・カウントダウン → ゲーム開始
/// ・スコア・Bomb生成・難易度上昇を管理
/// ・GameOver時にResultシーンへ遷移
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI参照")]
    public TMP_Text scoreText;
    public TMP_Text countdownText;

    [Header("Bomb生成関連")]
    public GameObject bombPrefab;
    public Transform spawnPoint;

    [Header("ゲームパラメータ")]
    public float baseSpawnInterval = 2f;
    public float baseFallSpeed = 2f;
    public float minSpawnInterval = 0.4f;
    public int scorePerLevel = 500;

    private int score = 0;
    private bool isPlaying = false;
    private float spawnInterval;
    private float fallSpeed;
    private float spawnTimer = 0f;

    // ★ 修正: private から public に変更し、Bomb.csからも参照・代入できるようにする
    public GameObject currentBomb;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        spawnInterval = baseSpawnInterval;
        fallSpeed = baseFallSpeed;
        UpdateScoreUI(); // スコア初期表示
        StartCoroutine(GameStartRoutine());
    }

    /// <summary>
    /// 3秒カウントダウン → ゲーム開始
    /// </summary>
    private IEnumerator GameStartRoutine()
    {
        countdownText.gameObject.SetActive(true);
        countdownText.text = "3";
        yield return new WaitForSeconds(1f);
        countdownText.text = "2";
        yield return new WaitForSeconds(1f);
        countdownText.text = "1";
        yield return new WaitForSeconds(1f);
        countdownText.text = "START!";
        yield return new WaitForSeconds(0.5f);
        countdownText.gameObject.SetActive(false);

        isPlaying = true;
        score = 0;
        UpdateScoreUI();
        SpawnBomb(); // 最初のBombを生成
    }

    private void Update()
    {
        if (!isPlaying) return;

        // ★ 修正: キー入力処理をGameManagerに集約
        HandleInput();

        // Bombがなくなったら次を生成（currentBombがnullの場合）
        if (currentBomb == null)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                SpawnBomb();
                spawnTimer = 0f;
            }
        }

        // スコアに応じた難易度上昇
        AdjustDifficulty();
    }

    /// <summary>
    /// ★ 追加: キー入力処理をここに集約
    /// </summary>
    private void HandleInput()
    {
        if (currentBomb == null) return;

        Bomb bombComponent = currentBomb.GetComponent<Bomb>();
        if (bombComponent == null) return;

        // 左右入力で仕分け
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // 赤いボムをLeftBasketに入れるロジック (LeftBasketが赤用である前提)
            if (bombComponent.color == Bomb.BombColor.Red)
            {
                bombComponent.MoveToBasket("LeftBasket");
            }
            else
            {
                bombComponent.Wrong();
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // 青いボムをRightBasketに入れるロジック (RightBasketが青用である前提)
            if (bombComponent.color == Bomb.BombColor.Blue)
            {
                bombComponent.MoveToBasket("RightBasket");
            }
            else
            {
                bombComponent.Wrong();
            }
        }
    }


    /// <summary>
    /// 難易度上昇ロジック（スコアに応じて加速）
    /// </summary>
    private void AdjustDifficulty()
    {
        float difficulty = 1f + (score / (float)scorePerLevel); // 500点ごとに約1段階上昇
        fallSpeed = baseFallSpeed * difficulty;
        spawnInterval = Mathf.Max(baseSpawnInterval / difficulty, minSpawnInterval);
    }

    /// <summary>
    /// Bomb生成
    /// </summary>
    private void SpawnBomb()
    {
        int colorType = Random.Range(0, 2); // 0=Red, 1=Blue
        currentBomb = Instantiate(bombPrefab, spawnPoint.position, Quaternion.identity);

        Bomb bombComponent = currentBomb.GetComponent<Bomb>();
        if (bombComponent != null)
        {
            bombComponent.Init((Bomb.BombColor)colorType, fallSpeed);
        }
    }

    /// <summary>
    /// スコア加算
    /// </summary>
    public void AddScore(int value)
    {
        score += value;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "SCORE: " + score;
    }

    /// <summary>
    /// ゲームオーバー処理（Resultシーンへ）
    /// </summary>
    public void GameOver()
    {
        // currentBomb が null でない場合は、強制的に破棄してクリア
        if (currentBomb != null)
        {
            Destroy(currentBomb);
            currentBomb = null;
        }

        isPlaying = false;
        PlayerPrefs.SetInt("LastScore", score);
        SceneManager.LoadScene("Result");
    }

    /// <summary>
    /// スコア取得（Resultシーン用）
    /// </summary>
    public int GetScore() => score;
}