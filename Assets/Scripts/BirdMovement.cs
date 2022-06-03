using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.XR.Management;
using Random = UnityEngine.Random;

public class BirdMovement : MonoBehaviour
{
    // Flying
    public float turnSpeed;
    public float speed;
    public Transform target;
    public float turnAngleIntensity;
    
    //Rotation
    private Vector3 direction;
    private Quaternion rotationGoal;
    private float currentTurnSpeed;
    
    //Orbiting
    public float orbitSpeed;
    public float minOrbitRadius;
    private Vector3 previousPos;

    // Waypoints
    public float waypointRadius;
    public float waypointProximity;
    public float minHeight, maxHeight;

    private Vector3 currentWaypoint;

    // Animation
    public Transform birdhead;
    private Animator anim;
    private bool gliding;
    
    // Testing
    public GameObject testcube;

    
    // States
    private enum BirdState
    {
        Hunting, Welcoming, Exiting, Orbiting
    }

    // Start with Hunting
    private BirdState currentState = BirdState.Hunting;

    
    //BirdSettings class, a group of data we call BirdSettings. Just like we can do Trasform.position to access some data. we can say BirdSettings.turnAngleIntensity
    class BirdSettings
    {
        public float turnAngleIntensity;
        public float waypointRadius;
        public float waypointProximity;
        public float speed;
        public float turnSpeed;

        //** This is called a constructor, when you write new BirdSettings(...) in your code you can create a new BirdSettings object with specific data
        public BirdSettings(float tAngle, float wRadius, float wProximity, float vSpeed, float tSpeed )
        {
          turnAngleIntensity = tAngle;
          waypointRadius = wRadius;
          waypointProximity = wProximity;
          speed =vSpeed ;
          turnSpeed = tSpeed;
        }
    }

    private BirdSettings huntingSettings; 
    private BirdSettings welcomingSettings; 
    private BirdSettings exitingSettings;

    private float flappngRate = 1;
    private float originalFlappngRate = 1;
    
    private float originalGlidingRate = 5;
    private float glidingRate = 5;

    void Start()
    {
        anim = GetComponent<Animator>();
        SetBirdSettings();
        // Spawning a new waypoint every 10 seconds
        //InvokeRepeating("FindNewWaypoint", 1f, 10f);
        // Spawn a new waypoint at the beginning of the game
        FindNewWaypoint();
        StartCoroutine("Welcome");
        
        StartCoroutine("RandomFlapping");
        SwitchAnimation("Glide");

    }

     IEnumerator RandomFlapping()
     { 
         yield return new WaitForSeconds(Random.Range(glidingRate, glidingRate*2.2f));
        SwitchAnimation("Flap");
        yield return new WaitForSeconds(Random.Range(flappngRate, flappngRate*2));
        SwitchAnimation("Glide");
        StartCoroutine("RandomFlapping");
     }
     
    void SwitchAnimation(string triggerName)
    {
        gliding = triggerName == "Glide";
        anim.SetTrigger(triggerName);
    }
    
    void SetBirdSettings()
    {
        //**creating each bird setting for us to use. we can add custom speed, waypoint logic etc
        huntingSettings = new BirdSettings(turnAngleIntensity, waypointRadius, waypointProximity, speed, turnSpeed);
        welcomingSettings = new BirdSettings(0f, waypointRadius, 3, speed * 1.3f, turnSpeed *3);
        exitingSettings = new BirdSettings(0f, waypointRadius, 0, speed * 1.3f, turnSpeed);
    }
    
    // After 6 seconds greet player  
    IEnumerator Welcome()
    {
        yield return new WaitForSeconds(1f);
        SwitchState(BirdState.Welcoming);
    }

    void FindNewWaypoint()
    {
        // Pick a random point in a sphere of 1
        Vector3 randomPos = Random.insideUnitSphere;

        // Multiply the width and length * waypointRadius
        randomPos.x *= waypointRadius;
        randomPos.z *= waypointRadius;
        randomPos.y *= Random.Range(minHeight, maxHeight);

        // Multiply the height * a random number between minHeight and maxHeight
        currentWaypoint = target.position + randomPos;

        // For testing spawn a cube at the new waypoint
        Instantiate(testcube, currentWaypoint, Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {
        // Check which State we are in 
        switch (currentState)
        {
            case BirdState.Hunting:
                BasicFlying();

                break;

            case BirdState.Welcoming:
                BasicFlying();

                break;

            case BirdState.Orbiting:
                OrbitFlying();
                
                break;
            case BirdState.Exiting:

                break;

            default:
                break;
        }
        
        // work on this later - birdhead.LookAt(target);
    }
    
    // Change the bird from flying to meeting player to leaving
    private void SwitchState(BirdState birdState)
    {
        switch (birdState)
        {
            case BirdState.Hunting:
                break;

            case BirdState.Welcoming:
                currentWaypoint = target.position + Vector3.up *3f;
                //after 1 second set the bird to welcoming
                Invoke("SetWelcomeSettings", 1f);
                break;

            case BirdState.Exiting:
                break;

            default:
                break;
        }
        currentState = birdState;
    }
    
    void SetWelcomeSettings()
    {
        SetNewSettings(welcomingSettings);
    }

    void SetNewSettings(BirdSettings newSettings)
    {
        //go through each bird movement variable and switch it to the new setting
        turnAngleIntensity = newSettings.turnAngleIntensity;
        waypointRadius = newSettings.waypointRadius;
        waypointProximity = newSettings.waypointProximity;
        speed =newSettings.speed;
        turnSpeed = newSettings.turnSpeed;
    }
    
    private void Tilt()
    {
        // Compare the forward angle of the bird and the direction of the waypoint
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        angle *= turnAngleIntensity;
        // Angle bird towards waypoint
        if(Mathf.Abs(angle) > 14)         SwitchAnimation("Glide");

        transform.Rotate(Vector3.forward, angle);
    }

    private void DistanceCheck()
    {
        // Checking distance between waypoint and bird position, if it is less than distance find a new spot
        if (!(Vector3.Distance(currentWaypoint, transform.position) < waypointProximity)) return;
        
        if (currentState == BirdState.Welcoming)
        {
            SwitchState(BirdState.Orbiting);
        }
        else
        {
            FindNewWaypoint();
        }
    }

    private void BasicFlying()
    {
        ForwardMovement();
        DistanceCheck();
        Tilt();
    }

    private void OrbitFlying()
    {
        OrbitMovement();
        OrbitRotation();
        Tilt();
    }

    private void OrbitRotation()
    {
        Vector3 dir = transform.position - previousPos;
        transform.rotation = Quaternion.LookRotation(dir.normalized);
        previousPos = transform.position;
    }

    void OrbitMovement()
    {
        if (Vector3.Distance(target.position, transform.position) > minOrbitRadius)
        {
            transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);
        }
        else
        {   
            transform.Translate(Vector3.forward* speed * Time.deltaTime);
        }
    }
    
    private void ForwardMovement()
    {
        // Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Finding waypoint direction
        direction = (currentWaypoint - transform.position).normalized;
        rotationGoal = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotationGoal, turnSpeed);
        
        FlapWingCheck();
    }

    private void FlapWingCheck()
    {
        if (transform.forward.y < -.05f)
        {
            flappngRate = originalFlappngRate * .1f;
            glidingRate = originalGlidingRate * 2;

            SwitchAnimation("Glide");
        }
        else if (transform.forward.y > .1f)
        {
            flappngRate = originalFlappngRate * 2;
            glidingRate = originalGlidingRate * .1f;

            SwitchAnimation("Flap");
        }
        else
        {
            flappngRate = originalFlappngRate;
            glidingRate = originalGlidingRate;
        }
    }
}

