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
    private Dropdown chatterDropdown;

    [SerializeField]
    private InputField chatterSPPfield;

    [SerializeField]
    private InputField chatterAVfield;

    [SerializeField]
    private InputField chatterDefField;

    [SerializeField]
    private Text chatterSkills;

    [SerializeField]
    private DataHandler dataHandler;

    [SerializeField]
    private UIHandler uiHandler;

    private void Start()
    {
        canvas.enabled = false;

        champDropdown.enabled = false;

        chatterDropdown.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            champDropdown.enabled = !champDropdown.enabled;

            chatterDropdown.enabled = !chatterDropdown.enabled;

            canvas.enabled = !canvas.enabled;

            if (canvas.enabled)
            {
                PopulateChampDropdown();

                PopulateChatterDropdown();

                SelectChatter();
            }
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

    public void PopulateChatterDropdown()
    {
        chatterDropdown.ClearOptions();

        List<string> chatters = new List<string>();

        foreach (KeyValuePair<string, Chatter> chatterPair in dataHandler.Chatters)
        {
            chatters.Add(chatterPair.Key);
        }

        chatters.Sort();

        chatterDropdown.AddOptions(chatters);
    }

    public void SetChamp()
    {
        dataHandler.SetChamp(champDropdown.options[champDropdown.value].text);

        uiHandler.SetChampText();
    }

    public void SelectChatter()
    {
        string chatterName = chatterDropdown.options[chatterDropdown.value].text;

        if (!dataHandler.Chatters.ContainsKey(chatterName))
            return;

        chatterSPPfield.text = dataHandler.Chatters[chatterName].spp.ToString();

        chatterAVfield.text = dataHandler.Chatters[chatterName].av.ToString();

        chatterDefField.text = dataHandler.Chatters[chatterName].defences.ToString();

        string skillString = string.Empty;

        foreach (string skill in dataHandler.Chatters[chatterName].skills)
        {
            skillString += skill;
            skillString += ", ";
        }

        if (skillString != string.Empty)
            skillString = skillString.Remove(skillString.Length - 2, 2);

        chatterSkills.text = skillString;
    }

    public void ChatterSPPEdit()
    {
        if (!int.TryParse(chatterSPPfield.text, out int newSPP))
            return;

        dataHandler.SetSPP(chatterDropdown.options[chatterDropdown.value].text, newSPP);

        uiHandler.SetChampText();
    }

    public void ChatterAVEdit()
    {
        if (!int.TryParse(chatterAVfield.text, out int newAV))
            return;

        dataHandler.SetAV(chatterDropdown.options[chatterDropdown.value].text, newAV);

        uiHandler.SetChampText();
    }

    public void ChatterDefEdit()
    {
        if (!int.TryParse(chatterDefField.text, out int newDef))
            return;

        dataHandler.SetDef(chatterDropdown.options[chatterDropdown.value].text, newDef);

        uiHandler.SetChampText();
    }

}
