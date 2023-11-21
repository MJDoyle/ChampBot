using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    private Text challengerSkillsText;

    [SerializeField]
    private Image challengerImage;

    private List<GameObject> challengerSkills = new List<GameObject>();




    [SerializeField]
    private Text champNameText;

    [SerializeField]
    private Text champSPPText;

    [SerializeField]
    private Text champDefText;

    [SerializeField]
    private Text champAvText;

    [SerializeField]
    private Text champSkillsText;

    [SerializeField]
    private Image champImage;

    private List<GameObject> champSkills = new List<GameObject>();





    private Dictionary<string, GameObject> skillPrefabs = new Dictionary<string, GameObject>();



    [SerializeField]
    private GameObject blockPrefab;


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



    bool fighting = false;

    private DataHandler dataHandler;

    private TwitchClient twitchClient;

    // Start is called before the first frame update
    void Start()
    {
        dataHandler = GetComponent<DataHandler>();

        twitchClient = GetComponent<TwitchClient>();

        skillPrefabs["block"] = blockPrefab;

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

    private void Update()
    {
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

    public void StartFight()
    {
        //Get champ and next challenger details
        if (dataHandler.CurrentChamp == null)
        {
            twitchClient.SendChannelMessage("Can't fight, no champ!");

            return;
        }

        if (dataHandler.Challengers.Count <= 0)
        {
            twitchClient.SendChannelMessage("Can't fight, no challenger!");

            return;
        }

        Chatter champ = dataHandler.CurrentChamp;

        Challenger challenger = dataHandler.Challengers[0];


        if (challenger.numDice == 1)
            numDiceText.text = "ONE DICE";

        if (challenger.numDice == 1)
            numDiceText.text = "TWO DICE";

        if (challenger.numDice == 1)
            numDiceText.text = "THREE DICE";


        challengerNameText.text = challenger.chatter.name;

        challengerDefText.text = challenger.chatter.defences.ToString();

        challengerSPPText.text = challenger.chatter.spp.ToString();

        challengerAvText.text = challenger.chatter.av.ToString() + "+";

        //string challengerSkillString = "";

        //if (challenger.chatter.skills.Count > 0)
        //{

        //    foreach (string skill in challenger.chatter.skills)
        //    {
        //        challengerSkillString += skill + ", ";
        //    }

        //    challengerSkillString = challengerSkillString.Remove(challengerSkillString.Length - 2, 2);
        //}

        //challengerSkillsText.text = challengerSkillString;


        champNameText.text = champ.name;

        champDefText.text = champ.defences.ToString();

        champSPPText.text = champ.spp.ToString();

        champAvText.text = champ.av.ToString() + "+";

        //string champSkillString = "";

        //if (champ.skills.Count > 0)
        //{

        //    foreach (string skill in champ.skills)
        //    {
        //        champSkillString += skill + ", ";
        //    }

        //    champSkillString = champSkillString.Remove(champSkillString.Length - 2, 2);
        //}

        //champSkillsText.text = champSkillString;

        SetCardSkills(champ, challenger);


        //Load champ and challenger images

        if (File.Exists("GFX/" + champ.name + ".png"))
        {
            Texture2D champTexture = new Texture2D(2, 2);
            ImageConversion.LoadImage(champTexture, File.ReadAllBytes("GFX/" + champ.name + ".png"));

            champImage.sprite = Sprite.Create(champTexture, new Rect(0, 0, champTexture.width, champTexture.height), new Vector2(0.5f, 0.5f));
        }

        if (File.Exists("GFX/" + challenger.chatter.name + ".png"))
        {
            Texture2D challengerTexture = new Texture2D(2, 2);
            ImageConversion.LoadImage(challengerTexture, File.ReadAllBytes("GFX/" + challenger.chatter.name + ".png"));

            challengerImage.sprite = Sprite.Create(challengerTexture, new Rect(0, 0, challengerTexture.width, challengerTexture.height), new Vector2(0.5f, 0.5f));
        }



        fighting = true;
    }

    private void SetCardSkills(Chatter champ, Challenger challenger)
    {
        //Delete old skills
        foreach (GameObject skill in champSkills)
        {
            Destroy(skill);
        }

        champSkills.Clear();

        foreach (GameObject skill in challengerSkills)
        {
            Destroy(skill);
        }

        challengerSkills.Clear();

        //Champ skills

        string champSkillString = "";   //For skills that can't be found in the dictionary

        foreach (string skill in champ.skills)
        {
            if (skillPrefabs.ContainsKey(skill))
            {
                champSkills.Add(Instantiate(skillPrefabs[skill], champCard.transform));
            }

            else
            {
                champSkillString += skill + ", ";
            }
        }

        if (champSkillString.Length > 2)
            champSkillString = champSkillString.Remove(champSkillString.Length - 2, 2);

        champSkillsText.text = champSkillString;

        for (int i = 0; i < champSkills.Count; i++)
        {
            champSkills[i].transform.localPosition = new Vector3(-1.17f + (float)i * 0.5f, -1.45f, 0);
        }





        //Challenger skills

        string challengerSkillString = "";   //For skills that can't be found in the dictionary

        foreach (string skill in challenger.chatter.skills)
        {
            if (skillPrefabs.ContainsKey(skill))
            {
                challengerSkills.Add(Instantiate(skillPrefabs[skill], challengerCard.transform));
            }

            else
            {
                challengerSkillString += skill + ", ";
            }
        }

        if (challengerSkillString.Length > 2)
            challengerSkillString = challengerSkillString.Remove(challengerSkillString.Length - 2, 2);

        challengerSkillsText.text = challengerSkillString;

        for (int i = 0; i < challengerSkills.Count; i++)
        {
            challengerSkills[i].transform.localPosition = new Vector3(i, 0, 0);
        }




    }

    public void StopFight()
    {
        if (dataHandler.CurrentChamp == null)
        {
            return;
        }

        fighting = false;

        SetChampText();
    }

    private void SetChampText()
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
        }

        for (int i = 0; i < champSkillsBottom.Count; i++)
        {
            champSkillsBottom[i].transform.localPosition = new Vector3(-3.8f + (float)i * 0.5f, -0.25f, 0);
        }



    }







}
