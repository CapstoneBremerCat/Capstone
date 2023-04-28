using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    [Tooltip("This variable controls how long it will take for the sun to complete moving to a new position. A private variable (TimeForMinutes) is calculated for realtime by counting in-game minutes that have passed until 'MoveSunAfter' is reached. (TimeForMinutes) is then multiplied by 'WaitTimeMultiplier' to decide how much realtime it should take for the sun to move to its new position. (1=Full / .5=Half)")]
    [Range(.0f,1f)]
    public float WaitTimeMultiplier = .5f;

    //these variables all play a role in how long the sun will take to move to the new location
    float TimeForMinute;//the realtime that it takes for 'MoveSunAfter' in-game minutes have passed
    float startTime;//the time that is taken when 'count' is at 0
    float duration;//the result of TimeForMinute*WaitTimeMultiplier
    [HideInInspector]
    public int count = 0;//used to judge how many in-game minutes have passed

    [Tooltip("The last 2 digits of the current year.")]
    public string ShortYear;


    [Tooltip("Should the sun move continuously or move once after a certain time increment.")]
    public bool MoveSunContinuously = true;
    [Tooltip("Move the sun after this many in-game minutes have passed.")]
    public int MoveSunAfter = 10;

    [Tooltip("States if the sun is currently moving to a new position.")]
    public bool isLerping = false;


    [Tooltip("States if the current year is a Leap Year.")]
    public bool isLeapYear = false;



    [Tooltip("A reference to the Sun GameObject.")]
    public GameObject Sun;
    [Tooltip("A reference to the Sun's Light property.")]
    public Light SunLight;
    [Tooltip("A reference to the Sun's LensFlare property.")]
    public LensFlare LF;
    [Tooltip("A reference to the Moon GameObject.")]
    public GameObject Moon;



    [Tooltip("A reference to the Sun and Moon parent GameObject.")]
    public GameObject SunMoon;




    [Tooltip("The current day of the week based on the date. This variable will be automatically set from the date and time variables above. To change the names, see the foldout menu below.")]
    public string CurrentDayName;

    public string Day1Name = "Sunday";
    public string Day2Name = "Monday";
    public string Day3Name = "Tuesday";
    public string Day4Name = "Wednesday";
    public string Day5Name = "Thursday";
    public string Day6Name = "Friday";
    public string Day7Name = "Saturday";

    [Tooltip("The current month of the year based on the date. This variable will be automatically set from the date and time variables above. To change the names, see the foldout menu below.")]
    public string CurrentMonthName;

    public string Month1Name = "January";
    public string Month2Name = "February";
    public string Month3Name = "March";
    public string Month4Name = "April";
    public string Month5Name = "May";
    public string Month6Name = "June";
    public string Month7Name = "July";
    public string Month8Name = "August";
    public string Month9Name = "September";
    public string Month10Name = "October";
    public string Month11Name = "November";
    public string Month12Name = "December";


    [Tooltip("The current Time-of-Day. (Midnight, Afternoon, etc.)")]
    public string DayStatus;
    [Tooltip("The current time for hours.")]
    public float hours = 1;
    [Tooltip("The current time for minutes.")]
    public float minutes = 0;
    [Tooltip("The current day of the month.")]
    public int DayNum = 1;
    [Tooltip("The current month of the year.")]
    public int MonthNum = 1;
    [Tooltip("The current year. (Value must be higher than 10)")]
    public int YearNum = 0;
    [Tooltip("The number of in-game days that have passed.")]
    public int Days = 0;
    [Tooltip("Is it ante meridiem of post meridiem.")]
    public string AmPm = "am";
    [Tooltip("Multiplied by Time.deltaTime to determine the speed of the in game clock. (0.3 makes game speed ~3sec/1min)")]
    public float DaySpeed = .3f;
    [Tooltip("A reference to the StarSphere GameObject.")]
    public GameObject starsphere;


    [Tooltip("The time of day that the Midnight phase will begin for hours.")]
    public int StartHourMidnight;
    [Tooltip("The time of day that the Midnight phase will begin for minutes.")]
    public int StartMinMidnight;
    [Tooltip("The time of day that the Midnight phase will begin for AM or PM.")]
    public string StartAmPmMidnight;
    [Tooltip("The rotation that the sun will be when Midnight begins.")]
    public float MidnightStartRot;
    [Tooltip("The speed at which the current Time of Day settings will change to Midnight settings.")]
    public float MidnightChangeRate;
    [Tooltip("The lens flare intensity during Midnight.")]
    public float MidnightLensFlare;
    [Tooltip("The sun light intensity during Midnight.")]
    public float MidnightLightIntensity;
    [Tooltip("The color of the sun light during Midnight.")]
    public Color MidnightLightColor;
    [Tooltip("The color of the ambient light during Midnight.")]
    public Color MidnightAmbientColor;
    [Tooltip("The alpha cutoff value of the stars material during Midnight. (0 = Opaque / 1 = Transparent)")]
    public float MidnightStarAlpha;
    [Tooltip("The color of the fog during Midnight.")]
    public Color MidnightFogColor;
    [Tooltip("The density of the fog during Midnight.")]
    public float MidnightFogDensity;



    [Tooltip("The time of day that the Pre-Sunrise phase will begin for hours.")]
    public int StartHourPreSunRise;
    [Tooltip("The time of day that the Pre-Sunrise phase will begin for minutes.")]
    public int StartMinPreSunRise;
    [Tooltip("The time of day that the Pre-Sunrise phase will begin for AM or PM.")]
    public string StartAmPmPreSunRise;
    [Tooltip("The rotation that the sun will be when Pre-Sunrise begins.")]
    public float PreSunRiseStartRot;
    [Tooltip("The speed at which the current Time of Day settings will change to Pre-Sunrise settings.")]
    public float PreSunRiseChangeRate;
    [Tooltip("The lens flare intensity during Pre-Sunrise.")]
    public float PreSunRiseLensFlare;
    [Tooltip("The sun light intensity during Pre-Sunrise.")]
    public float PreSunRiseLightIntensity;
    [Tooltip("The color of the sun light during Pre-Sunrise.")]
    public Color PreSunRiseLightColor;
    [Tooltip("The color of the ambient light during Pre-Sunrise.")]
    public Color PreSunRiseAmbientColor;
    [Tooltip("The alpha cutoff value of the stars material during Pre-Sunrise. (0 = Opaque / 1 = Transparent)")]
    public float PreSunRiseStarAlpha;
    [Tooltip("The color of the fog during Pre-Sunrise.")]
    public Color PreSunRiseFogColor;
    [Tooltip("The density of the fog during Pre-Sunrise.")]
    public float PreSunRiseFogDensity;



    [Tooltip("The time of day that the Sunrise phase will begin for hours.")]
    public int StartHourSunRise;
    [Tooltip("The time of day that the Sunrise phase will begin for minutes.")]
    public int StartMinSunRise;
    [Tooltip("The time of day that the Sunrise phase will begin for AM or PM.")]
    public string StartAmPmSunRise;
    [Tooltip("The rotation that the sun will be when Sunrise begins.")]
    public float SunRiseStartRot;
    [Tooltip("The speed at which the current Time of Day settings will change to Sunrise settings.")]
    public float SunRiseChangeRate;
    [Tooltip("The lens flare intensity during Sunrise.")]
    public float SunRiseLensFlare;
    [Tooltip("The sun light intensity during Sunrise.")]
    public float SunRiseLightIntensity;
    [Tooltip("The color of the sun light during Sunrise.")]
    public Color SunRiseLightColor;
    [Tooltip("The color of the ambient light during Sunrise.")]
    public Color SunRiseAmbientColor;
    [Tooltip("The alpha cutoff value of the stars material during Sunrise. (0 = Opaque / 1 = Transparent)")]
    public float SunRiseStarAlpha;
    [Tooltip("The color of the fog during Sunrise.")]
    public Color SunRiseFogColor;
    [Tooltip("The density of the fog during Sunrise.")]
    public float SunRiseFogDensity;


    [Tooltip("The time of day that the Post-Sunrise phase will begin for hours.")]
    public int StartHourPostSunRise;
    [Tooltip("The time of day that the Post-Sunrise phase will begin for minutes.")]
    public int StartMinPostSunRise;
    [Tooltip("The time of day that the Post-Sunrise phase will begin for AM or PM.")]
    public string StartAmPmPostSunRise;
    [Tooltip("The rotation that the sun will be when Post-Sunrise begins.")]
    public float PostSunRiseStartRot;
    [Tooltip("The speed at which the current Time of Day settings will change to Post-Sunrise settings.")]
    public float PostSunRiseChangeRate;
    [Tooltip("The lens flare intensity during Post-Sunrise.")]
    public float PostSunRiseLensFlare;
    [Tooltip("The sun light intensity during Post-Sunrise.")]
    public float PostSunRiseLightIntensity;
    [Tooltip("The color of the sun light during Post-Sunrise.")]
    public Color PostSunRiseLightColor;
    [Tooltip("The color of the ambient light during Post-Sunrise.")]
    public Color PostSunRiseAmbientColor;
    [Tooltip("The alpha cutoff value of the stars material during Post-Sunrise. (0 = Opaque / 1 = Transparent)")]
    public float PostSunRiseStarAlpha;
    [Tooltip("The color of the fog during Post-Sunrise.")]
    public Color PostSunRiseFogColor;
    [Tooltip("The density of the fog during Post-Sunrise.")]
    public float PostSunRiseFogDensity;



    [Tooltip("The time of day that the Afternoon phase will begin for hours.")]
    public int StartHourAfternoon;
    [Tooltip("The time of day that the Afternoon phase will begin for minutes.")]
    public int StartMinAfternoon;
    [Tooltip("The time of day that the Afternoon phase will begin for AM or PM.")]
    public string StartAmPmAfternoon;
    [Tooltip("The rotation that the sun will be when Afternoon begins.")]
    public float AfternoonStartRot;
    [Tooltip("The speed at which the current Time of Day settings will change to Afternoon settings.")]
    public float AfternoonChangeRate;
    [Tooltip("The lens flare intensity during Afternoon.")]
    public float AfternoonLensFlare;
    [Tooltip("The sun light intensity during Afternoon.")]
    public float AfternoonLightIntensity;
    [Tooltip("The color of the sun light during Afternoon.")]
    public Color AfternoonLightColor;
    [Tooltip("The color of the ambient light during Afternoon.")]
    public Color AfternoonAmbientColor;
    [Tooltip("The alpha cutoff value of the stars material during Afternoon. (0 = Opaque / 1 = Transparent)")]
    public float AfternoonStarAlpha;
    [Tooltip("The color of the fog during Afternoon.")]
    public Color AfternoonFogColor;
    [Tooltip("The density of the fog during Afternoon.")]
    public float AfternoonFogDensity;



    [Tooltip("The time of day that the Pre-Sunset phase will begin for hours.")]
    public int StartHourPreSunSet;
    [Tooltip("The time of day that the Pre-Sunset phase will begin for minutes.")]
    public int StartMinPreSunSet;
    [Tooltip("The time of day that the Pre-Sunset phase will begin for AM or PM.")]
    public string StartAmPmPreSunSet;
    [Tooltip("The rotation that the sun will be when Pre-Sunset begins.")]
    public float PreSunSetStartRot;
    [Tooltip("The speed at which the current Time of Day settings will change to Pre-Sunset settings.")]
    public float PreSunSetChangeRate;
    [Tooltip("The lens flare intensity during Pre-Sunset.")]
    public float PreSunSetLensFlare;
    [Tooltip("The sun light intensity during Pre-Sunset.")]
    public float PreSunSetLightIntensity;
    [Tooltip("The color of the sun light during Pre-Sunset.")]
    public Color PreSunSetLightColor;
    [Tooltip("The color of the ambient light during Pre-Sunset.")]
    public Color PreSunSetAmbientColor;
    [Tooltip("The alpha cutoff value of the stars material during Pre-Sunset. (0 = Opaque / 1 = Transparent)")]
    public float PreSunSetStarAlpha;
    [Tooltip("The color of the fog during Pre-Sunset.")]
    public Color PreSunSetFogColor;
    [Tooltip("The density of the fog during Pre-Sunset.")]
    public float PreSunSetFogDensity;


    [Tooltip("The time of day that the Sunset phase will begin for hours.")]
    public int StartHourSunSet;
    [Tooltip("The time of day that the Sunset phase will begin for minutes.")]
    public int StartMinSunSet;
    [Tooltip("The time of day that the Sunset phase will begin for AM or PM.")]
    public string StartAmPmSunSet;
    [Tooltip("The rotation that the sun will be when Sunset begins.")]
    public float SunSetStartRot;
    [Tooltip("The speed at which the current Time of Day settings will change to Sunset settings.")]
    public float SunSetChangeRate;
    [Tooltip("The lens flare intensity during Sunset.")]
    public float SunSetLensFlare;
    [Tooltip("The sun light intensity during Sunset.")]
    public float SunSetLightIntensity;
    [Tooltip("The color of the sun light during Sunset.")]
    public Color SunSetLightColor;
    [Tooltip("The color of the ambient light during Sunset.")]
    public Color SunSetAmbientColor;
    [Tooltip("The alpha cutoff value of the stars material during Sunset. (0 = Opaque / 1 = Transparent)")]
    public float SunSetStarAlpha;
    [Tooltip("The color of the fog during Sunset.")]
    public Color SunSetFogColor;
    [Tooltip("The density of the fog during Sunset.")]
    public float SunSetFogDensity;



    [Tooltip("The time of day that the Post-Sunset phase will begin for hours.")]
    public int StartHourPostSunSet;
    [Tooltip("The time of day that the Post-Sunset phase will begin for minutes.")]
    public int StartMinPostSunSet;
    [Tooltip("The time of day that the Post-Sunset phase will begin for AM or PM.")]
    public string StartAmPmPostSunSet;
    [Tooltip("The rotation that the sun will be when Post-Sunset begins.")]
    public float PostSunSetStartRot;
    [Tooltip("The speed at which the current Time of Day settings will change to Post-Sunset settings.")]
    public float PostSunSetChangeRate;
    [Tooltip("The lens flare intensity during Post-Sunset.")]
    public float PostSunSetLensFlare;
    [Tooltip("The sun light intensity during Post-Sunset.")]
    public float PostSunSetLightIntensity;
    [Tooltip("The color of the sun light during Post-Sunset.")]
    public Color PostSunSetLightColor;
    [Tooltip("The color of the ambient light during Post-Sunset.")]
    public Color PostSunSetAmbientColor;
    [Tooltip("The alpha cutoff value of the stars material during Post-Sunset. (0 = Opaque / 1 = Transparent)")]
    public float PostSunSetStarAlpha;
    [Tooltip("The color of the fog during Post-Sunset.")]
    public Color PostSunSetFogColor;
    [Tooltip("The density of the fog during Post-Sunset.")]
    public float PostSunSetFogDensity;




    [Tooltip("Image 1/8 that will appear as a Moon phase.")]
    public Texture mp1;
    [Tooltip("Image 2/8 that will appear as a Moon phase.")]
    public Texture mp2;
    [Tooltip("Image 3/8 that will appear as a Moon phase.")]
    public Texture mp3;
    [Tooltip("Image 4/8 that will appear as a Moon phase.")]
    public Texture mp4;
    [Tooltip("Image 5/8 that will appear as a Moon phase.")]
    public Texture mp5;
    [Tooltip("Image 6/8 that will appear as a Moon phase.")]
    public Texture mp6;
    [Tooltip("Image 7/8 that will appear as a Moon phase.")]
    public Texture mp7;
    [Tooltip("Image 8/8 that will appear as a Moon phase.")]
    public Texture mp8;

    [Tooltip("Specify which moon phase should appear on start")]
    [Range(1,8)]
    public int StartingPhase = 1;

    [Tooltip("This number will be applied as a Vector3 to the Moon GameObject's scale.")]
    public float MoonScale;

    private int AddToMoonPhase;//used to change which moon phase appears at start
    private Material MoonMat;//the material that the Moon GameObject is currently using
    private int prevDays = -1;//this makes sure that the moon changes phases during the day rather than at midnight
    private float DayInPhase;//this keeps track of what day out of 28 the moon is in its cycle and sets the image according to that

    private int once;//used to make sure that am is flipped to pm and vice versa only once per 12 hours
    private float an;//A placeholder for FindDayStatus which references the Sun's current rotation
    private bool instantchange = true;//set for ChangeDayStatus on start so that it sets to the specified time of day instantly


    private Material StarMat;//the StarSphee's Material

    
    int lastMin;//used to keep track of how many in-game minutes have passed
    int CurMin;//used to keep track of how many in-game minutes have passed

    //each of these variables are used to track the rotation of the sun at a given time-of-day
    float hourAndMinute;
    [HideInInspector]
    public float hourAndMinuteA;//used as the start position for incremental sun movement
    [HideInInspector]
    public float hourAndMinuteB;//used as the end position for incremental sun movement

    
    private bool OnStart = true;//sets the correct moon phase once on start and then is set to false

    //this set of 2 letter variables are used to change the various light and color options smoothly
    float li;
    Color lc;
    Color ac;
    float lf;
    float sa;
    Color fc;
    float fd;


    float t;//a timer variable

   // private float MidnightAngle;
   // private bool trig = false;

    [Tooltip("A curve defining the way in which the shadows will move.")]
    public AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    // Start is called before the first frame updateDayName
    void Start()
    {
        Moon.transform.localScale = new Vector3(MoonScale, MoonScale, MoonScale);
        if (StartingPhase == 1)
        {
            AddToMoonPhase = 0;
        }
        if (StartingPhase == 2)
        {
            AddToMoonPhase = 4;
        }
        if (StartingPhase == 3)
        {
            AddToMoonPhase = 7;
        }
        if (StartingPhase == 4)
        {
            AddToMoonPhase = 11;
        }
        if (StartingPhase == 5)
        {
            AddToMoonPhase = 14;
        }
        if (StartingPhase == 6)
        {
            AddToMoonPhase = 18;
        }
        if (StartingPhase == 7)
        {
            AddToMoonPhase = 21;
        }
        if (StartingPhase == 8)
        {
            AddToMoonPhase = 25;
        }


        //fixing any data that would be out of range
        if (MonthNum < 1)
        {
            MonthNum = 1;
        }
        if (MonthNum > 12)
        {
            MonthNum = 12;
        }

        if (DayNum < 1)
        {
            DayNum = 1;
        }

        if (YearNum < 1)
        {
            YearNum = 1;
            Debug.LogWarning("'YearNum' has been set to 1. The variable 'YearNum' must be greater than 0 in order for FindDayName() to function. Please set the variable 'YearNum' to a value equal or higher than 1 in the inspector to avoid seeing this message");
        }


        //set both of the to the current time at start
        lastMin = (int)minutes;
        CurMin = (int)minutes;

        StarMat = starsphere.GetComponent<Renderer>().material;
        MoonMat = Moon.GetComponent<Renderer>().material;

        
        //set the day of the month without adding a day (false)
        SetDayMonth(false);

        //set the starting rotation for each day phase
        MidnightStartRot = Time2Rot(StartHourMidnight, StartMinMidnight, StartAmPmMidnight, true);
        PreSunRiseStartRot = Time2Rot(StartHourPreSunRise, StartMinPreSunRise, StartAmPmPreSunRise, true);
        SunRiseStartRot = Time2Rot(StartHourSunRise, StartMinSunRise, StartAmPmSunRise, true);
        PostSunRiseStartRot = Time2Rot(StartHourPostSunRise, StartMinPostSunRise, StartAmPmPostSunRise, true);
        AfternoonStartRot = Time2Rot(StartHourAfternoon, StartMinAfternoon, StartAmPmAfternoon, true);
        PreSunSetStartRot = Time2Rot(StartHourPreSunSet, StartMinPreSunSet, StartAmPmPreSunSet, true);
        SunSetStartRot = Time2Rot(StartHourSunSet, StartMinSunSet, StartAmPmSunSet, true);
        PostSunSetStartRot = Time2Rot(StartHourPostSunSet, StartMinPostSunSet, StartAmPmPostSunSet, true);

        //check to make sure hours is set to correct time
        if (hours <= 0)
        {
            hours = 12;
            once = 1;
            AmPm = "am";
        }
        if (hours >= 13)
        {
            hours = 1;
            once = 0;
            AmPm = "pm";
        }

        //---------Initialization--------\\
        //These functions are all called at later points in the script
        //they are called here to set the initial data based on the date and time the player has input in the inspector
        SetSunRot(hours, minutes);
        MoonUpdate();
        FindDayStatus();
        FindDayName(DayNum,MonthNum,YearNum);
        isLeapYear = DateTime.IsLeapYear(YearNum);
        ShortYear = YearNum.ToString().Substring(YearNum.ToString("00").Length - 2);
        //-------------------------------\\


        if (MoveSunContinuously == true)
        {//if the sun is moving continuously set static on all items to false
            SunMoon.isStatic = false;
            Sun.isStatic = false;
            Moon.isStatic = false;
        }
        if (MoveSunContinuously == false)
        {//if the sun is moving incrementally set static on all items to true
            SunMoon.isStatic = true;
            Sun.isStatic = true;
            Moon.isStatic = true;
        }


        //used to only change am to pm one time
        once = 0;

        //this is used for Incremental sun movement and sets the starting position of the sun based on the time
        hourAndMinuteA = Time2Rot(hours, minutes, AmPm, false);
    }

    // Update is called once per frame
    void Update()
    {
        //used to keep track of how many minutes have passed.
        lastMin = (int)minutes;






        //-----------WORLD CLOCK-----------\\
        minutes += Time.deltaTime * DaySpeed;
        if (minutes >= 60)
        {
            //Minutes are added frame-by-frame until they pass 60
            //at which point an hour is added and minutes are reset
            minutes = 0;
            hours++;

            if (hours >= 13)
            {
                //if hours pass 12 they they are reset to 1 and hte 'once' variable is activated
                hours = 1;
                once = 0;
            }

            if (hours == 12 && once == 0)
            {
                //the 'once' variable is needed so that AM and PM are not switched every time that a minute is added
                once = 1;
                if (AmPm == "am")
                {
                    //Am switches to Pm only once
                    AmPm = "pm";
                }
                else if (AmPm == "pm")
                {
                    AmPm = "am";
                    //if Pm switches to Am then a day is added to 'Days' which represents the total days passed in-game
                    Days++;
                    //SetDayMonth is actually what adds the day value to the date
                    SetDayMonth(true);
                    //And FindDayName is run after the new date has been updated
                    FindDayName(DayNum, MonthNum, YearNum);
                }
            }
        }
        //----------END WORLD CLOCK-----------\\

        //After the time is updated this variable is used with 'lastMin' to see if a minute has passed since the last frame
        CurMin = (int)minutes;



        //If the sun is set to Incremental
        if (MoveSunContinuously == false)
        {



            //the count variable is used to determine how many total in-game minutes have passed
            if (count == 0)
            {
                //startTime is then taken so that a measure of time can be gained for how long it takes for the minutes to pass
                startTime = Time.time;
            }

            if (count == MoveSunAfter)
            {
                //when the appropriate number of minutes have passed the end time is taken
                //TimeForMinute is how long it takes in Realtime for enough in-game minutes to pass to move the Sun
                TimeForMinute = Time.time - startTime;
            }


            if (CurMin != lastMin && count <= MoveSunAfter && count != -1)
            {
                //when CurMin and lastMin are not the same count is added to
                count++;
                if (count > MoveSunAfter)
                {
                    //if count is greater than MoveSunAfter then begin moving the sun
                    //count is set to -1 here so that the count does not begin again until Lerping is completed
                    count = -1;
                    if (isLerping == false)
                    {
                        isLerping = true;
                        StartCoroutine(LerpSunRot());

                    }
                }
            }
        }

        //If the Sun is set to Continuous
        if (MoveSunContinuously == true)
        {
            //Move the sun each frame based on the hours and minutes
            SetSunRot(hours, minutes);
            //Search for DayStatus each frame so that the appropriate Time-of-Day is set
            FindDayStatus();
        }
    }


    public void FindDayName(int day, int month, int year)
    {

        //this function returns a Day Name based on the input values
        DateTime dateValue = new DateTime(year, month, day);

        //A number is assigned based on the input date
        int DayNameNum = 0;
        DayNameNum = (int)dateValue.DayOfWeek;

        //this number corresponds with the day of the week, which is then set to the appropriate string
        if (DayNameNum == 0)
        {
            CurrentDayName = Day1Name;
        }
        else if (DayNameNum == 1)
        {
            CurrentDayName = Day2Name;
        }
        else if (DayNameNum == 2)
        {
            CurrentDayName = Day3Name;
        }
        else if (DayNameNum == 3)
        {
            CurrentDayName = Day4Name;
        }
        else if (DayNameNum == 4)
        {
            CurrentDayName = Day5Name;
        }
        else if (DayNameNum == 5)
        {
            CurrentDayName = Day6Name;
        }
        else if (DayNameNum == 6)
        {
            CurrentDayName = Day7Name;
        }
    }



    public void SetDayMonth(bool addDay)
    {
        //adds a day if specified to do so
        //this is only set to false in the Start() function
        if (addDay == true)
        {
            DayNum++;
        }


        //Each of these statements works essentially in the same way with the exception of February for leap years
        if (MonthNum == 1)
        {
            //based on the month number a month name is assigned
            CurrentMonthName = Month1Name;
            if (DayNum >= 32)
            {
                //if the day number is  greater than the total amount of days in that month,
                //then the month goes to the next and DayNum is set back to 1
                MonthNum = 2;
                DayNum = 1;
                //In Month 12 a Year is added as well
            }
        }
        else if (MonthNum == 2)
        {
            CurrentMonthName = Month2Name;
            isLeapYear = DateTime.IsLeapYear(YearNum);
            //February works the same way as described above, except if it is a Leap Year, the DayNum check changes
            if (isLeapYear==true)
            {
                if (DayNum >= 30)
                {
                    MonthNum = 3;
                    DayNum = 1;
                }
            }
            else if (isLeapYear ==false)
            {
                if (DayNum >= 29)
                {
                    MonthNum = 3;
                    DayNum = 1;
                }
            }
        }
        else if (MonthNum == 3)
        {
            CurrentMonthName = Month3Name;
            if (DayNum >= 32)
            {
                MonthNum = 4;
                DayNum = 1;
            }
        }
        else if (MonthNum == 4)
        {
            CurrentMonthName = Month4Name;
            if (DayNum >= 31)
            {
                MonthNum = 5;
                DayNum = 1;
            }
        }
        else if (MonthNum == 5)
        {
            CurrentMonthName = Month5Name;
            if (DayNum >= 32)
            {
                MonthNum = 6;
                DayNum = 1;
            }
        }
        else if (MonthNum == 6)
        {
            CurrentMonthName = Month6Name;
            if (DayNum >= 31)
            {
                MonthNum = 7;
                DayNum = 1;
            }
        }
        else if (MonthNum == 7)
        {
            CurrentMonthName = Month7Name;
            if (DayNum >= 32)
            {
                MonthNum = 8;
                DayNum = 1;
            }
        }
        else if (MonthNum == 8)
        {
            CurrentMonthName = Month8Name;
            if (DayNum >= 32)
            {
                MonthNum = 9;
                DayNum = 1;
            }
        }
        else if (MonthNum == 9)
        {
            CurrentMonthName = Month9Name;
            if (DayNum >= 31)
            {
                MonthNum = 10;
                DayNum = 1;
            }
        }
        else if (MonthNum == 10)
        {
            CurrentMonthName = Month10Name;
            if (DayNum >= 32)
            {
                MonthNum = 11;
                DayNum = 1;
            }
        }
        else if (MonthNum == 11)
        {
            CurrentMonthName = Month11Name;
            if (DayNum >= 31)
            {
                MonthNum = 12;
                DayNum = 1;
            }
        }
        else if (MonthNum == 12)
        {
            CurrentMonthName = Month12Name;
            if (DayNum >= 32)
            {
                MonthNum = 1;
                DayNum = 1;
                //For the last month a year is added and the 'ShortYear' variable is updated
                YearNum++;
                FindDayName(DayNum, MonthNum, YearNum);
                SetDayMonth(false);
                ShortYear = YearNum.ToString().Substring(YearNum.ToString("00").Length - 2);
            }
        }
    }

    public void SetSunRot(float hr, float min)
    {
        //This calls the Time2Rot function and applies it to the rotation of the Parent Object "SunMoon"
        hourAndMinute = Time2Rot(hr, min, AmPm, false);
        //if (trig == true)
        //{
        //    trig = false;
        //    Moon.transform.localScale = new Vector3(-MoonScale, MoonScale, -MoonScale);
        //}
        SunMoon.transform.eulerAngles = new Vector3(hourAndMinute, 0, 0);
        //Forces sun and moon lights to look at center
        Sun.transform.LookAt(Vector3.zero);
    }

   

   


    public IEnumerator LerpSunRot()
    {
        //This Coroutine moves the sun to a new position
        t = 0;//starts at 0 for timer
        duration = TimeForMinute * WaitTimeMultiplier;//the total time it will take for the sun to move to its new position
        
        
        float A = hourAndMinuteA;//hourAndMinuteA is set at start and at the end of this function
        float B = Time2Rot(hours, minutes, AmPm, false);//hourAndMinuteB is set to what the suns rotation should be based on the current time

        if (B < A)
        {
            B += 360;
        }


        Vector3 a = new Vector3(A, 0, 0);//a Vector3 is created using A as the x variable
        Vector3 b = new Vector3(B, 0, 0);//a Vector3 is created using B as the x variable

      

        SunMoon.isStatic = false;//Static is set to false
        Sun.isStatic = false;//Static is set to false
        Moon.isStatic = false;//Static is set to false



        while (t <= 1)
        {
            Sun.transform.LookAt(Vector3.zero);//sun is set to look at the game world

            //while t<=1 the sun moves from a to b based on the curve that was predefined in the inspector
            SunMoon.transform.eulerAngles = Vector3.Slerp(a, b, curve.Evaluate(t));
            


            //t is then added to with taking the total duration time into account
            t += Time.deltaTime / duration;
            if (t > 1)
            {
                //once the movement completes, the suns rotation is set to B as well as the new starting rotation for the next time this funtion runs
                SunMoon.transform.eulerAngles = new Vector3(B, 0, 0);
                hourAndMinuteA = B;

            }
            yield return new WaitForEndOfFrame();
        }
        FindDayStatus();//checks is the time-of-day need to be updated
        isLerping = false;//the sun is no longer moving
        count = 0;//restarts counting the amount of time for the specified minutes to pass

        SunMoon.isStatic = true;//Static is set back to true
        Sun.isStatic = true;//Static is set back to true
        Moon.isStatic = true;//Static is set back to true

    }


    public float Time2Rot(float hr, float min, string ap, bool FindingStatus)
    {
        //Converts time to a rotation

        //resetting the time variables for the next calculations
        if (ap == "am" && hr == 12)
        {
            hr = 0;
        }

        if (ap == "pm")
        {
            if (hr != 12)
            {
                hr += 12;
            }
        }

        //checks the rotation by converting to a 24 hour clock
        float HnM = hr + (((min * 100) / 60) / 100);
        HnM = ((HnM * 360f) / 24f) - 90f;//then making adjustments to get a 0-360 rotation based on the current time

        if (FindingStatus == true)
        {
            //this is used for the initial set up to decide what the sun rotation will be when a given time-of-day starts
            if ((ap == "am" && hr == 12) || (hr < 6 && ap == "am"))
            {
                HnM = 180 + (180 + HnM);
            }
        }

        return HnM;
    }



    public void MoonUpdate()
    {

        //changes the moon phase during the day rather than at midnight and also changes on Startup
        if ((Days > prevDays) && ((hours > 6 && hours != 12 && AmPm == "am") || (AmPm == "pm" && hours < 6) || (AmPm == "pm" && hours == 12) || OnStart==true))
        {
            //checks to make sure the function isnt running continuously
            OnStart = false;
            prevDays = Days;
            DayInPhase = Days + AddToMoonPhase;
            //calculates what day of the phase the moon is in 28 being end cycle
            while (DayInPhase >= 28)
            {
                //takes the total number of days that the player has spent and reduces it until it is within range
                DayInPhase -= 28;
            }

            //sets moon phase accordingly
            if (DayInPhase >= 0 && DayInPhase < 4)
            {
                MoonMat.mainTexture = mp1;
                MoonMat.SetColor("_EmissionColor", Color.gray);//for new moon the color is set to gray
                MoonMat.SetTextureOffset("_MainTex", new Vector2(0, 0));
            }
            if (DayInPhase >= 4 && DayInPhase < 7)
            {
                MoonMat.mainTexture = mp2;
                MoonMat.SetColor("_EmissionColor", Color.white);
                MoonMat.SetTextureOffset("_MainTex", new Vector2(.012f, 0));//texture is moved to the side to avoid unwanted clipping
            }
            if (DayInPhase >= 7 && DayInPhase < 11)
            {
                MoonMat.mainTexture = mp3;
                MoonMat.SetColor("_EmissionColor", Color.white);
                MoonMat.SetTextureOffset("_MainTex", new Vector2(.012f, 0));
            }
            if (DayInPhase >= 11 && DayInPhase < 14)
            {
                MoonMat.mainTexture = mp4;
                MoonMat.SetColor("_EmissionColor", Color.white);
                MoonMat.SetTextureOffset("_MainTex", new Vector2(.012f, 0));
            }
            if (DayInPhase >= 14 && DayInPhase < 18)
            {
                MoonMat.mainTexture = mp5;
                MoonMat.SetColor("_EmissionColor", Color.white);
                MoonMat.SetTextureOffset("_MainTex", new Vector2(0, 0));
            }
            if (DayInPhase >= 18 && DayInPhase < 21)
            {
                MoonMat.mainTexture = mp6;
                MoonMat.SetColor("_EmissionColor", Color.white);
                MoonMat.SetTextureOffset("_MainTex", new Vector2(-.012f, 0));
            }
            if (DayInPhase >= 21 && DayInPhase < 25)
            {
                MoonMat.mainTexture = mp7;
                MoonMat.SetColor("_EmissionColor", Color.white);
                MoonMat.SetTextureOffset("_MainTex", new Vector2(-.012f, 0));
            }
            if (DayInPhase >= 25 && DayInPhase < 28)
            {
                MoonMat.mainTexture = mp8;
                MoonMat.SetColor("_EmissionColor", Color.white);
                MoonMat.SetTextureOffset("_MainTex", new Vector2(-.012f, 0));
            }

        }


    }




    public void FindDayStatus()
    {
        //sets an to the suns current rotation for comparison
        an = Quaternion.Angle(SunMoon.transform.rotation, Quaternion.identity);

        MoonUpdate();//update the moon status to check if it should change

        //update the an variable based on a 12 hour clock
        if ((AmPm == "am" && hours < 6) || (AmPm == "am" && hours == 12) || (AmPm == "pm" && hours >= 6 && hours != 12))
        {
            an = 180 + (180 - (an));
        }

        //this section is where all of the previous rotations are compared to decide what time-of-day it should be
        //There is a "+360" variable that MUST be attached to the status that has the lowest start rotation
        //this is because the largetst state will be comparing to the smallest state to find its order
        // i.e  355 compared to 2
        //
        //it is important in this instance that the smaller value be bumped up by the full 360 degrees of the circle when comparing
        //this only applies to this ONE instance however without it, it will not function properly
        if (an > MidnightStartRot && an < PreSunRiseStartRot)
        {
            if (DayStatus != "Midnight")
            {
                DayStatus = "Midnight";
                StopAllCoroutines();//If it is currently not the correct time-of-day, coroutines are stopped and the ChangeDayStatus Coroutine is run with the pre-defined variables
                StartCoroutine(ChangeDayStatus("Midnight", MidnightChangeRate, MidnightLightIntensity, MidnightLightColor, MidnightAmbientColor, MidnightLensFlare, MidnightStarAlpha, MidnightFogColor, MidnightFogDensity));
            }
        }
        if (an > PreSunRiseStartRot && an < (SunRiseStartRot))
        {
            if (DayStatus != "PreSunRise")
            {
                DayStatus = "PreSunRise";
                StopAllCoroutines();
                StartCoroutine(ChangeDayStatus("PreSunRise", PreSunRiseChangeRate, PreSunRiseLightIntensity, PreSunRiseLightColor, PreSunRiseAmbientColor, PreSunRiseLensFlare, PreSunRiseStarAlpha, PreSunRiseFogColor, PreSunRiseFogDensity));
            }
        }
        if (an > SunRiseStartRot && an < PostSunRiseStartRot + 360)
        {
            if (DayStatus != "SunRise")
            {
                DayStatus = "SunRise";
                StopAllCoroutines();
                StartCoroutine(ChangeDayStatus("SunRise", SunRiseChangeRate, SunRiseLightIntensity, SunRiseLightColor, SunRiseAmbientColor, SunRiseLensFlare, SunRiseStarAlpha, SunRiseFogColor, SunRiseFogDensity));
            }
        }
        if (an > PostSunRiseStartRot && an < AfternoonStartRot)
        {
            if (DayStatus != "PostSunRise")
            {
                DayStatus = "PostSunRise";
                StopAllCoroutines();
                StartCoroutine(ChangeDayStatus("PostSunRise", PostSunRiseChangeRate, PostSunRiseLightIntensity, PostSunRiseLightColor, PostSunRiseAmbientColor, PostSunRiseLensFlare, PostSunRiseStarAlpha, PostSunRiseFogColor, PostSunRiseFogDensity));
            }
        }
        if (an > AfternoonStartRot && an < PreSunSetStartRot)
        {
            if (DayStatus != "Afternoon")
            {
                DayStatus = "Afternoon";
                StopAllCoroutines();
                StartCoroutine(ChangeDayStatus("Afternoon", AfternoonChangeRate, AfternoonLightIntensity, AfternoonLightColor, AfternoonAmbientColor, AfternoonLensFlare, AfternoonStarAlpha, AfternoonFogColor, AfternoonFogDensity));
            }
        }
        if (an > PreSunSetStartRot && an < SunSetStartRot)
        {
            if (DayStatus != "PreSunSet")
            {
                DayStatus = "PreSunSet";
                StopAllCoroutines();
                StartCoroutine(ChangeDayStatus("PreSunSet", PreSunSetChangeRate, PreSunSetLightIntensity, PreSunSetLightColor, PreSunSetAmbientColor, PreSunSetLensFlare, PreSunSetStarAlpha, PreSunSetFogColor, PreSunSetFogDensity));
            }
        }
        if (an > SunSetStartRot && an < PostSunSetStartRot)
        {
            if (DayStatus != "SunSet")
            {
                DayStatus = "SunSet";
                StopAllCoroutines();
                StartCoroutine(ChangeDayStatus("SunSet", SunSetChangeRate, SunSetLightIntensity, SunSetLightColor, SunSetAmbientColor, SunSetLensFlare, SunSetStarAlpha, SunSetFogColor, SunSetFogDensity));
            }
        }
        if (an > PostSunSetStartRot && an < MidnightStartRot)
        {
            if (DayStatus != "PostSunSet")
            {
                DayStatus = "PostSunSet";
                StopAllCoroutines();
                StartCoroutine(ChangeDayStatus("PostSunSet", PostSunSetChangeRate, PostSunSetLightIntensity, PostSunSetLightColor, PostSunSetAmbientColor, PostSunSetLensFlare, PostSunSetStarAlpha, PostSunSetFogColor, PostSunSetFogDensity));
            }
        }
    }

   

    IEnumerator ChangeDayStatus(string name, float Speed, float LightIntensity, Color LightColor, Color AmbientColor, float LensFlare, float StarAlpha, Color FogColor, float FogDensity)
    {
        //if the instant change variable is set to true (as it is onStart) then the time-of-day will be set immediately to its correct colors, rather than fading to them
        if (instantchange == true)
        {
            //sets each of the variable to the time-of-day's settings
            RenderSettings.ambientLight = AmbientColor;
            SunLight.color = LightColor;
            SunLight.intensity = LightIntensity;
            LF.brightness = LensFlare;
            StarMat.SetFloat("_Cutoff", StarAlpha);
            RenderSettings.fogColor = FogColor;
            RenderSettings.fogDensity = FogDensity;


            instantchange = false;
            yield return new WaitForEndOfFrame();
        }

        else if (instantchange == false)
        {
            Debug.Log("ChangeStatus: " + name + " STARTED at:" + hours + ":" + minutes + " " + AmPm);
            //starting 'position' of each item so that the lerp works correctly
            li = SunLight.intensity;
            lc = SunLight.color;
            ac = RenderSettings.ambientLight;
            lf = LF.brightness;
            sa = StarMat.GetFloat("_Cutoff");
            fc = RenderSettings.fogColor;
            fd = RenderSettings.fogDensity;
            



            float t2 = 0;

            
            //starting position is needed so that the item can lerp at the same speed consistantly instead of having slowdown
            while (t2 < 1)
            {
                //this function simply increments the time while taking in the various variables and lerps each of the settings to the appropriate values
                t2 += Speed * DaySpeed * Time.deltaTime;
                if (RenderSettings.ambientLight != AmbientColor)
                {
                    Color a;
                    a = Color.Lerp(ac, AmbientColor, t2);
                    RenderSettings.ambientLight = a;
                }

                if (SunLight.intensity != LightIntensity)
                {
                    float a;
                    a = Mathf.Lerp(li, LightIntensity, t2);
                    SunLight.intensity = a;
                }

                if (SunLight.color != LightColor)
                {
                    Color a;
                    a = Color.Lerp(lc, LightColor, t2);
                    SunLight.color = a;
                }

                if (LF.brightness != LensFlare)
                {
                    float a;
                    a = Mathf.Lerp(lf, LensFlare, t2);
                    LF.brightness = a;
                }

                if (StarMat.GetFloat("_Cutoff") != StarAlpha)
                {
                    float a;
                    a = Mathf.Lerp(sa, StarAlpha, t2);
                    StarMat.SetFloat("_Cutoff", a);
                }

                if (RenderSettings.fogColor != FogColor)
                {
                    Color a;
                    a = Color.Lerp(fc, FogColor, t2);
                    RenderSettings.fogColor = a;
                }

                if (RenderSettings.fogDensity != FogDensity)
                {
                    float a;
                    a = Mathf.Lerp(fd, FogDensity, t2);
                    RenderSettings.fogDensity = a;
                }

                if (t2 >= 1)
                {
                    //once t2 has reached 1, as a failsafe, each of the variables are set to their end values
                    RenderSettings.ambientLight = AmbientColor;
                    SunLight.color = LightColor;
                    SunLight.intensity = LightIntensity;
                    LF.brightness = LensFlare;
                    StarMat.SetFloat("_Cutoff", StarAlpha);
                    RenderSettings.fogColor = FogColor;
                    RenderSettings.fogDensity = FogDensity;
                }

                yield return new WaitForEndOfFrame();
            }
            Debug.Log("ChangeStatus: " + name + " COMPLETED at:" + hours + ":" + minutes + " " + AmPm);
        }
    }
}
