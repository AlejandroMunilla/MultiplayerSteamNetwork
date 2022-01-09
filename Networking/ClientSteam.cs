using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;
using System.Text;
using System.Globalization;

public class ClientSteam : MonoBehaviour
{
    protected Callback<LobbyCreated_t> Callback_lobbyCreated;
    protected Callback<LobbyMatchList_t> Callback_lobbyList;
    protected Callback<LobbyEnter_t> Callback_lobbyEnter;
    protected Callback<LobbyDataUpdate_t> Callback_lobbyInfo;
    ulong current_lobbyID;
    List<CSteamID> lobbyIDS;


    private Callback<P2PSessionRequest_t> _p2PSessionRequestCallback;
    SteamAPICall_t listLobbies;
    CSteamID serverID;
    private CallResult<LobbyMatchList_t> m_CallResultLobbyMatchList;
    // Start is called before the first frame update
    void Start()
    {
        _p2PSessionRequestCallback = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
        GetListFriends();

        lobbyIDS = new List<CSteamID>();
        Callback_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
        Callback_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);

        if (SteamAPI.Init())
            Debug.Log("Steam API init -- SUCCESS!");
        else
            Debug.Log("Steam API init -- failure ...");
    }

    // Update is called once per frame
    void Update()
    {
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
            if (SteamNetworking.ReadP2PPacket(buffer, size, out bytesRead, out serverID))
            {
                // convert to string
                char[] chars = new char[bytesRead / sizeof(char)];
                Buffer.BlockCopy(buffer, 0, chars, 0, chars.Length);

                string message = new string(chars, 0, chars.Length);

                byte[] databyte = buffer;
                string data = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
        
                Debug.Log("Received a message: " + message + "/" + data + "/");
                Debug.Log(data);

                HandleMessageFrom(serverID, data);
            }        

        }

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
    }

    void HandleMessageFrom(CSteamID steamid, string data)
    {
        char[] separators = new char[] {'*', '/' };

        string[] split = data.Split(separators);

        string goID = split[0];
        string healString = split[1];
        int heal = Convert.ToInt32 (healString);
        string attacker = split[2];

        bool hit = false;
        string hitString = split[3];  if (hitString == "true") { hit = true; } else {hit = false; }

        string state = split[4];
        string effect = split[5];
        int timeEffect = Convert.ToInt32(split[6]);
        float x = float.Parse(split[7], CultureInfo.InvariantCulture.NumberFormat); float y = float.Parse(split[8], CultureInfo.InvariantCulture.NumberFormat); float z = float.Parse(split[9], CultureInfo.InvariantCulture.NumberFormat);
        Vector3 position = new Vector3(x, y, z);
        float xRot = float.Parse(split[10]); float yRot = float.Parse(split[11]); float zRot = float.Parse(split[12]);
        Vector3 rotation = new Vector3(xRot, yRot, zRot);
        string target = split[13];
        float realTime = float.Parse(split[14], CultureInfo.InvariantCulture.NumberFormat);


        Debug.Log("ID: " + goID + "/heal: " + heal + "/attacker: " + attacker + "/hit " + hit + "/state " + state);
        Debug.Log("effect: " + effect + "/timeEffect " + timeEffect + "/position " + x  + "," + y + "," + z + "/rotation " + xRot + "," + yRot + "," + zRot + "/target: " + target);
        Debug.Log("Realtime: " + realTime);



    }

    void OnP2PSessionRequest(P2PSessionRequest_t request)
    {
        CSteamID clientId = request.m_steamIDRemote;
        if (clientId == serverID)
        {
            SteamNetworking.AcceptP2PSessionWithUser(clientId);
        }
        else
        {
            Debug.LogWarning("Unexpected session request from " + clientId);
        }
    }

    private void GetListFriends ()
    {
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);

        Debug.Log(friendCount);
        for (int i=0; i< friendCount; i++)
        {
            CSteamID friendSteamId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            string friendName = SteamFriends.GetFriendPersonaName(friendSteamId);

            Debug.Log(friendName);

            if (friendName == "alexhapki")
            {
                serverID = friendSteamId;
            }
        }
        

    }

    private void ListLobbies ()
    {
        listLobbies = SteamMatchmaking.RequestLobbyList();
        //     m_CallResultLobbyMatchList.Set(ListLobbies, OnLobbyMatchList);
        SteamAPICall_t try_lobby = SteamMatchmaking.JoinLobby(SteamMatchmaking.GetLobbyByIndex(0));
    //    if (listLobbies)

    }

    private void OnLobbyMatchList (LobbyMatchList_t lobbyMatchList, bool bIOFailure)
    {

    }


    void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult == EResult.k_EResultOK)
            Debug.Log("Lobby created -- SUCCESS!");
        else
            Debug.Log("Lobby created -- failure ...");

        string personalName = SteamFriends.GetPersonaName();
        SteamMatchmaking.SetLobbyData((CSteamID)result.m_ulSteamIDLobby, "name", personalName + "'s game");
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



}
