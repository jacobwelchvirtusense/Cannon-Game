/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: DefaultCompany
 * Project: Apple Basket
 * Creation Date: 1/6/2023 10:21:52 AM
 * 
 * Description: Handles the state of the game between countdowns,
 *              spawning, and ending the game.
*********************************/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static InspectorValues;
using static ValidCheck;
using static SoundPlayer;
using static SettingsManager;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Threading.Tasks;
using Assets.Scripts;

[RequireComponent(typeof(AudioSource))]
public class GameController : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The scene instance of the GameController.
    /// </summary>
    public static GameController Instance;

    /// <summary>
    /// The routine that designates active gameplay.
    /// </summary>
    private Coroutine gameplayRoutine;

    /// <summary>
    /// Returns true if the gameplay is currently active.
    /// </summary>
    public static bool GameplayActive
    {
        get
        {
            if (Instance) return Instance.gameplayActive;

            return false;
        }
    }

    private bool gameplayActive = false;

    [Range(0.0f, 5.0f)]
    [Tooltip("The count down time before starting")]
    [SerializeField] private float timeBeforeEnd = 0.0f;

    #region Game Routines
    [Tooltip("The starter for singleplayer routines")]
    [SerializeField] private IGameRoutine singleplayerRoutine;

    [Tooltip("The starter for multiplayer routines")]
    [SerializeField] private IGameRoutine multiplayerRoutine;

    [Tooltip("Set to true if the game should use the lives routine")]
    [SerializeField] private bool isSingleplayer = false;

    /// <summary>
    /// The reference to the current routine being used for gameplay.
    /// </summary>
    private IGameRoutine currentGameplayroutine;
    #endregion

    [Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]

    [Tooltip("An event that is invoked whenever the gameplay is officially started")]
    public UnityEvent StartGameplay;

    [Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]

    [Tooltip("An event that is invoked whenever the gameplay is officially ended")]
    public UnityEvent EndGameplay;

    #region Score and Combos
    [Header("Score and Combos")]
    [Tooltip("The text prefab for increments to score")]
    [SerializeField] private GameObject scoreText;
    #endregion

    #region Sound
    /// <summary>
    /// The AudioSource for game state events.
    /// </summary>
    private AudioSource audioSource;

    [Header("Sound")]
    #region End Sound
    [Tooltip("The sound made when the game ends")]
    [SerializeField]
    private AudioClip endSound;

    [Range(0.0f, 1.0f)]
    [Tooltip("The volume of the end sound")]
    [SerializeField]
    private float endSoundVolume = 1.0f;
    #endregion
    #endregion

    #region Output Data
    #region Number Of Times Dodged
    private int numberOfTimesDodgedP1 = 0;

    public static int NumberOfTimesDodgedP1
    { 
        get
        {
            return Instance.numberOfTimesDodgedP1;
        }
        set
        {
            Instance.numberOfTimesDodgedP1 = value;

            UIManager.UpdateEndGameData(2, value.ToString(), 1);
        }
    }

    private int numberOfTimesDodgedP2 = 0;

    public static int NumberOfTimesDodgedP2
    {
        get
        {
            return Instance.numberOfTimesDodgedP2;
        }
        set
        {
            Instance.numberOfTimesDodgedP2 = value;

            UIManager.UpdateEndGameData(2, value.ToString(), 2);
        }
    }
    #endregion
    #endregion
    #endregion

    #region Functions
    #region Initialization
    /// <summary>
    /// Initializes components and starts the game.
    /// </summary>
    private void Awake()
    {
        Instance = this;

        singleplayerRoutine.Initialize();
        multiplayerRoutine.Initialize();

        Slot1OnValueChanged.AddListener(UpdateNumberOfUsers);

        InitializeComponents();
    }

    /// <summary>
    /// Performs calls on components to initialize them.
    /// </summary>
    private void Start()
    {
        if (BodySourceManager.NumberOfUsersToTrack == 1)
        {
            currentGameplayroutine = singleplayerRoutine;

            singleplayerRoutine.SetActive(true);
            multiplayerRoutine.SetActive(false);
        }
        else
        {
            currentGameplayroutine = multiplayerRoutine;

            multiplayerRoutine.SetActive(true);
            singleplayerRoutine.SetActive(false);
        }
    }

    /// <summary>
    /// Initializes any components needed.
    /// </summary>
    private void InitializeComponents()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void UpdateNumberOfUsers(int newNumberOfUsers)
    {
        switch (newNumberOfUsers)
        {
            case 0:
                currentGameplayroutine = singleplayerRoutine;
                singleplayerRoutine.SetActive(true);
                multiplayerRoutine.SetActive(false);
                break;
            default:
                currentGameplayroutine = multiplayerRoutine;
                singleplayerRoutine.SetActive(false);
                multiplayerRoutine.SetActive(true);
                break;
        }
    }
    #endregion

    #region Testing Editor Keys
    /* Editor Testing Keys
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.E))
        {
            UpdateSceneScore(100, Vector2.zero);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            UpdateSceneScore(-100, Vector2.zero);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            IncreaseCombo();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ResetCombo();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            LivesLeft--;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            LivesLeft++;
        }
#endif
    }*/
    #endregion

    #region Updating Score
    /// <summary>
    /// Updates the score the player has.
    /// </summary>
    /// <param name="increment">Increments the player's score by this amount.</param>
    public static void UpdateScore(int increment, Vector2 location)
    {
        Instance.UpdateSceneScore(increment, location);
    }

    /// <summary>
    /// Updates the non-static score for the game.
    /// </summary>
    /// <param name="increment">Increments the player's score by this amount.</param>
    public void UpdateSceneScore(int increment, Vector2 location)
    {
        var actualIncrement = ComboModifier(increment);
        //CurrentScore += actualIncrement;

        // Spawns text at score location
        var text = Instantiate(scoreText, location, Quaternion.identity);
        text.GetComponent<ScoreIncrementText>().InitializeScore(actualIncrement);

        DisplayGameData();
    }

    /// <summary>
    /// Applies the combo modifier to the score increment.
    /// </summary>
    /// <param name="increment">The score increment.</param>
    /// <returns></returns>
    private int ComboModifier(int increment)
    {
        var amount = increment;

        //if(increment > 0) amount += (int)((currentCombo * comboModifier) * increment);

        return amount;
    }
    #endregion

    #region Countdown
    /// <summary>
    /// Starts the countdown for the game to begin.
    /// </summary>
    public void StartGameCountdown()
    {
        StartCoroutine(CountdownLoop());
    }

    /// <summary>
    /// Counts down before starting the game again.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CountdownLoop()
    {
        yield return gameplayRoutine = StartCoroutine(Countdown.CountdownLoop());

        StartGame();
    }

    /// <summary>
    /// Begins the game (spawning of apples and game timer).
    /// </summary>
    private void StartGame()
    {
        gameplayRoutine = StartCoroutine(GameplayRoutine());

        StartGameplay.Invoke();
    }

    /// <summary>
    /// Handles the routine of gameplay and the end game routine.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameplayRoutine()
    {
        gameplayActive = true;

        yield return currentGameplayroutine.GameplayRoutine();

        yield return EndGame();

        gameplayActive = false;
        gameplayRoutine = null;
    }
    #endregion

    public static void UpdateLives(int playerNumber, int modifier)
    {
        Instance.currentGameplayroutine.UpdateLives(playerNumber, modifier);
    }

    #region End Game/Output Data
    /// <summary>
    /// Displays end message and outputs the game data.
    /// </summary>
    private IEnumerator EndGame()
    {
        EndGameplay.Invoke();

        // Waits for game to fully complete
        if(timeBeforeEnd != 0)
        yield return new WaitForSeconds(timeBeforeEnd);

        // Displays all data
        UIManager.DisplayEndScreen();
        DisplayGameData();
        OutputData();
        PlaySound(audioSource, endSound, endSoundVolume);
    }

    /// <summary>
    /// Displays all of the game data in the end game displays.
    /// </summary>
    private void DisplayGameData()
    {
        //UIManager.UpdateEndGameData(2, "Temp Text");
        //UIManager.UpdateEndGameData(3, "Temp Text");
        //UIManager.UpdateEndGameData(4, "Temp Text");
        //UIManager.UpdateEndGameData(5, "Temp Text");
    }

    /// <summary>
    /// Outputs the data from the players session.
    /// </summary>
    private void OutputData()
    {
        // TODO Add all of the data to be sent
        PipeDataToBalance.SendEndGameData();
    }

    public void QuitGameEarly()
    {
        PipeDataToBalance.SendSkippedXml();
        ExitGame();
    }

    /// <summary>
    /// Exits the game both in editor and in builds.
    /// </summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        BodySourceManager.CloseSensor();

        Application.Quit();
    }
    #endregion

    /// <summary>
    /// Calls for the scene to be loaded effectively reloading the game.
    /// </summary>
    public async void PlayAgain()
    {
        BodySourceManager.CloseSensor();

        if(BodySourceManager.IsAzureKinnect) await Task.Delay(1000); // Waits 1 second to ensure that the sensor gets uninitialized

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion
}
