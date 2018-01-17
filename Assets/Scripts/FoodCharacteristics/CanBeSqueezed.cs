using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CanBeSqueezed : FoodCharacteristic
{

    private string myTag = "Squeezable";
    public Texture2D original;
    public Texture2D squeezedTexture;

    private void Awake()
    {
        base.createTag(myTag);
    }

    // Use this for initialization
    void Start()
    {
        gameObject.tag = myTag;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collide");
        if (collision.gameObject.CompareTag("Squeezer"))
        {
            Debug.Log("with squeezer");
            gameObject.GetComponent<Renderer>().materials[1].mainTexture = squeezedTexture;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
