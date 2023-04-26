/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Cannon Fodder
 * Creation Date: 4/24/2023 10:19:33 AM
 * 
 * Description: TODO
******************************************************************/
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InspectorValues;
using static ValidCheck;

public class GenerateImpulsOnAwake : MonoBehaviour
{
    #region Functions
    // Start is called before the first frame update
    private void Awake()
    {
        var cameraShake = GetComponent<CinemachineImpulseSource>();

        if (cameraShake) cameraShake.GenerateImpulse();
    }
    #endregion
}
