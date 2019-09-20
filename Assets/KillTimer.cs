using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTimer : MonoBehaviour {

    public float killTime;

	// Use this for initialization
	void Start () {
        Invoke("KillThis", killTime);
	}

    void KillThis() {
        Destroy(gameObject);
    }

}
