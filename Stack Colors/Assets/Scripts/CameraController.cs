using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    PlayerController pC;
    private float speed = 25f;
    // Start is called before the first frame update
    void Start()
    {
        pC = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        var step = speed * Time.deltaTime;
        //camera will follow the last cube when we reach the finish point
        //otherwise camera will follow the player
        if(pC.isFinished == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, pC.cubes[pC.cubes.Count - 1].transform.position.z - 6.9f),step);
        }
        else
            transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z - 6.9f);
    }
}
