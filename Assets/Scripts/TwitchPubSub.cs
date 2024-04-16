using TwitchLib.Unity;
using UnityEngine;
using System.IO;

public class TwitchPubSub : MonoBehaviour
{
    private PubSub pubSub;

    private DataHandler dataHandler;

    private UIHandler uiHandler;

    private TwitchClient twitchClient;

    private void Start()
    {
        dataHandler = GetComponent<DataHandler>();

        uiHandler = GetComponent<UIHandler>();

        twitchClient = GetComponent<TwitchClient>();
        
         // Create new instance of PubSub Client
        pubSub = new PubSub();

        // Subscribe to Events
        pubSub.OnPubSubServiceConnected += OnPubSubServiceConnected;

        pubSub.OnRewardRedeemed += OnPubSubRewardRedeemed;

        // Connect
        pubSub.Connect();
    }

    public void Disconnect()
    {
        pubSub.Disconnect();
    }

    private void OnPubSubServiceConnected(object sender, System.EventArgs e)
    {
        StreamReader channelIDReader = new StreamReader("Config/channelID.txt");

        string channelID = channelIDReader.ReadLine();

        channelIDReader.Close();

        StreamReader oauthReader = new StreamReader("Config/oauth.txt");

        string oauth = oauthReader.ReadLine();

        oauthReader.Close();

        Debug.Log("PubSubServiceConnected!");

        pubSub.ListenToRewards(channelID);

        // SendTopics accepts an oauth optionally, which is necessary for some topics, such as bit events.
        pubSub.SendTopics(oauth);
    }


    private void OnPubSubRewardRedeemed(object sender, TwitchLib.PubSub.Events.OnRewardRedeemedArgs e)
    {
        //Return on point refunds etc.
        if (e.Status != "UNFULFILLED")
            return;

        if (e.RewardTitle.Contains("1 Dice"))
        {
            dataHandler.AddChallenger(e.DisplayName, 1);

            uiHandler.SetChallengerListItems();
            uiHandler.ShowChallengerListItems();

            twitchClient.SendChannelMessage(e.DisplayName + " wants to 1 dice the champ!");
        }

        else if (e.RewardTitle.Contains("2 Dice"))
        {
            dataHandler.AddChallenger(e.DisplayName, 2);

            uiHandler.SetChallengerListItems();
            uiHandler.ShowChallengerListItems();

            twitchClient.SendChannelMessage(e.DisplayName + " wants to 2 dice the champ!");
        }

        else if (e.RewardTitle.Contains("3 Dice"))
        {
            dataHandler.AddChallenger(e.DisplayName, 3);

            uiHandler.SetChallengerListItems();
            uiHandler.ShowChallengerListItems();

            twitchClient.SendChannelMessage(e.DisplayName + " wants to 3 dice the champ!");
        }
    }
}