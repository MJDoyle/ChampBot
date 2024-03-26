using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{ 
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private Dropdown champDropdown;

    [SerializeField]
    private DataHandler dataHandler;

    [SerializeField]
    private UIHandler uiHandler;

    private void Start()
    {
        canvas.enabled = false;

        champDropdown.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            champDropdown.enabled = !champDropdown.enabled;

            canvas.enabled = !canvas.enabled;

            if (canvas.enabled)
                PopulateChampDropdown();
        }
    }

    public void Exit()
    {
        Application.Quit();  
    }

    public void PopulateChampDropdown()
    {
        champDropdown.ClearOptions();

        if (dataHandler.Chatters.Count == 0)
            return;

        List<string> chatters = new List<string>();

        foreach (KeyValuePair<string, Chatter> chatterPair in dataHandler.Chatters)
        {
            chatters.Add(chatterPair.Key);
        }

        chatters.Sort();

        int champIndex = 0;

        int i = 0;

        foreach (string chatter in chatters)
        {
            if (dataHandler.CurrentChamp != null && dataHandler.CurrentChamp.name == chatter)
                champIndex = i;

            i++;
        }

        champDropdown.AddOptions(chatters);

        champDropdown.SetValueWithoutNotify(champIndex);   
    }

    public void SetChamp()
    {
        dataHandler.SetChamp(champDropdown.options[champDropdown.value].text);

        uiHandler.SetChampText();
    }
}
