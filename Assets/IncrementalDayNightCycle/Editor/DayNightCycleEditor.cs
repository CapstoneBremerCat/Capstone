using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(DayNightCycle))]

public class DayNightCycleEditor : Editor
{
    SerializedProperty curve;
    SerializedProperty WaitTimeMultiplier;
    SerializedProperty ShortYear;
    SerializedProperty MoveSunContinuously;
    SerializedProperty MoveSunAfter;
    SerializedProperty isLerping;
    SerializedProperty isLeapYear;
    SerializedProperty Sun;
    SerializedProperty SunLight;
    SerializedProperty LF;
    SerializedProperty Moon;
    SerializedProperty SunMoon;
    SerializedProperty DayStatus;
    SerializedProperty hours;
    SerializedProperty minutes;
    SerializedProperty CurrentDayName;
    SerializedProperty Day1Name;
    SerializedProperty Day2Name;
    SerializedProperty Day3Name;
    SerializedProperty Day4Name;
    SerializedProperty Day5Name;
    SerializedProperty Day6Name;
    SerializedProperty Day7Name;
    SerializedProperty CurrentMonthName;
    SerializedProperty Month1Name;
    SerializedProperty Month2Name;
    SerializedProperty Month3Name;
    SerializedProperty Month4Name;
    SerializedProperty Month5Name;
    SerializedProperty Month6Name;
    SerializedProperty Month7Name;
    SerializedProperty Month8Name;
    SerializedProperty Month9Name;
    SerializedProperty Month10Name;
    SerializedProperty Month11Name;
    SerializedProperty Month12Name;
    SerializedProperty DayNum;
    SerializedProperty MonthNum;
    SerializedProperty YearNum;
    SerializedProperty Days;
    SerializedProperty AmPm;
    SerializedProperty DaySpeed;
    SerializedProperty starsphere;
    SerializedProperty StartHourMidnight;
    SerializedProperty StartMinMidnight;
    SerializedProperty StartAmPmMidnight;
    SerializedProperty MidnightStartRot;
    SerializedProperty MidnightChangeRate;
    SerializedProperty MidnightLensFlare;
    SerializedProperty MidnightLightIntensity;
    SerializedProperty MidnightLightColor;
    SerializedProperty MidnightAmbientColor;
    SerializedProperty MidnightStarAlpha;
    SerializedProperty MidnightFogColor;
    SerializedProperty MidnightFogDensity;
    SerializedProperty StartHourPreSunRise;
    SerializedProperty StartMinPreSunRise;
    SerializedProperty StartAmPmPreSunRise;
    SerializedProperty PreSunRiseStartRot;
    SerializedProperty PreSunRiseChangeRate;
    SerializedProperty PreSunRiseLensFlare;
    SerializedProperty PreSunRiseLightIntensity;
    SerializedProperty PreSunRiseLightColor;
    SerializedProperty PreSunRiseAmbientColor;
    SerializedProperty PreSunRiseStarAlpha;
    SerializedProperty PreSunRiseFogColor;
    SerializedProperty PreSunRiseFogDensity;
    SerializedProperty StartHourSunRise;
    SerializedProperty StartMinSunRise;
    SerializedProperty StartAmPmSunRise;
    SerializedProperty SunRiseStartRot;
    SerializedProperty SunRiseChangeRate;
    SerializedProperty SunRiseLensFlare;
    SerializedProperty SunRiseLightIntensity;
    SerializedProperty SunRiseLightColor;
    SerializedProperty SunRiseAmbientColor;
    SerializedProperty SunRiseStarAlpha;
    SerializedProperty SunRiseFogColor;
    SerializedProperty SunRiseFogDensity;
    SerializedProperty StartHourPostSunRise;
    SerializedProperty StartMinPostSunRise;
    SerializedProperty StartAmPmPostSunRise;
    SerializedProperty PostSunRiseStartRot;
    SerializedProperty PostSunRiseChangeRate;
    SerializedProperty PostSunRiseLensFlare;
    SerializedProperty PostSunRiseLightIntensity;
    SerializedProperty PostSunRiseLightColor;
    SerializedProperty PostSunRiseAmbientColor;
    SerializedProperty PostSunRiseStarAlpha;
    SerializedProperty PostSunRiseFogColor;
    SerializedProperty PostSunRiseFogDensity;
    SerializedProperty StartHourAfternoon;
    SerializedProperty StartMinAfternoon;
    SerializedProperty StartAmPmAfternoon;
    SerializedProperty AfternoonStartRot;
    SerializedProperty AfternoonChangeRate;
    SerializedProperty AfternoonLensFlare;
    SerializedProperty AfternoonLightIntensity;
    SerializedProperty AfternoonLightColor;
    SerializedProperty AfternoonAmbientColor;
    SerializedProperty AfternoonStarAlpha;
    SerializedProperty AfternoonFogColor;
    SerializedProperty AfternoonFogDensity;
    SerializedProperty StartHourPreSunSet;
    SerializedProperty StartMinPreSunSet;
    SerializedProperty StartAmPmPreSunSet;
    SerializedProperty PreSunSetStartRot;
    SerializedProperty PreSunSetChangeRate;
    SerializedProperty PreSunSetLensFlare;
    SerializedProperty PreSunSetLightIntensity;
    SerializedProperty PreSunSetLightColor;
    SerializedProperty PreSunSetAmbientColor;
    SerializedProperty PreSunSetStarAlpha;
    SerializedProperty PreSunSetFogColor;
    SerializedProperty PreSunSetFogDensity;
    SerializedProperty StartHourSunSet;
    SerializedProperty StartMinSunSet;
    SerializedProperty StartAmPmSunSet;
    SerializedProperty SunSetStartRot;
    SerializedProperty SunSetChangeRate;
    SerializedProperty SunSetLensFlare;
    SerializedProperty SunSetLightIntensity;
    SerializedProperty SunSetLightColor;
    SerializedProperty SunSetAmbientColor;
    SerializedProperty SunSetStarAlpha;
    SerializedProperty SunSetFogColor;
    SerializedProperty SunSetFogDensity;
    SerializedProperty StartHourPostSunSet;
    SerializedProperty StartMinPostSunSet;
    SerializedProperty StartAmPmPostSunSet;
    SerializedProperty PostSunSetStartRot;
    SerializedProperty PostSunSetChangeRate;
    SerializedProperty PostSunSetLensFlare;
    SerializedProperty PostSunSetLightIntensity;
    SerializedProperty PostSunSetLightColor;
    SerializedProperty PostSunSetAmbientColor;
    SerializedProperty PostSunSetStarAlpha;
    SerializedProperty PostSunSetFogColor;
    SerializedProperty PostSunSetFogDensity;
    SerializedProperty mp1;
    SerializedProperty mp2;
    SerializedProperty mp3;
    SerializedProperty mp4;
    SerializedProperty mp5;
    SerializedProperty mp6;
    SerializedProperty mp7;
    SerializedProperty mp8;
    SerializedProperty MoonScale;
    SerializedProperty StartingPhase;


    bool CycleFoldout;
    bool GameObjectsFoldout;
    bool DateInfoFoldout;
    bool TimeOfDayFoldout;
    bool MoonFoldout;

    bool DayFoldout;
    bool MonthFoldout;

    bool MidnightFoldout;
    bool PreSunRiseFoldout;
    bool SunRiseFoldout;
    bool PostSunRiseFoldout;
    bool AfternoonFoldout;
    bool PreSunSetFoldout;
    bool SunSetFoldout;
    bool PostSunSetFoldout;
    bool Increment;

    void OnEnable()
    {
        StartingPhase = serializedObject.FindProperty("StartingPhase");
        curve = serializedObject.FindProperty("curve");
        WaitTimeMultiplier = serializedObject.FindProperty("WaitTimeMultiplier");
        ShortYear = serializedObject.FindProperty("ShortYear");
        MoveSunContinuously = serializedObject.FindProperty("MoveSunContinuously");
        MoveSunAfter = serializedObject.FindProperty("MoveSunAfter");
        isLerping = serializedObject.FindProperty("isLerping");
        isLeapYear = serializedObject.FindProperty("isLeapYear");
        Sun = serializedObject.FindProperty("Sun");
        SunLight = serializedObject.FindProperty("SunLight");
        LF = serializedObject.FindProperty("LF");
        Moon = serializedObject.FindProperty("Moon");
        SunMoon = serializedObject.FindProperty("SunMoon");
        DayStatus = serializedObject.FindProperty("DayStatus");
        hours = serializedObject.FindProperty("hours");
        minutes = serializedObject.FindProperty("minutes");
        CurrentDayName = serializedObject.FindProperty("CurrentDayName");
        Day1Name = serializedObject.FindProperty("Day1Name");
        Day2Name = serializedObject.FindProperty("Day2Name");
        Day3Name = serializedObject.FindProperty("Day3Name");
        Day4Name = serializedObject.FindProperty("Day4Name");
        Day5Name = serializedObject.FindProperty("Day5Name");
        Day6Name = serializedObject.FindProperty("Day6Name");
        Day7Name = serializedObject.FindProperty("Day7Name");
        CurrentMonthName = serializedObject.FindProperty("CurrentMonthName");
        Month1Name = serializedObject.FindProperty("Month1Name");
        Month2Name = serializedObject.FindProperty("Month2Name");
        Month3Name = serializedObject.FindProperty("Month3Name");
        Month4Name = serializedObject.FindProperty("Month4Name");
        Month5Name = serializedObject.FindProperty("Month5Name");
        Month6Name = serializedObject.FindProperty("Month6Name");
        Month7Name = serializedObject.FindProperty("Month7Name");
        Month8Name = serializedObject.FindProperty("Month8Name");
        Month9Name = serializedObject.FindProperty("Month9Name");
        Month10Name = serializedObject.FindProperty("Month10Name");
        Month11Name = serializedObject.FindProperty("Month11Name");
        Month12Name = serializedObject.FindProperty("Month12Name");
        DayNum = serializedObject.FindProperty("DayNum");
        MonthNum = serializedObject.FindProperty("MonthNum");
        YearNum = serializedObject.FindProperty("YearNum");
        Days = serializedObject.FindProperty("Days");
        AmPm = serializedObject.FindProperty("AmPm");
        DaySpeed = serializedObject.FindProperty("DaySpeed");
        starsphere = serializedObject.FindProperty("starsphere");
        StartHourMidnight = serializedObject.FindProperty("StartHourMidnight");
        StartMinMidnight = serializedObject.FindProperty("StartMinMidnight");
        StartAmPmMidnight = serializedObject.FindProperty("StartAmPmMidnight");
        MidnightStartRot = serializedObject.FindProperty("MidnightStartRot");
        MidnightChangeRate = serializedObject.FindProperty("MidnightChangeRate");
        MidnightLensFlare = serializedObject.FindProperty("MidnightLensFlare");
        MidnightLightIntensity = serializedObject.FindProperty("MidnightLightIntensity");
        MidnightLightColor = serializedObject.FindProperty("MidnightLightColor");
        MidnightAmbientColor = serializedObject.FindProperty("MidnightAmbientColor");
        MidnightStarAlpha = serializedObject.FindProperty("MidnightStarAlpha");
        MidnightFogColor = serializedObject.FindProperty("MidnightFogColor");
        MidnightFogDensity = serializedObject.FindProperty("MidnightFogDensity");
        StartHourPreSunRise = serializedObject.FindProperty("StartHourPreSunRise");
        StartMinPreSunRise = serializedObject.FindProperty("StartMinPreSunRise");
        StartAmPmPreSunRise = serializedObject.FindProperty("StartAmPmPreSunRise");
        PreSunRiseStartRot = serializedObject.FindProperty("PreSunRiseStartRot");
        PreSunRiseChangeRate = serializedObject.FindProperty("PreSunRiseChangeRate");
        PreSunRiseLensFlare = serializedObject.FindProperty("PreSunRiseLensFlare");
        PreSunRiseLightIntensity = serializedObject.FindProperty("PreSunRiseLightIntensity");
        PreSunRiseLightColor = serializedObject.FindProperty("PreSunRiseLightColor");
        PreSunRiseAmbientColor = serializedObject.FindProperty("PreSunRiseAmbientColor");
        PreSunRiseStarAlpha = serializedObject.FindProperty("PreSunRiseStarAlpha");
        PreSunRiseFogColor = serializedObject.FindProperty("PreSunRiseFogColor");
        PreSunRiseFogDensity = serializedObject.FindProperty("PreSunRiseFogDensity");
        StartHourSunRise = serializedObject.FindProperty("StartHourSunRise");
        StartMinSunRise = serializedObject.FindProperty("StartMinSunRise");
        StartAmPmSunRise = serializedObject.FindProperty("StartAmPmSunRise");
        SunRiseStartRot = serializedObject.FindProperty("SunRiseStartRot");
        SunRiseChangeRate = serializedObject.FindProperty("SunRiseChangeRate");
        SunRiseLensFlare = serializedObject.FindProperty("SunRiseLensFlare");
        SunRiseLightIntensity = serializedObject.FindProperty("SunRiseLightIntensity");
        SunRiseLightColor = serializedObject.FindProperty("SunRiseLightColor");
        SunRiseAmbientColor = serializedObject.FindProperty("SunRiseAmbientColor");
        SunRiseStarAlpha = serializedObject.FindProperty("SunRiseStarAlpha");
        SunRiseFogColor = serializedObject.FindProperty("SunRiseFogColor");
        SunRiseFogDensity = serializedObject.FindProperty("SunRiseFogDensity");
        StartHourPostSunRise = serializedObject.FindProperty("StartHourPostSunRise");
        StartMinPostSunRise = serializedObject.FindProperty("StartMinPostSunRise");
        StartAmPmPostSunRise = serializedObject.FindProperty("StartAmPmPostSunRise");
        PostSunRiseStartRot = serializedObject.FindProperty("PostSunRiseStartRot");
        PostSunRiseChangeRate = serializedObject.FindProperty("PostSunRiseChangeRate");
        PostSunRiseLensFlare = serializedObject.FindProperty("PostSunRiseLensFlare");
        PostSunRiseLightIntensity = serializedObject.FindProperty("PostSunRiseLightIntensity");
        PostSunRiseLightColor = serializedObject.FindProperty("PostSunRiseLightColor");
        PostSunRiseAmbientColor = serializedObject.FindProperty("PostSunRiseAmbientColor");
        PostSunRiseStarAlpha = serializedObject.FindProperty("PostSunRiseStarAlpha");
        PostSunRiseFogColor = serializedObject.FindProperty("PostSunRiseFogColor");
        PostSunRiseFogDensity = serializedObject.FindProperty("PostSunRiseFogDensity");
        StartHourAfternoon = serializedObject.FindProperty("StartHourAfternoon");
        StartMinAfternoon = serializedObject.FindProperty("StartMinAfternoon");
        StartAmPmAfternoon = serializedObject.FindProperty("StartAmPmAfternoon");
        AfternoonStartRot = serializedObject.FindProperty("AfternoonStartRot");
        AfternoonChangeRate = serializedObject.FindProperty("AfternoonChangeRate");
        AfternoonLensFlare = serializedObject.FindProperty("AfternoonLensFlare");
        AfternoonLightIntensity = serializedObject.FindProperty("AfternoonLightIntensity");
        AfternoonLightColor = serializedObject.FindProperty("AfternoonLightColor");
        AfternoonAmbientColor = serializedObject.FindProperty("AfternoonAmbientColor");
        AfternoonStarAlpha = serializedObject.FindProperty("AfternoonStarAlpha");
        AfternoonFogColor = serializedObject.FindProperty("AfternoonFogColor");
        AfternoonFogDensity = serializedObject.FindProperty("AfternoonFogDensity");
        StartHourPreSunSet = serializedObject.FindProperty("StartHourPreSunSet");
        StartMinPreSunSet = serializedObject.FindProperty("StartMinPreSunSet");
        StartAmPmPreSunSet = serializedObject.FindProperty("StartAmPmPreSunSet");
        PreSunSetStartRot = serializedObject.FindProperty("PreSunSetStartRot");
        PreSunSetChangeRate = serializedObject.FindProperty("PreSunSetChangeRate");
        PreSunSetLensFlare = serializedObject.FindProperty("PreSunSetLensFlare");
        PreSunSetLightIntensity = serializedObject.FindProperty("PreSunSetLightIntensity");
        PreSunSetLightColor = serializedObject.FindProperty("PreSunSetLightColor");
        PreSunSetAmbientColor = serializedObject.FindProperty("PreSunSetAmbientColor");
        PreSunSetStarAlpha = serializedObject.FindProperty("PreSunSetStarAlpha");
        PreSunSetFogColor = serializedObject.FindProperty("PreSunSetFogColor");
        PreSunSetFogDensity = serializedObject.FindProperty("PreSunSetFogDensity");
        StartHourSunSet = serializedObject.FindProperty("StartHourSunSet");
        StartMinSunSet = serializedObject.FindProperty("StartMinSunSet");
        StartAmPmSunSet = serializedObject.FindProperty("StartAmPmSunSet");
        SunSetStartRot = serializedObject.FindProperty("SunSetStartRot");
        SunSetChangeRate = serializedObject.FindProperty("SunSetChangeRate");
        SunSetLensFlare = serializedObject.FindProperty("SunSetLensFlare");
        SunSetLightIntensity = serializedObject.FindProperty("SunSetLightIntensity");
        SunSetLightColor = serializedObject.FindProperty("SunSetLightColor");
        SunSetAmbientColor = serializedObject.FindProperty("SunSetAmbientColor");
        SunSetStarAlpha = serializedObject.FindProperty("SunSetStarAlpha");
        SunSetFogColor = serializedObject.FindProperty("SunSetFogColor");
        SunSetFogDensity = serializedObject.FindProperty("SunSetFogDensity");
        StartHourPostSunSet = serializedObject.FindProperty("StartHourPostSunSet");
        StartMinPostSunSet = serializedObject.FindProperty("StartMinPostSunSet");
        StartAmPmPostSunSet = serializedObject.FindProperty("StartAmPmPostSunSet");
        PostSunSetStartRot = serializedObject.FindProperty("PostSunSetStartRot");
        PostSunSetChangeRate = serializedObject.FindProperty("PostSunSetChangeRate");
        PostSunSetLensFlare = serializedObject.FindProperty("PostSunSetLensFlare");
        PostSunSetLightIntensity = serializedObject.FindProperty("PostSunSetLightIntensity");
        PostSunSetLightColor = serializedObject.FindProperty("PostSunSetLightColor");
        PostSunSetAmbientColor = serializedObject.FindProperty("PostSunSetAmbientColor");
        PostSunSetStarAlpha = serializedObject.FindProperty("PostSunSetStarAlpha");
        PostSunSetFogColor = serializedObject.FindProperty("PostSunSetFogColor");
        PostSunSetFogDensity = serializedObject.FindProperty("PostSunSetFogDensity");
        mp1 = serializedObject.FindProperty("mp1");
        mp2 = serializedObject.FindProperty("mp2");
        mp3 = serializedObject.FindProperty("mp3");
        mp4 = serializedObject.FindProperty("mp4");
        mp5 = serializedObject.FindProperty("mp5");
        mp6 = serializedObject.FindProperty("mp6");
        mp7 = serializedObject.FindProperty("mp7");
        mp8 = serializedObject.FindProperty("mp8");
        MoonScale = serializedObject.FindProperty("MoonScale");


    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        myFoldoutStyle.fontStyle = FontStyle.Bold;
        myFoldoutStyle.fontSize = 15;

        GUIStyle myFoldoutStyle2 = new GUIStyle(EditorStyles.foldout);
        myFoldoutStyle2.fontStyle = FontStyle.Bold;

        CycleFoldout = EditorGUILayout.Foldout(CycleFoldout, "Cycle Settings", true, myFoldoutStyle);
        if (CycleFoldout)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(DaySpeed);
            EditorGUILayout.PropertyField(MoveSunContinuously);
            Increment = MoveSunContinuously.boolValue;
            if (!Increment)
                if (Selection.activeTransform)
                {
                    EditorGUI.indentLevel = 2;
                    EditorGUILayout.PropertyField(MoveSunAfter);
                    EditorGUILayout.PropertyField(WaitTimeMultiplier);
                    EditorGUILayout.PropertyField(curve);
                    EditorGUILayout.PropertyField(isLerping);
                    EditorGUILayout.Space();
                }
            EditorGUI.indentLevel = 0;

            EditorGUILayout.Space();
        }
        EditorGUI.indentLevel = 0;






        DateInfoFoldout = EditorGUILayout.Foldout(DateInfoFoldout, "Date & Time Info", true, myFoldoutStyle);
        if (DateInfoFoldout)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(hours);
            EditorGUILayout.PropertyField(minutes);
            EditorGUILayout.PropertyField(AmPm);
            EditorGUILayout.PropertyField(DayNum);
            EditorGUILayout.PropertyField(MonthNum);
            EditorGUILayout.PropertyField(YearNum);
            EditorGUILayout.Space();


            EditorGUILayout.PropertyField(ShortYear);
            EditorGUILayout.PropertyField(isLeapYear);
            EditorGUILayout.PropertyField(DayStatus);
            EditorGUILayout.PropertyField(Days);
            EditorGUILayout.PropertyField(CurrentDayName);
            DayFoldout = EditorGUILayout.Foldout(DayFoldout, "Day Names", true, myFoldoutStyle2);
            if (DayFoldout)
                if (Selection.activeTransform)
                {
                    EditorGUI.indentLevel = 2;
                    EditorGUILayout.PropertyField(Day1Name);
                    EditorGUILayout.PropertyField(Day2Name);
                    EditorGUILayout.PropertyField(Day3Name);
                    EditorGUILayout.PropertyField(Day4Name);
                    EditorGUILayout.PropertyField(Day5Name);
                    EditorGUILayout.PropertyField(Day6Name);
                    EditorGUILayout.PropertyField(Day7Name);
                    EditorGUILayout.Space();
                }

            EditorGUILayout.PropertyField(CurrentMonthName);

            MonthFoldout = EditorGUILayout.Foldout(MonthFoldout, "Month Names", true, myFoldoutStyle2);
            if (MonthFoldout)
                if (Selection.activeTransform)
                {
                    EditorGUI.indentLevel = 2;
                    EditorGUILayout.PropertyField(Month1Name);
                    EditorGUILayout.PropertyField(Month2Name);
                    EditorGUILayout.PropertyField(Month3Name);
                    EditorGUILayout.PropertyField(Month4Name);
                    EditorGUILayout.PropertyField(Month5Name);
                    EditorGUILayout.PropertyField(Month6Name);
                    EditorGUILayout.PropertyField(Month7Name);
                    EditorGUILayout.PropertyField(Month8Name);
                    EditorGUILayout.PropertyField(Month9Name);
                    EditorGUILayout.PropertyField(Month10Name);
                    EditorGUILayout.PropertyField(Month11Name);
                    EditorGUILayout.PropertyField(Month12Name);
                    EditorGUILayout.Space();
                }


            EditorGUILayout.Space();
        }
        EditorGUI.indentLevel = 0;

        TimeOfDayFoldout = EditorGUILayout.Foldout(TimeOfDayFoldout, "Time of Day Settings", true, myFoldoutStyle);
        if (TimeOfDayFoldout)
        {
            EditorGUI.indentLevel = 1;
            MidnightFoldout = EditorGUILayout.Foldout(MidnightFoldout, "Midnight Settings", true, myFoldoutStyle2);
            if (MidnightFoldout)
                if (Selection.activeTransform)
                {
                    EditorGUI.indentLevel = 2;
                    EditorGUILayout.PropertyField(StartHourMidnight);
                    EditorGUILayout.PropertyField(StartMinMidnight);
                    EditorGUILayout.PropertyField(StartAmPmMidnight);
                    EditorGUILayout.PropertyField(MidnightChangeRate);
                    EditorGUILayout.PropertyField(MidnightLensFlare);
                    EditorGUILayout.PropertyField(MidnightLightIntensity);
                    EditorGUILayout.PropertyField(MidnightLightColor);
                    EditorGUILayout.PropertyField(MidnightAmbientColor);
                    EditorGUILayout.PropertyField(MidnightStarAlpha);
                    EditorGUILayout.PropertyField(MidnightFogColor);
                    EditorGUILayout.PropertyField(MidnightFogDensity);
                    EditorGUILayout.Space();
                }


            EditorGUI.indentLevel = 1;
            PreSunRiseFoldout = EditorGUILayout.Foldout(PreSunRiseFoldout, "Pre-Sunrise Settings", true, myFoldoutStyle2);
            if (PreSunRiseFoldout)
                if (Selection.activeTransform)
                {
                    EditorGUI.indentLevel = 2;
                    EditorGUILayout.PropertyField(StartHourPreSunRise);
                    EditorGUILayout.PropertyField(StartMinPreSunRise);
                    EditorGUILayout.PropertyField(StartAmPmPreSunRise);
                    EditorGUILayout.PropertyField(PreSunRiseChangeRate);
                    EditorGUILayout.PropertyField(PreSunRiseLensFlare);
                    EditorGUILayout.PropertyField(PreSunRiseLightIntensity);
                    EditorGUILayout.PropertyField(PreSunRiseLightColor);
                    EditorGUILayout.PropertyField(PreSunRiseAmbientColor);
                    EditorGUILayout.PropertyField(PreSunRiseStarAlpha);
                    EditorGUILayout.PropertyField(PreSunRiseFogColor);
                    EditorGUILayout.PropertyField(PreSunRiseFogDensity);
                    EditorGUILayout.Space();
                }


            EditorGUI.indentLevel = 1;
            SunRiseFoldout = EditorGUILayout.Foldout(SunRiseFoldout, "Sunrise Settings", true, myFoldoutStyle2);
            if (SunRiseFoldout)
                if (Selection.activeTransform)
                {
                    EditorGUI.indentLevel = 2;
                    EditorGUILayout.PropertyField(StartHourSunRise);
                    EditorGUILayout.PropertyField(StartMinSunRise);
                    EditorGUILayout.PropertyField(StartAmPmSunRise);
                    EditorGUILayout.PropertyField(SunRiseChangeRate);
                    EditorGUILayout.PropertyField(SunRiseLensFlare);
                    EditorGUILayout.PropertyField(SunRiseLightIntensity);
                    EditorGUILayout.PropertyField(SunRiseLightColor);
                    EditorGUILayout.PropertyField(SunRiseAmbientColor);
                    EditorGUILayout.PropertyField(SunRiseStarAlpha);
                    EditorGUILayout.PropertyField(SunRiseFogColor);
                    EditorGUILayout.PropertyField(SunRiseFogDensity);
                    EditorGUILayout.Space();
                }


            EditorGUI.indentLevel = 1;
            PostSunRiseFoldout = EditorGUILayout.Foldout(PostSunRiseFoldout, "Post-Sunrise Settings", true, myFoldoutStyle2);
            if (PostSunRiseFoldout)
                if (Selection.activeTransform)
                {
                    EditorGUI.indentLevel = 2;
                    EditorGUILayout.PropertyField(StartHourPostSunRise);
                    EditorGUILayout.PropertyField(StartMinPostSunRise);
                    EditorGUILayout.PropertyField(StartAmPmPostSunRise);
                    EditorGUILayout.PropertyField(PostSunRiseChangeRate);
                    EditorGUILayout.PropertyField(PostSunRiseLensFlare);
                    EditorGUILayout.PropertyField(PostSunRiseLightIntensity);
                    EditorGUILayout.PropertyField(PostSunRiseLightColor);
                    EditorGUILayout.PropertyField(PostSunRiseAmbientColor);
                    EditorGUILayout.PropertyField(PostSunRiseStarAlpha);
                    EditorGUILayout.PropertyField(PostSunRiseFogColor);
                    EditorGUILayout.PropertyField(PostSunRiseFogDensity);
                    EditorGUILayout.Space();
                }



            EditorGUI.indentLevel = 1;
            AfternoonFoldout = EditorGUILayout.Foldout(AfternoonFoldout, "Afternoon Settings", true, myFoldoutStyle2);
            if (AfternoonFoldout)
                if (Selection.activeTransform)
                {
                    EditorGUI.indentLevel = 2;
                    EditorGUILayout.PropertyField(StartHourAfternoon);
                    EditorGUILayout.PropertyField(StartMinAfternoon);
                    EditorGUILayout.PropertyField(StartAmPmAfternoon);
                    EditorGUILayout.PropertyField(AfternoonChangeRate);
                    EditorGUILayout.PropertyField(AfternoonLensFlare);
                    EditorGUILayout.PropertyField(AfternoonLightIntensity);
                    EditorGUILayout.PropertyField(AfternoonLightColor);
                    EditorGUILayout.PropertyField(AfternoonAmbientColor);
                    EditorGUILayout.PropertyField(AfternoonStarAlpha);
                    EditorGUILayout.PropertyField(AfternoonFogColor);
                    EditorGUILayout.PropertyField(AfternoonFogDensity);
                    EditorGUILayout.Space();
                }



            EditorGUI.indentLevel = 1;
            PreSunSetFoldout = EditorGUILayout.Foldout(PreSunSetFoldout, "Pre-Sunset Settings", true, myFoldoutStyle2);
            if (PreSunSetFoldout)
                if (Selection.activeTransform)
                {
                    EditorGUI.indentLevel = 2;
                    EditorGUILayout.PropertyField(StartHourPreSunSet);
                    EditorGUILayout.PropertyField(StartMinPreSunSet);
                    EditorGUILayout.PropertyField(StartAmPmPreSunSet);
                    EditorGUILayout.PropertyField(PreSunSetChangeRate);
                    EditorGUILayout.PropertyField(PreSunSetLensFlare);
                    EditorGUILayout.PropertyField(PreSunSetLightIntensity);
                    EditorGUILayout.PropertyField(PreSunSetLightColor);
                    EditorGUILayout.PropertyField(PreSunSetAmbientColor);
                    EditorGUILayout.PropertyField(PreSunSetStarAlpha);
                    EditorGUILayout.PropertyField(PreSunSetFogColor);
                    EditorGUILayout.PropertyField(PreSunSetFogDensity);
                    EditorGUILayout.Space();
                }


            EditorGUI.indentLevel = 1;
            SunSetFoldout = EditorGUILayout.Foldout(SunSetFoldout, "Sunset Settings", true, myFoldoutStyle2);
            if (SunSetFoldout)
                if (Selection.activeTransform)
                {
                    EditorGUI.indentLevel = 2;
                    EditorGUILayout.PropertyField(StartHourSunSet);
                    EditorGUILayout.PropertyField(StartMinSunSet);
                    EditorGUILayout.PropertyField(StartAmPmSunSet);
                    EditorGUILayout.PropertyField(SunSetChangeRate);
                    EditorGUILayout.PropertyField(SunSetLensFlare);
                    EditorGUILayout.PropertyField(SunSetLightIntensity);
                    EditorGUILayout.PropertyField(SunSetLightColor);
                    EditorGUILayout.PropertyField(SunSetAmbientColor);
                    EditorGUILayout.PropertyField(SunSetStarAlpha);
                    EditorGUILayout.PropertyField(SunSetFogColor);
                    EditorGUILayout.PropertyField(SunSetFogDensity);
                    EditorGUILayout.Space();
                }


            EditorGUI.indentLevel = 1;
            PostSunSetFoldout = EditorGUILayout.Foldout(PostSunSetFoldout, "Post-Sunset Settings", true, myFoldoutStyle2);
            if (PostSunSetFoldout)
                if (Selection.activeTransform)
                {
                    EditorGUI.indentLevel = 2;
                    EditorGUILayout.PropertyField(StartHourPostSunSet);
                    EditorGUILayout.PropertyField(StartMinPostSunSet);
                    EditorGUILayout.PropertyField(StartAmPmPostSunSet);
                    EditorGUILayout.PropertyField(PostSunSetChangeRate);
                    EditorGUILayout.PropertyField(PostSunSetLensFlare);
                    EditorGUILayout.PropertyField(PostSunSetLightIntensity);
                    EditorGUILayout.PropertyField(PostSunSetLightColor);
                    EditorGUILayout.PropertyField(PostSunSetAmbientColor);
                    EditorGUILayout.PropertyField(PostSunSetStarAlpha);
                    EditorGUILayout.PropertyField(PostSunSetFogColor);
                    EditorGUILayout.PropertyField(PostSunSetFogDensity);
                    EditorGUILayout.Space();
                }
            EditorGUILayout.Space();
        }
        EditorGUI.indentLevel = 0;
        

        


        EditorGUI.indentLevel = 0;

        MoonFoldout = EditorGUILayout.Foldout(MoonFoldout, "Moon Settings", true, myFoldoutStyle);
        if (MoonFoldout)
            if (Selection.activeTransform)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(StartingPhase);
                EditorGUILayout.PropertyField(mp1);
                EditorGUILayout.PropertyField(mp2);
                EditorGUILayout.PropertyField(mp3);
                EditorGUILayout.PropertyField(mp4);
                EditorGUILayout.PropertyField(mp5);
                EditorGUILayout.PropertyField(mp6);
                EditorGUILayout.PropertyField(mp7);
                EditorGUILayout.PropertyField(mp8);
                EditorGUILayout.PropertyField(MoonScale);
                EditorGUILayout.Space();
            }
        EditorGUI.indentLevel = 0;

        GameObjectsFoldout = EditorGUILayout.Foldout(GameObjectsFoldout, "Game Objects", true, myFoldoutStyle);
        if (GameObjectsFoldout)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(Sun);
            EditorGUILayout.PropertyField(SunLight);
            EditorGUILayout.PropertyField(LF);
            EditorGUILayout.PropertyField(Moon);
            EditorGUILayout.PropertyField(SunMoon);
            EditorGUILayout.PropertyField(starsphere);
            EditorGUILayout.Space();
        }
        EditorGUI.indentLevel = 0;


        serializedObject.ApplyModifiedProperties();
    }

}
