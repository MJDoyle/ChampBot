using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{

    private Vector2 challengerHiddenPosition = new Vector2(-18, 0);

    private Vector2 challengerShownPosition = new Vector2(-7, 0);

    private Vector2 champHiddenPosition = new Vector2(18, 0);

    private Vector2 champShownPosition = new Vector2(7, 0);

    [SerializeField]
    private GameObject challengerCard;

    [SerializeField]
    private GameObject champCard;

    private float cardSpeed = 10f;




    [SerializeField]
    private GameObject currentChampCard;

    [SerializeField]
    private Text champName;

    [SerializeField]
    private Text champAV;

    [SerializeField]
    private Text champSPP;

    [SerializeField]
    private Text champDef;

    private List<GameObject> champSkillsBottom = new List<GameObject>();

    private List<GameObject> champNigglesBottom = new List<GameObject>();







    [SerializeField]
    private Text numDiceText;





    [SerializeField]
    private Text challengerNameText;

    [SerializeField]
    private Text challengerSPPText;

    [SerializeField]
    private Text challengerDefText;

    [SerializeField]
    private Text challengerAvText;

    [SerializeField]
    private Image challengerImage;

    private List<GameObject> challengerSkills = new List<GameObject>();

    private List<GameObject> challengerNiggles = new List<GameObject>();




    [SerializeField]
    private Text champNameText;

    [SerializeField]
    private Text champSPPText;

    [SerializeField]
    private Text champDefText;

    [SerializeField]
    private Text champAvText;

    [SerializeField]
    private Image champImage;

    private List<GameObject> champSkills = new List<GameObject>();

    private List<GameObject> champNiggles = new List<GameObject>();





    private Dictionary<string, GameObject> skillPrefabs = new Dictionary<string, GameObject>();



    [SerializeField]
    private GameObject blockPrefab;

    [SerializeField]
    private GameObject ironHardSkinPrefab;

    [SerializeField]
    private GameObject brawlerPrefab;

    [SerializeField]
    private GameObject clawPrefab;

    [SerializeField]
    private GameObject dirtyPlayerPrefab;

    [SerializeField]
    private GameObject dodgePrefab;

    [SerializeField]
    private GameObject fendPrefab;

    [SerializeField]
    private GameObject foulAppearancePrefab;

    [SerializeField]
    private GameObject frenzyPrefab;

    [SerializeField]
    private GameObject juggernautPrefab;

    [SerializeField]
    private GameObject mightyBlowPrefab;

    [SerializeField]
    private GameObject pileDriverPrefab;

    [SerializeField]
    private GameObject proPrefab;

    [SerializeField]
    private GameObject tacklePrefab;

    [SerializeField]
    private GameObject thickSkullPrefab;

    [SerializeField]
    private GameObject wrestlePrefab;

    [SerializeField]
    private GameObject nigglePrefab;

    [SerializeField]
    private GameObject placeholderPrefab;



    [SerializeField]
    private GameObject chaliceBackground;

    [SerializeField]
    private GameObject roundBackground;



    [SerializeField]
    private ChallengerListItem challengerListItemPrefab;

    private List<ChallengerListItem> challengerListItems = new List<ChallengerListItem>();




    [SerializeField]
    private List<Text> r_8s = new List<Text>();

    [SerializeField]
    private List<Text> r_4s = new List<Text>();

    [SerializeField]
    private List<Text> r_2s = new List<Text>();

    [SerializeField]
    private Text r_1;




    bool fighting = false;

    private DataHandler dataHandler;

    private TwitchClient twitchClient;

    bool fullscreen = true;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

        chaliceBackground.SetActive(false);

        roundBackground.SetActive(false);

        dataHandler = GetComponent<DataHandler>();

        twitchClient = GetComponent<TwitchClient>();

        skillPrefabs["block"] = blockPrefab;

        skillPrefabs["iron hard skin"] = ironHardSkinPrefab;
        skillPrefabs["ironhardskin"] = ironHardSkinPrefab;

        skillPrefabs["brawler"] = brawlerPrefab;

        skillPrefabs["claw"] = clawPrefab;

        skillPrefabs["dirty player"] = dirtyPlayerPrefab;
        skillPrefabs["dirtyplayer"] = dirtyPlayerPrefab;

        skillPrefabs["dodge"] = dodgePrefab;

        skillPrefabs["fend"] = fendPrefab;

        skillPrefabs["foul appearance"] = foulAppearancePrefab;
        skillPrefabs["foulappearance"] = foulAppearancePrefab;

        skillPrefabs["frenzy"] = frenzyPrefab;

        skillPrefabs["juggernaut"] = juggernautPrefab;

        skillPrefabs["mighty blow"] = mightyBlowPrefab;
        skillPrefabs["mightyblow"] = mightyBlowPrefab;

        skillPrefabs["pile driver"] = pileDriverPrefab;
        skillPrefabs["piledriver"] = pileDriverPrefab;

        skillPrefabs["pro"] = proPrefab;

        skillPrefabs["tackle"] = tacklePrefab;

        skillPrefabs["thick skull"] = thickSkullPrefab;
        skillPrefabs["thickskull"] = thickSkullPrefab;

        skillPrefabs["wrestle"] = wrestlePrefab;
    }

    public void StartChalice()
    {
        chaliceBackground.SetActive(true);

        Debug.Log("Check1");

        if (dataHandler.ChalicePairings.Count != 7)
            return;

        Debug.Log("Check2");

        if (r_8s.Count != 8)
            return;

        Debug.Log("Check3");

        for (int i = 0; i < 4; i ++)
        {
            r_8s[2 * i].text = dataHandler.ChalicePairings[i][0].name;

            r_8s[2 * i + 1].text = dataHandler.ChalicePairings[i][1].name;
        }


        for (int i = 0; i < 4; i++)
        {
            r_4s[i].text = "";
        }

        for (int i = 0; i < 2; i++)
        {
            r_2s[i].text = "";
        }

        r_1.text = "";
    }

    public string StopChalice()
    {
        chaliceBackground.SetActive(false);

        roundBackground.SetActive(false);

        return r_1.text;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            fullscreen = !fullscreen;

            if (fullscreen)
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

            else
                Screen.fullScreenMode = FullScreenMode.Windowed;
        }



        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (champName.text == "" )
            SetChampText();


        if (fighting)
        {
            if ((challengerCard.transform.position - (Vector3)challengerShownPosition).magnitude < Time.deltaTime * cardSpeed)
                challengerCard.transform.position = challengerShownPosition;

            else
                challengerCard.transform.position += new Vector3(cardSpeed * Time.deltaTime, 0, 0);

            if ((champCard.transform.position - (Vector3)champShownPosition).magnitude < Time.deltaTime * cardSpeed)
                champCard.transform.position = champShownPosition;

            else
                champCard.transform.position -= new Vector3(cardSpeed * Time.deltaTime, 0, 0);
        }



        else
        {
            if ((challengerCard.transform.position - (Vector3)challengerHiddenPosition).magnitude < Time.deltaTime * cardSpeed)
                challengerCard.transform.position = challengerHiddenPosition;

            else
                challengerCard.transform.position -= new Vector3(cardSpeed * Time.deltaTime, 0, 0);

            if ((champCard.transform.position - (Vector3)champHiddenPosition).magnitude < Time.deltaTime * cardSpeed)
                champCard.transform.position = champHiddenPosition;

            else
                champCard.transform.position += new Vector3(cardSpeed * Time.deltaTime, 0, 0);
        }
    }

    public void SetChallengerListItems()
    {
        //Debug.Log("Set challenger list items");

        foreach (ChallengerListItem challengerListItem in challengerListItems)
        {
            //Debug.Log(challengerListItem);

            Destroy(challengerListItem.gameObject);
        }

        challengerListItems.Clear();

        for (int i = 0; i < dataHandler.Challengers.Count; i++)
        {
            challengerListItems.Add(Instantiate(challengerListItemPrefab));


            challengerListItems[i].transform.position = new Vector3(-8.35f, -4.83f + 0.3f * i, 0);


            challengerListItems[i].NameText.text = dataHandler.Challengers[i].chatter.name.Substring(0, Mathf.Min(8, dataHandler.Challengers[i].chatter.name.Length));

            challengerListItems[i].DiceText.text = dataHandler.Challengers[i].numDice.ToString();

            challengerListItems[i].gameObject.SetActive(false);
        }


    }

    public async void ShowChallengerListItems()
    {
        foreach (ChallengerListItem challengerListItem in challengerListItems)
        {
            challengerListItem.gameObject.SetActive(true);
        }

        await Task.Delay(5000);

        foreach (ChallengerListItem challengerListItem in challengerListItems)
        {
            challengerListItem.gameObject.SetActive(false);
        }
    }

    public void StartRound()
    {
        if (dataHandler.ChaliceRound >= 7)
            return;

        roundBackground.SetActive(true);

        Chatter leftChatter = dataHandler.ChalicePairings[dataHandler.ChaliceRound][0];

        Chatter rightChatter = dataHandler.ChalicePairings[dataHandler.ChaliceRound][1];

        //string chatter_1_name = string.Empty;

        //string chatter_2_name = string.Empty;

        //if (dataHandler.ChaliceRound < 4)
        //{
        //    chatter_1_name = r_8s[dataHandler.ChaliceRound * 2].text;

        //    chatter_2_name = r_8s[dataHandler.ChaliceRound * 2 + 1].text;
        //}

        //else if (dataHandler.ChaliceRound < 6)
        //{
        //    chatter_1_name = r_4s[(dataHandler.ChaliceRound - 4) * 2].text;

        //    chatter_2_name = r_4s[(dataHandler.ChaliceRound - 4) * 2 + 1].text;
        //}

        //else if (dataHandler.ChaliceRound < 7)
        //{
        //    chatter_1_name = r_2s[0].text;

        //    chatter_2_name = r_2s[1].text;
        //}

        //else
        //    return;


        //if (!dataHandler.Chatters.ContainsKey(chatter_1_name))
        //    return;

        //if (!dataHandler.Chatters.ContainsKey(chatter_2_name))
        //    return;


        //Chatter chatter_1 = dataHandler.Chatters[chatter_1_name];

        //Chatter chatter_2 = dataHandler.Chatters[chatter_2_name];

        SoundHandler.PlayBlockSound();

        if (leftChatter.skills.Contains("foul appearance") || rightChatter.skills.Contains("foul appearance"))
            SoundHandler.PlayFASound();

        numDiceText.text = "THREE DICE";


        challengerNameText.text = leftChatter.name;

        challengerDefText.text = leftChatter.defences.ToString();

        challengerSPPText.text = leftChatter.spp.ToString();

        challengerAvText.text = leftChatter.av.ToString() + "+";




        champNameText.text = rightChatter.name;

        champDefText.text = rightChatter.defences.ToString();

        champSPPText.text = rightChatter.spp.ToString();

        champAvText.text = rightChatter.av.ToString() + "+";




        SetCardSkills(rightChatter, leftChatter);


        //Load champ and challenger images

        if (File.Exists("GFX/" + rightChatter.name + ".png"))
        {
            Texture2D champTexture = new Texture2D(2, 2);
            ImageConversion.LoadImage(champTexture, File.ReadAllBytes("GFX/" + rightChatter.name + ".png"));

            champImage.sprite = Sprite.Create(champTexture, new Rect(0, 0, champTexture.width, champTexture.height), new Vector2(0.5f, 0.5f));
        }

        else
        {
            Texture2D champTexture = new Texture2D(2, 2);

            champImage.sprite = Sprite.Create(champTexture, new Rect(0, 0, champTexture.width, champTexture.height), new Vector2(0.5f, 0.5f));
        }

        if (File.Exists("GFX/" + leftChatter.name + ".png"))
        {
            Texture2D challengerTexture = new Texture2D(2, 2);
            ImageConversion.LoadImage(challengerTexture, File.ReadAllBytes("GFX/" + leftChatter.name + ".png"));

            challengerImage.sprite = Sprite.Create(challengerTexture, new Rect(0, 0, challengerTexture.width, challengerTexture.height), new Vector2(0.5f, 0.5f));
        }

        else
        {
            Texture2D challengerTexture = new Texture2D(2, 2);

            challengerImage.sprite = Sprite.Create(challengerTexture, new Rect(0, 0, challengerTexture.width, challengerTexture.height), new Vector2(0.5f, 0.5f));
        }



        fighting = true;
    }


    public void StartFight()
    {
        //Get champ and next challenger details
        if (dataHandler.CurrentChamp == null)
        {
            twitchClient.SendChannelMessage("Can't fight, no champ!");

            return;
        }

        if (!dataHandler.CanStartFight())
        {
            twitchClient.SendChannelMessage("Can't fight, no challenger!");

            return;
        }

        SoundHandler.PlayBlockSound();

        if (dataHandler.CurrentChamp.skills.Contains("foul appearance"))
            SoundHandler.PlayFASound();

        Chatter champ = dataHandler.CurrentChamp;

        Challenger challenger = dataHandler.GetNextChallenger();


        if (challenger.numDice == 1)
            numDiceText.text = "ONE DICE";

        if (challenger.numDice == 2)
            numDiceText.text = "TWO DICE";

        if (challenger.numDice == 3)
            numDiceText.text = "THREE DICE";


        challengerNameText.text = challenger.chatter.name;

        challengerDefText.text = challenger.chatter.defences.ToString();

        challengerSPPText.text = challenger.chatter.spp.ToString();

        challengerAvText.text = challenger.chatter.av.ToString() + "+";




        champNameText.text = champ.name;

        champDefText.text = champ.defences.ToString();

        champSPPText.text = champ.spp.ToString();

        champAvText.text = champ.av.ToString() + "+";




        SetCardSkills(champ, challenger);


        //Load champ and challenger images

        if (File.Exists("GFX/" + champ.name + ".png"))
        {
            Texture2D champTexture = new Texture2D(2, 2);
            ImageConversion.LoadImage(champTexture, File.ReadAllBytes("GFX/" + champ.name + ".png"));

            champImage.sprite = Sprite.Create(champTexture, new Rect(0, 0, champTexture.width, champTexture.height), new Vector2(0.5f, 0.5f));
        }

        else
        {
            Texture2D champTexture = new Texture2D(2, 2);

            champImage.sprite = Sprite.Create(champTexture, new Rect(0, 0, champTexture.width, champTexture.height), new Vector2(0.5f, 0.5f));
        }

        if (File.Exists("GFX/" + challenger.chatter.name + ".png"))
        {
            Texture2D challengerTexture = new Texture2D(2, 2);
            ImageConversion.LoadImage(challengerTexture, File.ReadAllBytes("GFX/" + challenger.chatter.name + ".png"));

            challengerImage.sprite = Sprite.Create(challengerTexture, new Rect(0, 0, challengerTexture.width, challengerTexture.height), new Vector2(0.5f, 0.5f));
        }

        else
        {
            Texture2D challengerTexture = new Texture2D(2, 2);

            challengerImage.sprite = Sprite.Create(challengerTexture, new Rect(0, 0, challengerTexture.width, challengerTexture.height), new Vector2(0.5f, 0.5f));
        }



        fighting = true;
    }

    private void SetCardSkills(Chatter chatter_1, Chatter chatter_2)
    {
        //Delete old skills and niggles
        foreach (GameObject skill in champSkills)
        {
            Destroy(skill);
        }

        champSkills.Clear();

        foreach (GameObject niggle in champNiggles)
        {
            Destroy(niggle);
        }

        champNiggles.Clear();

        foreach (GameObject skill in challengerSkills)
        {
            Destroy(skill);
        }

        challengerSkills.Clear();

        foreach (GameObject niggle in challengerNiggles)
        {
            Destroy(niggle);
        }

        challengerNiggles.Clear();

        //Champ skills

        foreach (string skill in chatter_1.skills)
        {
            if (skillPrefabs.ContainsKey(skill))
            {
                champSkills.Add(Instantiate(skillPrefabs[skill], champCard.transform));
            }

            else
            {
                champSkills.Add(Instantiate(placeholderPrefab, champCard.transform));
            }
        }

        for (int i = 0; i < champSkills.Count; i++)
        {
            if (i <= 5)
            {
                champSkills[i].transform.localPosition = new Vector3(-1.17f + i * 0.47f, -1.43f, 0);
            }

            else
            {
                champSkills[i].transform.localPosition = new Vector3(-1.17f + (i - 6) * 0.47f, -1.9f, 0);
            }
        }


        for (int i = 0; i < chatter_1.niggles; i++)
        {
            champNiggles.Add(Instantiate(nigglePrefab, champCard.transform));

            champNiggles[i].transform.localPosition = new Vector3(-1.17f + i * 0.47f, 1.4f, 0);
        }




        //Challenger skills

        foreach (string skill in chatter_2.skills)
        {
            if (skillPrefabs.ContainsKey(skill))
            {
                challengerSkills.Add(Instantiate(skillPrefabs[skill], challengerCard.transform));
            }

            else
            {
                challengerSkills.Add(Instantiate(placeholderPrefab, challengerCard.transform));
            }
        }

        for (int i = 0; i < challengerSkills.Count; i++)
        {
            if (i <= 5)
            {
                challengerSkills[i].transform.localPosition = new Vector3(-1.17f + i * 0.47f, -1.43f, 0);
            }

            else
            {
                challengerSkills[i].transform.localPosition = new Vector3(-1.17f + (i - 6) * 0.47f, -1.9f, 0);
            }
        }

        for (int i = 0; i < chatter_2.niggles; i++)
        {
            challengerNiggles.Add(Instantiate(nigglePrefab, challengerCard.transform));

            challengerNiggles[i].transform.localPosition = new Vector3(-1.17f + i * 0.47f, 1.4f, 0);
        }
    }

    private void SetCardSkills(Chatter champ, Challenger challenger)
    {
        //Delete old skills and niggles
        foreach (GameObject skill in champSkills)
        {
            Destroy(skill);
        }

        champSkills.Clear();

        foreach (GameObject niggle in champNiggles)
        {
            Destroy(niggle);
        }

        champNiggles.Clear();

        foreach (GameObject skill in challengerSkills)
        {
            Destroy(skill);
        }

        challengerSkills.Clear();

        foreach (GameObject niggle in challengerNiggles)
        {
            Destroy(niggle);
        }

        challengerNiggles.Clear();

        //Champ skills

        foreach (string skill in champ.skills)
        {
            if (skillPrefabs.ContainsKey(skill))
            {
                champSkills.Add(Instantiate(skillPrefabs[skill], champCard.transform));
            }

            else
            {
                champSkills.Add(Instantiate(placeholderPrefab, champCard.transform));
            }
        }

        for (int i = 0; i < champSkills.Count; i++)
        {
            if (i <= 5)
            {
                champSkills[i].transform.localPosition = new Vector3(-1.17f + i * 0.47f, -1.43f, 0);
            }

            else
            {
                champSkills[i].transform.localPosition = new Vector3(-1.17f + (i - 6) * 0.47f, -1.9f, 0);
            }
        }


        for (int i = 0; i < champ.niggles; i++)
        {
            champNiggles.Add(Instantiate(nigglePrefab, champCard.transform));

            champNiggles[i].transform.localPosition = new Vector3(-1.17f + i * 0.47f, 1.4f, 0);
        }




        //Challenger skills

        foreach (string skill in challenger.chatter.skills)
        {
            if (skillPrefabs.ContainsKey(skill))
            {
                challengerSkills.Add(Instantiate(skillPrefabs[skill], challengerCard.transform));
            }

            else
            {
                challengerSkills.Add(Instantiate(placeholderPrefab, challengerCard.transform));
            }
        }

        for (int i = 0; i < challengerSkills.Count; i++)
        {
            if (i <= 5)
            {
                challengerSkills[i].transform.localPosition = new Vector3(-1.17f + i * 0.47f, -1.43f, 0);
            }

            else
            {
                challengerSkills[i].transform.localPosition = new Vector3(-1.17f + (i - 6) * 0.47f, -1.9f, 0);
            }
        }

        for (int i = 0; i < challenger.chatter.niggles; i++)
        {
            challengerNiggles.Add(Instantiate(nigglePrefab, challengerCard.transform));

            challengerNiggles[i].transform.localPosition = new Vector3(-1.17f + i * 0.47f, 1.4f, 0);
        }
    }

    public void StopRound(string winner, string leftInjury, string rightInjury)
    {
        if (leftInjury == "d" || rightInjury == "d")
        {
            SoundHandler.PlayDeathSound();
        }

        if (leftInjury == "n" || rightInjury == "n" || leftInjury == "c" || rightInjury == "c")
        {
            SoundHandler.PlayInjurySound();
        }

        if (leftInjury == "k" || rightInjury == "k")
        {
            SoundHandler.PlayKOSound();
        }

        roundBackground.SetActive(false);

        fighting = false;

        if (dataHandler.ChaliceRound < 4)
        {
            int r_8_index = dataHandler.ChaliceRound * 2;

            if (winner.Contains("r") || winner.Contains("R"))
                r_8_index++;

            r_4s[dataHandler.ChaliceRound].text = r_8s[r_8_index].text;
        }

        else if (dataHandler.ChaliceRound < 6)
        {
            int r_4_index = (dataHandler.ChaliceRound - 4) * 2;

            if (winner.Contains("r") || winner.Contains("R"))
                r_4_index++;

            r_2s[dataHandler.ChaliceRound - 4].text = r_4s[r_4_index].text;
        }

        else if (dataHandler.ChaliceRound < 7)
        {
            int r_2_index = (dataHandler.ChaliceRound - 6) * 2;

            if (winner.Contains("r") || winner.Contains("R"))
                r_2_index++;

            r_1.text = r_2s[r_2_index].text;
        }
    }

    public void StopFight(string challengerInjury, string champInjury)
    {
        if (dataHandler.CurrentChamp == null)
        {
            return;
        }

        if (challengerInjury == "d" || champInjury == "d")
        {
            SoundHandler.PlayDeathSound();
        }

        if (challengerInjury == "n" || champInjury == "n" || challengerInjury == "c" || champInjury == "c")
        {
            SoundHandler.PlayInjurySound();
        }

        if (challengerInjury == "k" || champInjury == "k")
        {
            SoundHandler.PlayKOSound();
        }


        fighting = false;

        SetChampText();
    }

    public void SetChampText()
    {
        if (dataHandler.CurrentChamp == null)
        {
            return;
        }

        champName.text = dataHandler.CurrentChamp.name + " the champ";

        champSPP.text = dataHandler.CurrentChamp.spp.ToString();

        champAV.text = dataHandler.CurrentChamp.av.ToString() + "+";

        champDef.text = dataHandler.CurrentChamp.defences.ToString();

        //Delete old skills
        foreach (GameObject skill in champSkillsBottom)
        {
            Destroy(skill);
        }

        champSkillsBottom.Clear();

        foreach (string skillString in dataHandler.CurrentChamp.skills)
        {
            if (skillPrefabs.ContainsKey(skillString))
            {
                champSkillsBottom.Add(Instantiate(skillPrefabs[skillString], currentChampCard.transform));
            }

            else
            {
                champSkillsBottom.Add(Instantiate(placeholderPrefab, currentChampCard.transform));
            }
        }

        for (int i = 0; i < champSkillsBottom.Count; i++)
        {
            champSkillsBottom[i].transform.localPosition = new Vector3(-3.8f + i * 0.5f, -0.25f, 0);
        }

        //Add niggles
        foreach (GameObject niggle in champNigglesBottom)
        {
            Destroy(niggle);
        }

        champNigglesBottom.Clear();

        for (int i = 0; i < dataHandler.CurrentChamp.niggles; i++)
        {
            champNigglesBottom.Add(Instantiate(nigglePrefab, currentChampCard.transform));

            champNigglesBottom[i].transform.localPosition = new Vector3(-3.8f + (i + champSkillsBottom.Count) * 0.5f, -0.25f, 0);
        }
    }
}
