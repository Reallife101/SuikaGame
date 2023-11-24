using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class photonGameManager : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        photonBallCombine bc = collision.gameObject.GetComponent<photonBallCombine>();

        if (bc && bc.canLose)
        {
            bc.loseGame();
        }
    }
}
