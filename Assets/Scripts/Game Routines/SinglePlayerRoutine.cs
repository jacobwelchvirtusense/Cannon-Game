/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Cannon Fodder
 * Creation Date: 4/14/2023 4:47:05 PM
 * 
 * Description: TODO
******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InspectorValues;
using static ValidCheck;

public class SinglePlayerRoutine : IGameRoutine
{
    #region Fields
    /// <summary>
    /// The current setting slot of lives to be used.
    /// </summary>
    private int currentSlot = 0;

    /// <summary>
    /// The scene instance of the LiveGameRoutine.
    /// </summary>
    private static SinglePlayerRoutine Instance;

    /// <summary>
    /// The amount of time the user has played for.
    /// </summary>
    private float timePlayedFor = 0.0f;
    #endregion

    #region Functions
    /// <summary>
    /// Initializes the hooks for this object.
    /// </summary>
    public override void Initialize()
    {
        Instance = this;
        SettingsManager.Slot2OnValueChanged.AddListener(UpdateToNewLifeSlot);
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
        var npc = FindObjectOfType<NPCShipMovement>();
        npc.StartNPC();

        while (livesLeft != 0)
        {
            yield return new WaitForEndOfFrame();
            timePlayedFor += Time.deltaTime;
            UIManager.UpdateTimer(timePlayedFor);
        }

        npc.StopNPC();
    }

    /// <summary>
    /// Returns the current starting lives that is being used in the game.
    /// </summary>
    /// <returns></returns>
    public static int GetStartingLifeAmount()
    {
        if (Instance == null || Instance.numberOfLives.Length <= Instance.currentSlot) return 0;

        return Instance.numberOfLives[Instance.currentSlot];
    }

    public override void SetActive(bool shouldBeActive)
    {
        base.SetActive(shouldBeActive);
        UIManager.InitializeTimer(0);
    }
    #endregion
}
