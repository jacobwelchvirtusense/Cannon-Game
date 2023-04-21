/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 4/14/2023 2:44:20 PM
 * 
 * Description: An abstract functionality for all routines that 
 *              could be had for gameplay.
******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InspectorValues;
using static ValidCheck;

public abstract class IGameRoutine : MonoBehaviour
{
    #region Fields
    [Tooltip("The minimum value for the timer in seconds")]
    [SerializeField] protected int[] numberOfLives = new int[] { 3, 5, 7 };

    /// <summary>
    /// The current number of lives left for the user.
    /// </summary>
    protected int livesLeft = 3;

    /// <summary>
    /// A getter and setter for the number of lives left.
    /// </summary>
    public int LivesLeftPlayer1
    {
        get => livesLeft;
        set
        {
            livesLeft = value;
            livesLeft = Mathf.Clamp(livesLeft, 0, livesLeft);
            UIManager.UpdateLivesLeft(livesLeft);
        }
    }

    [SerializeField] protected GameObject[] objectsToEnable;

    [SerializeField] protected GameObject[] objectsToDisable;
    #endregion

    #region Functions
    /// <summary>
    /// Sets this routine as the active routine and initializes it.
    /// </summary>
    public virtual void SetActive(bool shouldBeActive)
    {
        foreach(var obj in objectsToEnable)
        {
            obj.SetActive(shouldBeActive);
        }

        foreach (var obj in objectsToDisable)
        {
            obj.SetActive(!shouldBeActive);
        }
    }

    public virtual void Initialize()
    {

    }

    public virtual void UpdateLives(int playerNumber, int modifier)
    {
        LivesLeftPlayer1 += modifier;
    }

    /// <summary>
    /// The routine of gameplay to be handled by this class.
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerator GameplayRoutine();
    #endregion
}
