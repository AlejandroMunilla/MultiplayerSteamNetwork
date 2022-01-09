using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSendInfo : MonoBehaviour
{
    public string distribution = "All";
    public string internalID;
    public string action = "None";
    public int addjustHealth = 0;
    public float forward;
    public float turn;

    private Server server;
    private PlayerStats ps;
    private Animator anim;
    private string internaID;
    private MouseOrbitImproved mouseOrbit;
    private ThirdPersonUserControl tpu;
    private PlayerAttack pa;
    private EnemyAI ea;
    private bool player = true;
    private float timer = 0.05f;


    // Start is called before the first frame update
    void OnEnable ()
    {
        ps = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
        tpu = GetComponent<ThirdPersonUserControl>();
        pa = GetComponent<PlayerAttack>();
        ea = GetComponent<EnemyAI>();

        internaID = GetComponent<PlayerStats>().multiplayerID.ToString();
  //      Debug.Log(GetComponent<PlayerStats>().multiplayerID.ToString() + "/" + GetComponent<PlayerStats>().multiplayerID);
        if (gameObject.tag == "Enemy")
        {
            timer = 0.1f;
            player = false;
        }
        else
        {
            Invoke("GetCamera", 0);
        }

        InvokeRepeating("SendInfo", 0.5f, timer);
        if (GameObject.FindGameObjectWithTag("SteamManager").GetComponent<Server>() != null)
        {
            server = GameObject.FindGameObjectWithTag("SteamManager").GetComponent<Server>();
        }
        else
        {
            GameObject.FindGameObjectWithTag("SteamManager").AddComponent<Server>();
            server = GameObject.FindGameObjectWithTag("SteamManager").GetComponent<Server>();
        }

       

    }
  

    public void SendInfo ()
    {
        //      (int packageNo, string steamName, string name, int level, bool aiming, Vector3 pos, Vector3 rot, string internalID)

        //      Debug.Log(mouseOrbit.transform.eulerAngles);
        Quaternion aimRot = Quaternion.identity;
        bool jumping = false;
        bool aiming = false;
        bool block = false;
        if (player == true)
        {
            aimRot = tpu.cam.transform.rotation;
            jumping = tpu.jumping;
            aiming = mouseOrbit.aiming;
            block = anim.GetBool("Blocking");
        }

        
        server.SendInfo(distribution, (int)Time.realtimeSinceStartup, server.steamName, gameObject.name, 1, aiming, transform.position, transform.rotation.eulerAngles, internaID, anim.GetFloat ("Forward"), anim.GetFloat("Turn"), jumping, action, aimRot, ea.state.ToString(), ps.multiplayerID.ToString(), addjustHealth, ps.health, block);
   //     Debug.Log(ps.health);
        if (player == true)
        {
            if (tpu.jumping == true)
            {
                tpu.jumping = false;
            }
        }

        if (action != "None")
        {
      //      Debug.Log(mouseOrbit.transform.rotation);
            action = "None";
        }
        if (addjustHealth != 0)
        {
            addjustHealth = 0;
        }
        if (distribution != "All")
        {
            distribution = "All";
        }
    }


    private void GetCamera ()
    {
        mouseOrbit = GetComponent<ThirdPersonUserControl>().cam.GetComponent<MouseOrbitImproved>();
    }
     

}
