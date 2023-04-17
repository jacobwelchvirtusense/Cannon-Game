/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Cannon Fodder
 * Creation Date: 4/14/2023 4:06:46 PM
 * 
 * Description: Handles movement and shooting for NPCs.
******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InspectorValues;
using static ValidCheck;

public class NPCShipMovement : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The routine that handles the NPCs actions.
    /// </summary>
    private Coroutine NPCRoutine;

    [Tooltip("The max position that the NPC can go to")]
    [SerializeField] private float maxYPosition = 6;

    [Range(0.0f, 50.0f)]
    [Tooltip("The base speed that the ship will move at")]
    [SerializeField] private float moveSpeed = 1;

    private ShootCannon shootCannon;

    [Tooltip("The transform that this ship targets")]
    [SerializeField] private Transform targetPlayer;

    private Vector2 targetPosition;

    [Header("Dynamic Difficulty")]
    [Range(0.0f, 1.0f)]
    [Tooltip("The rate at which ship move speed increases over time (increases over a second)")]
    [SerializeField] private float moveSpeedIncreaseRate = 1;

    private float currentMoveSpeedMod = 1;

    [Range(0.0f, 1.0f)]
    [Tooltip("The rate at which the cannon firing increases over time (increases over a second)")]
    [SerializeField] private float cannonShootSpeedIncreaseRate = 1;

    private float currentShootSpeedMod = 0;

    [Range(0.0f, 1.0f)]
    [Tooltip("The rate at which the cannon balls movement speed increases (increases over a second)")]
    [SerializeField] private float cannonBallSpeedIncreaseRate = 1;

    private float currentCannonballMoveSpeedMod = 1;
    #endregion

    #region Functions
    // Start is called before the first frame update
    private void Awake()
    {
        shootCannon = GetComponentInChildren<ShootCannon>();
    }

    public void StartNPC()
    {
        if(NPCRoutine == null)
        NPCRoutine = StartCoroutine(MovementShootingRoutine());
    }

    public void StopNPC()
    {
        if(NPCRoutine != null)
        {
            StopCoroutine(NPCRoutine);
            NPCRoutine = null;
        }
    }

    private IEnumerator MovementShootingRoutine()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            IncrementGameSpeedModifiers();
            MoveNPC();
            shootCannon.ShootCannonball(currentCannonballMoveSpeedMod, currentShootSpeedMod);
        }
    }

    private void IncrementGameSpeedModifiers()
    {
        currentCannonballMoveSpeedMod += cannonBallSpeedIncreaseRate * Time.fixedDeltaTime;
        currentShootSpeedMod += cannonShootSpeedIncreaseRate * Time.fixedDeltaTime;
        currentMoveSpeedMod += moveSpeedIncreaseRate * Time.fixedDeltaTime;
    }

    private void MoveNPC()
    {
        targetPosition = targetPlayer.position;
        targetPosition.x = transform.position.x;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime*moveSpeed*currentMoveSpeedMod);
    }
    #endregion
}
