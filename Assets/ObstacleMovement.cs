/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Cannon Fodder
 * Creation Date: 4/18/2023 4:01:16 PM
 * 
 * Description: TODO
******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InspectorValues;
using static ValidCheck;

public class ObstacleMovement : MonoBehaviour
{
    #region Fields
    [Tooltip("The y position below the screen to have obstacles returned to the pool")]
    [SerializeField] private float lowestYPosition = -8.0f;

    [Tooltip("The speed at which the obstacle moves downward")]
    [SerializeField] private float moveSpeed = -2.0f;
    #endregion

    #region Functions
    // Update is called once per frame
    private void FixedUpdate()
    {
        MoveDownward();
        CheckLowPosition();
    }

    private void MoveDownward()
    {

    }

    private void CheckLowPosition()
    {

    }
    #endregion
}
