using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class photonPlayerController : MonoBehaviour
{
    //Components
    private PlayerManager myPM;
    private PhotonView myPV;
    private Rigidbody2D myRB;

    [SerializeField]
    float speed;

    [SerializeField]
    float maxRight;

    [SerializeField]
    float maxLeft;

    [SerializeField]
    List<GameObject> objects;

    [SerializeField]
    List<GameObject> UIObjects;

    [SerializeField]
    GameObject UICanvas;

    [SerializeField]
    GameObject hinderanceCanvas;

    [SerializeField]
    float spawnRate;

    public int nextObjectIndex;

    private float spawnTimer;

    [SerializeField]
    TMP_Text scoreText;

    private int score;
    private int hinderanceTracker;

    [SerializeField]
    List<GameObject> hinderanceSpawns;

    private void Awake()
    {
        myPV = GetComponent<PhotonView>();
        myRB = GetComponent<Rigidbody2D>();
        myPM = PhotonView.Find((int)myPV.InstantiationData[0]).GetComponent<PlayerManager>();

        spawnTimer = 0f;
        nextObjectIndex = Random.Range(0, UIObjects.Count);


        if (!myPV.IsMine)
        {
            UICanvas.SetActive(false);
        }
        else
        {
            showNextUI();
            score = 0;
            hinderanceTracker = 0;
            scoreText.text = "Score: " + score;

        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!myPV.IsMine)
        {
            return;
        }

        float horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        if ((horizontal < 0 && transform.position.x > maxLeft) || (horizontal > 0 && transform.position.x < maxRight))
        {
            transform.Translate(horizontal, 0, 0);
        }

        if (Input.GetKey(KeyCode.Space) && spawnTimer > spawnRate)
        {
            spawnTimer = 0f;
            GameObject ball = PhotonNetwork.Instantiate(objects[nextObjectIndex].name, transform.position + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0), Quaternion.identity);
            ball.GetComponent<photonBallCombine>().player = this;

            scoreAdd((nextObjectIndex+1) * 100);
            nextObjectIndex = Random.Range(0, objects.Count);
            showNextUI();
        }

        //update spawn timer
        spawnTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(hinderanceCoroutine());
        }
    }

    void showNextUI()
    {
        foreach (GameObject go in UIObjects)
        {
            go.SetActive(false);
        }
        UIObjects[nextObjectIndex].SetActive(true);
    }

    public void scoreAdd(int i)
    {
        score += i;
        scoreText.text = "Score: " + score;

        hinderanceTracker += i;
        
        if (hinderanceTracker > 1000)
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Game Manager"))
            {
                PhotonView pm = go.GetComponent<PhotonView>();
                if (pm != null && pm.ViewID != myPV.ViewID)
                {
                    Debug.Log("found other player");
                    pm.gameObject.GetComponent<photonPlayerController>().spawnHinder();
                    hinderanceTracker = 0;
                }
            }
        }
    }
    
    public void spawnHinder()
    {
        myPV.RPC(nameof(RPC_SpawnHinderance), myPV.Owner);
    }
    
    public void loseGame()
    {
        if (myPV.IsMine)
        {
            myPM.loseGame();
        }
    }
    
    [PunRPC]
    void RPC_SpawnHinderance()
    {
        //Debug.Log("Spawn hinderance");
        StartCoroutine(hinderanceCoroutine());
    }
    
    private IEnumerator hinderanceCoroutine()
    {
        hinderanceCanvas.SetActive(true);

        yield return new WaitForSecondsRealtime(.1f);
        hinderanceCanvas.SetActive(false);
        Vector3 t = hinderanceSpawns[Random.Range(0, hinderanceSpawns.Count)].transform.localPosition + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
        PhotonNetwork.Instantiate(objects[Random.Range(0, objects.Count)].name, t, Quaternion.identity);

    }
    
}
