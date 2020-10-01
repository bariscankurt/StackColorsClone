using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //we have 3 variable category: public ones, private ones, and hidden public ones
    //
    public Touch touch;
    public PhysicMaterial physicMaterial;
    public Text powerCounter, score, bonus;
    private int  bonusValue;
    [HideInInspector]
    public int scoreValue;
    [HideInInspector]
    public int powerCounterNumber;
    [HideInInspector]
    public List<GameObject> cubes = new List<GameObject>();
    public float playerSpeed;
    public float moveSpeed;
    public GameObject generatedCube, generatedBigCube;
    public Material[] Materials;
    [HideInInspector]
    public Material cubeMaterial;
    private bool isPowerUp = false, allColorChange = true, isMoving = true;
    private Material toggleMaterial;
    [HideInInspector]
    public bool isFinished = false;
    public Text restartInfo;
    //awake method runs before start
    private void Awake()
    {
        // for demo purposes, we need to make things simple
        //because of that i didn't use rnd but if i need, i can use it easily
        //int rnd = Random.Range(0, 3);
        listOfChildCubes();
        //choosing a material for our cubes
        cubes[0].GetComponent<MeshRenderer>().material = Materials[1];
        //and one for future toggling situations
        cubeMaterial = cubes[0].GetComponent<MeshRenderer>().material;
    }
    // Start is called before the first frame update
    void Start()
    {
        powerCounterNumber = 70;
        scoreValue = 0;
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        //here, we are checking if powerup activated or not
        if (powerCounterNumber < 0)
        {
            isPowerUp = true;
            //a simple coroutine function for powerupping our cubes for a short period of time
            StartCoroutine(normalPowerAgain());
            Move(isPowerUp,isMoving);
            powerCounter.text = "--GO!--";
        }
        else
        {
            Move(isPowerUp,isMoving);
            powerCounter.text = "Power Up: " + powerCounterNumber.ToString();
        }


        bonus.text = "Bonus: " + bonusValue.ToString();
        score.text = "Score: " + scoreValue.ToString();
        
    }
    //move function takes two parameter, we are doing this because we are calling move method in update method.
    //because of that our only option is to control state changes through boolean parameters
    void Move(bool isPowerUp,bool isMoving)
    {
        if(isMoving == true)
        {
            if (isPowerUp == true)
                {
                    cubes[0].transform.localScale = new Vector3(10, cubes[0].transform.localScale.y, cubes[0].transform.localScale.z);
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 2f * playerSpeed);
                    transform.position = new Vector3(0, transform.position.y, transform.position.z);
                }
            else
                {
                    cubes[0].transform.localScale = new Vector3(2, cubes[0].transform.localScale.y, cubes[0].transform.localScale.z);
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 1f * playerSpeed);
                }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    transform.position = new Vector3(Mathf.Clamp(transform.position.x + moveSpeed * Time.deltaTime, -4, 4), transform.position.y, transform.position.z);
                }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.position = new Vector3(Mathf.Clamp(transform.position.x - moveSpeed * Time.deltaTime, -4, 4), transform.position.y, transform.position.z);
                }
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    transform.position = new Vector3(Mathf.Clamp(transform.position.x + touch.deltaPosition.x * 0.01f,-4,4), transform.position.y, transform.position.z);
                }
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        
        //this is the part that things get difficult, we need to write a perfect control mechanism
        //a simple mistake can ruin our game instantly and because of that
        //first we are checking if other object's material is equal to our cubes material
        
        cubeMaterial = cubes[0].GetComponent<MeshRenderer>().material;
        if (cubeMaterial.name.ToString() == other.GetComponent<MeshRenderer>().material.name.ToString() && other.tag != "waste" && other.tag != "redBonus" && other.tag != "pinkBonus" && other.tag != "greenBonus" )
        {
            //after that, we know we crash into an object which has the same material
            //and we know other object isn't one of the any other game objects(like obstacle, waste or bonus)
            //at this point of time we are creating a toggleMaterial to toggle cubes list color when we hit a bonus 
            toggleMaterial = cubes[0].GetComponent<MeshRenderer>().material;
            
            Destroy(other.gameObject);
            listOfChildCubes();
            
            if (other.tag == "BigCube")
            {

                
                if (cubes[cubes.Count - 1].tag == "cube")
                {
                    //if we hit a bigcube and our last object in cubes list is a cube, we are doing a little calculation
                    //and assignment to translate our block in y axis
                    
                    GameObject newBigCube = Instantiate(generatedBigCube, new Vector3(transform.position.x, cubes[cubes.Count - 1].transform.position.y + 0.2f, transform.position.z), Quaternion.Euler(0, 0, 0));
                    newBigCube.transform.parent = gameObject.transform;
                    newBigCube.GetComponent<MeshRenderer>().material = cubeMaterial;
                    newBigCube.layer = LayerMask.NameToLayer("layer2");
                    listOfChildCubes();
                    //and we are controlling the "powerUp" situation
                    if(isPowerUp == false)
                    {
                        powerCounterNumber -= 1;
                    } 
                    else
                    {
                        powerUpActivated(allColorChange);
                    }
                        
                    scoreValue += 100;
                }
                else if (cubes[cubes.Count - 1].tag == "BigCube")
                {
                    //logic is similar, only difference is this time we are doing it for a bigcube
                    
                    GameObject newBigCube = Instantiate(generatedBigCube, new Vector3(transform.position.x, cubes[cubes.Count - 1].transform.position.y + 0.3f, transform.position.z), Quaternion.Euler(0, 0, 0));
                    newBigCube.transform.parent = gameObject.transform;
                    newBigCube.GetComponent<MeshRenderer>().material = cubeMaterial;
                    newBigCube.layer = LayerMask.NameToLayer("layer2");
                    listOfChildCubes();
                    if(isPowerUp == false)
                    {
                        powerCounterNumber -= 1;
                    }
                        
                    else
                    {
                        
                        powerUpActivated(allColorChange);
                    }
                        
                    scoreValue += 100;
                }
            }
            else //if we are in here, it means we hit a regular cube not a bigcube :)
            {

                if (cubes[cubes.Count - 1].tag == "cube")
                {
                    GameObject newCube = Instantiate(generatedCube, new Vector3(transform.position.x, cubes[cubes.Count - 1].transform.position.y + 0.1f, transform.position.z), Quaternion.Euler(0, 0, 0));
                    newCube.transform.parent = gameObject.transform;
                    newCube.GetComponent<MeshRenderer>().material = cubeMaterial;
                    newCube.layer = LayerMask.NameToLayer("layer2");
                    listOfChildCubes();
                    if (isPowerUp == false)
                    {
                        powerCounterNumber -= 1;
                    }
                    else
                    {
                        powerUpActivated(allColorChange);
                    }  
                    scoreValue += 50;
                    
                }
                else if (cubes[cubes.Count - 1].tag == "BigCube")
                {
                    GameObject newCube = Instantiate(generatedCube, new Vector3(transform.position.x, cubes[cubes.Count - 1].transform.position.y + 0.2f, transform.position.z), Quaternion.Euler(0, 0, 0));
                    newCube.transform.parent = gameObject.transform;
                    newCube.GetComponent<MeshRenderer>().material = cubeMaterial;
                    newCube.layer = LayerMask.NameToLayer("layer2");
                    newCube.layer = LayerMask.NameToLayer("layer2");
                    listOfChildCubes();
                    if(isPowerUp == false)
                    {
                        powerCounterNumber -= 1;
                    }
                    else
                    {
                        powerUpActivated(allColorChange);
                    }
                    scoreValue += 50;
                }
            }
            


        }
        else //in here we know we didn't hit a similar object, that means we need to check the other object to know what did we hit
        {
            if(other.tag == "finish")
            {
                //if we hit a finish object we want to see our cubes are falling
                float power = 2f;
                cubes[0].GetComponent<BoxCollider>().isTrigger = false;
                isMoving = false;
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
                for (int i = 1; i <cubes.Count; i++)
                {

                    cubes[i].transform.parent = null;
                    cubes[i].tag = "waste";
                    cubes[i].gameObject.AddComponent<Rigidbody>();
                    cubes[i].gameObject.GetComponent<Rigidbody>().drag = 0.4f;
                    cubes[i].gameObject.GetComponent<Rigidbody>().angularDrag = 0.4f;
                    cubes[i].gameObject.GetComponent<BoxCollider>().material = physicMaterial;
                    cubes[i].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, power);
                    power += 0.5f;
                }
                isFinished = true;
                //after the falling process is done, we need to restart the game
                //because of it, we are calling a coroutine method
                StartCoroutine(RestartIn3());
            }
            else if (other.tag == "turnRed")
            {
                //in here we are changing every object's color in cubes list
                for (int i = 0; i < cubes.Count; i++)
                {
                    cubes[i].GetComponent<MeshRenderer>().material = Materials[0];
                }
                //and of course we need to change toggleMaterial too, because now we have a color block which has totally different color
                toggleMaterial = cubes[0].GetComponent<MeshRenderer>().material;
            }
            else
            {
                //now, we need to check other situations
                listOfChildCubes();
                if(cubes.Count == 1)
                {
                    //cubes.Count == 1 means we lost the game and because of that we are calling loadscene method to restart our game
                    SceneManager.LoadScene(0);
                }
                else if(other.tag != "obstacle" && other.tag != "redBonus" && other.tag != "pinkBonus" && other.tag != "greenBonus")
                {
                    //in here we know we hit a different cube, that's why we are adjusting our cubes list again
                    if(cubes[1].tag == "BigCube")
                    {
                        //adjusting for a bigcube
                        cubes[1].transform.parent = null;
                        Destroy(cubes[1]);
                        if(isPowerUp == false)
                            powerCounterNumber += 1;
                        scoreValue -= 100;
                        listOfChildCubes();
                        for (int i = 1; i < cubes.Count; i++)
                        {
                            cubes[i].transform.position = new Vector3(cubes[i].transform.position.x, cubes[i].transform.position.y - 0.3f, cubes[i].transform.position.z);
                        }
                    }
                    else if(cubes[1].tag == "cube")
                    {
                        //adjusting for a regular cube
                        cubes[1].transform.parent = null;
                        Destroy(cubes[1]);
                        listOfChildCubes();
                        if(isPowerUp == false)
                            powerCounterNumber += 1;
                        scoreValue -= 50;
                        for (int i = 1;i<cubes.Count;i++)
                        {
                            cubes[i].transform.position = new Vector3(cubes[i].transform.position.x, cubes[i].transform.position.y - 0.1f, cubes[i].transform.position.z);
                        }
                    }
                    
                    
                }
                else
                {
                    //i know, you thought our control statements are never gonna end, don't worry this is the last part
                    //now we know exactly what we hit, we hit a bonus object and we need to find it's color
                    //to make a simple but good looking color toggling effect
                    if (other.tag == "redBonus")
                    {
                        Destroy(other.gameObject);
                        //of course we need to use coroutine, because we don't want a permanent color change
                        StartCoroutine(toggleBonusColor(2));
                    }
                        
                    if (other.tag == "pinkBonus")
                    {
                        Destroy(other.gameObject);
                        StartCoroutine(toggleBonusColor(3));
                    }
                        
                    if (other.tag == "greenBonus")
                    {
                        Destroy(other.gameObject);
                        StartCoroutine(toggleBonusColor(4));
                    }
                }
            }
        }
    }

    //a function to hold all cubes in a list and we need to recall this function everytime
    //when we want to change cubes list
    public void listOfChildCubes()
    {
        cubes.Clear();
        
        foreach (Transform child in transform)
        {
            cubes.Add(child.gameObject);
        }
        for(int i = 2; i < cubes.Count; i++)
        {
            cubes[i].GetComponent<BoxCollider>().isTrigger = false;
        }

        if(cubes.Count>1)
        cubes[1].GetComponent<BoxCollider>().enabled = true;

        
    }
    
    //this function helps us to make a powerUp simulation
    private void powerUpActivated(bool allColorChange)
    {
        if(allColorChange == true)
        {
            //we need to change all cube colors like in the stack color game
            var allgameobjects = GameObject.FindGameObjectsWithTag("cube");
            var allBiggameobjects = GameObject.FindGameObjectsWithTag("BigCube");
            foreach (GameObject obj in allgameobjects)
            {
                obj.GetComponent<MeshRenderer>().material = Materials[0];
            }
            foreach(GameObject obj in allBiggameobjects)
            {
                obj.GetComponent<MeshRenderer>().material = Materials[0];
            }
        }
        var allObstacles = GameObject.FindGameObjectsWithTag("obstacle");
        foreach(GameObject obj in allObstacles)
        {
            //and we need to destroy every obstacle for our powerUp works correctly
            Destroy(obj);
        }
        allColorChange = false;
    }

    //this is for returning back to our normal situation
    IEnumerator normalPowerAgain()
    {
        yield return new WaitForSeconds(3);
        powerCounterNumber = 70;
        isPowerUp = false;
    }
    //this function helps us to toggling colors
    IEnumerator toggleBonusColor(int index)
    {
        foreach (GameObject obj in cubes)
        {
            obj.GetComponent<MeshRenderer>().material = Materials[index];
        }
        yield return new WaitForSeconds(0.1f);
        foreach(GameObject obj in cubes)
        {
            obj.GetComponent<MeshRenderer>().material = toggleMaterial;
            
        }
        scoreValue += 500;
        bonusValue += 1;
    }
    //and finally our restart function
    IEnumerator RestartIn3()
    {
        restartInfo.gameObject.SetActive(true);
        restartInfo.text = "Game Restarting In 3 Seconds";
        yield return new WaitForSeconds(5);
        restartInfo.gameObject.SetActive(false);
        SceneManager.LoadScene(0);
    }
}

//i want to add a little info here. Maybe code looks complicated, but believe me it's not.
//First, don't forget to look inside obstaclecontroller script. I made a separate controller to avoid physic complicity.
//After I've tried a lot of different approach, i think this (PlayerController script) is the best way to control our cubes list.
//Right now, game is running very smooth and steady.
//Please give it a couple of tries to see what it is capable of.
//I hope this prototype could satisfy your needs. 