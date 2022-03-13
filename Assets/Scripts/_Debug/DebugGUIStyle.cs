using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGUIStyle : MonoBehaviour
{
    [SerializeField] Rect buttonPosition = Rect.zero;
    [SerializeField] GUIStyle style = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        GUI.Button(buttonPosition, "This is a button", style);
    }
}
