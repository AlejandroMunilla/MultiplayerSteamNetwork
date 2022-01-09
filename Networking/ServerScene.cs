using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Mirror;
using Steamworks;
using System.Text;
using System.Globalization;
using System;

public class ServerScene : MonoBehaviour
{
    public bool lobbyCreated = false;
    public int packageNo = 0;
    public string serverNameSteam;
    private List<string> NamesSteam;
    private GameController gc;


    private LobbyMatchList_t m_CallResultLobbyMatchList;
    private Callback<P2PSessionRequest_t> _p2PSessionRequestCallback;
    protected Callback<LobbyCreated_t> Callback_lobbyCreated;
    protected Callback<LobbyMatchList_t> Callback_lobbyList;
    protected Callback<LobbyEnter_t> Callback_lobbyEnter;
    protected Callback<LobbyDataUpdate_t> Callback_lobbyInfo;

    public ulong current_lobbyID;
    public List<CSteamID> lobbyIDS;
    public CSteamID mySteamID;
    public CSteamID myServerID;
    CSteamID friendToInvite;

    void OnEnable ()
    {
        GameObject gcon = GameObject.FindGameObjectWithTag("GameController");

        gc = gcon.GetComponent<GameController>();

        lobbyIDS = new List<CSteamID>();
        _p2PSessionRequestCallback = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
        Callback_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
        Callback_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);

        if (SteamAPI.Init())
            Debug.Log("Steam API init -- SUCCESS!"); 


        else
            Debug.Log("Steam API init -- failure ...");

        GetListOfFriends();


        
  //      string testFloat = "129.23";

  //      float newFloat = float.Parse(testFloat);
//        float test2 = float.Parse("1.5", CultureInfo.InvariantCulture.NumberFormat);
   //     Debug.Log( test2);


    }

    // Update is called once per frame

    void Update()
    {
        SteamAPI.RunCallbacks();

        uint size;

        // repeat while there's a P2P message available
        // will write its size to size variable
        while (SteamNetworking.IsP2PPacketAvailable(out size))
        {
            // allocate buffer and needed variables
            var buffer = new byte[size];
            uint bytesRead;
            //       CSteamID remoteId;

            // read the message into the buffer
            if (SteamNetworking.ReadP2PPacket(buffer, size, out bytesRead, out mySteamID))
            {
                // convert to string
                char[] chars = new char[bytesRead / sizeof(char)];
                Buffer.BlockCopy(buffer, 0, chars, 0, chars.Length);

                string message = new string(chars, 0, chars.Length);

                byte[] databyte = buffer;
                string data = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

          //      Debug.Log("Received a message: " + message + "/" + data + "/");
       //         Debug.Log(data);

                HandleMessageFromLobby (mySteamID, data);
            }

        }

        // Command - Create new lobby
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Trying to create lobby ...");
            SteamAPICall_t try_toHost = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4);
        }

        // Command - List lobbies
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Trying to get list of available lobbies ...");
            SteamAPICall_t try_getList = SteamMatchmaking.RequestLobbyList();
        }

        // Command - Join lobby at index 0 (testing purposes)
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Trying to join FIRST listed lobby ...");
            SteamAPICall_t try_joinLobby = SteamMatchmaking.JoinLobby(SteamMatchmaking.GetLobbyByIndex(0));
        }

        // Command - List lobby members
        if (Input.GetKeyDown(KeyCode.Q))
        {
            int numPlayers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)current_lobbyID);

            Debug.Log("\t Number of players currently in lobby : " + numPlayers);
            for (int i = 0; i < numPlayers; i++)
            {
                Debug.Log("\t Player(" + i + ") == " + SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i)));
            }
        }


        
    }

    void HandleMessageFromLobby (CSteamID steamid, string data)
    {
        //int packageNo, string steamName, string name, int level, Ready  Vector3 pos, Vector3 rot, string internalID
        char[] separators = new char[] { '*', '/' };

        string[] split = data.Split(separators);

        string packageString = split[0];
        
    //    int packageInt = Convert.ToInt32(packageString);
  //      int internalID = Convert.ToInt32(split[1]);
        string steamMember = split[1];
        string characterMember = split[2];   
        string levelMember = split[3];        
        bool readyMember = false;
        string readyString = split[4];
 
        if (readyString == "true")
        { 
            readyMember = true; 
        } 
        else 
        { 
            readyMember = false; 
        }
        int levelMemberInt = Convert.ToInt32(levelMember);

        float x = float.Parse(split[5], CultureInfo.InvariantCulture.NumberFormat);
        float y = float.Parse(split[6], CultureInfo.InvariantCulture.NumberFormat);
        float z = float.Parse(split[7], CultureInfo.InvariantCulture.NumberFormat);
        Vector3 pos = new Vector3(x, y, z);
        Vector3 rot = new Vector3(0, 0, 0);
         Debug.Log( split[1]);

     //   Debug.Log( split[5] + "/" + split[6] + "/" + split[7] + "/" + split[8] + "/" + split[10]);
    //    gc.ReceiveServerInfo(packageNo, steamMember, characterMember, levelMemberInt, false, pos, rot, 1, );


    }

    void OnP2PSessionRequest(P2PSessionRequest_t request)
    {
        CSteamID clientId = request.m_steamIDRemote;
        if (clientId == mySteamID)
        {
            SteamNetworking.AcceptP2PSessionWithUser(clientId);
        }
        else
        {
            Debug.LogWarning("Unexpected session request from " + clientId);
        }
    }

    private void ServerSendMessage ()
    {

        /*
        byte[] data = Encoding.UTF8.GetBytes(messageType + ";" + message);
        Debug.Log(messageType + ";" + message);
        client.Networking.SendP2PPacket(friendToInvite, data, data.Length, networkType, channel);*/
        Debug.Log("Hello");   
        byte[] data = Encoding.UTF8.GetBytes("Hello" );
    //    SteamNetworking.SendP2PPacket(friendToInvite, data, (uint) data.Length, EP2PSend.k_EP2PSendReliable);
    }

    public void SendInfo (int packageNo, string steamName, string name, int level, bool ready, Vector3 pos, Vector3 rot, string internalID)
    {
        // packagaNo * ID * name(e.g. Skeleton or Fred) * Level * PlayerNo * health * total health * hit * state * effect * timeEffect * x,y,z * rotation x,y,z * target * realtime


        //     string messageToSend = "1*AddjustHealth*-8/Player1/true/

        float realTime = Time.realtimeSinceStartup;
        string realTimeString = realTime.ToString();
        string hitString = "";
        string xPos = pos.x.ToString();
        string yPos = pos.y.ToString();
        string zPos = pos.z.ToString();
        string xRot = rot.x.ToString();
        string yRot = rot.y.ToString();
        string zRot = rot.z.ToString();

        /*
        if (hit == true)
        {
            hitString = "true";
        }
        else
        {
            hitString = "false";
        }*/
        //     string messageToSend = "1*100*Fred*Level*Player1*15*30*true*MoveToEngage*None*0*120.10/0.20/301.10*0/180/0*Player1*" + realTimeString;
        string messageToSend = packageNo.ToString() + "*"  + steamName + "*" + level.ToString() + "*" + xPos + "*" + yPos + "*" + zPos + "*" + xRot + "*" + yRot + "*" + zRot + "*" + internalID;

        byte[] data = Encoding.UTF8.GetBytes(messageToSend);
        SteamNetworking.SendP2PPacket(friendToInvite, data, (uint)data.Length, EP2PSend.k_EP2PSendReliable);

        int numPlayers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)current_lobbyID);
   //     Debug.Log("\t Number of players currently in lobby : " + numPlayers);
        for (int i = 0; i < numPlayers; i++)
        {
      //      Debug.Log("\t Player(" + i + ") == " + SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i)));
            CSteamID memberSteamId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            string memberName = SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i));
            //     if (memberSteamId.)
            SteamNetworking.SendP2PPacket(memberSteamId, data, (uint)data.Length, EP2PSend.k_EP2PSendReliable);
        }


    }

    public void SendInfoLobby (string steamName, string character, string level, string ready)
    {
        string messageToSend = "";
  
        messageToSend = packageNo.ToString() + "*" + steamName + "*" + character + "*" + level + "*" + ready;

        //packageNo + steamName + character + level + ready

        byte[] data = Encoding.UTF8.GetBytes(messageToSend);
        int numPlayers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)current_lobbyID);
        Debug.Log("\t Number of players currently in lobby : " + numPlayers);
        for (int i = 0; i < numPlayers; i++)
        {
            Debug.Log("\t Player(" + i + ") == " + SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i)));
       //     CSteamID memberSteamId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
           CSteamID memberSteamId = SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i);
            
            string memberName = SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i));
       //     Debug.Log(SteamFriends.GetFriendPersonaName(memberSteamId) + "/" + memberName);
            //     if (memberSteamId.)
            SteamNetworking.SendP2PPacket(memberSteamId, data, (uint)data.Length, EP2PSend.k_EP2PSendReliable);
        }

        packageNo++;

    }

    private void ReceiveInfoClients ()
    {

    }

    private void ClientReceiveMessage ()
    {

    }

    void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult == EResult.k_EResultOK)
            Debug.Log("Lobby created -- SUCCESS!");
  //      lobbyCreated = true;
        else
            Debug.Log("Lobby created -- failure ...");

        string personalName = SteamFriends.GetPersonaName();
        SteamMatchmaking.SetLobbyData((CSteamID)result.m_ulSteamIDLobby, "name", personalName + "'s game");
        SteamMatchmaking.SetLobbyData((CSteamID)result.m_ulSteamIDLobby, "Scene", "MultiMenu");

    }

    void OnGetLobbiesList(LobbyMatchList_t result)
    {
        Debug.Log("Found " + result.m_nLobbiesMatching + " lobbies!");
        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDS.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }

        Invoke("JoinLobby", 2.0f);
    }

    void OnGetLobbyInfo(LobbyDataUpdate_t result)
    {
        for (int i = 0; i < lobbyIDS.Count; i++)
        {
            if (lobbyIDS[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                Debug.Log("Lobby " + i + " :: " + SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDS[i].m_SteamID, "name"));
                return;
            }

        }

    }

    void OnLobbyEntered(LobbyEnter_t result)
    {
        current_lobbyID = result.m_ulSteamIDLobby;

        if (result.m_EChatRoomEnterResponse == 1)
            Debug.Log("Lobby joined!");
        else
            Debug.Log("Failed to join lobby.");
    }

    private void GetListOfFriends()
    {
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
    //    Debug.Log("[STEAM-FRIENDS] Listing " + friendCount + " Friends.");
        for (int i = 0; i < friendCount; ++i)
        {
            CSteamID friendSteamId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            string friendName = SteamFriends.GetFriendPersonaName(friendSteamId);
            EPersonaState friendState = SteamFriends.GetFriendPersonaState(friendSteamId);

   //         Debug.Log(friendName + " is " + friendState + "/" + friendSteamId);

            if (friendName == "murkentropic")
            {
                friendToInvite = friendSteamId;
       //         InviteFriend(friendToInvite);
            }
        }
    }

    public void InviteFriend(CSteamID steamFriendData )
    {

        SteamFriends.InviteUserToGame(steamFriendData, "");
    }

    private void GetSteamID ()
    {
          serverNameSteam =  SteamFriends.GetPersonaName();
          mySteamID = SteamUser.GetSteamID();
     
        Debug.Log(serverNameSteam);
    }

    public void CreateLobboy ()
    {
        Debug.Log("Trying to create lobby ...");
        SteamAPICall_t try_toHost = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4);


   //     static CSteamID Steamworks.SteamMatchmaking.GetLobbyByIndex(int     iLobby)



    }

    public void GetListAndJoin ()
    {
        Debug.Log("Trying to get list of available lobbies ...");
        SteamAPICall_t try_getList = SteamMatchmaking.RequestLobbyList();
        
    }

    private void JoinLobby ()
    {
        for (int cnt = 0; cnt<lobbyIDS.Count; cnt++)
        {
            if (SteamMatchmaking.GetNumLobbyMembers(lobbyIDS[cnt]) < 4)
            {
                SteamAPICall_t try_joinLobby = SteamMatchmaking.JoinLobby(SteamMatchmaking.GetLobbyByIndex(cnt));
            }
        }
    //    SteamAPICall_t try_joinLobby = SteamMatchmaking.JoinLobby(SteamMatchmaking.GetLobbyByIndex(0));
    }

    public List<string> GetLobbyMembers ()
    {
        int numPlayers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)current_lobbyID);
        List<string> lobbyMembers = new List<string>();
        Debug.Log("\t Number of players currently in lobby : " + numPlayers);
        for (int i = 0; i < numPlayers; i++)
        {
            Debug.Log("\t Player(" + i + ") == " + SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i)));
            lobbyMembers.Add(SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i)));
        }

        return lobbyMembers;
    }

    
    public bool SetLimitPlayers (int memberLimit)
    {
       return SteamMatchmaking.SetLobbyMemberLimit((CSteamID)current_lobbyID, memberLimit);
    }


    public bool SetDarkLord (string turnOnBool)
    {
       
        return SteamMatchmaking.SetLobbyData((CSteamID)current_lobbyID, "DarkLord", turnOnBool);
    }

    public void LeaveLobby ()
    {
        SteamMatchmaking.LeaveLobby((CSteamID) current_lobbyID);
    }

    public void SetScene (string scene)
    {
        bool setScene = SteamMatchmaking.SetLobbyData((CSteamID)current_lobbyID, "Scene", scene);
    }

    public string GetCurrentScene ()
    {
        return SteamMatchmaking.GetLobbyData((CSteamID)current_lobbyID, "Scene");
    }

}
