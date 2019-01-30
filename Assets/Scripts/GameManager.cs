using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    // Static game manager (only one in game)
    static public GameManager gm;

    #region Public Field
    [Tooltip("Target Score")]
    public int TargetScore = 5;
    [Tooltip("Game State")]
    public Enums.GameState gameState;
    [Tooltip("Main player")]
    public GameObject player;

    [Header("Game Play UI")]
    public GameObject playingCanvas;
    public GameObject gameResultCanvas;
    public GameObject mobileControlRigCanvas;
    public Text scoreText;
    public Text timeText;
    public Slider healthSlider;
    [Tooltip("Image to show the hurt effect")]
    public Image hurtImage;
    
    [Header("Game Result UI")]
    [Tooltip("Show the first place player's current info")]
    public GameObject firstUserText;
    [Tooltip("Show the second place player's current info")]
    public GameObject secondUserText;
    [Tooltip("Show the third place player's current info")]
    public GameObject thirdUserText;
    [Tooltip("Show the player's current info")]
    public GameObject userText;
    [Tooltip("To show if the player go into the first three place")]
    public Text gameMessage;
    
    [Header("Audio")]
    public AudioClip gameWinAudio;
    public AudioClip gameOverAudio;
    
    [Tooltip("Toggle to lock the cursor during the game")]
    public bool lockCursor = true;
    #endregion
    

    #region Private Field
    private bool m_cursorIsLocked;
    
    private int currentScore;
    /// <summary>
    /// The time to start the scenea.
    /// </summary>
    private float startTime;
    private float currentTime;
    private PlayerHealth playerHealth;

    private bool cursor;
    private AudioListener audioListener;    //摄像机的AudioListener组件
    private Color flashColor = new Color (1.0f, 0.0f, 0.0f, 0.3f);  //玩家受伤时，hurtImage的颜色
    private float flashSpeed = 2.0f;                                //hurtImage颜色的渐变速度

    private UserData firstUserData;     //排名第一的玩家的相关数据
    private UserData secondUserData;    //排名第二的玩家的相关数据
    private UserData thirdUserData;     //排名第三的玩家的相关数据
    private UserData currentUserData;   //当前玩家的相关数据
    private UserData[] userDataArray = new UserData[4];

    private bool isGameOver=false;      //标识，保证游戏结束时的相关行为只执行一次
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization.
    /// </summary>
    void Start ()
    {

        // Intiate the static instance.
        if (gm == null)
            gm = GetComponent<GameManager>();

        // Set the game state to playing.
        gm.gameState = Enums.GameState.Playing;
        // Set the start score as 0.
        currentScore = 0;
        // Set the start time.
        startTime = Time.time;

        PlayerInit();
        UIInit();
        AudioInit();

        m_cursorIsLocked = lockCursor;
    }
    
    /// <summary>
    /// Initialize the UI.
    /// </summary>
    private void UIInit()
    {
    
        // Get the main player.
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
            
        playingCanvas.SetActive(true);
        gameResultCanvas.SetActive(false);

        // Save the first three places information.
        if (PlayerPrefs.GetString("FirstUser") != "")
        {
            firstUserData = new UserData(PlayerPrefs.GetString("FirstUser"));
        }
        else
            firstUserData = new UserData();
        if (PlayerPrefs.GetString("SecondUser") != "")
        {
            secondUserData = new UserData(PlayerPrefs.GetString("SecondUser"));
        }
        else
            secondUserData = new UserData();
        if (PlayerPrefs.GetString("ThirdUser") != "")
        {
            thirdUserData = new UserData(PlayerPrefs.GetString("ThirdUser"));
        }
        else
            thirdUserData = new UserData();
    }
    
    /// <summary>
    /// Initialize the player.
    /// </summary>
    private void PlayerInit()
    {
        // Get the player health component and intiate the parameters.
        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth)
        {
            healthSlider.maxValue = playerHealth.startHealth;
            healthSlider.minValue = 0;
            healthSlider.value = playerHealth.currentHealth;
        }
        
        // Set the user name as no one if the player has not name.
        if (PlayerPrefs.GetString("Username") == "")
            PlayerPrefs.SetString("Username", "No One");
    }
    
    /// <summary>
    /// Initialize the audio.
    /// </summary>
    private void AudioInit()
    {
        // Get the audio listener.
        if (GameObject.FindGameObjectWithTag("MainCamera") != null)
            audioListener = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioListener>();
        // Set up the audio playing mode as the setting.
        if (audioListener != null)
            audioListener.enabled = (PlayerPrefs.GetInt("SoundOn") == 1);
    }
    #endregion

    void Update () {
		//更新hurtImage的颜色（线性插值）
		hurtImage.color = Color.Lerp (
			hurtImage.color, 
			Color.clear, 
			flashSpeed * Time.deltaTime
		);

		//根据游戏状态执行不同的操作
		switch (gameState) {	
		//游戏进行时
		case Enums.GameState.Playing:		
			if (playerHealth!=null&&playerHealth.isAlive == false && playerHealth.restLife <=0)			//若玩家死亡，游戏状态切换到游戏失败
				gm.gameState = Enums.GameState.GameOver;
			else if (currentScore >= TargetScore) {		//若当前得分大于等于目标分数，游戏状态切换到游戏胜利
				currentScore = TargetScore;
				gm.gameState = Enums.GameState.Winning;
			}
			//否则，当前游戏状态还是游戏进行时状态
			else {							
				scoreText.text = "Score: " + currentScore;	//显示当前游戏得分
				if(gm.playerHealth!=null)
					healthSlider.value = gm.playerHealth.currentHealth;	//根据玩家当前生命值显示玩家生命值
				currentTime = Time.time - startTime;				//根据当前时刻与场景加载时刻计算游戏场景运行的时间
				timeText.text = "Time: " + currentTime.ToString ("0.00");	//显示已用时间
				if (mobileControlRigCanvas != null)					//启用移动端控制Canvas
					mobileControlRigCanvas.SetActive (true);
			}
			if(lockCursor)
				InternalLockUpdate();
			break;
		//游戏胜利
		case Enums.GameState.Winning:
			if (!isGameOver) {
				AudioSource.PlayClipAtPoint (gameWinAudio, player.transform.position);	//播放游戏胜利音效
				Cursor.visible = true;					//将鼠标光标显示
				playingCanvas.SetActive (false);		//禁用游戏进行中Canvas
				gameResultCanvas.SetActive (true);		//启用游戏结果Canvas
				if (mobileControlRigCanvas != null)		//禁用移动端控制Canvas
					mobileControlRigCanvas.SetActive (false);
				isGameOver = true;
				EditGameOverCanvas();	//编辑游戏结束Canvas中的排行榜
			}
                ReleaseCursorLock();
			break;
		case Enums.GameState.GameOver:
			if (!isGameOver) {
				AudioSource.PlayClipAtPoint (gameOverAudio, player.transform.position);	//播放游戏失败音效
				Cursor.visible = true;					//将鼠标光标显示
				playingCanvas.SetActive (false);		//禁用游戏进行中Canvas
				gameResultCanvas.SetActive (true);		//启用游戏结果Canvas
				if (mobileControlRigCanvas != null)		//禁用移动端控制Canvas
					mobileControlRigCanvas.SetActive (false);
				isGameOver = true;
				EditGameOverCanvas();	
			}
			ReleaseCursorLock();
			break;
		}
	}

	//编辑游戏结束Canvas中的排行版
	void EditGameOverCanvas()
    {
        //根据当前玩家的姓名、得分、所用时间生成新的用户数据
        currentUserData = new UserData(PlayerPrefs.GetString("Username") + " 0 " + currentScore.ToString() + " " + currentTime.ToString("0.00"))
        {
            isUser = true
        };
        //将当前玩家以及第一至第三名玩家的信息保存在userDataArray数组里
        userDataArray[0] = currentUserData;	
		int arrayLength = 1;
		if (firstUserData.order != "0")
			userDataArray [arrayLength++] = firstUserData;
		if (secondUserData.order != "0")
			userDataArray [arrayLength++] = secondUserData;
		if (thirdUserData.order != "0")
			userDataArray [arrayLength++] = thirdUserData;

		//排序函数
		mySort (arrayLength);
		//排序完毕后重新设置用户的名词
		foreach (UserData i in userDataArray) {
			if (i.isUser == true) {
				currentUserData = i;
				break;
			}
		}
		//若玩家进入前三名，则显示相应的游戏信息
		switch (currentUserData.order) {
		case "1":
            gameMessage.text = "First place, congratulations!";
			break;
		case "2":
            gameMessage.text = "Second place, congratulations!";
			break;
		case "3":
            gameMessage.text = "Third place, congratulations!";
			break;
		default:
			gameMessage.text = "";
			break;
		}

		//将更新后的排名信息显示在排行榜上
		Text[] texts;
		if (arrayLength > 0) {
			PlayerPrefs.SetString ("FirstUser", userDataArray [0].DataToString ());
			texts = firstUserText.GetComponentsInChildren<Text> ();
			LeaderBoardChange(texts,userDataArray [0]);
			arrayLength--;
		}
		if (arrayLength > 0) {
			PlayerPrefs.SetString ("SecondUser", userDataArray [1].DataToString ());
			texts = secondUserText.GetComponentsInChildren<Text> ();
			LeaderBoardChange(texts,userDataArray [1]);
			arrayLength--;
		}
		if (arrayLength > 0) {
			PlayerPrefs.SetString ("ThirdUser", userDataArray [2].DataToString ());
			texts = thirdUserText.GetComponentsInChildren<Text> ();
			LeaderBoardChange(texts,userDataArray [2]);
			arrayLength--;
		}

		//如果玩家未进入前三名，则显示玩家信息，并将显示玩家信息的Text内容加粗
		if (currentUserData.order != "1" && currentUserData.order != "2" && currentUserData.order != "3") {
			texts = userText.GetComponentsInChildren<Text> ();
			LeaderBoardChange (texts, currentUserData);
		} else {
			userText.SetActive (false);	//若玩家进入前三名，则不显示玩家信息，直接在前三名显示当前玩家的成绩
		}

	}

	//排序函数
	void mySort(int arrayLength){
		UserData temp;
		for (int i = 0; i < arrayLength; i++) {
			for (int j = i+1; j < arrayLength; j++) {
				if (userDataArray [i] < userDataArray [j]) {
					temp = userDataArray [j];
					userDataArray [j] = userDataArray [i];
					userDataArray [i] = temp;
				}
			}
		}
		//排序后更新玩家排名
		for (int i = 0; i < arrayLength; i++)
			userDataArray [i].order = (i + 1).ToString();
	}

	//将玩家信息显示在对应的text中
	void LeaderBoardChange(Text[] texts,UserData data){
		texts [0].text = data.username;
		texts [1].text = data.score.ToString();
		texts [2].text = data.time.ToString();
		if (data.isUser) {
			texts [0].fontStyle = FontStyle.Bold;
			texts [1].fontStyle = FontStyle.Bold;
			texts [2].fontStyle = FontStyle.Bold;
		}
	}

	/// <summary>
    /// Adds the score.
    /// </summary>
    /// <param name="value">Value.</param>
	public void AddScore(int value){
		currentScore += value;
	}
	
    /// <summary>
    /// Players the take damage.
    /// </summary>
    /// <param name="value">Value.</param>
	public void PlayerTakeDamage(int value){
		if (playerHealth != null)
			playerHealth.TakeDamage(value);
		hurtImage.color = flashColor;
	}
	
    /// <summary>
    /// Players the add health.
    /// </summary>
    /// <param name="value">Value.</param>
	public void PlayerAddHealth(int value){
		if (playerHealth != null)
			playerHealth.AddHealth(value);
	}

	/// <summary>
    /// Replay the game.
    /// </summary>
	public void PlayAgain(){
		SceneManager.LoadScene("GamePlay");
	}
    
	/// <summary>
    /// Backs to main.
    /// </summary>
	public void BackToMain(){
		SceneManager.LoadScene("GameStart");
	}

	/// <summary>
    /// Update the mouse lock.
    /// </summary>
	private void InternalLockUpdate()
	{
		if(Input.GetKeyUp(KeyCode.Escape))
		{
			m_cursorIsLocked = false;
		}
		else if(Input.GetMouseButtonUp(0))
		{
			m_cursorIsLocked = true;
		}

		if (m_cursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else if (!m_cursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
	
    /// <summary>
    /// Show the mouse.
    /// </summary>
	private void ReleaseCursorLock()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
}
