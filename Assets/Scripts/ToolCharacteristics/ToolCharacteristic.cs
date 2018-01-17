using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRTK;

public class ToolCharacteristic : MonoBehaviour
{
    protected void createTag(string tag)
    {
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        // For Unity 5 we need this too
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        // First check if it is not already present
        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(tag)) { found = true; break; }
        }

        // if not found, add it
        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = tag;
        }

        // Setting a Layer (Let's set Layer 10)
        string layerName = tag;

        // --- Unity 5 ---
        SerializedProperty sp = layersProp.GetArrayElementAtIndex(10);
        if (sp != null) sp.stringValue = layerName;
        // and to save the changes
        tagManager.ApplyModifiedProperties();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool GetIsGrabbed()
    {
        return GetComponent<VRTK_InteractableObject>().IsGrabbed();
    }
}
