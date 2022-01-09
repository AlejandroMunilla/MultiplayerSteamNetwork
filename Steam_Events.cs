using UnityEngine;
using System.Collections;
using Steamworks;
using UnityEngine.UI;


public class Steam_Events : MonoBehaviour
{
    //Overlay checker callback
    //This is needed so we can check if steam overlay is lifed/activated
    protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;
    // Use this for initialization
    void Start()
    {
        if (SteamManager.Initialized)
        {
            Debug.LogWarning("Loaded SteamManager!");
        }
    }

    //This enables the checking if the Steam Overlay is enabled.
    //And the testing part to check for players how many are playing currently
    private void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
        }
        if (SteamManager.Initialized)
        {
            m_NumberOfCurrentPlayers = CallResult<NumberOfCurrentPlayers_t>.Create(OnNumberOfCurrentPlayers);
        }
    }
    //This function handles what will happen when the overlay is lifed. My tactic is to set the timescale to 0 to pause everything.
    private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
    {
        if (pCallback.m_bActive != 0)
        {
            Debug.Log("Steam Overlay has been activated");
            Time.timeScale = 0;
        }
        else
        {
            Debug.Log("Steam Overlay has been closed");
            Time.timeScale = 1;
        }
    }
    //This is a callresult that seeks the number of players, as listen in the C# SteamWorks documentation
    private CallResult<NumberOfCurrentPlayers_t> m_NumberOfCurrentPlayers;

    //Not needed, just like the callresult but its useful for checking how many players are playing.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SteamAPICall_t handle = SteamUserStats.GetNumberOfCurrentPlayers();
            m_NumberOfCurrentPlayers.Set(handle);
            Debug.Log("Called GetNumberOfCurrentPlayers()");

            int Stats;
            SteamUserStats.GetStat("EnemyKills", out Stats);
            Debug.Log(Stats);
            Stats++;
            SteamUserStats.SetStat("EnemyKills", Stats);
            SteamUserStats.StoreStats();
        }


    }
    //Function for checking how many players are playing
    private void OnNumberOfCurrentPlayers(NumberOfCurrentPlayers_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_bSuccess != 1 || bIOFailure)
        {
            Debug.Log("There was an error retrieving the NumberOfCurrentPlayers.");
        }
        else
        {
            Debug.Log("The number of players playing your game: " + pCallback.m_cPlayers);
        }
    }
    // Here begings my coding fully, where I made special functions to handle different types of things to tackle
    //The UnlockAchive functions, unlocks an achivment, on calling
    static public void UnlockAchive(string achievement)
    {
        if (SteamManager.Initialized)
        {
            SteamUserStats.SetAchievement(achievement);
            StoreStats();
        }
    }
    //This function is used to set Stats of your SteamWorks. Only works for floats currently, int is broken for handling
    static public void SetStats(string StatusName, float Value, bool toINT)
    {
        if (SteamManager.Initialized)
        {
            if (toINT)
            {
                int intData = (int)Value;
                SteamUserStats.SetStat (StatusName, intData);
            }
            Debug.Log("Setting stats of: " + StatusName + ", To: " + Value);
            SteamUserStats.SetStat(StatusName, Value);
            StoreStats();
        }
    }
    //This function is used to Get The Stats of a Stat, in your SteamWorks.
    static public float GetStats(string StatusName)
    {
        float Stats;
        SteamUserStats.GetStat(StatusName, out Stats);
        return Stats;
    }
    //This function saves the stats, after being handled by
    static public void StoreStats()
    {
        SteamUserStats.StoreStats();
    }
    //Function to reset all the achivements and stats for you only
    //You'll be using this function a lot, in order to test properly.
    static public void Reset()
    {
        SteamUserStats.ResetAllStats(true);
    }
}
