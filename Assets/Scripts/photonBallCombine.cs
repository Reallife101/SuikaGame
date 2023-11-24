using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class photonBallCombine : MonoBehaviour
{
    [SerializeField]
    public int level;

    public bool spawn = true;

    public bool canLose = false;

    private PhotonView myPV;

    [SerializeField]
    List<GameObject> objects;

    public photonPlayerController player;


    private void Awake()
    {
        myPV = GetComponent<PhotonView>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        photonBallCombine bc = collision.gameObject.GetComponent<photonBallCombine>();

        if (bc && myPV.IsMine)
        {
            canLose = true;
            if (bc.level == level)
            {
                //Prevent double spawning
                bc.spawn = false;
                if (spawn)
                {
                    //Calculate midpoint to spawn at
                    Vector3 bcPos = bc.gameObject.transform.position;
                    Vector3 spawnLocation = new Vector3(bcPos.x + (transform.position.x - bcPos.x) / 2, bcPos.y + (transform.position.y - bcPos.y) / 2, bcPos.z + (transform.position.z - bcPos.z) / 2);

                    //Delete both and spawn new one
                    PhotonNetwork.Destroy(bc.gameObject);
                    GameObject ball = PhotonNetwork.Instantiate(objects[Mathf.Min(level, objects.Count - 1)].name, spawnLocation, Quaternion.identity); //dont index out of range
                    ball.GetComponent<photonBallCombine>().player = player;

                    player.scoreAdd(Mathf.Min(level, objects.Count - 1) * 100);
                    PhotonNetwork.Destroy(gameObject);
                }
            }

        }
    }

    public void loseGame()
    {
        if (myPV.IsMine)
        {
            player.loseGame();
        }
    }
}
