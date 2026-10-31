
using UnityEngine;
using UnityEngine.SceneManagement;
 
public class SimplePlane : MonoBehaviour
{
    public float speed = 100f;            // 前進速度
    public float turnSpeed = 20f;        // 轉向速度
 
    public int score = 0;                // 目前分數
    public int obstaclePenalty = 10;     // 撞到障礙物扣幾分
    public int collectibleReward = 10;   // 撞到加分物加幾分
 
    public float gameTime = 60f;         // 遊戲總時間(秒)
    float timeLeft;                      // 剩餘時間
    bool gameOver = false;
 
    int highScore = 0;                   // 最高分
    bool newRecord = false;              // 這局是否破紀錄
 
    [Header("音效")]
    public AudioClip flightSound;        // 飛行音效(循環播放)
    public AudioClip explosionSound;     // 爆炸音效(撞到障礙物)
    public AudioClip scoreSound;         // 得分音效(撞到加分物)
    [Range(0f, 1f)] public float flightVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
 
    AudioSource flightSource;            // 播放飛行音效
    AudioSource sfxSource;               // 播放爆炸、得分音效
 
    void Start()
    {
        timeLeft = gameTime;
        Time.timeScale = 1f;
        highScore = PlayerPrefs.GetInt("HighScore", 0); // 讀取已儲存的最高分
 
        // 飛行音效:循環播放
        flightSource = gameObject.AddComponent<AudioSource>();
        flightSource.clip = flightSound;
        flightSource.loop = true;
        flightSource.playOnAwake = false;
        flightSource.volume = flightVolume;
        if (flightSound != null) flightSource.Play();
 
        // 一次性音效
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }
 
    void Update()
    {
        if (gameOver) return;
 
        // 倒數計時
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            EndGame();
            return;
        }
 
        // 自動往前飛(機頭朝 X 軸正方向)
        transform.Translate(Vector3.right * speed * Time.deltaTime);
 
        if (Input.GetKey(KeyCode.W)) transform.Rotate(0, 0, turnSpeed * Time.deltaTime);  // 向上仰
        if (Input.GetKey(KeyCode.S)) transform.Rotate(0, 0, -turnSpeed * Time.deltaTime); // 向下垂
        if (Input.GetKey(KeyCode.A)) transform.Rotate(0, -turnSpeed * Time.deltaTime, 0, Space.World); // 向左轉
        if (Input.GetKey(KeyCode.D)) transform.Rotate(0, turnSpeed * Time.deltaTime, 0, Space.World);  // 向右轉
    }
 
    void EndGame()
    {
        gameOver = true;
        Time.timeScale = 0f;   // 暫停整個遊戲(飛機、生成器都會停止)
        flightSource.Stop();   // 停止飛行音效
 
        // 破紀錄就儲存最高分
        if (score > highScore)
        {
            highScore = score;
            newRecord = true;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }
 
    void OnTriggerEnter(Collider other)
    {
        if (gameOver) return;
 
        if (other.CompareTag("Obstacle"))
        {
            score -= obstaclePenalty;
            if (explosionSound != null) sfxSource.PlayOneShot(explosionSound);
        }
        else if (other.CompareTag("Collectible"))
        {
            score += collectibleReward;
            if (scoreSound != null) sfxSource.PlayOneShot(scoreSound);
            Destroy(other.gameObject); // 吃到後讓加分物消失
        }
    }
 
    // 畫面 UI:進行中顯示分數與剩餘時間,結束後顯示結算畫面與再玩一次按鈕
    void OnGUI()
    {
        GUI.skin.label.fontSize = 28;
 
        if (!gameOver)
        {
            // 遊戲進行中:顯示分數與剩餘時間
            GUI.Label(new Rect(80, 40, 300, 50), "分數: " + score);
            GUI.Label(new Rect(80, 80, 300, 50), "剩餘時間: " + Mathf.CeilToInt(timeLeft) + " 秒");
        }
        else
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 48;
            style.alignment = TextAnchor.MiddleCenter;
 
            string message = "遊戲結束\n最終分數: " + score + "\n最高分: " + highScore;
            if (newRecord) message += "\n新紀錄!";
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), message, style);
 
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 32;
            if (GUI.Button(new Rect(Screen.width / 2 - 120, Screen.height / 2 + 100, 240, 70),
                           "再玩一次", buttonStyle))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}