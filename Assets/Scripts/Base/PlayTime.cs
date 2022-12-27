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

        Hour = (int)(time / SecondsPerHour);
        seconds -= Hour * SecondsPerHour;

        Minute = seconds / SecondsPerMinute;
        seconds -= Minute * SecondsPerMinute;

        Second = seconds;
    }
}

public class PlayTime
{
    private float initTime;
    private float loadedTime;

    private float timeScale;

    public int Morning { get; private set; }    // 낮 시작 시간
    public int Night { get; private set; }      // 밤 시작 시간

    public PlayTime()
    {
        SetTimeScale(1440.0f);       // 24 * 60. 하루 1분
        SetMorningAndNight(8, 20);  // 낮은 8시부터, 밤은 20시부터 시작
        ResetTime();
    }

    public TimeData GetTime()
    {
        return new TimeData((Time.time - initTime) * timeScale);
    }

    public void SetTimeScale(float scale)
    {
        timeScale = scale;
    }

    // 낮, 밤 시작 시간 설정.
    public void SetMorningAndNight(int morning, int night)
    {
        Morning = morning;
        Night = night;
    }

    // 시간 정보 초기화.
    public void ResetTime()
    {
        //if(DataManager) loadedTime = DataManager.Instance.GetSavedTime();
        //else loadedTime = 0.0f;
        initTime = Time.time + loadedTime;
    }
}
