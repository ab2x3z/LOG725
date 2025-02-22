using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using KartGame.KartSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using KartGame.Track;

public enum GameState{Play, Won, Lost}

public class GameFlowManager : MonoBehaviour
{
    const int MAX_CAR_AVAILABLE = 20;

    [Header("Parameters")]
    [Tooltip("Duration of the fade-to-black at the end of the game")]
    public float endSceneLoadDelay = 3f;
    [Tooltip("The canvas group of the fade-to-black screen")]
    public CanvasGroup endGameFadeCanvasGroup;

    [Header("Win")]
    [Tooltip("This string has to be the name of the scene you want to load when winning")]
    public string winSceneName = "WinScene";
    [Tooltip("Duration of delay before the fade-to-black, if winning")]
    public float delayBeforeFadeToBlack = 4f;
    [Tooltip("Duration of delay before the win message")]
    public float delayBeforeWinMessage = 2f;
    [Tooltip("Sound played on win")]
    public AudioClip victorySound;

    [Tooltip("Prefab for the win game message")]
    public DisplayMessage winDisplayMessage;

    public PlayableDirector raceCountdownTrigger;

    [Header("Lose")]
    [Tooltip("This string has to be the name of the scene you want to load when losing")]
    public string loseSceneName = "LoseScene";
    [Tooltip("Prefab for the lose game message")]
    public DisplayMessage loseDisplayMessage;


    [SerializeField] Toggle audioToggle;

    public GameState gameState { get; private set; }

    public bool autoFindKarts = true;
    
    public CarManager carManager;
    private  ArcadeKart playerKart;

    public PositionManager positionManager;
    

    ArcadeKart[] karts;
    ObjectiveManager m_ObjectiveManager;
    TimeDisplay m_TimeDisplay;
    float m_TimeLoadEndGameScene;
    string m_SceneToLoad;
    float elapsedTimeBeforeEndScene = 0;

    void Start()
    {
        if (carManager == null)
        {
            carManager = FindObjectOfType<CarManager>();
        }

        if (playerKart == null)
        {
            playerKart = carManager.GetKart();    
        }
        
        
        if (autoFindKarts)
        {
            karts = FindObjectsOfType<ArcadeKart>();
            if (karts.Length > 0)
            {
                if (!playerKart) playerKart = karts[0];
            }
            DebugUtility.HandleErrorIfNullFindObject<ArcadeKart, GameFlowManager>(playerKart, this);
        }

        m_ObjectiveManager = FindObjectOfType<ObjectiveManager>();
		DebugUtility.HandleErrorIfNullFindObject<ObjectiveManager, GameFlowManager>(m_ObjectiveManager, this);

        m_TimeDisplay = FindObjectOfType<TimeDisplay>();
        DebugUtility.HandleErrorIfNullFindObject<TimeDisplay, GameFlowManager>(m_TimeDisplay, this);

        AudioUtility.SetMasterVolume(1);

        winDisplayMessage.gameObject.SetActive(false);
        loseDisplayMessage.gameObject.SetActive(false);

        foreach (ArcadeKart k in karts)
        {
			k.SetCanMove(false);
        }

        if (positionManager == null)
        {
            positionManager = FindObjectOfType<PositionManager>();
        }

        //run race countdown animation
        ShowRaceCountdownAnimation();
        StartCoroutine(ShowObjectivesRoutine());

        StartCoroutine(CountdownThenStartRaceRoutine());
    }

    IEnumerator CountdownThenStartRaceRoutine() {
        yield return new WaitForSeconds(3f);
        StartRace();
    }

    void StartRace() {
        foreach (ArcadeKart k in karts)
        {
			k.SetCanMove(true);
        }
    }

    void ShowRaceCountdownAnimation() {
        raceCountdownTrigger.Play();
    }

    IEnumerator ShowObjectivesRoutine() {
        while (m_ObjectiveManager.Objectives.Count == 0)
            yield return null;
        yield return new WaitForSecondsRealtime(0.2f);
        for (int i = 0; i < m_ObjectiveManager.Objectives.Count; i++)
        {
           if (m_ObjectiveManager.Objectives[i].displayMessage)m_ObjectiveManager.Objectives[i].displayMessage.Display();
           yield return new WaitForSecondsRealtime(1f);
        }
    }


    void Update()
    {

        if (gameState != GameState.Play)
        {
            elapsedTimeBeforeEndScene += Time.deltaTime;
            if(elapsedTimeBeforeEndScene >= endSceneLoadDelay)
            {

                float timeRatio = 1 - (m_TimeLoadEndGameScene - Time.time) / endSceneLoadDelay;
                endGameFadeCanvasGroup.alpha = timeRatio;

                float volumeRatio = Mathf.Abs(timeRatio);
                float volume = Mathf.Clamp(1 - volumeRatio, 0, 1);
                AudioUtility.SetMasterVolume(volume);

                // See if it's time to load the end scene (after the delay)
                if (Time.time >= m_TimeLoadEndGameScene)
                {
                    SceneManager.LoadScene(m_SceneToLoad);
                    gameState = GameState.Play;
                }
            }
        }
        else
        {
            if (m_ObjectiveManager.AreAllObjectivesCompleted() && positionManager.IsPlayerFirst())
                EndGame(true);

            if (m_ObjectiveManager.AreAllObjectivesCompleted() && !positionManager.IsPlayerFirst())
                EndGame(false);
        }
    }

    void EndGame(bool win)
    {
        positionManager.UpdateLeaderboardToggle();
        positionManager.StoreLeaderboard();

        // unlocks the cursor before leaving the scene, to be able to click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Save best position if it's a new record
        int playerPosition = positionManager.GetPlayerPosition();
        if (PlayerPrefs.GetInt("BestPosition_" + SceneManager.GetActiveScene().name, int.MaxValue) > playerPosition)  { 
            PlayerPrefs.SetInt("BestPosition_" + SceneManager.GetActiveScene().name, playerPosition);
        }
        // Save best time if it's a new record
        if (PlayerPrefs.GetFloat("BestTime_" + SceneManager.GetActiveScene().name, float.MaxValue) > m_TimeDisplay.GetRaceLapTime())
        {
            PlayerPrefs.SetFloat("BestTime_" + SceneManager.GetActiveScene().name, m_TimeDisplay.GetRaceLapTime());
            PlayerPrefs.SetString("BestTimePlayer_" + SceneManager.GetActiveScene().name, playerKart.gameObject.name); //TODO
            PlayerPrefs.SetString("BestTimeReadable_" + SceneManager.GetActiveScene().name, m_TimeDisplay.GetRaceLapTime_Readable());
        }

        if (win && PlayerPrefs.GetInt("UnlockedCars", 5) < MAX_CAR_AVAILABLE)
        {
            PlayerPrefs.SetInt("UnlockedCars", PlayerPrefs.GetInt("UnlockedCars", 5) + 1);
        }

        // Remember that we need to load the appropriate end scene after a delay
        gameState = win ? GameState.Won : GameState.Lost;
        endGameFadeCanvasGroup.gameObject.SetActive(true);
        if (win)
        {
            m_SceneToLoad = winSceneName;
            m_TimeLoadEndGameScene = Time.time + endSceneLoadDelay + delayBeforeFadeToBlack;

            // play a sound on win
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = victorySound;
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.HUDVictory);
            audioSource.PlayScheduled(AudioSettings.dspTime + delayBeforeWinMessage);

            // create a game message
            winDisplayMessage.delayBeforeShowing = delayBeforeWinMessage;
            winDisplayMessage.gameObject.SetActive(true);
        }
        else
        {
            m_SceneToLoad = loseSceneName;
            m_TimeLoadEndGameScene = Time.time + endSceneLoadDelay + delayBeforeFadeToBlack;

            // create a game message
            loseDisplayMessage.delayBeforeShowing = delayBeforeWinMessage;
            loseDisplayMessage.gameObject.SetActive(true);
        }
    }
}
