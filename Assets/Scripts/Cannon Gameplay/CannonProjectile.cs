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
    }

    public void InitializeMovement(float moveSpeed, string collisionTag)
    {
        rigidbodyComponent.velocity = transform.right * moveSpeed;
        this.collisionTag = collisionTag;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collisionTag == "") return;

        if (other.gameObject.CompareTag(collisionTag))
        {
            var playerNumber = collisionTag == "Player" ? 1 : 2;
            GameController.UpdateLives(playerNumber , -1);

            ObjectPooler.ReturnObjectToPool(gameObject.name, gameObject);
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            ObjectPooler.ReturnObjectToPool(gameObject.name, gameObject);
        }
    }
    #endregion
}
