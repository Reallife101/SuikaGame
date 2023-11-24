using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    //Player manager will keep track of scoring, tarot cards, and respawning for the player (ie all data we want to persist past player death)
    private PhotonView PV;
    public GameObject[] playerPrefabs;
    private GameObject leftSpawn;
    private GameObject rightSpawn;

    GameObject controller;
    playerController PlayerController;


    private void Awake()
    {
        
        rightSpawn = GameObject.FindWithTag("RightSpawn");
        leftSpawn = GameObject.FindWithTag("LeftSpawn");
        PV = GetComponent<PhotonView>();

    }

    private void Start()
    {
        if (PV.IsMine)
        {
            Spawn();
        }
    }

    //Spawns/Respawn player
    private void Spawn()
    {

        Transform spawnPoint;

        if (PhotonNetwork.IsMasterClient)
        {
            spawnPoint = leftSpawn.transform;
        }
        else
        {
            spawnPoint = rightSpawn.transform;
        }

        //Debug.Log("AVATAR NUMBER " + (int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]);
        Debug.Log("Found Spawn");
        GameObject playerToSpawn = playerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        Debug.Log("Found Spawn Game Object");
        controller = PhotonNetwork.Instantiate(playerToSpawn.name, spawnPoint.position, Quaternion.identity, 0, new object[] { PV.ViewID });
        Debug.Log("End Spawn");

        PlayerController = controller.GetComponent<playerController>();

       
    }

    public void SetController(GameObject _controller)
    {
        controller = _controller;
    }

    public GameObject GetController()
    {
        return controller;
    }

    public playerController GetPlayerController()
    {
        return controller.GetComponent<playerController>();
    }


}
