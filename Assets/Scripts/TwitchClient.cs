using System.Collections;
using System.Collections.Generic;

using TwitchLib.Client.Models;
using TwitchLib.Unity;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

public class TwitchClient : MonoBehaviour
{

    public Client client;
    private string channelName;

    private UIHandler UIhandler;

    private DataHandler dataHandler;


    bool connectMessage = false;

    // Start is called before the first frame update
    void Start()
    {
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
        client.OnCommunitySubscription += OnCommunitySubscription;
        client.OnGiftedSubscription += OnGiftedSubscription;
        client.OnContinuedGiftedSubscription += OnContinuedGiftedSubscription;
        client.OnNewSubscriber += OnNewSubscriber;
        client.OnPrimePaidSubscriber += OnPrimePaidSubscriber;
        client.OnReSubscriber += OnResubscriber;

        client.Connect();
    }


    //Mass gift
    private void OnCommunitySubscription(object sender, TwitchLib.Client.Events.OnCommunitySubscriptionArgs e)
    {
        client.SendMessage(e.Channel, $"Community subscription from " + e.GiftedSubscription.DisplayName + "!");
    }

    //Individual gift
    private void OnGiftedSubscription(object sender, TwitchLib.Client.Events.OnGiftedSubscriptionArgs e)
    {
        client.SendMessage(e.Channel, $"Gift sub from " + e.GiftedSubscription.DisplayName + "!");

        dataHandler.AddChallenger(e.GiftedSubscription.DisplayName, 3);
    }

    private void OnContinuedGiftedSubscription(object sender, TwitchLib.Client.Events.OnContinuedGiftedSubscriptionArgs e)
    {
        client.SendMessage(e.Channel, $"Continued gifted subscription by " + e.ContinuedGiftedSubscription.DisplayName + "!");

        dataHandler.AddChallenger(e.ContinuedGiftedSubscription.DisplayName, 3);
    }

    private void OnNewSubscriber(object sender, TwitchLib.Client.Events.OnNewSubscriberArgs e)
    {
        client.SendMessage(e.Channel, $"New subscription from " + e.Subscriber.DisplayName + "!");

        dataHandler.AddChallenger(e.Subscriber.DisplayName, 3);
    }

    private void OnPrimePaidSubscriber(object sender, TwitchLib.Client.Events.OnPrimePaidSubscriberArgs e)
    {
        client.SendMessage(e.Channel, $"Prime subscription from " + e.PrimePaidSubscriber.DisplayName + "!");

        dataHandler.AddChallenger(e.PrimePaidSubscriber.DisplayName, 3);
    }

    private void OnResubscriber(object sender, TwitchLib.Client.Events.OnReSubscriberArgs e)
    {
        client.SendMessage(e.Channel, $"Resubscription from " + e.ReSubscriber.DisplayName + "!");

        dataHandler.AddChallenger(e.ReSubscriber.DisplayName, 3);
    }



    private void OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
    {
        switch (e.Command.CommandText)
        {
            case "champhelp":

                SendChannelMessage("!fight");
                SendChannelMessage("!champwins [challengersppgain]");
                SendChannelMessage("!champloses [challengersppgain]");
                SendChannelMessage("!addskill [chattername] [skill]");
                SendChannelMessage("!addav [chattername]");

                SendChannelMessage("!getcontender [chattername]");
                SendChannelMessage("!mycontendor");
                SendChannelMessage("!champtop5");
                SendChannelMessage("!listskills");
                SendChannelMessage("!addmyskill [skillname]");

                SendChannelMessage("!listchallengers");
                SendChannelMessage("!champreload --- reload from data files if editing during operation");
                SendChannelMessage("!setspp [challengername] [spp]");
                SendChannelMessage("!setchamp [champname] --- only use if something's gone wrong");
                SendChannelMessage("!setchatter [chattername] [def] [spp] [av] [niggles] ... --- only use if something's gone wrong - then set skills");
                SendChannelMessage("!setfreeskill [chattername] [skill] ... --- only use if something's gone wrong. No ssp cost");
                SendChannelMessage("!addchallenger [challengername] [numdice] --- only use if something's gone wrong");

                break;

            case "listskills":

                string skillsString = "Available skills: ";

                foreach (string skill in dataHandler.PossibleSkills)
                {
                    skillsString += skill + " | ";
                }

                SendChannelMessage(skillsString);

                break;

            case "listchallengers":

                if (!e.Command.ChatMessage.IsBroadcaster && !e.Command.ChatMessage.IsModerator)
                    break;

                foreach (Challenger challenger in dataHandler.Challengers)
                {
                    SendChannelMessage(challenger.chatter.name + " " + challenger.numDice.ToString() + " dice");
                }

                break;

            case "champreload":

                if (!e.Command.ChatMessage.IsBroadcaster)
                    break;

                dataHandler.LoadData();

                SendChannelMessage("Reloading champ data from file");

                break;

            case "setspp":

                if (!e.Command.ChatMessage.IsBroadcaster)
                    break;

                if (e.Command.ArgumentsAsList.Count == 2 && int.TryParse(e.Command.ArgumentsAsList[1], out int numSPP))
                {
                    if (dataHandler.SetSPP(e.Command.ArgumentsAsList[0], numSPP))
                        SendChannelMessage(e.Command.ArgumentsAsList[0] + " set to " + numSPP + " spp");

                    else
                        SendChannelMessage("Chatter cannot be found");


                }

                else
                {
                    SendChannelMessage("Wrong number or type of arguments");
                }

                break;

            case "setchamp":

                if (!e.Command.ChatMessage.IsBroadcaster)
                    break;

                if (e.Command.ArgumentsAsList.Count == 1 && dataHandler.SetChamp(e.Command.ArgumentsAsList[0]))
                {
                    SendChannelMessage("New champ " + e.Command.ArgumentsAsList[0] + " set succesfully");
                }

                else
                {
                    SendChannelMessage("New champ can't be set. Do they exist as a chatter? Use !setchatter first if not");
                }


                break;

            case "addchallenger":

                if (!e.Command.ChatMessage.IsBroadcaster)
                    break;

                if (e.Command.ArgumentsAsList.Count == 2 && int.TryParse(e.Command.ArgumentsAsList[1], out int numDice))
                {
                    dataHandler.AddChallenger(e.Command.ArgumentsAsList[0], numDice);

                    SendChannelMessage(e.Command.ArgumentsAsList[0] + " wants to " + numDice + " dice the champ!");
                }

                else if (e.Command.ArgumentsAsList.Count == 3 && int.TryParse(e.Command.ArgumentsAsList[1], out int numDice2) && int.TryParse(e.Command.ArgumentsAsList[2], out int numAttempts))
                {
                    for (int i = 0; i < numAttempts; i ++)
                        dataHandler.AddChallenger(e.Command.ArgumentsAsList[0], numDice2);

                    SendChannelMessage(e.Command.ArgumentsAsList[0] + " wants to " + numDice2 + " dice the champ " + numAttempts + " times!");
                }

                else
                {
                    SendChannelMessage("Challenger can't be added. Have you formatted the request correctly?");
                }

                break;


            case "setchatter":

                if (!e.Command.ChatMessage.IsBroadcaster)
                    break;

                if (e.Command.ArgumentsAsList.Count == 5 && int.TryParse(e.Command.ArgumentsAsList[1], out int def) && int.TryParse(e.Command.ArgumentsAsList[2], out int spp) && int.TryParse(e.Command.ArgumentsAsList[3], out int av) && int.TryParse(e.Command.ArgumentsAsList[4], out int niggles))
                {
                    dataHandler.SetChatter(e.Command.ArgumentsAsList[0], def, spp, av, niggles);
                }

                else
                {
                    SendChannelMessage("Chatter can't be set. Have you formatted the request correctly?");
                }

                break;

            case "setfreeskill":

                if (!e.Command.ChatMessage.IsBroadcaster)
                    break;

                if (e.Command.ArgumentsAsList.Count >= 2)
                {
                    string skillString = "";

                    for (int i = 1; i < e.Command.ArgumentsAsList.Count; i++)
                    {
                        skillString += e.Command.ArgumentsAsList[i] + " ";
                    }

                    skillString = skillString.Remove(skillString.Length - 1, 1);



                    if (dataHandler.AddFreeSkill(e.Command.ArgumentsAsList[0], skillString))
                    {
                        SendChannelMessage(e.Command.ArgumentsAsList[0] + " has learned " + skillString);

                        UIhandler.SetChampText();
                    }

                    else
                        SendChannelMessage("Skill can't be added. Either the chatter doesn't exist, the skill doesn't exist, or the skill is a duplicate.");

                }

                else
                {
                    SendChannelMessage("Skill can't be added. Have you formatted the request correctly?");
                }


                break;

            case "addmyskill":

                if (e.Command.ArgumentsAsList.Count >= 1)
                {
                    string skillString = "";

                    for (int i = 0; i < e.Command.ArgumentsAsList.Count; i++)
                    {
                        skillString += e.Command.ArgumentsAsList[i] + " ";
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
                    SendChannelMessage("Skill can't be added. Have you formatted the request correctly?");
                }

                break;

            case "addskill":

                if (!e.Command.ChatMessage.IsBroadcaster && !e.Command.ChatMessage.IsModerator)
                    break;

                if (e.Command.ArgumentsAsList.Count >= 2)
                {
                    string skillString = "";

                    for (int i = 1; i < e.Command.ArgumentsAsList.Count; i++)
                    {
                        skillString += e.Command.ArgumentsAsList[i] + " "; 
                    }

                    skillString = skillString.Remove(skillString.Length - 1, 1);



                    if (dataHandler.AddSkill(e.Command.ArgumentsAsList[0], skillString))
                    {
                        SendChannelMessage(e.Command.ArgumentsAsList[0] + " has learned " + skillString);

                        UIhandler.SetChampText();
                    }

                    else
                        SendChannelMessage("Skill can't be added. Either the chatter doesn't exist, the chatter has insufficient spp, the skill doesn't exist, or the skill is a duplicate.");

                }

                else
                {
                    SendChannelMessage("Skill can't be added. Have you formatted the request correctly?");
                }


                break;


            case "addav":

                if (!e.Command.ChatMessage.IsBroadcaster && !e.Command.ChatMessage.IsModerator)
                    break;

                if (e.Command.ArgumentsAsList.Count == 1)
                {
                    if (dataHandler.AddAv(e.Command.ArgumentsAsList[0]))
                        SendChannelMessage(e.Command.ArgumentsAsList[0] + " has increased their av");

                    else
                        SendChannelMessage("Av can't be added. The chatter doesn't exist or has insufficient spp.");

                }

                else
                {
                    SendChannelMessage("Av can't be added. Have you formatted the request correctly?");
                }


                break;


            case "champtop5":

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


            case "mychatter":
            case "mychallenger":
            case "mycontender":

                Chatter chatter1 = dataHandler.GetChatter(e.Command.ChatMessage.Username);

                if (chatter1 != null)
                {
                    string chatterString = "";

                    chatterString += chatter1.name + ", ";
                    chatterString += chatter1.defences + " defences, ";
                    chatterString += chatter1.spp + " spp, ";
                    chatterString += chatter1.av + "+ av, ";

                    if (chatter1.niggles == 1)
                    {
                        chatterString += chatter1.niggles + "niggle, ";
                    }

                    else if (chatter1.niggles > 1)
                    {
                        chatterString += chatter1.niggles + "niggles, ";
                    }

                    foreach (string skill in chatter1.skills)
                    {
                        chatterString += skill + ", ";
                    }

                    chatterString = chatterString.Remove(chatterString.Length - 2, 2);


                    SendChannelMessage(chatterString);
                }

                else
                    SendChannelMessage("Chatter not found, you haven't challenged yet!");


                break;

            case "getchatter":
            case "getcontender":
            case "getchallenger":

                if (e.Command.ArgumentsAsList.Count == 1)
                {
                    Chatter chatter2 = dataHandler.GetChatter(e.Command.ArgumentsAsList[0]);



                    if (chatter2 != null)
                    {
                        string chatterString = "";

                        chatterString += chatter2.name + ", ";
                        chatterString += chatter2.defences + " defences, ";
                        chatterString += chatter2.spp + " spp, ";
                        chatterString += chatter2.av + "+ av, ";

                        if (chatter2.niggles == 1)
                        {
                            chatterString += chatter2.niggles + " niggle, ";
                        }

                        else if (chatter2.niggles > 1)
                        {
                            chatterString += chatter2.niggles + " niggles, ";
                        }

                        foreach (string skill in chatter2.skills)
                        {
                            chatterString += skill + ", ";
                        }

                        chatterString = chatterString.Remove(chatterString.Length - 2, 2);


                        SendChannelMessage(chatterString);
                    }

                    else
                        SendChannelMessage("Chatter not found, have you spelt the name correctly?");
                }

                else
                {
                    SendChannelMessage("Can't find chatter. Have you formatted the request correctly?");
                }

                break;


            case "fight":

                if (!e.Command.ChatMessage.IsBroadcaster)
                    break;

                UIhandler.StartFight();

                break;

            case "champwins":

                if (!e.Command.ChatMessage.IsBroadcaster && !e.Command.ChatMessage.IsModerator)
                    break;

                if (e.Command.ArgumentsAsList.Count == 1 && int.TryParse(e.Command.ArgumentsAsList[0], out int spp1))
                {
                    dataHandler.ChampWins(spp1);

                    UIhandler.StopFight();
                }

                else
                {
                    client.SendMessage(e.Command.ChatMessage.Channel, "Command format incorrect (did you include spp gain?)");
                }

                break;

            


            case "champloses":

                if (!e.Command.ChatMessage.IsBroadcaster && !e.Command.ChatMessage.IsModerator)
                    break;

                if (e.Command.ArgumentsAsList.Count == 1 && int.TryParse(e.Command.ArgumentsAsList[0], out int spp2))
                {
                    dataHandler.ChampLoses(spp2);

                    UIhandler.StopFight();
                }

                else
                {
                    client.SendMessage(e.Command.ChatMessage.Channel, "Command format incorrect (did you include spp gain?)");
                }

                break;




            default:
                //SendChannelMessage($"Unknown chat command: {e.Command.CommandIdentifier}{e.Command.CommandText}");
                break;


        }
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
