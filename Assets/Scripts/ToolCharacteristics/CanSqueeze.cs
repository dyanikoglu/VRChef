using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanSqueeze : ToolCharacteristic
{
    private string myTag = "Squeezer";

    private void Awake()
    {
        base.createTag(myTag);
    }
    // Use this for initialization
    void Start()
    {
        gameObject.tag = myTag;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
