using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using UnityEngine.Events;

public class GameController : MonoBehaviour {

    public bool pause = false;
    public bool inBattle = false;
    public bool sceneFP = false;
    public bool multiplayer = false;
    public bool isRPG = true;
    public bool server = false;
    public bool safeArea = false;
    public bool arenaMode = false;
    public string profile;
    public bool inConversation = false;
    public bool friendlyFire = false;
    public bool allDone = false;
    public int playerMultiID = 4;
    public int enemyMultiID = 3000;
    public int playerID = 0;

    public GameObject player1 = null;
    public GameObject player2 = null;
    public GameObject player3 = null;
    public GameObject player4 = null;
    public GameObject hideSide;
    public GameObject activePlayer;
    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> allies = new List<GameObject>();
    public List<GameObject> enemyAllies = new List<GameObject>();
    public List<GameObject> reinitialize = new List<GameObject>();
    public List<GameObject> enemiesDead = new List<GameObject>();
    public List<string> inventory;                                      //Only when RPG game

    private bool alive = true;
    private AudioController audioController;
    private NetworkManager manager;
    private Vector3 camPos;
    private Quaternion camRot;
    private Camera camera;

    //FPS scenes
    public Camera cam1;
    public Camera cam2;
    private myGUI myGui;
    private Server serverScript;

    //RPG
    public List<InventoryRPG> inventoryRPG = new List<InventoryRPG>();
    public List<InventoryRPG> quickInventoryRPG = new List<InventoryRPG>();
    public List<InventoryRPG> books = new List<InventoryRPG>();
    public List<InventoryRPG> questItems = new List<InventoryRPG>();

    // Use this for initialization
    void Start ()
    {
        audioController = GetComponent<AudioController>();
        StartCoroutine("CheckDead");
        camera = Camera.main;

        foreach (Camera ca in Camera.allCameras)
        {
            if (ca.name == "Camera1")
            {
                camera = ca;
            }
        }


   //     manager = GameObject.FindGameObjectWithTag("Network Manager").GetComponent<NetworkManager>();
        InvokeRepeating("RegenerateMana", 1, 10);
        myGui = GetComponent<myGUI>();

        if (server == true)
        {
            serverScript = GameObject.FindGameObjectWithTag("SteamManager").GetComponent<Server>();
        }


    }

    public void RegenerateMana ()
    {
     //   Debug.Log("mana");
        foreach (GameObject go in players)
        {
            go.GetComponent<PlayerStats>().AddjustMana(1, gameObject);
        }
    }

    public void TogglePause ()
    {
        if (pause == false)
        {
            pause = true;
            GetComponent<myGUI>().enabled = false;
            GetComponent<MenuInGame>().enabled = true;
            Time.timeScale = 0;

        }
        else
        {
            pause = false;
            GetComponent<myGUI>().enabled = true;
            GetComponent<MenuInGame>().enabled = false;
       //     Debug.Log("Toggle2");
            Time.timeScale = 1;
        }
    }

    public void StartServer()
    {
        manager.StartServer();
    }

    public void StartCinematic ()
    {
        pause = true;
        GetComponent<myGUI>().enabled = false;
        if (camera.transform.Find("GreyReticle") != null)
        {
            camera.transform.Find("GreyReticle").gameObject.SetActive(false);
        }
        

        foreach (GameObject go in players)
        {
            Debug.Log(go.name);
            go.GetComponent<ThirdPersonUserControl>().enabled = false;
            go.GetComponent<ThirdPersonCharacter>().enabled = false;
         //   go.GetComponent<NavMeshAgent>().isStopped = true;
            go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            go.GetComponent<Animator>().SetFloat("Forward", 0);
            go.GetComponent<Animator>().SetFloat("Turn", 0);
        }

        camPos =  camera.transform.position;
        camRot = camera.transform.rotation;
    }    

    public void EndCinematic ()
    {
        pause = false;
        GetComponent<myGUI>().enabled = true;
        if (camera.transform.Find("GreyReticle") != null)
        {
            camera.transform.Find("GreyReticle").gameObject.SetActive(true);
        }

        foreach (GameObject go in players)
        {
            Debug.Log(go.name);
            go.GetComponent<ThirdPersonUserControl>().enabled = true;
            go.GetComponent<ThirdPersonCharacter>().enabled = true;
       //     go.GetComponent<NavMeshAgent>().isStopped = false;
            go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }

        camera.transform.position = camPos;
        camera.transform.rotation = camRot;

    }

 

    public void ChangeToBattle ()
    {
        inBattle = true;
        GetComponent<AudioSource>().clip = audioController.combat1;
        GetComponent<AudioSource>().Play();
        Debug.Log("Battle");
        foreach (GameObject go in players)
        {
      //      Debug.Log(go.name);
            string tempName = go.name;
            if (go.name == "Nanna")
            {
                tempName = "Balder";
            }

            if (isRPG == true)
            {
                if (Resources.Load("Animator/" + tempName + "FPS") != null)
                {
                    go.GetComponent<Animator>().runtimeAnimatorController = (RuntimeAnimatorController)(Resources.Load("Animator" + tempName + "FPS"));
                }
                else
                {
                    Debug.Log("no name " + go.name);
                }
                EnemyAI ea = go.GetComponent<EnemyAI>();
                if (ea.enabled == true)
                {
                    ea.state = EnemyAI.State.Search;
                }    
                else
                {
                    ea.StopCoroutine("FSM");
                }

            }
        }
    }

    public void ChangeToPeace ()
    {
        Debug.Log("Change to Peace");
        inBattle = false;
        GetComponent<AudioSource>().clip = audioController.audio1;
        foreach (GameObject go in players)
        {
            string tempName = go.name;
            if (go.name == "Nanna")
            {
                tempName = "Balder";
            }

            if (isRPG == true)
            {
                if (Resources.Load("Animator/" + tempName + "RPG") != null)
                {
                    go.GetComponent<Animator>().runtimeAnimatorController = (RuntimeAnimatorController)(Resources.Load("Animator" + tempName + "FPS"));
                }
                EnemyAI ea = go.GetComponent<EnemyAI>();
                if (ea.enabled == true)
                {
                    ea.state = EnemyAI.State.PlayerIdle;
                }
            }

        }
    }

    public void FPCamera ()
    {
        int labelWithTemp = (int)(Screen.width * 0.2f);
        int labelHeightTemp = (int)(Screen.height * 0.08f);

        if (multiplayer == true || isRPG == true)
        {
            Camera1Player(labelWithTemp, labelHeightTemp);
        }
        else
        {
            //   Debug.Log(players.Count);
            if (players.Count == 1)
            {
                Camera1Player(labelWithTemp, labelHeightTemp);
            }
            else if (players.Count == 2)
            {
                foreach (Camera ca in Camera.allCameras)
                {
                    if (ca.name == "Camera1")
                    {
                        ca.rect = new Rect(0, 0, 1, 0.5f);
                        GameObject zoom = Instantiate(Resources.Load("Help/Zoom"), ca.transform.position, ca.transform.rotation) as GameObject;
                        zoom.name = "Zoom";
                        zoom.transform.parent = ca.transform;
                        zoom.GetComponent<Camera>().rect = new Rect(0.3f, 0.3f, 0.3f, 0.3f);
                        player1.GetComponent<CureCounter>().center = new Rect(Screen.width * 0.5f, Screen.height * 0.75f, labelWithTemp, labelHeightTemp);

                    }

                    else if (ca.name == "Camera2")
                    {
                        ca.rect = new Rect(0, 0.5f, 1, 0.5f);
                        GameObject zoom = Instantiate(Resources.Load("Help/Zoom"), ca.transform.position, ca.transform.rotation) as GameObject;
                        zoom.name = "Zoom";
                        zoom.transform.parent = ca.transform;
                        zoom.GetComponent<Camera>().rect = new Rect(0.3f, 0.6f, 0.3f, 0.3f);
                        player2.GetComponent<CureCounter>().center = new Rect(Screen.width * 0.5f, Screen.height * 0.25f, labelWithTemp, labelHeightTemp);

                    }
                }
            }
            else if (players.Count == 3)
            {
                foreach (Camera ca in Camera.allCameras)
                {
                    if (ca.name == "Camera1")
                    {
                        ca.rect = new Rect(0, 0, 1, 0.5f);
                        GameObject zoom = Instantiate(Resources.Load("Help/Zoom"), ca.transform.position, ca.transform.rotation) as GameObject;
                        zoom.name = "Zoom";
                        zoom.transform.parent = ca.transform;
                        zoom.GetComponent<Camera>().rect = new Rect(0.3f, 0.10f, 0.30f, 0.3f);
                        player1.GetComponent<CureCounter>().center = new Rect(Screen.width * 0.5f, Screen.height * 0.75f, labelWithTemp, labelHeightTemp);

                    }

                    if (ca.name == "Camera2")
                    {
                        ca.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                        GameObject zoom = Instantiate(Resources.Load("Help/Zoom"), ca.transform.position, ca.transform.rotation) as GameObject;
                        zoom.name = "Zoom";
                        zoom.transform.parent = ca.transform;
                        zoom.GetComponent<Camera>().rect = new Rect(0.1f, 0.6f, 0.3f, 0.3f);
                        player2.GetComponent<CureCounter>().center = new Rect(Screen.width * 0.75f, Screen.height * 0.25f, labelWithTemp, labelHeightTemp);

                    }

                    if (ca.name == "Camera3")
                    {
                        ca.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                        GameObject zoom = Instantiate(Resources.Load("Help/Zoom"), ca.transform.position, ca.transform.rotation) as GameObject;
                        zoom.name = "Zoom";
                        zoom.transform.parent = ca.transform;
                        zoom.GetComponent<Camera>().rect = new Rect(0.6f, 0.6f, 0.3f, 0.3f);
                        player3.GetComponent<CureCounter>().center = new Rect(Screen.width * 0.75f, Screen.height * 0.75f, labelWithTemp, labelHeightTemp);

                    }
                }
            }
            else if (players.Count == 4)
            {
                foreach (Camera ca in Camera.allCameras)
                {
                    if (ca.name == "Camera1")
                    {
                        ca.rect = new Rect(0, 0, 0.5f, 0.5f);
                        GameObject zoom = Instantiate(Resources.Load("Help/Zoom"), ca.transform.position, ca.transform.rotation) as GameObject;
                        zoom.name = "Zoom";
                        zoom.transform.parent = ca.transform;
                        zoom.GetComponent<Camera>().rect = new Rect(0.1f, 0.1f, 0.3f, 0.3f);
                        player1.GetComponent<CureCounter>().center = new Rect(Screen.width * 0.25f, Screen.height * 0.25f, labelWithTemp, labelHeightTemp);

                    }

                    if (ca.name == "Camera2")
                    {
                        ca.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                        GameObject zoom = Instantiate(Resources.Load("Help/Zoom"), ca.transform.position, ca.transform.rotation) as GameObject;
                        zoom.name = "Zoom";
                        zoom.transform.parent = ca.transform;
                        zoom.GetComponent<Camera>().rect = new Rect(0.1f, 0.6f, 0.3f, 0.3f);
                        player2.GetComponent<CureCounter>().center = new Rect(Screen.width * 0.75f, Screen.height * 0.25f, labelWithTemp, labelHeightTemp);

                    }

                    if (ca.name == "Camera3")
                    {
                        ca.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                        GameObject zoom = Instantiate(Resources.Load("Help/Zoom"), ca.transform.position, ca.transform.rotation) as GameObject;
                        zoom.name = "Zoom";
                        zoom.transform.parent = ca.transform;
                        zoom.GetComponent<Camera>().rect = new Rect(0.6f, 0.1f, 0.3f, 0.3f);
                        player3.GetComponent<CureCounter>().center = new Rect(Screen.width * 0.75f, Screen.height * 0.75f, labelWithTemp, labelHeightTemp);

                    }

                    if (ca.name == "Camera4")
                    {
                        ca.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                        GameObject zoom = Instantiate(Resources.Load("Help/Zoom"), ca.transform.position, ca.transform.rotation) as GameObject;
                        zoom.name = "Zoom";
                        zoom.transform.parent = ca.transform;
                        zoom.GetComponent<Camera>().rect = new Rect(0.6f, 0.6f, 0.3f, 0.3f);
                        player1.GetComponent<CureCounter>().center = new Rect(Screen.width * 0.25f, Screen.height * 0.75f, labelWithTemp, labelHeightTemp);

                    }
                }
            }

        }


    }

    private void Camera1Player(int labelWithTemp, int labelHeightTemp)
    {
        foreach (Camera ca in Camera.allCameras)
        {
            //       Debug.Log(ca.name);
            if (ca.name == "Camera1")
            {
                ca.rect = new Rect(0, 0, 1, 1);
                GameObject zoom = Instantiate(Resources.Load("Help/Zoom"), ca.transform.position, ca.transform.rotation) as GameObject;
                zoom.name = "Zoom";
                zoom.transform.parent = ca.transform;
                zoom.GetComponent<Camera>().rect = new Rect(0.3f, 0.3f, 0.3f, 0.3f);
                player1.GetComponent<CureCounter>().center = new Rect(Screen.width * 0.5f, Screen.height * 0.5f, labelWithTemp, labelHeightTemp);

            }
            else
            {
                ca.enabled = false;
            }
        }
    }

    public void Reinitialize ()
    {
        foreach (GameObject go in reinitialize)
        {
            go.SetActive(true);
        }
    }

    public int GetMultiplayerID ()
    {
        return playerMultiID;
        playerMultiID++;
    }

    public int GetMultiEnemyID()
    {
        return enemyMultiID;
        enemyMultiID++;
    }

    public void ReceiveServerInfo(int packageNo,  string steamMember, string name, int level, bool aiming, Vector3 pos, Vector3 rot, int internalID, float forward, float turn, string jump, string action, Quaternion aimPos, string state, int targetID, int addjustHealth, int health, string block)
    {
        //Players from 0 to 4
        if (internalID <= 4)
        {
            if (internalID != playerID)
            {
                foreach (GameObject go in players)
                {
                    if (go.GetComponent<PlayerStats>().multiplayerID == internalID)
                    {
                        GameObject ghostAvatar = go.GetComponent<EnemyAI>().ghostAvatar;
                        EnemyAI ea = go.GetComponent<EnemyAI>();
                        PlayerAttack pa = go.GetComponent<PlayerAttack>();
                        Animator anim = go.GetComponent<Animator>();
                        pa.aiming = true;
                        ghostAvatar.transform.position = pos;
                        ghostAvatar.transform.rotation = Quaternion.Euler(rot);
                        
                        ea.forward = forward;
                        ea.turn = turn;
          //              Debug.Log(aiming + "/" + ea.aiming);
                        if (aiming == true && ea.aiming == false)
                        {
                            ea.GhostAim();
                        }
                        else if (aiming == false && ea.aiming == true)
                        {
                            ea.GhostStopAim();
                        }

                        if (jump == "true")
                        {
                            ea.GhostJump();
                        }

          //              Debug.Log(action + "/" + jump);
                        if (action == "At")
                        {
                            bool aimData = false;
                            Debug.Log(aimPos);
                            ea.GhostAttack(aimPos);
                        }

                        if (block == "true")
                        {
           
                        }
                        else
                        {

                        }
                    }
                }
            }
        }

        //Enemies from 3000 to 5000
        if (internalID >= 3000)
        {
            bool enemyOnList = false;
            GameObject enemyGO = null;
            PlayerStats ps = null;
            foreach (GameObject go in enemies)
            {
                if (go.GetComponent<PlayerStats>().multiplayerID == internalID)
                {
                    enemyOnList = true;
                    enemyGO = go;
                    ps = go.GetComponent<PlayerStats>();
                    break;
                }
            }

            //If enemy not found on Enemy list, look on enemydead list. 
            if (enemyOnList == false)
            {
                foreach (GameObject go in enemiesDead)
                {
                    if (go.GetComponent<PlayerStats>().multiplayerID == internalID)
                    {
                        enemyOnList = true;
                        enemyGO = go;
                        ps = go.GetComponent<PlayerStats>();
                        break;
                    }
                }
            }

            //If enemy doesnt exit 
            if (enemyOnList == false)
            {
                //Create if not server, ensure it is destroyed if server. 
                if (server == false)
                {
                    enemyGO = Instantiate(Resources.Load("Enemy/" + name), pos, Quaternion.Euler(rot)) as GameObject;
                    enemyGO.name = name;
                    ps = enemyGO.GetComponent<PlayerStats>();
                    ps.multiplayerID = internalID;
                    if (enemyGO.activeSelf == false)
                    {
                        enemyGO.SetActive(true);
                    }
                }
                else
                {
                    serverScript.SendInfo("All", serverScript.packageNo, serverScript.steamName, name, level, aiming, pos, rot, internalID.ToString(), forward, turn, false, action, aimPos, state, targetID.ToString(), addjustHealth, health, false); 
                }
            }
            
            if (server == true)
            {
                if (addjustHealth != 0)
                {
                    
                    GameObject playerGO = null;
                    foreach (GameObject go in players)
                    {
                        if (go.GetComponent<PlayerStats>().multiplayerID == targetID)
                        {
                            playerGO = go;
                        }
                    }

                    if (playerGO == null)
                    {
                        playerGO = enemyGO;
                    }
                    
                    ps.AddjustHealth(-addjustHealth, playerGO, true);
                }
            }
            else
            {
                Debug.Log(health);
                EnemyAI ea = enemyGO.GetComponent<EnemyAI>();
                ps.health = health;
                if (health <= 0)
                {
                    ps.Death();
                    ea.state = EnemyAI.State.Dead;
                }
      
                if (state != ea.state.ToString())
                {
                    bool assignTarget = false;

                    if (ea.target == null)
                    {
                        assignTarget = true;
                    }
                    else if (targetID != ea.target.GetComponent<PlayerStats>().multiplayerID)
                    {
                        assignTarget = true;
                    }

                    if (assignTarget == true)
                    {

                        foreach (GameObject pa in players)
                        {
                            if (pa.GetComponent<PlayerStats>().multiplayerID == targetID)
                            {
                                ea.target = pa;
                            }
                        }
                    }

                    Debug.Log(state);
                    if (state == "Attack")
                    {
                        ea.state = EnemyAI.State.Attack;
                    }
                    else if (state == "MoveToEngage")
                    {
                        ea.state = EnemyAI.State.MoveToEngage;
                    }
                    else if (state == "Search")
                    {
                        ea.state = EnemyAI.State.Search;
                    }
                }

                float distanceAddjust = Vector3.Distance(enemyGO.transform.position, pos);
                if (distanceAddjust > 0.75f)
                {
                    enemyGO.transform.position = pos;
                }
            }
        }
    }


    private IEnumerator CheckDead ()
    {
        while (alive)
        {
            for (int cnt = 0; cnt < enemies.Count; cnt++)
            {
                if (enemies[cnt].GetComponent<PlayerStats>().health <= 0)
                {
                    enemies[cnt].SetActive(false);
                    enemies.RemoveAt(cnt);
                    if (enemiesDead.Contains(enemies[cnt]) == false)
                    {
                        enemiesDead.Add(enemies[cnt]);
                    }
                }
            }

            foreach (GameObject go in enemiesDead)
            {
                if (go.GetComponent<PlayerStats>() != null)
                {
                    go.GetComponent<PlayerStats>().deadCounter--;
                    if (go.GetComponent<PlayerStats>().deadCounter <= 0)
                    {
                        if (go.activeSelf)
                        {
                            go.SetActive(false);
                        }
                    }
                }
            }

            yield return new WaitForSeconds(1);
        }

        yield return null;

       
    }

    public void ChangeCharacter()
    {
   //     Debug.Log("Change");
        int tempINT = 0;
        for (int cnt = 0; cnt < players.Count; cnt++)
        {
            if (activePlayer == players[cnt])
            {
                tempINT = cnt;
            }
        }

        if (tempINT == players.Count - 1)
        {
            tempINT = 0;
        }
        else
        {
            tempINT++;
        }

        for (int cnt = 0; cnt < players.Count; cnt++)
        {
       //     Debug.Log(players[cnt] + "/" + cnt + "/" + tempINT);
            if (cnt == tempINT)
            {
                activePlayer = players[cnt];
     
                players[cnt].GetComponent<EnemyAI>().enabled = false;
                players[cnt].GetComponent<ThirdPersonCharacter>().enabled = true;
                players[cnt].GetComponent<ThirdPersonUserControl>().enabled = true;
                camera.GetComponent<MouseOrbitImproved>().target = players[cnt].transform.Find("Camera");

            }
            else
            {
                players[cnt].GetComponent<EnemyAI>().state = EnemyAI.State.PlayerIdle;
                players[cnt].GetComponent<EnemyAI>().enabled = true;
                players[cnt].GetComponent<ThirdPersonCharacter>().enabled = false;
                players[cnt].GetComponent<ThirdPersonUserControl>().enabled = false;
            }
        }
    }

 
}
