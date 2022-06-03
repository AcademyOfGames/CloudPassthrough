using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalMovement : MonoBehaviour
{

    public Transform portal;
    public GameObject[] portalObjects;

    
    private bool inside = false;
    private int insidePortalLayer;
    private int outsidePortalLayer;
    public float camPos;
    public float portalPos;

    private bool portalActivated;
    // Start is called before the first frame update
    void Start()
    {
        insidePortalLayer = LayerMask.NameToLayer( "InsidePortal" );
        outsidePortalLayer = LayerMask.NameToLayer( "OutsidePortal" );
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (var v  in portalObjects)
        {
            v.layer = insidePortalLayer;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        foreach (var v  in portalObjects)
        {
            v.layer = outsidePortalLayer;
        }
    }


    /*
    // Update is called once per frame
    void Update()
    {
        camPos = transform.position.z;
        portalPos = portal.position.z;
        if (transform.position.z > portal.position.z && !inside)
        {
            foreach (var v  in portalObjects)
            {
                v.layer = insidePortalLayer;
            }

            inside = true;
        }
        
        if (transform.position.z < portal.position.z && inside)
        {
            foreach (var v  in portalObjects)
            {
                v.layer = outsidePortalLayer;
            }

            inside = false;
        }
        
        
    }*/
}
