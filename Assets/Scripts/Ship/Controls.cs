using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{

    public string forwardAxisName = "Vertical";         //name of forward movement axis
    public string turningAxisName = "Horizontal";     // name of turning axis
    public string thrusterAxisName = "SideThrusters";    // side thruster contros
    public string brakeKey = "Brakes";                     //braking button

    [HideInInspector] public float acceleration;
    [HideInInspector] public float thruster;
    [HideInInspector] public float steering;
    [HideInInspector] public bool isBraking;

    void Update()
    {
        //if the player hits the escape key inside a build, close the application
        if (Input.GetButtonDown("Escape") && !Application.isEditor)
        {
            Application.Quit();
        }

        //get the values of the accelerator, side thrusters, steering, and brakes from the input class
        acceleration = Input.GetAxis(forwardAxisName);
        thruster = Input.GetAxis(thrusterAxisName);
        steering = Input.GetAxis(turningAxisName);
        isBraking = Input.GetButton(brakeKey);

    }


}
