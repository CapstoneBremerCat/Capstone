using UnityEngine;
using UnityEngine.UI;

public class InfoToText : MonoBehaviour
{
    //This script is meant solely for the purpose of the demo scene.
    //It should be used as a reference to see how the date information is retrieved from DayNightCycle.cs
    //This script contains no vital code and can safely be deleted without causing any damage to DayNightCycle.cs

    [Tooltip("A reference to DayNightCycle.cs")]
    public DayNightCycle DNC;
    [Tooltip("The current DaySpeed value.")]
    public Text txtDaySpeed;
    [Tooltip("The current Frames per second value.")]
    public Text txtFPS;
    [Tooltip("The current Time.")]
    public Text txtTime;
    [Tooltip("The current Date in written form.")]
    public Text txtDate;
    [Tooltip("The current date in numerical values.")]
    public Text txtShortDate;
    [Tooltip("The total days that have passed since start.")]
    public Text txtDaysSpent;
    [Tooltip("The current Time-of-Day.")]
    public Text txtDayStatus;
    [Tooltip("The current movement type of the sun.")]
    public Text txtSunStatus;
    [Tooltip("Should the camera move with the mouse?")]
    public bool RotateCamera = true;
    [Tooltip("The speed at which the camera rotates.")]
    public float speed = 2;

    Vector2 rotation = Vector2.zero;//a reference to the mouse movement
    public GameObject WarningBox;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //called to check the movement type of the sun at start and set the correct text
        if (DNC.MoveSunContinuously == true)
        {
            txtSunStatus.text = "Sun Movement: Constant";
        }
        if (DNC.MoveSunContinuously == false)
        {
            txtSunStatus.text = "Sun Movement: Incremental";
        }

        //set the default rotation of the camera
        rotation = new Vector3(-13.18f, -155.87f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //used for simple camera movement
        if (RotateCamera == true)
        {
            //the rotation of the mouse is taken and applied to the rotation variable
            //rotation.y is multiplied by a bit more to account for the widtch/height of the average computer screen
            rotation.y += Input.GetAxis("Mouse X") * speed *1.2f;
            rotation.x += -Input.GetAxis("Mouse Y") * speed;
            //rotation.x is clamped so that the player cannot look too far up or down
            rotation.x = Mathf.Clamp(rotation.x, -85, 85);
            //the rotation is the applied to the camera
            transform.eulerAngles = (Vector2)rotation;
        }

        //failsafe to make sure that DaySpeed is never set below 0 or above 250
        //note: DayNightCycle CAN go faster than 250, but it is capped for the demo scene
        if (DNC.DaySpeed < 0)
        {
            DNC.DaySpeed = 0;
        }
        if (DNC.DaySpeed >250)
        {
            DNC.DaySpeed = 250;
        }

        if (DNC.DaySpeed > 5 && WarningBox.activeSelf==false && DNC.MoveSunContinuously == false)
        {
            WarningBox.SetActive(true);
        }
        if ((WarningBox.activeSelf == true && DNC.DaySpeed <= 5 && DNC.MoveSunContinuously == false) || DNC.MoveSunContinuously == true)
        {
            WarningBox.SetActive(false);
        }

        //if the left key is pressed, the the DaySpeed is reduced
        if (Input.GetKeyDown("left"))
        {
            if (DNC.DaySpeed > 0)
            {
                //this resets the 'count' variable on DayNightCycle.cs so that the correct time is applied for Minutes Passed
                DNC.count = 0;
                if (DNC.DaySpeed > 0 && DNC.DaySpeed<=1)
                {
                    DNC.DaySpeed -= .1f;
                }
                if (DNC.DaySpeed > 1 && DNC.DaySpeed <= 10)
                {
                    DNC.DaySpeed -= 1f;
                }

                if (DNC.DaySpeed > 10 && DNC.DaySpeed <= 50)
                {
                    DNC.DaySpeed -= 5f;
                }

                if (DNC.DaySpeed > 50 && DNC.DaySpeed <= 250)
                {
                    DNC.DaySpeed -= 10f;
                }
            }
        }

        //if the right key is pressed, the the DaySpeed is increased
        if (Input.GetKeyDown("right"))
        {
            if (DNC.DaySpeed < 250)
            {
                //this resets the 'count' variable on DayNightCycle.cs so that the correct time is applied for Minutes Passed
                DNC.count = 0;
                if (DNC.DaySpeed >= 0 && DNC.DaySpeed < 1)
                {
                    DNC.DaySpeed += .1f;
                }
                if (DNC.DaySpeed >= 1 && DNC.DaySpeed < 10)
                {
                    DNC.DaySpeed += 1f;
                }

                if (DNC.DaySpeed >= 10 && DNC.DaySpeed < 50)
                {
                    DNC.DaySpeed += 5f;
                }

                if (DNC.DaySpeed >= 50 && DNC.DaySpeed < 250)
                {
                    DNC.DaySpeed += 10f;
                }
            }
        }

        //the spacebar changes from continuous to Incremental Sun Movement
        if (Input.GetKeyDown("space"))
        {
            if (DNC.MoveSunContinuously == true)
            {
                txtSunStatus.text = "Sun Movement: Incremental";
                //a few variable are reset in order to give a smoother transition when switching between the two movement types
                DNC.count = 0;
                DNC.hourAndMinuteA = DNC.Time2Rot(DNC.hours, DNC.minutes, DNC.AmPm, false);
                DNC.MoveSunContinuously = false;

                DNC.SunMoon.isStatic = true;
                DNC.Sun.isStatic = true;
                DNC.Moon.isStatic = true;
            }

            else if (DNC.MoveSunContinuously == false)
            {
                txtSunStatus.text = "Sun Movement: Constant";
                DNC.MoveSunContinuously = true;

                DNC.SunMoon.isStatic = false;
                DNC.Sun.isStatic = false;
                DNC.Moon.isStatic = false;
            }
        }


        //these lines set all of the information into a text format that is easliy read by the user.
        //use this as a reference if you want to add time sensitive code to your own scripts
        txtDaySpeed.text = "DaySpeed: " + DNC.DaySpeed;
        txtFPS.text = "FPS: " + Mathf.RoundToInt(1 / Time.deltaTime);
        txtTime.text = "Time: " + DNC.hours.ToString("00") + ":" + ((int)DNC.minutes).ToString("00") + DNC.AmPm;
        txtDate.text = "Date: "+ DNC.CurrentDayName + ", " + DNC.CurrentMonthName + " " + DNC.DayNum + ", " + DNC.YearNum;
        txtShortDate.text = "Short Date: " + DNC.MonthNum + "/" + DNC.DayNum + "/" + DNC.ShortYear;
        txtDaysSpent.text = "Days Spent: " + DNC.Days;
        txtDayStatus.text = "Day Status: " + DNC.DayStatus;
    }
}
