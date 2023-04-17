/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Cannon Fodder
 * Creation Date: 4/14/2023 4:47:19 PM
 * 
 * Description: TODO
******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InspectorValues;
using static ValidCheck;

public class MultiplayerRoutine : IGameRoutine
{
    #region Fields
    /// <summary>
    /// The current setting slot of lives to be used.
    /// </summary>
    private int currentSlot = 0;

    private static MultiplayerRoutine Instance;

    /// <summary>
    /// The amount of time the user has played for.
    /// </summary>
    private float timePlayedFor = 0.0f;

    /// <summary>
    /// The current number of lives left for the user.
    /// </summary>
    protected int livesLeft2 = 3;

    /// <summary>
    /// A getter and setter for the number of lives left.
    /// </summary>
    public int LivesLeftPlayer2
    {
        get => livesLeft;
        set
        {
            livesLeft2 = value;
            UIManager.UpdateLivesLeft(livesLeft2, 2);
        }
    }
    #endregion

    #region Functions
    /// <summary>
    /// Initializes the hooks for this object.
    /// </summary>
    public override void Initialize()
    {
        Instance = this;
        SettingsManager.Slot2Update.AddListener(UpdateToNewLifeSlot);
    }

    /// <summary>
    /// Updates the current life slot within the array of life options.
    /// </summary>
    /// <param name="newLifeSlot">The new slot of the life array to use.</param>
    private void UpdateToNewLifeSlot(int newLifeSlot)
    {
        currentSlot = newLifeSlot;

        if (numberOfLives.Length > currentSlot)
        {
            LivesLeftPlayer1 = numberOfLives[currentSlot];
        }
    }

    public override IEnumerator GameplayRoutine()
    {
        while (livesLeft != 0 && livesLeft2 != 0)
        {
            yield return new WaitForEndOfFrame();
            timePlayedFor += Time.deltaTime;
            UIManager.UpdateTimer(timePlayedFor);
        }
    }

    public override void SetActive(bool shouldBeActive)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
