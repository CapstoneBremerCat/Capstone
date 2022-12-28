using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public readonly struct TimeData
{
    private const int SecondsPerMinute = 60;
    private const int SecondsPerHour = 60 * SecondsPerMinute;
    private const int SecondsPerDay = 24 * SecondsPerHour;

    public int Day { get; }
    public int Hour { get; }
    public int Minute { get; }
    public int Second { get; }

    public TimeData(float time)
    {
        // first round the delta to int -> no floating point inaccuracies 
        // according to your needs you could also use Floor or Ceil here
        var seconds = Mathf.RoundToInt(time);

        // Now we can use the reminder free integer divisions to get the target values 
        Day = seconds / SecondsPerDay;
        seconds -= Day * SecondsPerDay;

        Hour = (int)(seconds / SecondsPerHour);
        seconds -= Hour * SecondsPerHour;

        Minute = seconds / SecondsPerMinute;
        seconds -= Minute * SecondsPerMinute;

        Second = seconds;
    }
}

public class PlayTime
{
    private float initTime;
    private float loadedTime;   // 저장된 이전 플레이타임 시간

    private float timeScale;    // 시간 배속

    public PlayTime()
    {
        SetTimeScale(1440.0f);       // 24 * 60. 하루 1분
        InitTime();
    }

    public TimeData GetTime()
    {
        // 흘러간 시간 * 시간 배속 + 저장된 시간
        return new TimeData((Time.time - initTime) * timeScale + loadedTime);
    }

    public void SetTimeScale(float scale)
    {
        timeScale = scale;
    }

    // 시간 정보 초기화.
    public void InitTime()
    {
        // DataManager에 저장되어있는 플레이타임을 가져온다.
        //if(DataManager) loadedTime = DataManager.Instance.GetSavedTime();
        //else
        loadedTime = 28800.0f;  //08시
        initTime = Time.time;
    }
}
