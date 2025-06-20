using System.Collections;
using System.Collections.Generic;

using TwitchLib.Client.Models;
using TwitchLib.Unity;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;
using System;

public class TwitchClient : MonoBehaviour
{
    private Client client;

    private string channelName;

    private UIHandler UIhandler;

    private DataHandler dataHandler;

    bool connectMessage = false;

    private int[] bitLevels = { 0, 0, 0 };

    // Start is called before the first frame update
    void Start()
    {
        LoadBitLevels();

        dataHandler = GetComponent<DataHandler>();

        UIhandler = GetComponent<UIHandler>();

        Application.runInBackground = true;
 
        StreamReader channelNameReader = new StreamReader("Config/channelName.txt");

        channelName = channelNameReader.ReadLine();

        channelNameReader.Close();

        string oauth = "";

        StreamReader oauthReader = new StreamReader("Config/oauth.txt");

        oauth = oauthReader.ReadLine();

        oauthReader.Close();

        ConnectionCredentials credentials = new ConnectionCredentials(channelName, oauth);

        client = new Client();
        client.Initialize(credentials, channelName);

        client.OnChatCommandReceived += OnChatCommandReceived;

        client.OnGiftedSubscription += OnGiftedSubscription;
        client.OnContinuedGiftedSubscription += OnContinuedGiftedSubscription;
        client.OnNewSubscriber += OnNewSubscriber;
        client.OnPrimePaidSubscriber += OnPrimePaidSubscriber;
        client.OnReSubscriber += OnResubscriber;

        client.OnMessageReceived += OnMessageReceived;

        Debug.Log("Client connecting");

        bool connected = client.Connect();

        Debug.Log(connected);
    }

    public void Disconnect()
    {
        client.Disconnect();
    }

    private void LoadBitLevels()
    {
        StreamReader bitLevelsReader = new StreamReader("Config/bits.txt");

        int bitNum = 0;

        string line;
        while ((line = bitLevelsReader.ReadLine()) != null)
        {
            if (bitNum > 2)
                break;

            int lineNum = 0;

            if (!int.TryParse(line, out lineNum))
                break;

            bitLevels[bitNum] = lineNum;

            bitNum++;
        }

        bitLevelsReader.Close();

    }

    //Bits
    private void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
        int numBits = e.ChatMessage.Bits;

        if (numBits > 0)
            SendChannelMessage(e.ChatMessage.Bits + " bits from " + e.ChatMessage.DisplayName);

        if (bitLevels[0] <= 0 || bitLevels[1] <= 0 || bitLevels[2] <= 0)
            return;

        while (numBits > 0)
        {
            if (numBits >= bitLevels[2])
            {
                numBits -= bitLevels[2];

                dataHandler.AddChallenger(e.ChatMessage.DisplayName, 3);

                UIhandler.SetChallengerListItems();
                UIhandler.ShowChallengerListItems();

                client.SendMessage(e.ChatMessage.Channel, $"Three dice block for " + e.ChatMessage.DisplayName + "!");
            }

            else if (numBits >= bitLevels[1])
            {
                numBits -= bitLevels[1];

                dataHandler.AddChallenger(e.ChatMessage.DisplayName, 2);

                UIhandler.SetChallengerListItems();
                UIhandler.ShowChallengerListItems();

                client.SendMessage(e.ChatMessage.Channel, $"Two dice block for " + e.ChatMessage.DisplayName + "!");
            }

            else if (numBits >= bitLevels[0])
            {
                numBits -= bitLevels[0];

                dataHandler.AddChallenger(e.ChatMessage.DisplayName, 1);

                UIhandler.SetChallengerListItems();
                UIhandler.ShowChallengerListItems();

                client.SendMessage(e.ChatMessage.Channel, $"One dice block for " + e.ChatMessage.DisplayName + "!");
            }

            else
                numBits = 0;
        }
    }

    //Individual gift
    private void OnGiftedSubscription(object sender, TwitchLib.Client.Events.OnGiftedSubscriptionArgs e)
    {
        client.SendMessage(e.Channel, $"Three dice block for " + e.GiftedSubscription.DisplayName + "!");

        dataHandler.AddChallenger(e.GiftedSubscription.DisplayName, 3);

        UIhandler.SetChallengerListItems();
        UIhandler.ShowChallengerListItems();
    }

    private void OnContinuedGiftedSubscription(object sender, TwitchLib.Client.Events.OnContinuedGiftedSubscriptionArgs e)
    {
        client.SendMessage(e.Channel, $"Three dice block for " + e.ContinuedGiftedSubscription.DisplayName + "!");

        dataHandler.AddChallenger(e.ContinuedGiftedSubscription.DisplayName, 3);

        UIhandler.SetChallengerListItems();
        UIhandler.ShowChallengerListItems();
    }

    private void OnNewSubscriber(object sender, TwitchLib.Client.Events.OnNewSubscriberArgs e)
    {
        client.SendMessage(e.Channel, $"Three dice block for " + e.Subscriber.DisplayName + "!");

        dataHandler.AddChallenger(e.Subscriber.DisplayName, 3);

        UIhandler.SetChallengerListItems();
        UIhandler.ShowChallengerListItems();
    }

    private void OnPrimePaidSubscriber(object sender, TwitchLib.Client.Events.OnPrimePaidSubscriberArgs e)
    {
        client.SendMessage(e.Channel, $"Three dice block for " + e.PrimePaidSubscriber.DisplayName + "!");

        dataHandler.AddChallenger(e.PrimePaidSubscriber.DisplayName, 3);

        UIhandler.SetChallengerListItems();
        UIhandler.ShowChallengerListItems();
    }

    private void OnResubscriber(object sender, TwitchLib.Client.Events.OnReSubscriberArgs e)
    {
        client.SendMessage(e.Channel, $"Three dice block for " + e.ReSubscriber.DisplayName + "!");

        dataHandler.AddChallenger(e.ReSubscriber.DisplayName, 3);

        UIhandler.SetChallengerListItems();
        UIhandler.ShowChallengerListItems();
    }



    private void OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
    {
        List<string> args = e.Command.ArgumentsAsList;

        switch (e.Command.CommandText.ToLower())
        {
            case "champhelp":
            case "champbothelp":
            case "helpchamp":
            case "helpchampbot":

                SendChannelMessage("!(f)ight");
                SendChannelMessage("!addskill [chattername] [skill]");
                SendChannelMessage("!addav [chattername]");

                SendChannelMessage("!getcontender [chattername]");
                SendChannelMessage("!mycontendor");
                SendChannelMessage("!champtop5");
                SendChannelMessage("!listskills");
                SendChannelMessage("!addmyskill [skillname]");

                SendChannelMessage("!(l)ist(c)hallengers");
                SendChannelMessage("!champreload --- reload from data files if editing during operation");
                SendChannelMessage("!setspp [challengername] [spp]");
                SendChannelMessage("!setchamp [champname] --- only use if something's gone wrong");
                SendChannelMessage("!setchatter [chattername] [def] [spp] [av] [niggles] ... --- only use if something's gone wrong - then set skills");
                SendChannelMessage("!setfreeskill [chattername] [skill] ... --- only use if something's gone wrong. No ssp cost");
                SendChannelMessage("!addchallenger [challengername] [numdice] --- only use if something's gone wrong");
                SendChannelMessage("!addniggles [chattername] [numniggles]");

                break;

            case "sppneeded":

                string sppString = "SPP requirements: ";

                foreach (int sppNeeded in dataHandler.SppRequirements)
                {
                    sppString += sppNeeded.ToString() + " | ";
                }

                SendChannelMessage(sppString);

                break;

            case "listskills":
            case "availableskills":
            case "skillslist":
            case "skillsavailable":
            case "champskills":

                string skillsString = "Available skills: ";

                foreach (string skill in dataHandler.PossibleSkills)
                {
                    skillsString += skill + " | ";
                }

                SendChannelMessage(skillsString);

                break;

            case "listchallengers":
            case "challengerslist":
            case "champchallengers":
            case "ch":
            case "lc":
            case "LC":

                UIhandler.SetChallengerListItems();

                UIhandler.ShowChallengerListItems();

                if (dataHandler.Challengers.Count == 0)
                    SendChannelMessage("No current challengers");

                foreach (Challenger challenger in dataHandler.Challengers)
                {
                    SendChannelMessage(challenger.chatter.name + " " + challenger.numDice.ToString() + " dice");
                }

                break;

            case "champreload":
            case "reloadchamp":

                if (!e.Command.ChatMessage.IsBroadcaster)
                {
                    SendChannelMessage("Streamer only command");

                    break;
                }

                dataHandler.LoadData();

                LoadBitLevels();

                SendChannelMessage("Reloading champ data from file");

                break;

            case "addniggles":
            case "nigglesadd":

                if (!e.Command.ChatMessage.IsBroadcaster && !e.Command.ChatMessage.IsModerator)
                {
                    SendChannelMessage("Streamer or moderator only command");

                    break;
                }

                AddNiggles(args);

                UIhandler.SetChampText();

                break;

            case "setspp":
            case "sppset":

                if (!e.Command.ChatMessage.IsBroadcaster && !e.Command.ChatMessage.IsModerator)
                {
                    SendChannelMessage("Streamer or moderator only command");

                    break;
                }

                if (args.Count == 2 && int.TryParse(args[1], out int numSPP))
                {
                    if (dataHandler.SetSPP(args[0], numSPP))
                    {
                        SendChannelMessage(args[0] + " set to " + numSPP + " spp");

                        UIhandler.SetChampText();
                    }

                    else
                        SendChannelMessage("Chatter cannot be found");


                }

                else
                {
                    SendChannelMessage("Can't set spp. Have you formatted the request correctly? [chatter] [spp]");
                }

                break;

            case "setchamp":
            case "champset":

                if (!e.Command.ChatMessage.IsBroadcaster)
                {
                    SendChannelMessage("Streamer only command");

                    break;
                }

                if (args.Count == 1 && dataHandler.SetChamp(args[0]))
                {
                    SendChannelMessage("New champ " + args[0] + " set succesfully");

                    UIhandler.SetChampText();
                }

                else
                {
                    SendChannelMessage("New champ can't be set. Do they exist as a chatter? Use !setchatter first if not");
                }

                break;

            case "addchallenger":
            case "challengeradd":
            case "ac":
            case "AC":

                if (!e.Command.ChatMessage.IsBroadcaster && !e.Command.ChatMessage.IsModerator)
                {
                    SendChannelMessage("Streamer or moderator only command");

                    break;
                }

                if (args.Count == 2 && int.TryParse(args[1], out int numDice))
                {
                    dataHandler.AddChallenger(args[0], numDice);

                    SendChannelMessage(args[0] + " wants to " + numDice + " dice the champ!");

                    UIhandler.SetChallengerListItems();
                    UIhandler.ShowChallengerListItems();
                }

                else if (args.Count == 3 && int.TryParse(args[1], out int numDice2) && int.TryParse(args[2], out int numAttempts))
                {
                    for (int i = 0; i < numAttempts; i ++)
                        dataHandler.AddChallenger(args[0], numDice2);

                    SendChannelMessage(args[0] + " wants to " + numDice2 + " dice the champ " + numAttempts + " times!");

                    UIhandler.SetChallengerListItems();
                    UIhandler.ShowChallengerListItems();
                }

                else
                {
                    SendChannelMessage("Challenger can't be added. Have you formatted the request correctly? [chatter] [numdice] [numattempts]");
                }

                break;


            case "setchatter":
            case "chatterset":

                if (!e.Command.ChatMessage.IsBroadcaster && !e.Command.ChatMessage.IsModerator)
                {
                    SendChannelMessage("Streamer or moderator only command");

                    break;
                }

                if (args.Count == 5 && int.TryParse(args[1], out int def) && int.TryParse(args[2], out int spp) && int.TryParse(args[3], out int av) && int.TryParse(args[4], out int niggles))
                {
                    dataHandler.SetChatter(args[0], def, spp, av, niggles);

                    SendChannelMessage("Chatter set");

                    UIhandler.SetChampText();
                }

                else
                {
                    SendChannelMessage("Chatter can't be set. Have you formatted the request correctly? [chatter] [defences] [spp] [av] [niggles]");
                }

                break;

            case "setfreeskill":
            case "addfreeskill":
            case "freeskill":

                if (!e.Command.ChatMessage.IsBroadcaster && !e.Command.ChatMessage.IsModerator)
                {
                    SendChannelMessage("Streamer or moderator only command");

                    break;
                }

                if (args.Count >= 2)
                {
                    string skillString = "";

                    for (int i = 1; i < args.Count; i++)
                    {
                        skillString += args[i] + " ";
                    }

                    skillString = skillString.Remove(skillString.Length - 1, 1);



                    if (dataHandler.AddFreeSkill(args[0], skillString))
                    {
                        SendChannelMessage(args[0] + " has learned " + skillString);

                        UIhandler.SetChampText();
                    }

                    else
                        SendChannelMessage("Skill can't be added. Either the chatter doesn't exist, the skill doesn't exist, or the skill is a duplicate.");

                }

                else
                {
                    SendChannelMessage("Skill can't be added. Have you formatted the request correctly? [chatter] [skill]");
                }


                break;

            case "addmyskill":
            case "newmyskill":
            case "level":
            case "lvl":
            case "levelup":
            case "lvlup":

                if (args.Count >= 1)
                {
                    string skillString = "";

                    for (int i = 0; i < args.Count; i++)
                    {
                        skillString += args[i] + " ";
                    }

                    skillString = skillString.Remove(skillString.Length - 1, 1);

                    if (dataHandler.AddSkill(e.Command.ChatMessage.DisplayName, skillString))
                    {
                        SendChannelMessage(e.Command.ChatMessage.DisplayName + " has learned " + skillString);

                        UIhandler.SetChampText();
                    }

                    else
                        SendChannelMessage("Skill can't be added. Either the chatter doesn't exist, the chatter has insufficient spp, the skill doesn't exist, or the skill is a duplicate.");

                }

                else
                {
                    SendChannelMessage("Skill can't be added. Have you formatted the request correctly? [skill]");
                }

                break;

            case "addskill":
            case "newskill":

                if (!e.Command.ChatMessage.IsBroadcaster && !e.Command.ChatMessage.IsModerator)
                {
                    SendChannelMessage("Streamer or moderator only command");

                    break;
                }

                if (args.Count >= 2)
                {
                    string skillString = "";

                    for (int i = 1; i < args.Count; i++)
                    {
                        skillString += args[i] + " "; 
                    }

                    skillString = skillString.Remove(skillString.Length - 1, 1);



                    if (dataHandler.AddSkill(args[0], skillString))
                    {
                        SendChannelMessage(args[0] + " has learned " + skillString);

                        UIhandler.SetChampText();
                    }

                    else
                        SendChannelMessage("Skill can't be added. Either the chatter doesn't exist, the chatter has insufficient spp, the skill doesn't exist, or the skill is a duplicate.");

                }

                else
                {
                    SendChannelMessage("Skill can't be added. Have you formatted the request correctly? [chatter] [skill]");
                }


                break;


            case "addav":

                if (!e.Command.ChatMessage.IsBroadcaster && !e.Command.ChatMessage.IsModerator)
                {
                    SendChannelMessage("Streamer or moderator only command");

                    break;
                }

                if (args.Count == 1)
                {
                    if (dataHandler.AddAv(args[0]))
                    {
                        SendChannelMessage(args[0] + " has increased their av");

                        UIhandler.SetChampText();
                    }

                    else
                        SendChannelMessage("Av can't be added. The chatter doesn't exist or has insufficient spp.");

                }

                else
                {
                    SendChannelMessage("Av can't be added. Have you formatted the request correctly?");
                }


                break;


            case "champtop5":
            case "top5champ":
            case "top5defences":

                List<Chatter> top5 = dataHandler.GetTop5();

                string top5String = "";

                foreach (Chatter chatter in top5)
                {
                    top5String += chatter.name + " " + chatter.defences + " def |";
                }

                if (top5String.Length > 1)
                    top5String = top5String.Remove(top5String.Length - 2, 2);

                SendChannelMessage(top5String);

                break;


            case "chalice":
            case "topchalice":
            case "champtop10":
            case "top10champ":
            case "top10defences":

                List<Chatter> top10 = dataHandler.GetTopChalice();

                string top10String = "";

                int count = 0;

                foreach (Chatter chatter in top10)
                {
                    top10String += " " + (count + 1) + ". " + chatter.name + " " + chatter.defences + " def |";

                    if (count == 7)
                        top10String += "||";

                    count++;
                }

                if (top10String.Length > 1)
                    top10String = top10String.Remove(top10String.Length - 2, 2);

                SendChannelMessage(top10String);

                break;

            case "power":
            case "toppower":

                List<Tuple<Chatter, int>> topPower = dataHandler.GetTopPower();

                string topPowerString = "";

                foreach (Tuple<Chatter, int> chatterPair in topPower)
                {
                    topPowerString += " " + chatterPair.Item1.name + " " + chatterPair.Item2 + " spp |";
                }

                if (topPowerString.Length > 1)
                    topPowerString = topPowerString.Remove(topPowerString.Length - 2, 2);

                SendChannelMessage(topPowerString);

                break;

                break;

            case "topkillers":
            case "topkills":

                List<Chatter> topKills = dataHandler.GetTopKills();

                string topKillsString = "";

                foreach (Chatter chatter in topKills)
                {
                    topKillsString += " " + chatter.name + " " + chatter.kills + " |";
                }

                if (topKillsString.Length > 1)
                    topKillsString = topKillsString.Remove(topKillsString.Length - 2, 2);

                SendChannelMessage(topKillsString);

                break;

            case "topdead":
            case "topdeaths":

                List<Chatter> topDeaths = dataHandler.GetTopDeaths();

                string topDeathsString = "";

                foreach (Chatter chatter in topDeaths)
                {
                    topDeathsString += " " + chatter.name + " " + chatter.deaths + " |";
                }

                if (topDeathsString.Length > 1)
                    topDeathsString = topDeathsString.Remove(topDeathsString.Length - 2, 2);

                SendChannelMessage(topDeathsString);

                break;


            case "mychatter":
            case "mychallenger":
            case "mycontender":
            case "me":

                Chatter chatter1 = dataHandler.GetChatter(e.Command.ChatMessage.Username);

                if (chatter1 != null)
                {
                    string chatterString = "";

                    chatterString += chatter1.name + " | ";
                    chatterString += "Level " + (chatter1.skills.Count + (chatter1.av - 8) + 1) + " | ";
                    chatterString += chatter1.defences + " defences | ";
                    chatterString += chatter1.offences + " takedowns | ";
                    chatterString += chatter1.blocks + " blocks | ";
                    chatterString += chatter1.cas + " cas | ";
                    chatterString += chatter1.kills + " kills | ";
                    chatterString += chatter1.deaths + " deaths | ";
                    chatterString += chatter1.spp + " spp | ";
                    chatterString += chatter1.av + "+ av | ";

                    if (chatter1.niggles == 1)
                    {
                        chatterString += chatter1.niggles + " niggle | ";
                    }

                    else if (chatter1.niggles > 1)
                    {
                        chatterString += chatter1.niggles + " niggles | ";
                    }

                    foreach (string skill in chatter1.skills)
                    {
                        chatterString += skill + ", ";
                    }

                    if (chatter1.skills.Count > 0)
                    {
                        chatterString = chatterString.Remove(chatterString.Length - 2, 2);

                        chatterString += " | ";
                    }

                    int sppNeeded = dataHandler.SPPNeededToLevel(chatter1);

                    if (sppNeeded == 0)
                        chatterString += "level up possible!";

                    else if (sppNeeded > 0)
                        chatterString += sppNeeded + " spp needed to level";

                    else if (sppNeeded < 0)
                        chatterString += "max level reached!";


                    SendChannelMessage(chatterString);
                }

                else
                    SendChannelMessage("Chatter not found, you haven't challenged yet!");


                break;

            case "graveyard":

                for (int i = 0; i <= dataHandler.Deaths.Count - 1; i += 5)
                {
                    string graveyardString = string.Empty;

                    for (int j = i; j <= Mathf.Min(dataHandler.Deaths.Count - 1, i + 4); j ++)
                    {
                        graveyardString += dataHandler.Deaths[j].killer + " killed " + dataHandler.Deaths[j].killee + " on " + dataHandler.Deaths[j].date + " | ";
                    }

                    SendChannelMessage(graveyardString);
                }

                break;

            case "gy":

                for (int i = 0; i <= dataHandler.Deaths.Count - 1; i += 10)
                {
                    string graveyardString = string.Empty;

                    for (int j = i; j <= Mathf.Min(dataHandler.Deaths.Count - 1, i + 9); j++)
                    {
                        graveyardString += dataHandler.Deaths[j].killee + " " + dataHandler.Deaths[j].date + " | ";
                    }

                    SendChannelMessage(graveyardString);
                }

                break;

            case "startchalice":

                if (!e.Command.ChatMessage.IsBroadcaster)
                {
                    SendChannelMessage("Streamer only command");

                    break;
                }

                if (e.Command.ArgumentsAsList.Count > 0)
                    break;

                if (dataHandler.Chatters.Count < 8)
                {
                    SendChannelMessage("Insufficient chatters for chalice");

                    break;
                }

                if (!dataHandler.ChangeState(DataHandler.State.CHALICE))
                {
                    SendChannelMessage("Can't start chalice in current state");

                    break;
                }

                SendChannelMessage("CHALICE STARTING");

                dataHandler.StartChalice();

                UIhandler.StartChalice();

                break;

            case "endchalice":
            case "stopchalice":

                if (!e.Command.ChatMessage.IsBroadcaster)
                {
                    SendChannelMessage("Streamer only command");

                    break;
                }

                if (e.Command.ArgumentsAsList.Count > 0)
                    break;

                if (!dataHandler.ChangeState(DataHandler.State.NORMAL))
                {
                    SendChannelMessage("Can't stop chalice in current state");

                    break;
                }

                SendChannelMessage("Chalice over");

                string winner = UIhandler.StopChalice();

                if (winner != string.Empty)
                {
                    if (!dataHandler.AddChaliceWin(winner))
                    {
                        SendChannelMessage("Chalice winner not in chatter list");

                        break;
                    }

                    SendChannelMessage(winner + " wins chalice!");

                    dataHandler.ResetDefences();
                }

                break;

            case "getchatter":
            case "getcontender":
            case "getchallenger":

                if (args.Count == 1)
                {
                    Chatter chatter2 = dataHandler.GetChatter(args[0]);



                    if (chatter2 != null)
                    {
                        string chatterString = "";

                        chatterString += chatter2.name + " | ";
                        chatterString += "Level " + (chatter2.skills.Count + (chatter2.av - 8) + 1) + " | ";
                        chatterString += chatter2.defences + " defences | ";
                        chatterString += chatter2.offences + " takedowns | ";
                        chatterString += chatter2.blocks + " blocks | ";
                        chatterString += chatter2.cas + " cas | ";
                        chatterString += chatter2.kills + " kills | ";
                        chatterString += chatter2.deaths + " deaths | ";
                        chatterString += chatter2.spp + " spp | ";
                        chatterString += chatter2.av + "+ av | ";

                        if (chatter2.niggles == 1)
                        {
                            chatterString += chatter2.niggles + " niggle | ";
                        }

                        else if (chatter2.niggles > 1)
                        {
                            chatterString += chatter2.niggles + " niggles | ";
                        }

                        foreach (string skill in chatter2.skills)
                        {
                            chatterString += skill + ", ";
                        }

                        if (chatter2.skills.Count > 0)
                        {
                            chatterString = chatterString.Remove(chatterString.Length - 2, 2);

                            chatterString += " | ";
                        }

                        int sppNeeded = dataHandler.SPPNeededToLevel(chatter2);

                        if (sppNeeded == 0)
                            chatterString += "level up possible!";

                        else if (sppNeeded > 0)
                            chatterString += sppNeeded + " spp needed to level";

                        else if (sppNeeded < 0)
                            chatterString += "max level reached!";


                        SendChannelMessage(chatterString);
                    }

                    else
                        SendChannelMessage("Chatter not found, have you spelt the name correctly?");
                }

                else
                {
                    SendChannelMessage("Can't find chatter. Have you formatted the request correctly? [chatter]");
                }

                break;


            case "fight":
            case "f":

                if (e.Command.ArgumentsAsList.Count > 0)
                    break;

                if (!e.Command.ChatMessage.IsBroadcaster)
                {
                    SendChannelMessage("Streamer only command");

                    break;
                }

                if (dataHandler.CurrentState == DataHandler.State.NORMAL)
                {
                    if (!dataHandler.CanStartFight())
                    {
                        SendChannelMessage("Can't start fight, no appropriate challenger");

                        break;
                    }

                    if (!dataHandler.ChangeState(DataHandler.State.FIGHT))
                    {
                        SendChannelMessage("Can't start fight in current state");

                        break;
                    }

                    SendChannelMessage("Fight starting!");

                    UIhandler.StartFight();
                }

                else if (dataHandler.CurrentState == DataHandler.State.CHALICE)
                {
                    if (dataHandler.ChaliceRound >= 7)
                        break;

                    if (!dataHandler.ChangeState(DataHandler.State.ROUND))
                    {
                        SendChannelMessage("Can't start round in current state");

                        break;
                    }

                    SendChannelMessage("Round starting!");

                    UIhandler.StartRound();
                }



                break;


            //Challenger champ

            //nothing nothing   champ wins      
            //KO nothing        champ wins  
            //niggle nothing    champ wins          niggle attacker
            //death nothing     champ wins          death attacker

            //nothing KO        challenger wins     
            //KO KO             champ wins
            //niggle KO         champ wins          niggle attacker
            //death KO          champ wins          death attacker

            //nothing niggle    challenger wins     niggle champ
            //KO niggle         champ wins          niggle champ
            //niggle niggle     champ wins          niggle champ niggle attacker
            //death niggle      champ wins          niggle champ death attacker

            //nothing death     challenger wins     death champ
            //KO death          challenger wins     death champ
            //niggle death      challenger wins     death champ niggle attacker
            //death death       dimmy wins          death champ death attacker

            case "fightover":
            case "fo":

                if (!e.Command.ChatMessage.IsBroadcaster && !e.Command.ChatMessage.IsModerator)
                {
                    SendChannelMessage("Streamer or moderator only command");
                    break;
                }

                if (dataHandler.CurrentState == DataHandler.State.FIGHT)
                    FightOver(e.Command.ArgumentsAsList);

                else if (dataHandler.CurrentState == DataHandler.State.ROUND)
                    RoundOver(e.Command.ArgumentsAsList);

                break;

            //case "champwins":

            //    //Only broadcaster or moderator can use this command
            //    if (!e.Command.ChatMessage.IsBroadcaster && !e.Command.ChatMessage.IsModerator)
            //    {
            //        SendChannelMessage("Streamer or moderator only command");

            //        break;
            //    }

            //    SendChannelMessage("The champ wins!");

            //    ChampWins(args);

            //    break;

            //case "challengerwins":

            //    //Only broadcaster or moderator can use this command
            //    if (!e.Command.ChatMessage.IsBroadcaster && !e.Command.ChatMessage.IsModerator)
            //    {
            //        SendChannelMessage("Streamer or moderator only command");

            //        break;
            //    }

            //    SendChannelMessage("The challenger wins!");

            //    ChallengerWins(args);

            //    break;

            //case "dimmywins":

            //    //Only broadcaster or moderator can use this command
            //    if (!e.Command.ChatMessage.IsBroadcaster && !e.Command.ChatMessage.IsModerator)
            //    {
            //        SendChannelMessage("Streamer or moderator only command");

            //        break;
            //    }

            //    SendChannelMessage("Dimmy wins!?");

            //    DimmyWins();

            //    break;

            default:
                break;
        }
    }

    private void AddNiggles(List<string> args)
    {
        if (args.Count != 2)
        {
            SendChannelMessage("Wrong number of arguments. Need [chattername] [numniggles]");

            return;
        }

        string chatter = args[0];

        if (int.TryParse(args[1], out int numNiggles))
        {
            SendChannelMessage("Cannot parse number of niggles. Need [chattername] [numniggles]");

            return;
        }

        if (!dataHandler.AddNiggles(chatter, numNiggles))
        {
            SendChannelMessage("Cannot find chatter");

            return;
        }

        SendChannelMessage(numNiggles + " niggles added to " + chatter);
    }

    
    private void RoundOver(List<string> args)
    {

        //Command must have one or two arguments
        if (args.Count != 2)
        {
            SendChannelMessage("Incorrect number of arguments. Needs [injuryleft] [injuryright]");

            return;
        }

        string leftInjury = args[0].ToLower();
        string rightInjury = args[1].ToLower();

        //Both arguments must be within the sppDict

        if (!Config.SppDict.ContainsKey(leftInjury))
        {
            SendChannelMessage("Left injury incorrect. Must be (b)lock, (p)ow, (s)tun, (k)o, (c)as, (n)iggle, or (d)ead");

            return;
        }

        if (!Config.SppDict.ContainsKey(rightInjury))
        {
            SendChannelMessage("Right injury incorrect. Must be (b)lock, (p)ow, (s)tun, (k)o, (c)as, (n)iggle, or (d)ead");

            return;
        }

        int leftSpp = Config.SppDict[rightInjury];
        int rightSpp = Config.SppDict[leftInjury];

        string winner = string.Empty;

        //Both same result, random
        if (rightSpp == leftSpp)
        {
            if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                winner = "left";

            else
                winner = "right";
        }

        else if (leftSpp > rightSpp)
        {
            winner = "left";
        }

        else
        {
            winner = "right";
        }

        UIhandler.StopRound(winner, leftInjury, rightInjury);

        dataHandler.StopRound(winner, leftInjury, rightInjury, leftSpp, rightSpp);
    }


    //challengerinjury champinjury challengerspp
    private void FightOver(List<string> args)
    {
        //Command must have one or two arguments
        if (args.Count < 1 || args.Count > 2)
        {
            SendChannelMessage("Incorrect number of arguments. Needs [champinjury] [challengerinjury*]");

            return;
        }

        string champInjury = args[0].ToLower();
        string challengerInjury = "";

        if (args.Count == 2)
            challengerInjury = args[1].ToLower();




        //The third argument (spp) must be parseable as an int
        //if (!int.TryParse(args[2], out int spp))
        //{
        //    SendChannelMessage("SPP can't be parsed. Needs [challengerinjury] [champinjury] [spp]");

        //    return;
        //}

        Debug.Log("Champ injury: " + champInjury);
        Debug.Log("Challenger injury: " + challengerInjury);

        //Both arguments must be within the sppDict

        if (!Config.SppDict.ContainsKey(champInjury))
        {
            SendChannelMessage("Champ injury incorrect. Must be (b)lock, (p)ow, (s)tun, (k)o, (c)as, (n)iggle, or (d)ead");

            return;
        }

        if (!Config.SppDict.ContainsKey(challengerInjury))
        {
            SendChannelMessage("Challenger injury incorrect. Must be (b)lock, (p)ow, (s)tun, (k)o, (c)as, (n)iggle, or (d)ead");

            return;
        }

        int challengerSpp = Config.SppDict[champInjury];
        int champSpp = Config.SppDict[challengerInjury];

        dataHandler.StopFight(challengerInjury, champInjury, challengerSpp, champSpp);

        UIhandler.StopFight(challengerInjury, champInjury);
    }

    private void Update()
    {
        if (!connectMessage && client.JoinedChannels.Count > 0)
        {
            connectMessage = true;

            SendChannelMessage("Champbot online");
        }
    }

    public void SendChannelMessage(string message)
    {
        client.SendMessage(channelName, message);
    }
}
