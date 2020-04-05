using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickBall : MonoBehaviour
{
    [SerializeField]
    private GameObject item;

    public float MaxObjectSpeed = 40;

    public float flickSpeed = 0.4f;

    public string respawnName = "";
    public float howClose = 9.5f;

    float startTime, endTime, swipeDistance, swipeTime;
    Vector2 startPos, endPos, swipeDirection, objectDirection, flickDirection;
    float tempTime;

    float flickLenght;
    float objectVelocity = 0;
    public float objectSpeed = 0;
    Vector3 angle;

    bool thrown, holding;
    Vector3 newPosition, velocity;

    // Start is called before the first frame update
    void Start()
    {
        item.GetComponent<Rigidbody>().useGravity = false;     
    }

    void OnTouch()
    {
        Vector3 mousePos = Input.GetTouch(0).position;
        mousePos.z = Camera.main.nearClipPlane * howClose;
        newPosition = Camera.main.ScreenToViewportPoint(mousePos);
        item.transform.localPosition = Vector3.Lerp(item.transform.localPosition, newPosition, 80f * Time.deltaTime);
    }
    // Update is called once per frame
    void Update()
    {
        if (holding)
        {
            OnTouch();
        }
        else if (thrown) 
        {
            return;
        }

        if (Input.touchCount > 0) 
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) 
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100f)) 
                {
                    if (hit.transform == item.transform) 
                    {
                        startTime = Time.time;
                        startPos = touch.position;
                        holding = true;
                        transform.SetParent(null);
                    }
                }
            }

            else if (touch.phase == TouchPhase.Ended && holding)
            {
                endTime = Time.time;
                endPos = touch.position;
                swipeDistance = (endPos - startPos).magnitude;
                swipeDirection = (endPos - startPos);
                swipeTime = endTime - startTime;

                if (swipeTime < flickSpeed && swipeDistance > 1f)
                {
                    float upForce = 600f;
                    float frontForce = 250f;
                    CalSpeedDir();
                    MoveAngle();
                    item.GetComponent<Rigidbody>().AddForce(new Vector3((angle.x * objectSpeed), (angle.y * objectSpeed), (angle.z * objectSpeed)));
                    item.GetComponent<Rigidbody>().AddForce(new Vector3(0, upForce, frontForce));
                    
                    item.GetComponent<Rigidbody>().useGravity = true;

                    holding = false;
                    thrown = true;
                    Invoke("_Reset", 5f);
                }

                else
                {
                    _Reset();
                }
            }

            if (startTime > 0)
                tempTime = Time.time - startTime;

            if (tempTime > flickSpeed)
            {
                startTime = Time.time;
                startPos = touch.position;
            }
        }
    }

    void _Reset()
    {
        Transform ReSpwanPoint = GameObject.Find(respawnName).transform;
        item.gameObject.transform.position = ReSpwanPoint.position;
        item.gameObject.transform.rotation = ReSpwanPoint.rotation;
        item.GetComponent<Rigidbody>().velocity = Vector3.zero;
        item.GetComponent<Rigidbody>().useGravity = false;
        thrown = holding = false;
    }

    void CalSpeedDir()
    {
        flickDirection = swipeDirection;
        flickLenght = swipeDistance;
        if (swipeTime > 0)
        {
            objectVelocity = flickLenght / (flickLenght - swipeTime);
            objectDirection = flickDirection / (flickDirection - swipeDirection);
        }

        objectSpeed = objectVelocity * 50;
        objectSpeed = objectSpeed - (objectSpeed * 1.7f);

        objectDirection = objectDirection * 50;
        objectDirection = objectDirection - (objectDirection * 1.7f);

        if(objectSpeed <= MaxObjectSpeed)
        {
            objectSpeed = -MaxObjectSpeed;
        }

        swipeTime = 0;
    }

    void MoveAngle()
    {
        angle = Camera.main.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(endPos.y, (Camera.main.GetComponent<Camera>().nearClipPlane - howClose)));
    }
}
