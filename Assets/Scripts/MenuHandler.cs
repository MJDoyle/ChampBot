using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{ 
    [SerializeField]
    private Canvas dataCanvas;

    [SerializeField]
    private Canvas configCanvas;

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
    private Dropdown skillRemovalDropdown;

    [SerializeField]
    private Dropdown skillAdditionDropdown;

    [SerializeField]
    private Dropdown challengerRemovalDropdown;

    [SerializeField]
    private DataHandler dataHandler;

    [SerializeField]
    private UIHandler uiHandler;

    [SerializeField]
    private TwitchClient client;

    [SerializeField]
    private TwitchPubSub pubSub;

    private void Start()
    {
        dataCanvas.enabled = false;

        configCanvas.enabled = false;

        champDropdown.enabled = false;

        chatterDropdown.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            configCanvas.enabled = false;

            champDropdown.enabled = !champDropdown.enabled;

            chatterDropdown.enabled = !chatterDropdown.enabled;

            dataCanvas.enabled = !dataCanvas.enabled;

            if (dataCanvas.enabled)
            {
                PopulateChampDropdown();

                PopulateChatterDropdown();

                PopulateChallengerRemovalDropdown();

                SelectChatter();
            }
        }
    }

    public void Exit()
    {
        Application.Quit();  
    }

    public void Reset()
    {
        client.Disconnect();
        pubSub.Disconnect();

        //Reload the active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);             
    }

    public void MoveToConfig()
    {
        dataCanvas.enabled = false;

        chatterDropdown.enabled = false;

        champDropdown.enabled = false;

        configCanvas.enabled = true;
    }

    public void MoveToData()
    {
        dataCanvas.enabled = true;

        chatterDropdown.enabled = true;

        champDropdown.enabled = true;

        configCanvas.enabled = false;

        PopulateChampDropdown();

        PopulateChatterDropdown();

        SelectChatter();
    }

    private void PopulateChallengerRemovalDropdown()
    {
        challengerRemovalDropdown.ClearOptions();

        List<string> challengers = new List<string>();

        foreach (Challenger challenger in dataHandler.Challengers)
        {
            challengers.Add(challenger.chatter.name + " " + challenger.numDice);
        }

        challengerRemovalDropdown.AddOptions(challengers);
    }

    public void RemoveChallenger()
    {
        //need to handle challengers being added while the remove challenger list is open - is this ok? make sure list gets updated properly

        //challengerRemovalDropdown.value

        //dataHandler.Challengers
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
        //Get the chatter name from the currently selected chatter in the dropdown list
        string chatterName = chatterDropdown.options[chatterDropdown.value].text;

        if (!dataHandler.Chatters.ContainsKey(chatterName))
            return;

        //Set the text, av, and def fields
        chatterSPPfield.text = dataHandler.Chatters[chatterName].spp.ToString();

        chatterAVfield.text = dataHandler.Chatters[chatterName].av.ToString();

        chatterDefField.text = dataHandler.Chatters[chatterName].defences.ToString();

        //Construct the skill string from each skill
        string skillString = string.Empty;

        foreach (string skill in dataHandler.Chatters[chatterName].skills)
        {
            skillString += skill;
            skillString += ", ";
        }

        if (skillString != string.Empty)
            skillString = skillString.Remove(skillString.Length - 2, 2);

        chatterSkills.text = skillString;

        //Populate the skill removal dropdown
        skillRemovalDropdown.ClearOptions();

        skillRemovalDropdown.AddOptions(dataHandler.Chatters[chatterName].skills);


        //Populate the skill addition dropdown
        skillAdditionDropdown.ClearOptions();

        List<string> possibleSkills = dataHandler.PossibleSkills;

        possibleSkills.Sort();

        foreach (string skill in dataHandler.Chatters[chatterName].skills)
        {
            possibleSkills.Remove(skill);
        }

        skillAdditionDropdown.AddOptions(possibleSkills);
    }

    public void ChatterSkillAddition()
    {
        string chatterName = chatterDropdown.options[chatterDropdown.value].text;

        if (!dataHandler.Chatters.ContainsKey(chatterName))
            return;

        string skill = skillAdditionDropdown.options[skillAdditionDropdown.value].text;

        if (dataHandler.Chatters[chatterName].skills.Contains(skill))
            return;

        //Add skill to the list
        dataHandler.Chatters[chatterName].skills.Add(skill);

        //Reselect the chatter
        SelectChatter();

        //Set champ text
        uiHandler.SetChampText();
    }

    public void ChatterSkillRemoval()
    {
        string chatterName = chatterDropdown.options[chatterDropdown.value].text;

        if (!dataHandler.Chatters.ContainsKey(chatterName))
            return;

        string skill = skillRemovalDropdown.options[skillRemovalDropdown.value].text;

        if (!dataHandler.Chatters[chatterName].skills.Contains(skill))
            return;

        //Remove the skill from the list
        dataHandler.Chatters[chatterName].skills.Remove(skill);

        //Reselect the chatter
        SelectChatter();

        //Set champ text
        uiHandler.SetChampText();
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
