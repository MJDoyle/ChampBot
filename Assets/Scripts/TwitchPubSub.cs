using TwitchLib.Unity;
using UnityEngine;
using System.IO;

public class TwitchPubSub : MonoBehaviour
{
    private PubSub _pubSub;

    private DataHandler dataHandler;

    private TwitchClient twitchClient;

    private void Start()
    {
        dataHandler = GetComponent<DataHandler>();

        twitchClient = GetComponent<TwitchClient>();
        

        // Create new instance of PubSub Client
        _pubSub = new PubSub();

        // Subscribe to Events
        _pubSub.OnPubSubServiceConnected += OnPubSubServiceConnected;

        _pubSub.OnRewardRedeemed += OnPubSubRewardRedeemed;


        // Connect
        _pubSub.Connect();
    }

    private void OnPubSubServiceConnected(object sender, System.EventArgs e)
    {
        string channelID = "";

        StreamReader channelIDReader = new StreamReader("Config/channelID.txt");

        channelID = channelIDReader.ReadLine();

        channelIDReader.Close();


        string oauth = "";

        StreamReader oauthReader = new StreamReader("Config/oauth.txt");

        oauth = oauthReader.ReadLine();

        oauthReader.Close();

        Debug.Log("PubSubServiceConnected!");

        _pubSub.ListenToRewards(channelID);

        // SendTopics accepts an oauth optionally, which is necessary for some topics, such as bit events.
        _pubSub.SendTopics(oauth);
    }


    private void OnPubSubRewardRedeemed(object sender, TwitchLib.PubSub.Events.OnRewardRedeemedArgs e)
    {
        if (e.RewardTitle.Contains("1 Dice"))
        {
            dataHandler.AddChallenger(e.DisplayName, 3);

            twitchClient.SendChannelMessage(e.DisplayName + " wants to 1 dice the champ!");
        }

        else if (e.RewardTitle.Contains("2 Dice"))
        {
            dataHandler.AddChallenger(e.DisplayName, 2);

            twitchClient.SendChannelMessage(e.DisplayName + " wants to 2 dice the champ!");
        }

        else if (e.RewardTitle.Contains("3 Dice"))
        {
            dataHandler.AddChallenger(e.DisplayName, 1);

            twitchClient.SendChannelMessage(e.DisplayName + " wants to 3 dice the champ!");
        }
    }
}