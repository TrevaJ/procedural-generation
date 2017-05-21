using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour// script to hve basic 3D player control to interact with environment
    
{
    Rigidbody rigidbody;
    Vector3 velocity;
	// Use this for initialization
	void Start ()
    {
        rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
         velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * 10;
	}
    void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
    }
}
