using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmadaSpawner : MonoBehaviour {

    public GameObject _armada;
    public float _spawnRate = 5f;
    public int _spawnLimit = 2;

    float _spawnTimer;

	// Use this for initialization
	void Start () {
        _spawnTimer = -_spawnRate;

    }
	
	// Update is called once per frame
	void Update () {
        
		if(transform.childCount < _spawnLimit) {
            if(_spawnTimer >= _spawnRate) {
                var child = Instantiate(_armada, GetUnitOnCircle(Random.Range(0f,360f),Random.Range(6f,8f)), Quaternion.identity, this.transform);
                _spawnTimer = 0;
            }
        }

        _spawnTimer += Time.deltaTime;
    }

    Vector2 GetUnitOnCircle(float angleDegrees, float radius) {

        // initialize calculation variables
        float _x = 0;
        float _y = 0;
        float angleRadians = 0;
        Vector2 _returnVector;

        // convert degrees to radians
        angleRadians = angleDegrees * Mathf.PI / 180.0f;

        // get the 2D dimensional coordinates
        _x = radius * Mathf.Cos(angleRadians);
        _y = radius * Mathf.Sin(angleRadians);

        // derive the 2D vector
        _returnVector = new Vector2(_x, _y);

        // return the vector info
        return _returnVector;
    }
}
