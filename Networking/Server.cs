using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Mirror;
using Steamworks;
using System.Text;
using System.Globalization;
using System;

public class Server : MonoBehaviour
{
    public bool lobbyCreated = false;
    public int packageNo = 0;
    public string serverNameSteam;
    public string steamName;
    public GameController gc = null;
    private List<string> NamesSteam;
    private MultiMenu multiMenu;

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
        if ( gcon.GetComponent<MultiMenu>() != null)
        {
            multiMenu = gcon.GetComponent<MultiMenu>();
        }
        lobbyIDS = new List<CSteamID>();
        _p2PSessionRequestCallback = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
        Callback_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
        Callback_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);
        steamName = SteamFriends.GetPersonaName();
        if (SteamAPI.Init())
            Debug.Log("Steam API init -- SUCCESS!"); 


        else
            Debug.Log("Steam API init -- failure ...");

        GetListOfFriends();
   }


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
     //           Debug.Log(data);

                HandleMessageFromLobby (mySteamID, data);
            }

        }


        /*
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

        if (Input.GetKeyDown(KeyCode.Space))
        {

            //    SendInfo();
             
              Debug.Log(SteamMatchmaking.GetLobbyMemberLimit((CSteamID)current_lobbyID));
        }
        */
        
    }

    void HandleMessageFromLobby (CSteamID steamid, string data)
    {
        char[] separators = new char[] { '*', '/' };
        string[] split = data.Split(separators);
        string packageString = split[0];
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

        if (gc != null)
        {
       //     int level = Convert.ToInt32(split[4]);
            float x = float.Parse(split[5], CultureInfo.InvariantCulture.NumberFormat);
            float y = float.Parse(split[6], CultureInfo.InvariantCulture.NumberFormat);
            float z = float.Parse(split[7], CultureInfo.InvariantCulture.NumberFormat);
            Vector3 pos = new Vector3(x, y, z);
            float xRot = float.Parse(split[8], CultureInfo.InvariantCulture.NumberFormat);
            float yRot = float.Parse(split[9], CultureInfo.InvariantCulture.NumberFormat);
            float zRot = float.Parse(split[10], CultureInfo.InvariantCulture.NumberFormat);
            Vector3 rot = new Vector3(xRot, yRot, zRot);
            int internalID = Convert.ToInt32(split[11]);
            float forward = float.Parse(split[12], CultureInfo.InvariantCulture.NumberFormat);
            float turn = float.Parse(split[13], CultureInfo.InvariantCulture.NumberFormat);
            string jumpString = split[14];
            string actionString = split[15];

            float xAimRot = float.Parse(split[16], CultureInfo.InvariantCulture.NumberFormat);
            float yAimRot = float.Parse(split[17], CultureInfo.InvariantCulture.NumberFormat);
            float ZAimRot = float.Parse(split[18], CultureInfo.InvariantCulture.NumberFormat);
            float wAimRot = float.Parse(split[19], CultureInfo.InvariantCulture.NumberFormat);
            Quaternion aimRot = new Quaternion (xAimRot, yAimRot, ZAimRot, wAimRot );

            string state = split[20];
            int targetID = Convert.ToInt32(split[21]);
            int addjustHealth = Convert.ToInt32(split[22]);
            int totalHealth = Convert.ToInt32(split[23]);
            string blockString = split[24];
            gc.ReceiveServerInfo(1, steamMember, characterMember, 1, readyMember, pos, rot, internalID, forward, turn, jumpString, actionString, aimRot, state, targetID, addjustHealth, totalHealth, blockString);
     //       Debug.Log("/" + split[1] + "/" + split[2] + "/" + steamMember);
        }
        else
        {
            Debug.Log(levelMember);
            multiMenu.ReceiveInfo(1, steamMember, characterMember, levelMember, readyMember);
        }
       


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

    public void SendInfo (string distribution, int packageNo, string steamName, string name, int level, bool hit, Vector3 pos, Vector3 rot, string internalID, float forward, float turn, bool jump, string action, Quaternion aimRot, string state, string targetID, int addjustHealth, int totalHealth, bool block)
    {
        string xPos = pos.x.ToString(CultureInfo.InvariantCulture.NumberFormat);
        string yPos = pos.y.ToString(CultureInfo.InvariantCulture.NumberFormat);
        string zPos = pos.z.ToString(CultureInfo.InvariantCulture.NumberFormat);
        string xRot = rot.x.ToString(CultureInfo.InvariantCulture.NumberFormat);
        string yRot = rot.y.ToString(CultureInfo.InvariantCulture.NumberFormat);
        string zRot = rot.z.ToString(CultureInfo.InvariantCulture.NumberFormat);
        string xAimRot = aimRot.x.ToString(CultureInfo.InvariantCulture.NumberFormat);
        string yAimRot = aimRot.y.ToString(CultureInfo.InvariantCulture.NumberFormat);
        string zAimRot = aimRot.z.ToString(CultureInfo.InvariantCulture.NumberFormat);
        string wAimRot = aimRot.w.ToString(CultureInfo.InvariantCulture.NumberFormat);
        string hitString;
        string jumpString;
        string blockString;
        if (hit == true)
        {
            hitString = "true";
        }
        else
        {
            hitString = "false";
        }
        if (jump == true)
        {
            jumpString = "true";
        }
        else
        {
            jumpString = "false";
        }
        if (block == true)
        {
            blockString = "true";
        }
        else
        {
            blockString = "false";
        }
        string messageToSend = packageNo.ToString() + "*" + steamName + "*" + name + "*" + level.ToString(CultureInfo.InvariantCulture) + "*" + hitString + "*" + xPos + "*" + yPos + "*" + zPos + "*" + xRot + "*" + yRot + "*" + zRot + "*" + internalID.ToString() + "*" + forward.ToString(CultureInfo.InvariantCulture.NumberFormat) + "*" + turn.ToString(CultureInfo.InvariantCulture.NumberFormat) + "*" + jumpString + "*" + action + "*" + xAimRot + "*" + yAimRot + "*" + zAimRot + "*" + wAimRot  + "*" + state + "*" + targetID + "*" + addjustHealth + "*" + totalHealth + "*" + blockString;
        byte[] data = Encoding.UTF8.GetBytes(messageToSend);
   //     SteamNetworking.SendP2PPacket(friendToInvite, data, (uint)data.Length, EP2PSend.k_EP2PSendReliable);

        if (distribution == "All")       {

            int numPlayers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)current_lobbyID);
            for (int i = 0; i < numPlayers; i++)
            {
                //       Debug.Log("\t Player(" + i + ") == " + SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i)));

                string steamNameTemp = SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i));
                //      CSteamID memberSteamId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
                CSteamID memberSteamId = SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i);
         //       string memberName = SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i));

                if (steamName != steamNameTemp)    //avoid to send the data back to the sender
                {
                    SteamNetworking.SendP2PPacket(memberSteamId, data, (uint)data.Length, EP2PSend.k_EP2PSendReliable);
                    //            Debug.Log(SteamFriends.GetFriendPersonaName(memberSteamId));
                }
            }
        }
        else if (distribution == "Server")
        {
            SteamNetworking.SendP2PPacket(myServerID, data, (uint)data.Length, EP2PSend.k_EP2PSendReliable);
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
        SteamMatchmaking.SetLobbyData((CSteamID)result.m_ulSteamIDLobby, "DarkLord", "False");
        myServerID = SteamUser.GetSteamID();

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
        if (SteamUser.GetSteamID() != SteamMatchmaking.GetLobbyOwner((CSteamID)current_lobbyID)) { myServerID = (CSteamID)current_lobbyID; }

        if (result.m_EChatRoomEnterResponse == 1)
            Debug.Log("Lobby joined!"); 
        else
            Debug.Log("Failed to join lobby.");

        Debug.Log(SteamFriends.GetFriendPersonaName((CSteamID)current_lobbyID));
        
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
