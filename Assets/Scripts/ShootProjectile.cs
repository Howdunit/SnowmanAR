using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.ARDK.AR;
using Niantic.ARDK.AR.Anchors;
using Niantic.ARDK.AR.Configuration;
using Niantic.ARDK.Utilities;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.Utilities.Logging;

public class ShootProjectile : MonoBehaviour
{
    public GameObject _shootObject;
    public int _projectileForce = 1;
    public float swipeThreshold = 0.01f;
    public float swipeForceMultiplier = 0.1f;
    public bool pickedUp = false;
    private float pickupTimer;
    public float timerThreshold = 4.0f;
    public GameObject currBall;

    public float pickupDistance;
    public float pickupOffset = 5.0f;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // ARSessionFactory.SessionInitialized += SessionInitialized ();

        // if (_arSession.CurrentFrame == null) {
        //     return;
        // }

        // Return Update if there's no touch input.
        if (PlatformAgnosticInput.touchCount == 0) {
            // Release ball if one is currently picked up.
            if (pickedUp && currBall) {
                Release ();
            }
            return;
        }
        // Debug.Log ("touch registered!");
        var touch = PlatformAgnosticInput.GetTouch (0);

        // Debug.Log ("current touch phase: " + touch.phase);
        // Debug.Log ("current tap count: " + touch.tapCount);

        if (pickupTimer > timerThreshold && !pickedUp) {
            PickUp ();
        }

        Ray r = Camera.main.ScreenPointToRay (touch.position);
        RaycastHit hit;

        if (Physics.Raycast (r, out hit) && hit.transform.CompareTag ("Snowball")) {

            // Check if the current object is same as prev touched ball. If so, increment pickup timer.
            if (currBall == hit.transform.gameObject) {
                pickupTimer += Time.deltaTime;
                Debug.Log ("Current pickup timer: " + pickupTimer);
            }
            Debug.Log ("touched a snowball!");

            currBall = hit.transform.gameObject;
            Debug.Log ("Touch delta position is: " + touch.deltaPosition);

            // Apply force to ball from swipe.
            if (touch.deltaPosition.magnitude > swipeThreshold) {
                pickupTimer = 0;

                Vector2 swipeDirection = touch.deltaPosition;
                Vector3 ZVector = new Vector3 (Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized * swipeDirection.y;
                Vector3 YVector = new Vector3 (Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized * swipeDirection.x;
                Vector3 ballForce = (ZVector + YVector) * swipeForceMultiplier;

                currBall.GetComponent <Rigidbody> ().AddForceAtPosition (ballForce, hit.point);
                Debug.Log ("Player is currently swiping! Applying force to ball");
            }
        }

        // if a ball has been currently picked up, then move it to a position offset from the ground / another ball.
        if (pickedUp) {
            if (Physics.Raycast (r, out hit) && (hit.transform.CompareTag ("Snow") || hit.transform.CompareTag ("Snowball")) && Vector3.Dot (Vector3.up, hit.normal) > 0.8f) {
                currBall.transform.position = new Vector3 (hit.point.x, hit.point.y + currBall.GetComponent <MeshRenderer> ().bounds.size.y, hit.point.z);
                Debug.Log ("Moving the ball to: " + currBall.transform.position);
            }
        }    
    }

    public void PickUp () {
        pickedUp = true;
        if (currBall) {
            currBall.GetComponent <MeshRenderer> ().material.color = Color.red;
            currBall.GetComponent <Rigidbody> ().isKinematic = true;
            Debug.Log ("The ball has been picked up!");
        }
    }

    public void Release () {
        pickedUp = false;
        pickupTimer = 0;
        if (currBall) {
            currBall.GetComponent <MeshRenderer> ().material.color = Color.white;
            currBall.GetComponent <Rigidbody> ().isKinematic = false;
            currBall = null;
            Debug.Log ("The ball has been released!");
        }
    }



    public void Shoot () {
        if (_shootObject) {
            GameObject shootObject = Instantiate (_shootObject, Camera.main.transform.position, Camera.main.transform.rotation);
            shootObject.GetComponent <Rigidbody> ().AddForce (Camera.main.transform.forward * _projectileForce);
            Debug.Log ("Spawned a snowball!");
        }
    }
}
