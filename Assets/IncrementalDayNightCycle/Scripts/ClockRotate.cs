using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockRotate : MonoBehaviour
{
    [Tooltip("Is this a digital clock?")]
    public bool isDigital = false;
    [Tooltip("The text field where the digital time will be displayed.")]
    public Text txtTime;
    [Tooltip("A reference to DayNightCycle.cs")]
    public DayNightCycle DNC;
    [Tooltip("The game object that represents the hour hand of the clock.")]
    public GameObject HourHand;
    [Tooltip("Should the hour hand rotate along the X axis?")]
    public bool RotateHourX;
    [Tooltip("Should the hour hand rotate along the Y axis?")]
    public bool RotateHourY;
    [Tooltip("Should the hour hand rotate along the Z axis?")]
    public bool RotateHourZ;
    [Tooltip("An offset value if the hour hand is not lining up with the correct time.")]
    public float HourOffset;
    [Tooltip("The game object that represents the minute hand of the clock.")]
    public GameObject MinuteHand;
    [Tooltip("Should the minute hand rotate along the X axis?")]
    public bool RotateMinuteX;
    [Tooltip("Should the minute hand rotate along the Y axis?")]
    public bool RotateMinuteY;
    [Tooltip("Should the minute hand rotate along the Z axis?")]
    public bool RotateMinuteZ;
    [Tooltip("An offset value if the minute hand is not lining up with the correct time.")]
    public float MinuteOffset;
    private float HourAndMinute;



    public void SetClockRot(float hr, float min)
    {
        //finds a rotation value based on the current time
        HourAndMinute = DNC.Time2Rot(hr, min, DNC.AmPm, false);

        //the rotation is then applied to the clock game object along with the offset value
        //the hour rotation is multiplied by 2 and the minute rotation is multiplied by 24
        //this is done to represent the number of times each will pass the 12 in any given day
        if (RotateHourX == true)
        {
            HourHand.transform.localEulerAngles = new Vector3((HourAndMinute * 2) + HourOffset, HourHand.transform.localEulerAngles.y, HourHand.transform.localEulerAngles.z);
        }
        if (RotateHourY == true)
        {
            HourHand.transform.localEulerAngles = new Vector3(HourHand.transform.localEulerAngles.x, (HourAndMinute * 2) + HourOffset, HourHand.transform.localEulerAngles.z);
        }
        if (RotateHourZ == true)
        {
            HourHand.transform.localEulerAngles = new Vector3(HourHand.transform.localEulerAngles.x, HourHand.transform.localEulerAngles.y, (HourAndMinute * 2) + HourOffset);
        }


        if (RotateMinuteX == true)
        {
            MinuteHand.transform.localEulerAngles = new Vector3((HourAndMinute * 24) + MinuteOffset, MinuteHand.transform.localEulerAngles.y, MinuteHand.transform.localEulerAngles.z);
        }
        if (RotateMinuteY == true)
        {
            MinuteHand.transform.localEulerAngles = new Vector3(MinuteHand.transform.localEulerAngles.x, (HourAndMinute * 24) + MinuteOffset, MinuteHand.transform.localEulerAngles.z);
        }
        if (RotateMinuteZ == true)
        {
            MinuteHand.transform.localEulerAngles = new Vector3(MinuteHand.transform.localEulerAngles.x, MinuteHand.transform.localEulerAngles.y, (HourAndMinute * 24) + MinuteOffset);
        }




    }



    // Update is called once per frame
    void Update()
    {
        //the current time is displayed based on what type of clock it is
            if (isDigital == false)
            {
                SetClockRot(DNC.hours, DNC.minutes);
            }
            if (isDigital == true)
            {
                txtTime.text = DNC.hours.ToString("00") + ":" + ((int)DNC.minutes).ToString("00") + DNC.AmPm;
            }
        
    }
}