using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonMonobehaviour<TimeManager>
{
    private const int DaysInSeason = 30;
    private const int HoursInDay = 24;
    private const int MinutesInHour = 60;
    private const int SecondsInMinute = 60;
    private const int SeasonsInYear = 4;
    private readonly string[] DaysOfWeek = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

    private int gameYear = 1;
    private Season gameSeason = Season.Spring;
    private int gameDay = 1;
    private int gameHour = 6;
    private int gameMinute = 30;
    private int gameSecond = 0;
    private string gameDayOfWeek = "Mon";
    private readonly bool gameClockPaused = false;
    private float gameTick = 0f;

    private void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private void Update()
    {
        if (!gameClockPaused)
        {
            GameTick();
        }
    }

    private void GameTick()
    {
        gameTick += Time.deltaTime;

        if (gameTick >= Settings.secondsPerGameSecond)
        {
            gameTick -= Settings.secondsPerGameSecond;
            UpdateGameSecond();
        }
    }
    private void UpdateGameSecond()
    {
        gameSecond++;
        if (gameSecond >= SecondsInMinute)
        {
            gameSecond = 0;
            gameMinute++;

            if (gameMinute >= MinutesInHour)
            {
                gameMinute = 0;
                gameHour++;

                if (gameHour >= HoursInDay)
                {
                    gameHour = 0;
                    gameDay++;

                    if (gameDay > DaysInSeason)
                    {
                        gameDay = 1;
                        gameSeason++;

                        if ((int)gameSeason >= SeasonsInYear)
                        {
                            gameSeason = 0;
                            gameYear++;
                            EventHandler.CallAdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                        }

                        EventHandler.CallAdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                    }

                    gameDayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                }

                EventHandler.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            }

            EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }


    private string GetDayOfWeek()
    {
        int totalDays = ((int)gameSeason * DaysInSeason) + gameDay;
        return DaysOfWeek[totalDays % 7];
    }

    // Test game time advancement (Advance by 60 seconds) TODO REMOVE
    public void TestAdvanceGameTime()
    {
        AdvanceTimeBySeconds(60);
    }

    // Test game day advancement (Advance by 1 day) TODO REMOVE
    public void TestAdvanceGameDay()
    {
        AdvanceTimeBySeconds(86400);
    }

    private void AdvanceTimeBySeconds(int totalSeconds)
    {
        for (int i = 0; i < totalSeconds; i++)
        {
            UpdateGameSecond();
        }
    }
}
