using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{ 
    [SerializeField]
    private Canvas canvas;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            canvas.enabled = !canvas.enabled;
        }
    }

    public void Exit()
    {
        Application.Quit();  
    }
}
