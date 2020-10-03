using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideMoveScript : MonoBehaviour {
    public Collider2D thisCollider;
    public Collider2D bottomCollider;

    public Rigidbody2D thisRigidbody;

    void Start() {
        thisRigidbody = GetComponent<Rigidbody2D>();
        thisCollider = GetComponent<BoxCollider2D>();
        bottomCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update() {

    }
}
