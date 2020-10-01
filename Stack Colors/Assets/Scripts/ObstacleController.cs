using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObstacleController : MonoBehaviour
{
    //as i said belove in the PlayerController script, i make a separate script to control obstacle hit.
    //i done that because i'm manipulating physic layers a little for crash visual. let's take a look at the code! 
    public GameObject Player;
    PlayerController playerController;
    public PhysicMaterial physicMaterial;
    // Start is called before the first frame update
    void Start()
    {
        //instantiating playercontroller script to reach it's public variables and methods
        playerController = Player.GetComponent<PlayerController>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //after one of our gameobjects in cubes list (doesn't matter which of them) hit an obstacle, we are giving a different physic layer to obstacle
        gameObject.layer = LayerMask.NameToLayer("layer3");
        foreach(Transform child in playerController.gameObject.transform)
        {
            //now we know our cubes block hit an obstacle.
            //all we have to do detach the ones which are above the obstacle's bottom position
            //and to do that, we are taking obstacle's mesh bound info to calculate where is the exact position of obstacle's bottom
            //of course we need to avoid hard coding, that means we need to think clever, and use mesh bouns extents to measue it 
            if (child.transform.position.y > transform.position.y - gameObject.GetComponent<MeshFilter>().mesh.bounds.extents.y * gameObject.transform.localScale.y)
            {
                //after a million tries, finally i found the correct way to do it right, you can see the sequence belove 
                child.gameObject.layer = LayerMask.NameToLayer("layer3");
                if (child.tag == "bigCube")
                    playerController.scoreValue -= 100;
                if (child.tag == "cube")
                    playerController.scoreValue -= 50;
                child.gameObject.tag = "waste";
                child.transform.parent = null;
                child.gameObject.AddComponent<Rigidbody>();
                child.gameObject.GetComponent<Rigidbody>().drag = 0.7f;
                child.gameObject.GetComponent<Rigidbody>().angularDrag = 0.7f;
                child.gameObject.GetComponent<BoxCollider>().material = physicMaterial;
                child.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 5);
                playerController.listOfChildCubes();
                playerController.powerCounterNumber += 1;
                
            }
            transform.GetComponent<BoxCollider>().isTrigger = false;
            
        }
        
        playerController.listOfChildCubes();
        
    }
}
