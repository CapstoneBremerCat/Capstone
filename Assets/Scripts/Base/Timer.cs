using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
public readonly struct TimeData
{
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
}*/

public class Timer
{
    private const int SecondsPerMinute = 60;
    private const int SecondsPerHour = 60 * SecondsPerMinute;
    private const int SecondsPerDay = 24 * SecondsPerHour;

    public int Day { get; private set; }
    public int Hour { get; private set; }
    public int Minute { get; private set; }
    public int Second { get; private set; }
    public float SavedTimeData { get; private set; }     // 저장하기 위한 시간 데이터

    private float initTime;
    private float loadedTime;       // 저장된 이전 플레이타임 시간
    private float modifiedTime;     // 인게임 중 수정된 플레이타임 시간
    private float timeScale;        // 시간 배속

    public Timer()
    {
        SetTimeScale(1440.0f);       // 24 * 60. 하루 1분
        //SetTimeScale(8640.0f);       // 24 * 60 * 6. 하루 10초
        InitTime();
    }

    // 시간 갱신
    public void UpdateTime()
    {
        // 흘러간 시간 * 시간 배속 + 저장된 시간
        SavedTimeData = (Time.time - initTime) * timeScale + loadedTime + modifiedTime;

        var seconds = Mathf.RoundToInt(SavedTimeData);

        // Now we can use the reminder free integer divisions to get the target values 
        Day = seconds / SecondsPerDay;
        seconds -= Day * SecondsPerDay;

        Hour = (int)(seconds / SecondsPerHour);
        seconds -= Hour * SecondsPerHour;

        Minute = seconds / SecondsPerMinute;
        seconds -= Minute * SecondsPerMinute;

        Second = seconds;
    }

    // 기존 시간에서 게임시간 추가
    public void AddTime(int day, int hour, int minute, int seconds)
    {
        modifiedTime += day * SecondsPerDay
            + hour * SecondsPerHour
            + minute * SecondsPerMinute 
            + seconds;
    }

    // 지정한 시간으로 게임시간 변경
    public void SetTime(int day, int hour, int minute, int seconds)
    {
        initTime = Time.time;

        // 주의! modifiedTime은 초기화.
        modifiedTime = 0.0f;

        // 저장된 시간을 변경한다.
        SetLoadedTime(day* SecondsPerDay
            + hour * SecondsPerHour
            + minute * SecondsPerMinute
            + seconds);
    }

    // 게임 배속 설정
    public void SetTimeScale(float scale)
    {
        timeScale = scale;
    }

    // 저장된 시간 설정
    public void SetLoadedTime(float time)
    {
        loadedTime = time;
    }

    // 시간 정보 초기화.
    public void InitTime()
    {
        // DataManager에 저장되어있는 플레이타임을 가져온다. 이건 매개변수로 가져오는게 나을듯
        //if(DataManager) SetLoadedTime(DataManager.Instance.GetSavedTimeData());
        //else
        loadedTime = 28800.0f;  //08시
        modifiedTime = 0.0f;
        initTime = Time.time;
    }
}
