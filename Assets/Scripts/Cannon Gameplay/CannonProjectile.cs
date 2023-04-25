/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Cannon Fodder
 * Creation Date: 4/14/2023 4:29:36 PM
 * 
 * Description: TODO
******************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static InspectorValues;
using static ValidCheck;

[RequireComponent(typeof(Rigidbody2D))]
public class CannonProjectile : MonoBehaviour
{
    #region Fields
    private Rigidbody2D rigidbodyComponent;
    private string collisionTag = "";

    private TrailRenderer trailRenderer;

    [Tooltip("The explosion prefab to be used when hitting something")]
    [SerializeField] private GameObject explosionEffect;

    [SerializeField] private LayerMask playerMask;

    private bool foundPlayer = false;
    #endregion

    #region Functions
    // Start is called before the first frame update
    private void Awake()
    {
        rigidbodyComponent = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void OnDisable()
    {
        trailRenderer.Clear();
        collisionTag = "";
        foundPlayer = false;
    }

    public void InitializeMovement(float moveSpeed, string collisionTag)
    {
        rigidbodyComponent.velocity = transform.right * moveSpeed;
        this.collisionTag = collisionTag;

        CheckForPlayer();
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(transform.position.x) > 12)
        {
            if (foundPlayer)
            {
                var playerNumber = collisionTag == "Player" ? 1 : 2;

                print("Dodge Increment");

                if (playerNumber == 1)
                {
                    print("Dodge Increment");

                    GameController.NumberOfTimesDodgedP1++;
                }
                else
                {
                    GameController.NumberOfTimesDodgedP2++;
                }
            }

            ObjectPooler.ReturnObjectToPool(gameObject.name, gameObject);
        }
    }

    private void CheckForPlayer()
    {
        if(Physics2D.BoxCast(transform.position, Vector2.one, 0, transform.right, 100, playerMask))
        {
            foundPlayer = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collisionTag == "") return;

        if (other.gameObject.CompareTag(collisionTag))
        {
            var playerNumber = collisionTag == "Player" ? 1 : 2;
            GameController.UpdateLives(playerNumber , -1);

            PlayHitEffect();

            ObjectPooler.ReturnObjectToPool(gameObject.name, gameObject);
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            PlayHitEffect();

            ObjectPooler.ReturnObjectToPool(gameObject.name, gameObject);
        }
    }

    private void PlayHitEffect()
    {
        var explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(explosion, 2.0f);
    }
    #endregion
}
