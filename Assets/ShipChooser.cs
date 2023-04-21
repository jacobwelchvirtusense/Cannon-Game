/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Cannon Fodder
 * Creation Date: 4/18/2023 1:44:41 PM
 * 
 * Description: Chooses a random sprite to be used by the ship.
******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InspectorValues;
using static ValidCheck;

public class ShipChooser : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The spriterenderer of the ship.
    /// </summary>
    private SpriteRenderer spriteRenderer;

    [Tooltip("All of the possible ship sprites")]
    [SerializeField] private Sprite[] shipSprites;
    #endregion

    #region Functions
    /// <summary>
    /// Initializes components and sets the ships model.
    /// </summary>
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ChooseRandomShipModel();
    }

    /// <summary>
    /// Chooses a random ship sprite to be used from the shipSprites array.
    /// </summary>
    private void ChooseRandomShipModel()
    {
        var shipIndex = Random.Range(0, shipSprites.Length);

        spriteRenderer.sprite = shipSprites[shipIndex];
    }
    #endregion
}
