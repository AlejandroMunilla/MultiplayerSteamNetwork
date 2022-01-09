using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using PixelCrushers.DialogueSystem;

public class MultiMenu : MonoBehaviour
{

    public GUIStyle myStyle;
    public GUIStyle styleButton;
    public GUISkin mySkin;
    public List<Texture2D> players = new List<Texture2D>();

    private bool displayMain = true;
    private bool serverBool = false;
    private bool ready = false;
    private bool lobbyJoined = false;
    private int internalCNT = 0;
    private int internalSceneCNT = 0;
    private int internalLobbyNo = 0;
    private int level = 1;
    private string privateString = "PUBLIC";
    private string darkLordSteamName = "";
   
    private GUIStyle smallText;
    private Texture2D background;
    private Texture2D board;
    private Texture2D boardRed;
    private Texture2D leftButton;
    private Texture2D rightButton;
    private Texture2D input;
    private Texture2D checkBox;
    private Texture2D uncheckBox;
    private Texture2D tickDarkLordTexture;
    private Texture2D tickReady;
    private Rect backgroundRect;
    private Rect quickMatchRect;
    private Rect createMatchRect;
    private Rect characterRect;
    private Rect multiplayerRect;
    private Rect playerNameRect;
    private Rect playerRect;
    private Rect buttonLeftRect;
    private Rect buttonRightRect;
    private Rect boardRedRect;
    private Rect leftPrivateRect;
    private Rect rightPrivateRect;
    private Rect privateRect;
    private Rect leftEnemyRect;
    private Rect rightEnemyRect;
    private Rect enemyRect;
    private Rect leftSceneRect;
    private Rect rightSceneRect;
    private Rect sceneRect;
    private Rect allowDarkLordRect;
    private Rect tickDarkLord;
    private Rect exitLobbyRect;
    private Rect exitMultiRect;
    private Rect readyRect;
    private Rect readyTicRect;
   

    private List<Rect> slots = new List<Rect>();
    private List<Rect> readyRectList = new List<Rect>();
    private List<Rect> steamNameRectList = new List<Rect>();
    private List<Rect> characterRectList = new List<Rect>();
    private List<string> lobbyMembers = new List<string>();
    private List<string> lobbyAlliesList = new List<string>();
    private List<string> enemyList = new List<string>();
    private List<string> sceneList = new List<string>();
    private List<string> characterNameList = new List<string>();
    private List<string> levelList = new List<string>();
    private List<Texture2D> readyTexturesList = new List<Texture2D>();
  //  private List<>

    private Server server;
    private CSteamID mySteamID;
    private string steamName;
    private MySteamManager steammanager;

    //DARK LORD VARIABLES
    private Texture2D readyDLTexture;
    private string characterDLName = "";
    private string levelDL = "";
    private Rect readyDLRect;
    private Rect darkLordNameRect;
    private Rect darkLordCharRect;


    // Start is called before the first frame update
    void Start()
    {
        
        mySteamID = SteamUser.GetSteamID();
        steamName = SteamFriends.GetPersonaName();
        GameObject stGO = GameObject.FindGameObjectWithTag("SteamManager");
        steammanager = stGO.GetComponent<MySteamManager>();
        server = stGO.GetComponent<Server>();

        enemyList.Add("Undead");

        sceneList.Add("The Cross");
        sceneList.Add("The Forest");


        background = (Texture2D)(Resources.Load("GUI/Bigboard"));
        leftButton = (Texture2D)(Resources.Load("GUI/Left"));
        rightButton = (Texture2D)(Resources.Load("GUI/Right"));
        board = (Texture2D)(Resources.Load("GUI/input"));
        boardRed = (Texture2D)(Resources.Load("GUI/inputRed"));
        input = (Texture2D)(Resources.Load("GUI/Small_Decor"));
        checkBox = (Texture2D)(Resources.Load("GUI/Checked"));
        uncheckBox = (Texture2D)(Resources.Load("GUI/Empty"));
        readyDLTexture = uncheckBox;
        tickDarkLordTexture = uncheckBox;
        tickReady = uncheckBox;


        int buttonHeight = (int)(Screen.height * 0.06f);
        int buttonX = (int)(Screen.width * 0.45f);
        int buttonWidth = (int)(Screen.width * 0.25f);
        int buttonY = (int)(Screen.height * 0.50f);
        int arrowButtonWidth = (int)(Screen.width * 0.04f);

        backgroundRect = new Rect(-Screen.width * 0.03f, -Screen.height * 0.03f, Screen.width * 1.06f, Screen.height * 1.06f);
        multiplayerRect = new Rect(Screen.width * 0.33f, Screen.height * 0.04f, Screen.width * 0.33f, buttonHeight);
        quickMatchRect = new Rect(buttonX, buttonY, buttonWidth , buttonHeight);
        createMatchRect = new Rect(buttonX, quickMatchRect.y + buttonHeight, buttonWidth, buttonHeight);
        characterRect = new Rect(buttonX, createMatchRect.y + buttonHeight, buttonWidth, buttonHeight);
        exitMultiRect = new Rect(buttonX, characterRect.y + buttonHeight, buttonWidth, buttonHeight);
        playerRect = new Rect(Screen.width * 0.01f, Screen.height * 0.5f, Screen.width * 0.3f, Screen.width * 0.3f);
        playerNameRect = new Rect(Screen.width * 0.05f, playerRect.y - buttonHeight, Screen.width * 0.12f, arrowButtonWidth);
        buttonLeftRect = new Rect((playerNameRect.x + playerNameRect.width), playerNameRect.y, arrowButtonWidth, arrowButtonWidth);
        buttonRightRect = new Rect(buttonLeftRect.x + arrowButtonWidth, playerNameRect.y, arrowButtonWidth, arrowButtonWidth);

        int leftXInfo = (int)(Screen.width * 0.04f);

        leftPrivateRect = new Rect(leftXInfo, Screen.height * 0.13f, arrowButtonWidth, arrowButtonWidth);
        privateRect = new Rect(leftPrivateRect.x + arrowButtonWidth, leftPrivateRect.y, Screen.width * 0.2f, arrowButtonWidth);
        rightPrivateRect = new Rect(privateRect.x + privateRect.width, leftPrivateRect.y, arrowButtonWidth, arrowButtonWidth);

        leftEnemyRect = new Rect(leftXInfo, leftPrivateRect.y +  arrowButtonWidth, arrowButtonWidth, arrowButtonWidth);
        enemyRect= new Rect(leftPrivateRect.x + arrowButtonWidth, leftEnemyRect.y, Screen.width * 0.2f, arrowButtonWidth);
        rightEnemyRect= new Rect(privateRect.x + privateRect.width, leftEnemyRect.y, arrowButtonWidth, arrowButtonWidth);

        leftSceneRect = new Rect(leftXInfo, leftEnemyRect.y + arrowButtonWidth, arrowButtonWidth, arrowButtonWidth);
        sceneRect = new Rect(leftSceneRect.x + arrowButtonWidth, leftSceneRect.y, Screen.width * 0.2f, arrowButtonWidth);
        rightSceneRect = new Rect(sceneRect.x + sceneRect.width, leftSceneRect.y, arrowButtonWidth, arrowButtonWidth);

        allowDarkLordRect = new Rect(leftXInfo, leftSceneRect.y + (2 * arrowButtonWidth), privateRect.width + (1 * arrowButtonWidth), arrowButtonWidth);
        tickDarkLord = new Rect(leftXInfo + allowDarkLordRect.width, leftSceneRect.y + (2 * arrowButtonWidth), arrowButtonWidth, arrowButtonWidth);

        exitLobbyRect = new Rect(Screen.width * 0.35f, leftPrivateRect.y, Screen.width * 0.12f + arrowButtonWidth * 1.5f, arrowButtonWidth * 1.5f) ;
        readyRect = new Rect(exitLobbyRect.x, leftPrivateRect.y + (2 *  buttonHeight), Screen.width * 0.12f, arrowButtonWidth * 1.5f);
        readyTicRect = new Rect(exitLobbyRect.x + readyRect.width - (int)(Screen.width * 0.0f), readyRect.y, readyRect.height, readyRect.height);


        int addSlotWidth = (int)(Screen.height * 0.17f);
        for (int cnt = 0; cnt < 4; cnt++)
        {
            Rect goRect = new Rect(Screen.width * 0.55f, Screen.height * 0.11f + (cnt * addSlotWidth), Screen.width * 0.41f, Screen.height * 0.21f);
            slots.Add(goRect);
            Rect goReadyRect = new Rect(slots[cnt].x + Screen.width * 0.02f, slots[cnt].y + (slots[cnt].height * 0.5f) - readyTicRect.width * 0.5f , readyTicRect.width, readyTicRect.height);
            readyRectList.Add(goReadyRect);
            readyTexturesList.Add(uncheckBox);
            characterNameList.Add("");
            Rect goSteamNameRect = new Rect(slots[0].x + Screen.width * 0.1f, goRect.y + (int)(Screen.height * 0.04f), Screen.width * 0.32f, buttonHeight);
            steamNameRectList.Add(goSteamNameRect);
            Rect goCharacterRect = new Rect(slots[0].x + Screen.width * 0.1f, goRect.y + (int)(Screen.height * 0.09f), Screen.width * 0.32f, buttonHeight);
            characterRectList.Add(goCharacterRect);
            levelList.Add("1");

        }
        //Dark Lord parameters
        boardRedRect =  new Rect(slots[0].x, slots[0].y + (4 * addSlotWidth), slots[0].width, slots[0].height);
        darkLordNameRect = new Rect(slots[0].x + Screen.width * 0.1f, boardRedRect.y + (int)(Screen.height * 0.04f), Screen.width * 0.32f, buttonHeight);
        darkLordCharRect = new Rect(slots[0].x + Screen.width * 0.1f, boardRedRect.y + (int)(Screen.height * 0.09f), Screen.width * 0.32f, buttonHeight);


        myStyle = mySkin.GetStyle("label");
        myStyle.fontSize = (int)(Screen.height * 0.045f);
        myStyle.normal.textColor = Color.white;
        myStyle.alignment = TextAnchor.MiddleCenter;

        styleButton = mySkin.GetStyle("button");
        styleButton.fontSize = (int)(Screen.height * 0.022f);
        styleButton.normal.textColor = Color.white;
        styleButton.alignment = TextAnchor.MiddleCenter;

        smallText = mySkin.GetStyle("smallText");
        smallText.fontSize = (int)(Screen.height * 0.025f);
        smallText.normal.textColor = Color.white;
        smallText.alignment = TextAnchor.MiddleLeft;

        if (SteamAPI.Init())
            GetExperience(players[0].name);


        else
            Debug.Log("Steam API init -- failure ...");

        //    playerText = players[internalCNT];
    }

    private void OnGUI()
    {
        GUI.DrawTexture(backgroundRect, background);
        GUI.Label (multiplayerRect, "MULTIPLAYER", myStyle);
        DisplayPlayer();

        if (displayMain == true)
        {
            DisplayMain();
        }
        else
        {
            if (server)
            {

                DisplayServer();
            }
            else
            {
                DisplayClient();
            }


            if (GUI.Button(readyRect, "READY", styleButton))
            {
      //          Debug.Log("ready");
                ready = !ready;


            }
            if (ready == true)
            {
                GUI.DrawTexture(readyTicRect, checkBox);
            }
            else
            {
                GUI.DrawTexture(readyTicRect, uncheckBox);
            }

            DisplayLobby();

            //Game info 
            GUI.DrawTexture(privateRect, input);
            GUI.Label(privateRect, privateString, smallText);

            GUI.DrawTexture(enemyRect, input);
            GUI.Label(enemyRect, enemyList[0], smallText);

            GUI.DrawTexture(sceneRect, input);
            GUI.Label(sceneRect, sceneList[internalSceneCNT], smallText);
            GUI.DrawTexture(tickDarkLord, tickDarkLordTexture);

            if (GUI.Button(exitLobbyRect, "EXIT LOBBY", styleButton))
            {
                lobbyJoined = false;
                displayMain = true;
                bool newFound = false;
                CancelInvoke("CoroutineInfo");
                lobbyMembers.Clear();
                
                if (server == true)
                {
                    CSteamID lobbyOwner = SteamMatchmaking.GetLobbyOwner((CSteamID) server.current_lobbyID);
                    if (lobbyOwner == mySteamID)
                    {
                        if (SteamMatchmaking. GetNumLobbyMembers((CSteamID) server.current_lobbyID) > 1 )
                        {
                            int numPlayers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)server.current_lobbyID);
                            for (int i = 0; i < numPlayers; i++)
                            {
                                CSteamID currentPlayer = SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)server.current_lobbyID, i);
                               
                                if (currentPlayer != lobbyOwner && newFound == false)
                                {
                                    newFound = true;
                                    newFound  = SteamMatchmaking.SetLobbyOwner((CSteamID)server.current_lobbyID, currentPlayer);
                                }
                              
                            }

                        }                     
                    }
                    Debug.Log(newFound);
                    SteamMatchmaking.LeaveLobby((CSteamID)server.current_lobbyID);
                }
                else
                {
                    SteamMatchmaking.LeaveLobby((CSteamID)server.current_lobbyID);
                }
            }
        }        
    }

    private void DisplayPlayer ()
    {
        GUI.Label(playerNameRect, players[internalCNT].name, smallText);
        if (GUI.Button(buttonLeftRect, leftButton, styleButton))
        {
            if (internalCNT == 0)
            {
                internalCNT = players.Count - 1;
            }
            else
            {
                internalCNT--;
            }
        }
        if (GUI.Button(buttonRightRect, rightButton, styleButton))
        {
            if (internalCNT == players.Count - 1)
            {
                internalCNT = 0;
            }
            else
            {
                internalCNT++;
            }
        }
        GUI.DrawTexture(playerRect, players[internalCNT]);

  
    }


    private void DisplayMain ()
    {
        if (lobbyJoined == false)
        {
            if (GUI.Button(quickMatchRect, "QUICK JOIN MATCH", styleButton))
            {
                displayMain = false;
                serverBool = false;
                characterNameList.Add(players[internalCNT].name);
                readyTexturesList.Add(uncheckBox);
                InvokeRepeating("CoroutineInfo", 0.5f, 0.2f);
                lobbyJoined = true;
                server.GetListAndJoin();
            }
            if (GUI.Button(createMatchRect, "CREATE MATCH", styleButton))
            {
                displayMain = false;
                serverBool = true;
                server.CreateLobboy();
                InvokeRepeating("CoroutineInfo", 0.5f, 0.2f);
                characterNameList.Add(players[internalCNT].name);
                readyTexturesList.Add(uncheckBox);
                internalLobbyNo = 0;
                lobbyJoined = true;

            }
            GUI.Button(characterRect, "CHARACTER", styleButton);
            GUI.Button(exitMultiRect, "EXIT", styleButton);
        }

    }

    private void DisplayLobby ()
    {
        for (int cnt = 0; cnt < 4; cnt++)
        {
            GUI.DrawTexture(slots[cnt], board);           
        }

        for (int cnt = 0; cnt < lobbyAlliesList.Count; cnt++)
        {
   
           
           if (readyTexturesList[cnt] != null)
            {
                GUI.DrawTexture(readyRectList[cnt], readyTexturesList[cnt]);
            }

            GUI.Label(steamNameRectList[cnt],  lobbyAlliesList[cnt], smallText);
            GUI.Label(characterRectList[cnt], characterNameList[cnt]  + " (level " + levelList[cnt] + ")", smallText);     
      //      Debug.Log(lobbyMembers[cnt]);
        }

        if (tickDarkLordTexture == checkBox)
        {
            GUI.DrawTexture(boardRedRect, boardRed);
            if (characterDLName != "")
            {
                GUI.Label(darkLordNameRect, darkLordSteamName, smallText);
                GUI.Label(darkLordCharRect, characterDLName+ " (level " + levelDL + ")", smallText);
            }
        }

        

        

            

    }

    private void DisplayClient ()
    {
        GUI.Label (allowDarkLordRect, "ALLOW DARK LORD", smallText);
    }

    private void DisplayServer()
    {

        GUI.Button(leftPrivateRect, leftButton, styleButton);
        GUI.Button(rightPrivateRect, rightButton, styleButton);     

       
        GUI.Button(leftEnemyRect, leftButton, styleButton);
        GUI.Button(rightEnemyRect, rightButton, styleButton);

        
      
        if ( GUI.Button(leftSceneRect, leftButton, styleButton))
        {
            if (internalSceneCNT == 0)
            {
                internalSceneCNT = sceneList.Count - 1;

            }
            else
            {
                internalSceneCNT--;
            }

            server.SetScene(sceneList[internalSceneCNT]);

        }
        if (GUI.Button(rightSceneRect, rightButton, styleButton))
        {
            if (internalSceneCNT == sceneList.Count - 1)
            {
                internalSceneCNT = 0;
            }
            else
            {
                internalSceneCNT++;
            }

            server.SetScene(sceneList[internalSceneCNT]);
        }
        

        if (GUI.Button(allowDarkLordRect, "ALLOW DARK LORD", styleButton))
        {
            if (tickDarkLordTexture.name == "Empty")
            {
                tickDarkLordTexture = checkBox;
            }
            bool setNumberLimit = server.SetLimitPlayers(5);
            bool isDarkLordActive = server.SetDarkLord("true");
            Debug.Log(setNumberLimit + "/" + isDarkLordActive);
        }     

    }

    private void CoroutineInfo ()
    {
        lobbyMembers =  server.GetLobbyMembers();
        string readyString = "";
        if (ready == true)
        {
            readyString = "true";
        }
        else
        {
            readyString = "false";
        }

        //  steamName * character * level * ready
        server.SendInfoLobby(steamName, players[internalCNT].name, level.ToString(), readyString);

        lobbyAlliesList.Clear();
        bool allready = true;
        for (int cnt = 0; cnt < lobbyMembers.Count; cnt ++)
        {
            if (darkLordSteamName != lobbyMembers[cnt])
            {
                lobbyAlliesList.Add(lobbyMembers[cnt]);
                
            }
        }
   //     Debug.Log(server.GetCurrentScene());

        for (int cnt = 0; cnt < lobbyAlliesList.Count; cnt ++)
        {
            if (readyTexturesList[cnt] != checkBox)
            {
  //              Debug.Log(lobbyAlliesList[cnt] + "/No Ready");
                allready = false;
            }
            else
            {
    //            Debug.Log(lobbyAlliesList[cnt] + "/Ready");
            }
        }

        if (allready == true)
        {
            VentureForth();
        }

       
    }

 

    private void GetLevel (string playerName)
    {
        
    }

    public void ReceiveInfo (int packageNoMember, string steamNameMember, string characterMember, string levelMember, bool readyMember)
    {
        //int packageNo, string steamName, string name, int level, Ready  Vector3 pos, Vector3 rot, string internalID
    //    Debug.Log(steamNameMember + "/" + characterMember + "/" + levelMember + "/" + readyMember);
        int packageNo = 0;
        if (steamNameMember != server.serverNameSteam)
        {

        }
        else
        {
            packageNo = server.packageNo;
        }

        if (lobbyAlliesList != null)
        {
            for (int cnt = 0; cnt < lobbyAlliesList.Count; cnt++)
            {
     //           Debug.Log(lobbyMembers[cnt] + "/" + steamNameMember);
                if (lobbyAlliesList[cnt] == steamNameMember)
                {
                    //         Debug.Log(characterMember + "/" + characterNameList[cnt]);
                    if (characterMember != characterNameList[cnt])
                    {
                        characterNameList[cnt] = characterMember;
                    }

                    if (readyMember == true)
                    {
                        if (readyTexturesList[cnt] != checkBox)
                        {
                            readyTexturesList[cnt] = checkBox;
                        }
                    }
                    else
                    {
                        if (readyTexturesList[cnt] != uncheckBox)
                        {
                            readyTexturesList[cnt] = uncheckBox;
                        }
                    }

                    if (levelList[cnt] != levelMember)
                    {
                        levelList[cnt] = levelMember;
                    }
                }
            }
        }

        if (darkLordSteamName == steamNameMember)
        {
            if (characterDLName != characterMember)
            {
                characterDLName = characterMember;
            }

            if (readyMember == true)
            {
                if (readyDLTexture != checkBox)
                {
                    readyDLTexture = checkBox;
                }
            }
            else
            {
                if (readyDLTexture != uncheckBox)
                {
                    readyDLTexture = uncheckBox;
                }
            }

            if (levelDL != levelMember.ToString())
            {
                levelDL = levelMember.ToString();
            }
        }
    }



    private void VentureForth ()
    {

        DialogueLua.SetActorField("Player1", "chosen", characterNameList[0]);
        DialogueLua.SetActorField("Player1", "steamName", lobbyAlliesList[0]);
        if (lobbyAlliesList.Count == 1)
        {
            DialogueLua.SetActorField("Player2", "chosen", "None");
            DialogueLua.SetActorField("Player2", "steamName", "");
            DialogueLua.SetActorField("Player3", "chosen", "None");
            DialogueLua.SetActorField("Player3", "steamName", "");
            DialogueLua.SetActorField("Player4", "chosen", "None");
            DialogueLua.SetActorField("Player4", "steamName", "");
        }
        if (lobbyAlliesList.Count == 2)
        {
            DialogueLua.SetActorField("Player2", "chosen", characterNameList[1]);
            DialogueLua.SetActorField("Player2", "steamName", lobbyAlliesList[1]);

            DialogueLua.SetActorField("Player3", "chosen", "None");
            DialogueLua.SetActorField("Player3", "steamName", "");
            DialogueLua.SetActorField("Player4", "chosen", "None");
            DialogueLua.SetActorField("Player4", "steamName", "");

        }
        if (lobbyAlliesList.Count == 3)
        {
            DialogueLua.SetActorField("Player2", "chosen", characterNameList[1]);
            DialogueLua.SetActorField("Player2", "steamName", lobbyAlliesList[1]);
            DialogueLua.SetActorField("Player3", "chosen", characterNameList[2]);
            DialogueLua.SetActorField("Player3", "steamName", lobbyAlliesList[2]);

            DialogueLua.SetActorField("Player4", "chosen", "None");
            DialogueLua.SetActorField("Player4", "steamName", "");

        }
        if (lobbyAlliesList.Count == 4)
        {
            DialogueLua.SetActorField("Player2", "chosen", characterNameList[1]);
            DialogueLua.SetActorField("Player2", "steamName", lobbyAlliesList[1]);
            DialogueLua.SetActorField("Player3", "chosen", characterNameList[2]);
            DialogueLua.SetActorField("Player3", "steamName", lobbyAlliesList[2]);
            DialogueLua.SetActorField("Player4", "chosen", characterNameList[3]);
            DialogueLua.SetActorField("Player4", "steamName", lobbyAlliesList[3]);
        }


        if (serverBool == true)
        {
            DialogueLua.SetVariable("test", "Yes");
            DialogueLua.SetVariable("enemies", enemyList[0]);
            DialogueLua.SetVariable("server", "Yes");
            DialogueLua.SetVariable("multiID", 0);
            server.SetScene(sceneList[internalSceneCNT]);
            GetComponent<SaveGame>().sceneToExit = sceneList[internalSceneCNT];
        }
        else
        {
            for (int cnt = 0; cnt < lobbyAlliesList.Count; cnt++)
            {
                if (lobbyAlliesList[cnt] == steamName)
                {
                    DialogueLua.SetVariable("multiID", cnt.ToString());
                }
            }
            GetComponent<SaveGame>().sceneToExit = server.GetCurrentScene();
            DialogueLua.SetVariable("server", "No");
        }
        DialogueLua.SetVariable("multiplayer", "Yes");
        Invoke("SaveData", 1);

    }

    public void SaveData()
    {
        string profile = DialogueLua.GetVariable("profile").asString;
        //    Debug.Log(profile);


        GetComponent<SaveGame>().saveAndExit = true;
        GetComponent<SaveGame>().SaveProfile("AutoSave", "ChooseLevel", true);

    }


    private void GetExperience (string playerName)
    {
        int luaExperience = steammanager.GetStatsInt(playerName + "_exp");
        //1000   2000   3500   5500  8000  11000   14500   18500   23000    26000
        int nextLevelExp = 500;
        int arimeticIncrease = 0;
        bool foundLevel = false;
        int levelTemp = 1;
        for (int cnt = 1; cnt < 11; cnt++)
        {
            //    Debug.Log(cnt);
            if (foundLevel == false)
            {
                arimeticIncrease = (cnt * 500);
                nextLevelExp = nextLevelExp + arimeticIncrease;
                if (luaExperience >= 26000)
                {
                    luaExperience = 26000;
                    levelTemp = 10;
                    foundLevel = true;
                    nextLevelExp = 26000;
                }

                else if (nextLevelExp > luaExperience)
                {
                    levelTemp = cnt;
                    foundLevel = true;
                }
            }
        }

    //    Debug.Log(luaExperience + "/" + nextLevelExp + "/"  + levelTemp + "/" + playerName);

        int levelSpent = DialogueLua.GetActorField(playerName, "levelSpent").asInt;
        int tempPoints = levelTemp - levelSpent;
        level = levelTemp;


    }
}
