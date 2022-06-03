using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{   //public Transform player;
    //public Vector3 offset;
    //public float cameraSmoothness;

    public float mouseSensitivity = 100f;
    public Transform playerBody;
    float xRotation = 0f;
    Camera cam;
    public GameObject waterScreen;


    // Start is called before the first frame update
    public IEnumerator WidenFOV()
    {
        float shortFOV = 60f;
        float longFOV = 70f;
        float transition = 0f;
        while(transition < 1.1f)
        {
            yield return null;
            cam.fieldOfView = Mathf.Lerp(shortFOV,longFOV,transition);
            transition += 0.05f;
        }
    }
    public IEnumerator LowerFOV()
    {
        float shortFOV = 60f;
        float longFOV = 70f;
        float transition = 0f;
        while (transition < 1.1f)
        {
            yield return null;
            cam.fieldOfView = Mathf.Lerp(longFOV,shortFOV,transition);
            transition += 0.05f;
        }
    }



    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = GetComponent<Camera>();
    }
    
    // Update is called once per frame
    void Update()
    { 
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f,90f);
        
        transform.localRotation = Quaternion.Euler(xRotation,0f,0f);
        playerBody.Rotate(Vector3.up * mouseX);
        
        
        
        /*
        Vector3 targetPos = player.position + offset;
        Vector3 smoothFollow = Vector3.Lerp(transform.position, targetPos, cameraSmoothness );
        transform.position = smoothFollow;*/

    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("water"))
        {
            waterScreen.SetActive(true);
        }
    }   
}