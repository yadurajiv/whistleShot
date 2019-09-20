using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmadaShip : MonoBehaviour {

    Vector3 target;
    public float speed = 1f;
    float shipSpeed;

    public Sprite[] sprites;

    public Sprite shipSunk;

    public bool isAlive = true;

    // Use this for initialization
    void Start () {
        target = new Vector3(0, 0, 0);

        this.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length - 1)];

        shipSpeed = Random.Range(0.05f, 0.25f);

        isAlive = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (isAlive) {
            Vector3 vectorToTarget = target - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed);
            GetComponent<Rigidbody2D>().velocity = transform.right * shipSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        this.GetComponent<SpriteRenderer>().sprite = shipSunk;
        this.GetComponent<PolygonCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<AudioSource>().Play();
        isAlive = false;
        this.transform.parent = GameObject.Find("SunkArmada").transform;
        Destroy(collision.gameObject);

        GetAudioData.playerKills += 1;
    }
}
