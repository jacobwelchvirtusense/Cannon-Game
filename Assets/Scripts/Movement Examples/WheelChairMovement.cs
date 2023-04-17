/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 3/20/2023 10:44:36 AM
 * 
 * Description: The start of a playermovement script for wheel chair 
 *              users.
******************************************************************/
using Assets.SensorAdapters;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static InspectorValues;
using static UnityEditor.PlayerSettings;
using static ValidCheck;

public class WheelChairMovement : SensorDataListener
{
    #region Fields
    [Header("Sensor Parameters")]
    [Tooltip("The x position the user can be relative to the sensor")]
    [SerializeField] private float maxUserXPos = 5.0f;

    [Tooltip("The minimum z position the user can be relative to the sensor")]
    [SerializeField] private float minUserZPos = 0.5f;

    [Tooltip("The maximum z position the user can be relative to the sensor")]
    [SerializeField] private float maxUserZPos = 5.0f;

    [Header("In-Game Character Parameters")]
    [Tooltip("The maximum x position the in-game character will be based on user XPos")]
    [SerializeField] private float maxCharacterXPos = 10.0f;

    [Tooltip("The maximum Y position the in-game character will be based on user ZPos")]
    [SerializeField] private float[] maxCharacterYPos = new float[] { 3.5f, 5.0f, 7.0f };

    /// <summary>
    /// The index to be used for the character's min max y position.
    /// </summary>
    private int currentDifficultyIndex = 0;

    [Tooltip("The amount of smoothing to be applied to the movement to reduce jittering")]
    [SerializeField] private float movementSmoothing = 10;

    [Header("Extra movement settings")]
    [Tooltip("Clamps the users movement to be solely on the x axis")]
    [SerializeField] private bool clampToX;

    [Tooltip("Clamps the users movement to be solely on the y axis")]
    [SerializeField] private bool clampToY;

    [Tooltip("Inverts the movement inputs of the user")]
    [SerializeField] private bool invertInput;

    [Header("Animation")]
    [Tooltip("The minimum amount of delta movement before showing an animtion")]
    [SerializeField] private float minimumMovementDelta = 0.25f;

    [Tooltip("The max amount of frames to check for movement deltas for animation")]
    [SerializeField] private int maxMovementQueueSize = 10;

    /// <summary>
    /// The position that this object started at.
    /// </summary>
    private Vector3 startingPosition;

    /// <summary>
    /// The time the last frame from the sensor was recieved.
    /// </summary>
    private float timeOfLastDataFrame = 0;

    /// <summary>
    /// The joint to be used in the calculations. Somewhere on the head is likely best for wheel chair users.
    /// </summary>
    private JointType headJoint = JointType.SpineMid;

    /// <summary>
    /// The animator of the player's character.
    /// </summary>
    private Animator characterAnimator;

    /// <summary>
    /// A queue of previous y positions that can be used for calculating if the user is moving.
    /// </summary>
    private Queue<float> yPositionQueue = new Queue<float>();
    #endregion

    #region Functions
    protected override void Awake()
    {
        base.Awake();

        startingPosition = transform.position;
        characterAnimator = GetComponent<Animator>();
    }

    public void InitializeSettingsHook()
    {
        SettingsManager.Slot3Update.AddListener(UpdateCurrentDifficultyIndex);
    }

    private void UpdateCurrentDifficultyIndex(int newIndex)
    {
        currentDifficultyIndex = newIndex;
    }

    /// <summary>
    /// Uses the users data to move their in-game character.
    /// </summary>
    /// <param name="skeleton">The skeleton of the user.</param>
    protected override void UseUserData(Skeleton skeleton)
    {
        #region Taking input and calculating target position
        var xInput = skeleton.joints[(int)headJoint].position.x;
        var zInput = skeleton.joints[(int)headJoint].position.z;

        var targetPositionLerpX = Mathf.InverseLerp(-maxUserXPos, maxUserXPos, xInput); // Calculates the lerp of the angle
        var targetPositionLerpZ = Mathf.InverseLerp(maxUserZPos, minUserZPos, zInput); // Calculates the lerp of the angle


        if (invertInput)
        {
            var holdXInput = targetPositionLerpX;
            targetPositionLerpX = targetPositionLerpZ;
            targetPositionLerpZ = holdXInput;
        }

        var targetX = Mathf.Lerp(-maxCharacterXPos, maxCharacterXPos, targetPositionLerpX) + startingPosition.x;
        var targetY = Mathf.Lerp(-maxCharacterYPos[currentDifficultyIndex], maxCharacterYPos[currentDifficultyIndex], targetPositionLerpZ) + startingPosition.y;
        #endregion

        #region Clamping Inputs
        if (clampToX)
        {
            targetY = transform.position.y;
        }

        if (clampToY)
        {
            targetX = transform.position.x;
        }
        #endregion

        #region Moving In-Game Character
        var pos = transform.position;
        var timeDelta = TimeSinceLastDataFrame();
        pos.x = Mathf.Lerp(pos.x, targetX, timeDelta * movementSmoothing);
        pos.y = Mathf.Lerp(pos.y, targetY, timeDelta * movementSmoothing);
        transform.position = pos;

        timeOfLastDataFrame = Time.time;
        #endregion

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if(yPositionQueue.Count == maxMovementQueueSize)
        {
            var lastElement = yPositionQueue.Dequeue();
            var delta = 0.0f;

            foreach(var item in yPositionQueue)
            {
                delta += lastElement - item;
                lastElement = item;
            }

            if(delta > minimumMovementDelta)
            {
                characterAnimator.SetBool("MoveForward", true);
            }
            else if(delta < -minimumMovementDelta)
            {
                characterAnimator.SetBool("MoveBackward", true);
            }
            else
            {
                characterAnimator.SetBool("MoveForward", false);
                characterAnimator.SetBool("MoveBackward", false);
            }
        }

        yPositionQueue.Enqueue(transform.position.y);
    }

    private float TimeSinceLastDataFrame()
    {
        return Time.time - timeOfLastDataFrame;
    }
    #endregion
}
