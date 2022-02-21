using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallRoll : MonoBehaviour
{
    // To Do
    // The ball takes more force to roll & more difficult to change its direction of travel as it gets bigger.
    // Due to this behavior, the ball should not be able to grow more than the maximum size.

    public float growthMultiplier = 0.01f;
    public float shrinkMultiplier = 0.01f;
    public float posThreshold = 0.1f;
    public float impulseThreshold = 0.1f;
    public float raycastDist = 10f;
    public bool isRolling = false;
    private Vector3 prevPos;
    private Vector3 currPos;


    // Start is called before the first frame update
    void Start()
    {
        currPos = transform.position;
        currPos = prevPos;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the ball has moved, and the ground underneath is snow. If so, grow the ball.
        currPos = transform.position;
        RaycastHit hit;
        raycastDist = gameObject.GetComponent <MeshRenderer> ().bounds.extents.y;
        Debug.DrawRay (transform.position, Vector3.down * raycastDist, Color.red);
        if ((currPos - prevPos).magnitude > posThreshold && Physics.Raycast (transform.position, Vector3.down, out hit, raycastDist) && hit.transform.CompareTag ("Snow")) {
            Debug.Log ("The ball has moved, and the ground is snow!");
            Grow ((currPos - prevPos).magnitude * growthMultiplier);
        }

        prevPos = currPos;
    }

    // Make the ball bigger
    public void Grow (float amount) {
        float currScale = gameObject.transform.localScale.x;
        Vector3 currPos = gameObject.transform.position;
        currScale += amount;
        gameObject.transform.localScale = new Vector3 (currScale, currScale, currScale);
        // gameObject.transform.position = new Vector3 (currPos.x, gameObject.GetComponent <MeshFilter> ().mesh.bounds.extents.y ,currPos.z);
        Debug.Log ("The ball has grown by: " + amount);
    }

    // Make the ball smaller
    public void Shrink (float amount) {
        float currScale = gameObject.transform.localScale.x;
        Vector3 currPos = gameObject.transform.position;
        currScale -= amount;
        gameObject.transform.localScale = new Vector3 (currScale, currScale, currScale);
        // gameObject.transform.position = new Vector3 (currPos.x, gameObject.GetComponent <MeshFilter> ().mesh.bounds.extents.y ,currPos.z);
        Debug.Log ("The ball has been hit. It has shrunk by: " + amount);
    }


    private void OnCollisionEnter(Collision other) {
        // Shrink ball if it collides with obstacles or other snowballs.
        if (other.impulse.magnitude > impulseThreshold && (other.transform.CompareTag ("Obstacle") || other.transform.CompareTag ("Snowball"))) {
            float hitDamage = other.impulse.magnitude * shrinkMultiplier;
            Shrink (hitDamage);
        }
    }

    private void OnCollisionStay(Collision other) {
        
    }

    private void OnCollisionExit(Collision other) {
        
    }
}
