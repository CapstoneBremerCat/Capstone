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
    public float SavedTimeData { get; private set; }     // �����ϱ� ���� �ð� ������

    private float initTime;
    private float loadedTime;       // ����� ���� �÷���Ÿ�� �ð�
    private float modifiedTime;     // �ΰ��� �� ������ �÷���Ÿ�� �ð�
    private float timeScale;        // �ð� ���

    public Timer()
    {
        SetTimeScale(1440.0f);       // 24 * 60. �Ϸ� 1��
        //SetTimeScale(8640.0f);       // 24 * 60 * 6. �Ϸ� 10��
        InitTime();
    }

    // �ð� ����
    public void UpdateTime()
    {
        // �귯�� �ð� * �ð� ��� + ����� �ð�
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

    // ���� �ð����� ���ӽð� �߰�
    public void AddTime(int day, int hour, int minute, int seconds)
    {
        modifiedTime += day * SecondsPerDay
            + hour * SecondsPerHour
            + minute * SecondsPerMinute 
            + seconds;
    }

    // ������ �ð����� ���ӽð� ����
    public void SetTime(int day, int hour, int minute, int seconds)
    {
        initTime = Time.time;

        // ����! modifiedTime�� �ʱ�ȭ.
        modifiedTime = 0.0f;

        // ����� �ð��� �����Ѵ�.
        SetLoadedTime(day* SecondsPerDay
            + hour * SecondsPerHour
            + minute * SecondsPerMinute
            + seconds);
    }

    // ���� ��� ����
    public void SetTimeScale(float scale)
    {
        timeScale = scale;
    }

    // ����� �ð� ����
    public void SetLoadedTime(float time)
    {
        loadedTime = time;
    }

    // �ð� ���� �ʱ�ȭ.
    public void InitTime()
    {
        // DataManager�� ����Ǿ��ִ� �÷���Ÿ���� �����´�. �̰� �Ű������� �������°� ������
        //if(DataManager) SetLoadedTime(DataManager.Instance.GetSavedTimeData());
        //else
        loadedTime = 28800.0f;  //08��
        modifiedTime = 0.0f;
        initTime = Time.time;
    }
}
