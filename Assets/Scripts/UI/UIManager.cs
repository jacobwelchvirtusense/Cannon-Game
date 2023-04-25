/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 1/6/2023 10:25:04 AM
 * 
 * Description: Handles the functionality of all
 *              UI assets.
*********************************/
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ValidCheck;

public class UIManager : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The instance of the UI manager in the scene.
    /// </summary>
    private static UIManager Instance;

    #region Lives Mode
    [Header("Lives Mode")]
    [Tooltip("The UI parent for the lives based mode")]
    [SerializeField] private GameObject livesUI;

    [Tooltip("The array of lives to be used in the lives based mode")]
    [SerializeField] private GameObject[] hearts;

    [Tooltip("The current number of lives player 1 has")]
    [SerializeField] private TextMeshProUGUI livesTextPlayer1;

    [Tooltip("The current number of lives player 2 has")]
    [SerializeField] private TextMeshProUGUI livesTextPlayer2;
    #endregion

    #region Timer
    [Header("Timer Mode")]
    [Tooltip("The current timer that the user has left")]
    [SerializeField] private TextMeshProUGUI timerUI;

    [Tooltip("The left image for the timer bar")]
    [SerializeField] private Image timerBar1;

    [Tooltip("The right image for the timer bar")]
    [SerializeField] private Image timerBar2;
    #endregion
    
    [Header("All modes")]
    [Tooltip("The score the user currently has")]
    [SerializeField] private TextMeshProUGUI score;

    [Tooltip("The end screen for the game")]
    [SerializeField] private GameObject endScreenSinglePlayer;

    [Tooltip("The end screen for the game")]
    [SerializeField] private GameObject endScreenMultiPlayer;

    #region End Game Data Displays
    [Tooltip("The end game display for the first slot")]
    [SerializeField] private EndGameDataDisplay[] endGameDisplay1;

    [Tooltip("The end game display for the second slot")]
    [SerializeField] private EndGameDataDisplay[] endGameDisplay2;

    [Tooltip("The end game display for the third slot")]
    [SerializeField] private EndGameDataDisplay[] endGameDisplay3;

    [Tooltip("The end game display for the fourth slot")]
    [SerializeField] private EndGameDataDisplay[] endGameDisplay4;

    [Tooltip("The end game display for the fifth slot")]
    [SerializeField] private EndGameDataDisplay[] endGameDisplay5;

    private int numberOfTimesHitP1 = 0;
    private int numberOfTimesHitP2 = 0;

    private int numberOfTimesLandedP1 = 0;
    private int numberOfTimesLandedP2 = 0;
    #endregion
    #endregion

    #region Functions
    /// <summary>
    /// Initializes all aspects of the UI manager.
    /// </summary>
    private void Awake()
    {
        Instance = this;

        for(int i = 1; i <= 5; i++)
        {
            for(int j = 1; j <= 2; j++)
            {
                UpdateEndGameData(i, 0.ToString(), j);
            }
        }
    }

    #region UI Updates
    /// <summary>
    /// Updates the dipslay of the current score.
    /// </summary>
    /// <param name="newScore">The current score the player has.</param>
    public static void UpdateScore(int newScore)
    {
        if (InstanceDoesntExist() || IsntValid(Instance.score)) return;

        // Updates the score UI 
        Instance.score.text = newScore.ToString();
        UpdateScoreEndGame(newScore);
    }

    /// <summary>
    /// Updates the displayed score for at the end of the game.
    /// </summary>
    /// <param name="newScore">The new score the player has.</param>
    private static void UpdateScoreEndGame(int newScore)
    {
        if (IsntValid(Instance.endGameDisplay1)) return;
        //Instance.endGameDisplay1.UpdateText(newScore.ToString());
    }

    /// <summary>
    /// Updates the displayed number of lives.
    /// </summary>
    /// <param name="newLives">The number of lives the user currently has.</param>
    public static void UpdateLivesLeft(int newLives, int playerNumber = 0)
    {
        if(playerNumber == 2)
        {
            Instance.livesTextPlayer2.text = "x" + newLives.ToString();

            if (!GameController.GameplayActive) return;

            UpdateEndGameData(3, (++Instance.numberOfTimesHitP2).ToString(), 2); // Updates the amount of times hit
            UpdateEndGameData(5, (++Instance.numberOfTimesLandedP1).ToString(), 1); // Updates the amount of times you hit the other player
        }
        else
        {
            Instance.livesTextPlayer1.text = "x" + newLives.ToString();

            if (!GameController.GameplayActive) return;

            UpdateEndGameData(3, (++Instance.numberOfTimesHitP1).ToString(), 1); // Updates the amount of times hit
            UpdateEndGameData(5, (++Instance.numberOfTimesLandedP2).ToString(), 2); // Updates the amount of times you hit the other player
        }
    }

    #region Timer
    /// <summary>
    /// Sets the timers initial value.
    /// </summary>
    public static void InitializeTimer(int timer = 0)
    {
        UpdateTimer(timer);
    }

    /// <summary>
    /// Updates the timer to its current time.
    /// </summary>
    /// <param name="newTime">The current time left of the timer.</param>
    public static void UpdateTimer(float newTime)
    {
        if (InstanceDoesntExist() || IsntValid(Instance.timerUI)) return;

        // Updates the timer UI 
        Instance.timerUI.text = GetTimerValue(newTime);
    }

    /// <summary>
    /// Gets the string for displaying how much time is left.
    /// </summary>
    /// <param name="newTime">The current amount of time left</param>
    /// <returns></returns>
    public static string GetTimerValue(float newTime)
    {
        var seconds = (int)newTime;
        var minutes = seconds / 60;
        var leftOverSeconds = (seconds - (minutes * 60));
        string secondsDisplayed = "";

        if (leftOverSeconds < 10) secondsDisplayed += "0";
        secondsDisplayed += leftOverSeconds;

        UpdateEndGameData(4, minutes.ToString() + ":" + secondsDisplayed);

        return minutes.ToString() + ":" + secondsDisplayed;
    }

    /*
    /// <summary>
    /// Updates the timer bars to be a percentage of the remaining time left.
    /// </summary>
    /// <param name="newTime">The current amount of time left.</param>
    private static void UpdateTimerBars(float newTime)
    {
        if (IsntValid(Instance.timerBar1) || IsntValid(Instance.timerBar2)) return;

        var isNotInfinite = newTime != -1;
        Instance.timerBar1.gameObject.SetActive(isNotInfinite);
        Instance.timerBar2.gameObject.SetActive(isNotInfinite);

        Instance.timerBar1.fillAmount = newTime / GetTimerAmount();
        Instance.timerBar2.fillAmount = newTime / GetTimerAmount();
    }*/
    #endregion

    /// <summary>
    /// Displays the message that should appear at the end of the game.
    /// </summary>
    public static void DisplayEndScreen()
    {
        if (InstanceDoesntExist() || IsntValid(Instance.endScreenSinglePlayer) || IsntValid(Instance.endScreenMultiPlayer)) return;

        if(BodySourceManager.NumberOfUsersToTrack == 1)
        {
            Instance.endScreenSinglePlayer.SetActive(true);
        }
        else
        {
            Instance.endScreenMultiPlayer.SetActive(true);
        }
    }
    #endregion

    /// <summary>
    /// Updates end game displays to have certain data.
    /// 1 is the amount of movement
    /// 2 is the amount dodged
    /// 3 is the amount of time hit
    /// 4 is the amount of time played for
    /// 5 is the amount of hits on the other player
    /// </summary>
    /// <param name="indexToUpdate">The index between 1-5 to update for end game data.</param>
    /// <param name="newData">The new data to put in the end game display.</param>
    public static void UpdateEndGameData(int indexToUpdate, string newData, int playerNumber = 1)
    {
        if (BodySourceManager.NumberOfUsersToTrack == 1 && playerNumber == 2) return;

        switch (indexToUpdate)
        {
            case 1:
                Instance.endGameDisplay1[playerNumber-1].UpdateText(newData);
                break;
            case 2:
                Instance.endGameDisplay2[playerNumber - 1].UpdateText(newData);
                break;
            case 3:
                Instance.endGameDisplay3[playerNumber - 1].UpdateText(newData);
                break;
            case 4:
                Instance.endGameDisplay4[playerNumber - 1].UpdateText(newData);
                break;
            case 5:
                Instance.endGameDisplay5[playerNumber - 1].UpdateText(newData);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Returns true if the Instance of the UIManager does not exist.
    /// </summary>
    /// <returns></returns>
    private static bool InstanceDoesntExist()
    {
        return IsntValid(Instance);
    }
    #endregion
}
