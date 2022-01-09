using UnityEngine;
using Mirror;

public class ConnectPlayers : MonoBehaviour
{
    private bool messageRegistered = false;
    public GameObject playerPuppet;
    private ThirdPersonUserControl tpc;
    //  private NetworkConnection netClient;
    private NetworkIdentity clientID;
    public class ScoreMessage : MessageBase
    {
        public float x;
        public float y;
        public int attack;
        public int block;
        public int jump;
        public int special;
        public int left;
        public int right;
        public string message;
    }



    private void Start()
    {
        Invoke("CheckConnection", 1);

    }

    public void CheckConnection()
    {

        if (NetworkServer.active)
        {
            Debug.Log("Server");
            if (messageRegistered == false)
            {
                NetworkServer.RegisterHandler<ScoreMessage>(OnScore);
                NetworkClient.RegisterHandler<ScoreMessage>(OnScore);
                messageRegistered = true;
            }

        }
        else
        {
       //     Debug.Log("No server");

        }

        Invoke("CheckConnection", 1);
    }

    private void Update()
    {

        /*
         if (Input.GetKeyUp (KeyCode.Space))
        {
            
            SendScore(5, 4, 1, 0, 0, 0, 1, 0);
        }*/
    }

    public void SendScore(float x, float y, int attack, int block, int jump, int special, int left, int right, string message)
    {
        ScoreMessage msg = new ScoreMessage()
        {
            x = x,
            y = y,
            attack = attack,
            block = block,
            jump = jump,
            special = special,
            left = left,
            right = right,
            message = message

        };

    }

    public void SetupClient()
    {
        NetworkClient.RegisterHandler<ScoreMessage>(OnScore);
        NetworkClient.Connect("localhost");
    }

    public void OnScore(NetworkConnection conn, ScoreMessage msg)
    {
        //   Debug.Log("OnScoreMessage " + msg.x + "/" + msg.y + msg.attack);
        Debug.Log(conn.identity + "/" + msg.attack + "/" + conn.address + "/" + conn.connectionId);


        clientID = conn.identity;

        tpc.attackWifi = msg.attack;

        tpc.blockWifi = msg.block;

        tpc.jumpWifi = msg.jump;

        tpc.specialWifi = msg.special;


        tpc.xAxis = msg.x;
        tpc.yAxis = msg.y;
    }
}
