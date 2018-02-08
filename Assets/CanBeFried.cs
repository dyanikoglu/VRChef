using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeFried : MonoBehaviour
{

    [SerializeField]
    Texture objectTexture;
    Material myMaterial;

    public Shader shaderWithBlending;
    public Texture friedTexture;

    // Use this for initialization
    void Start()
    {
        // get object's material.
        myMaterial = GetComponent<Renderer>().material;
        // if object has a texture assign it to objectTexture, otherwise create a texture from its color property
        objectTexture = myMaterial.mainTexture;
        if (!objectTexture)
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);

            // set pixel values
            Color color = myMaterial.color;
            texture.SetPixel(0, 0, color);
            texture.SetPixel(1, 0, color);
            texture.SetPixel(0, 1, color);
            texture.SetPixel(1, 1, color);

            // apply pixels
            texture.Apply();
            objectTexture = texture;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            myMaterial.shader = shaderWithBlending;
            myMaterial.SetTexture("_MainTex", objectTexture);
            //myMaterial.SetTexture("_SecondTex", friedTexture);

            myMaterial.SetTexture("_Texture2", friedTexture);

            //myMaterial.SetTexture("_TexMat1", objectTexture);
            //myMaterial.SetTexture("_TexMat2", friedTexture);

            //myMaterial.SetTexture("_TerrainFloor", objectTexture);
            //myMaterial.SetTexture("_Sand", friedTexture);

        }

    }
}
