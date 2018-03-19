using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeMixed : FoodCharacteristic {

	// Use this for initialization
	void Start () {
        base.Start();
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<CanMixedIn>()!=null && gameObject.transform.parent != collision.gameObject.transform)
        {
            if (gameObject.GetComponent<CanBeMerged>() != null)
            {
                Vector3 _scale = collision.gameObject.transform.GetChild(2).gameObject.transform.localScale;
                if(_scale.x==0 && _scale.y==0 && _scale.z == 0)
                {
                    _scale.x = _scale.x + 0.2f;
                    _scale.y = _scale.y + 0.2f;
                    _scale.z = _scale.z + 0.2f;
                    collision.gameObject.transform.GetChild(2).gameObject.transform.localScale = _scale;
                }
                else
                {
                    _scale.x = _scale.x + 0.02f;
                    _scale.y = _scale.y + 0.02f;
                    _scale.z = _scale.z + 0.02f;
                    Vector3 _position = collision.gameObject.transform.GetChild(2).gameObject.transform.position;
                    _position.y = _position.y + 0.001f;
                    collision.gameObject.transform.GetChild(2).gameObject.transform.localScale = _scale;
                    collision.gameObject.transform.GetChild(2).gameObject.transform.position = _position;
                }
                Destroy(gameObject);
            }
            else
            { 
                gameObject.transform.parent = collision.gameObject.transform;
            }
        }
    }
}
