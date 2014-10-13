﻿using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	public Vector2 initialForce = new Vector2(0, 0);
	public float initialForceMult = 10.0f;
	public float maxInitialForceVector = 50.0f;

	private Rigidbody2D body;
	private Animator animator;
	private ForceArrow initialForceArrow;

	private SpriteRenderer cabinRenderer;
	private Color baseColor;
	private Vector3 oldPos;

	private Vector3 zoomTarget;
	private bool isZooming;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		initialForceArrow = GetComponentInChildren<ForceArrow>();
		cabinRenderer = transform.Find("ship cabin").GetComponent<SpriteRenderer>();
		baseColor = cabinRenderer.color;
		oldPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (isZooming) {
			transform.position = Vector3.Lerp ( transform.position, zoomTarget, 5*Time.deltaTime );
		}
	}

	void OnMouseDrag () {
		Vector3 t = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 dir = t - transform.position;
		initialForce = new Vector2 ( dir.x, dir.y ) * initialForceMult;
		if (initialForce.magnitude > maxInitialForceVector) {
			initialForce = initialForce.normalized * maxInitialForceVector;
		}
		initialForceArrow.force = initialForce * 0.1f;
	}

	void OnTriggerEnter2D ( Collider2D coll ) {
		if (coll.gameObject.tag == "Target") {
			body.simulated = false;
			isZooming = true;
			zoomTarget = coll.gameObject.transform.position;
			animator.SetTrigger("won");
		}
	}

	void OnCollisionEnter2D ( Collision2D coll ) {
		string tag = coll.gameObject.tag;
		if (tag == "Enemy" || tag == "Repeller" || tag == "Attractor") {
			Kill();
			Camera.main.GetComponent<GameCamera>().Shake();
		} else if (tag == "Spring") {
			Camera.main.GetComponent<GameCamera>().Shake(0.2f);
		}
	}

	public void Kill() {
		body.velocity = Vector2.zero;
		body.simulated = false;
		animator.SetTrigger("death");
	}

	public void ResetSimulation () {
		Debug.Log ("Called");
		Camera.main.GetComponent<Simulation>().ResetSimulation();
	}

	public void ResetToStart () {
		body.simulated = true;
		body.isKinematic = true;
		isZooming = false;
		body.velocity = Vector2.zero;
		animator.SetTrigger("reset");
	}

	public void Freeze() {
		body.simulated = false;
	}

	public void Unfreeze() {
		body.simulated = false;
	}
}
