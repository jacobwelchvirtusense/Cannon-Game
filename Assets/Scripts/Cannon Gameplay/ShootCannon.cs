/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Cannon Fodder
 * Creation Date: 4/14/2023 4:06:56 PM
 * 
 * Description: Handles the shooting of cannon balls.
******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InspectorValues;
using static ValidCheck;

public class ShootCannon : MonoBehaviour
{
    #region Fields
    [Tooltip("The cannon ball that will be fired by this character")]
    [SerializeField] private GameObject cannonBall;

    [Range(0.0f, 5.0f)]
    [Tooltip("The base amount of time between shots fired by the ship")]
    [SerializeField] private float timeBetweenShots = 0.5f;

    [Range(0.0f, 50.0f)]
    [Tooltip("The base speed that the cannon ball will move at")]
    [SerializeField] private float cannonballSpeed = 1;

    [Tooltip("The tags that define which objects this cannon ball should collide with")]
    [SerializeField] private string collisionTag = "Player";

    /// <summary>
    /// The animator of the cannon.
    /// </summary>
    private Animator cannonAnimator;

    private const float distToSpawnFromCannonCenter = 0.75f;

    /// <summary>
    /// The time this ship fired it's last shot.
    /// </summary>
    private float timeOfLastShot = -Mathf.Infinity;
    #endregion

    #region Functions
    /// <summary>
    /// Initializes components.
    /// </summary>
    private void Awake()
    {
        cannonAnimator = GetComponent<Animator>();
    }

    /// <summary>
    /// Shoots the cannon ball in the current look rotation.
    /// </summary>
    public void ShootCannonball(float speedModifier = 1, float timeBetweenModifier = 0)
    {
        if(timeOfLastShot + timeBetweenShots - timeBetweenModifier < Time.time)
        {
            var cannonBallSpawned = ObjectPooler.SpawnFromPool(cannonBall.name, transform.position + transform.right*distToSpawnFromCannonCenter, transform.rotation);
            cannonBallSpawned.GetComponent<CannonProjectile>().InitializeMovement(cannonballSpeed*speedModifier, collisionTag);

            cannonAnimator.SetTrigger("Shoot");
            timeOfLastShot = Time.time;
        }
    }
    #endregion
}
