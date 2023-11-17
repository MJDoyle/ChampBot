using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;


public class Chatter
{
    public string name;

    public List<string> skills;

    public int spp;

    public int defences;

    public int av;
}

public struct Challenger
{
    public Chatter chatter;

    public int numDice;
}

public class DataHandler : MonoBehaviour
{


    private List<int> sppRequirements;


    public Dictionary<string, Chatter> Chatters { get; private set; }

    public Chatter CurrentChamp { get; private set; }

    public List<Challenger> Challengers { get; private set; }




    private GoogleCredential gCredential;

    private SheetsService sheetsService;




    private void Start()
    {
        //LoadGoogleCredentials();

        //LoadDataFromGoogleSheet();

        Chatters = new Dictionary<string, Chatter>();

        Challengers = new List<Challenger>();

        LoadData();

        sppRequirements = new List<int>();

        sppRequirements.Add(6);
        sppRequirements.Add(8);
        sppRequirements.Add(12);
        sppRequirements.Add(16);
        sppRequirements.Add(20);
        sppRequirements.Add(24);
        sppRequirements.Add(30);
        sppRequirements.Add(35);
        sppRequirements.Add(40);
    }


    private void LoadGoogleCredentials()
    {
        using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        {
            gCredential = GoogleCredential.FromStream(stream).CreateScoped(new string[] { SheetsService.Scope.Spreadsheets });
        }

        sheetsService = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = gCredential,
            ApplicationName = "ChampBot"        
        });
    }

    private async void LoadDataFromGoogleSheet()
    {
        Debug.Log("LOADING FROM SHEET");

        SpreadsheetsResource.ValuesResource.GetRequest getRequest = sheetsService.Spreadsheets.Values.Get("Sheet1!A1:B", "1pUEtbw1bLeNDNBusLJiTsK6p9Ew4XqurXDxydNtPS5s");

        var getResponse = await getRequest.ExecuteAsync();

        IList<IList<object>> values = getResponse.Values;

        if (values != null && values.Count > 0)
        {
            foreach (var row in values)
            {
                Debug.Log(row[0]);
                Debug.Log(row[1]);
            }
        }
    }


    private void LoadData()
    {
        Debug.Log("Loading from file");

        //READ ALL CHATTERS

        StreamReader chatterReader = new StreamReader("chatters.txt");

        string line;
        while ((line = chatterReader.ReadLine()) != null)
        {
            string[] chatterStringElements;

            chatterStringElements = line.Split(',');

            if (chatterStringElements.Length >= 4)
            {
                Chatter chatter = new Chatter()
                {
                    name = chatterStringElements[0].ToLower(),
                    defences = int.Parse(chatterStringElements[1]),
                    spp = int.Parse(chatterStringElements[2]),
                    av = int.Parse(chatterStringElements[3]),
                    skills = new List<string>()
                };


                for (int i = 4; i < chatterStringElements.Length; i++)
                {
                    chatter.skills.Add(chatterStringElements[i].ToLower());
                }

                Chatters[chatterStringElements[0].ToLower()] = chatter;

            }

            else
            {
                //Error
            }
        }

        chatterReader.Close();




        //READ CURRENT CHAMP

        StreamReader champReader = new StreamReader("currentChamp.txt");

        //TODO - what about name changes? Allow champs (and chatters and challengers) to be set manually

        string champ = champReader.ReadLine();

        if (Chatters.ContainsKey(champ.ToLower()))
        {
            CurrentChamp = Chatters[champ.ToLower()];
        }

        else
        {
            //Error
        }


        champReader.Close();



        //READ CHALLENGERS

        StreamReader challengerReader = new StreamReader("currentChallengers.txt");

        //TODO - what about name changes? Allow champs (and chatters and challengers) to be set manually

        while ((line = challengerReader.ReadLine()) != null)
        {
            string[] challengerStringElements;

            challengerStringElements = line.Split(',');

            if (challengerStringElements.Length == 2)
            {
                string name = challengerStringElements[0].ToLower();

                int numDice = int.Parse(challengerStringElements[1]);

                AddChallenger(name, numDice, false);
            }

            else
            {
            }
        }

        challengerReader.Close();
    }



    private void SaveData()
    {
        Debug.Log("Saving to file");

        //SAVE CHATTERS
        StreamWriter chatterWriter = new StreamWriter("chatters.txt", false);

        foreach (KeyValuePair<string, Chatter> chatterPair in Chatters)
        {
            string stringToWrite = "";

            stringToWrite += chatterPair.Value.name + ",";
            stringToWrite += chatterPair.Value.defences + ",";
            stringToWrite += chatterPair.Value.spp + ",";
            stringToWrite += chatterPair.Value.av + ",";

            foreach (string skill in chatterPair.Value.skills)
            {
                stringToWrite += skill + ",";
            }

            stringToWrite = stringToWrite.Remove(stringToWrite.Length - 1, 1);

            chatterWriter.WriteLine(stringToWrite);
        }        

        chatterWriter.Close();



        //SAVE CHAMP



        StreamWriter champWriter = new StreamWriter("currentChamp.txt", false);

        if (CurrentChamp != null)
            champWriter.WriteLine(CurrentChamp.name);

        champWriter.Close();

        //SAVE CHALLENGERS

        StreamWriter challengerWriter = new StreamWriter("currentChallengers.txt", false);

        foreach (Challenger challenger in Challengers)
        {
            challengerWriter.WriteLine(challenger.chatter.name + "," + challenger.numDice.ToString());
        }

        challengerWriter.Close();
    }

    public List<Chatter> GetTop5()
    {
        List<Chatter> top5 = new List<Chatter>();

        while (top5.Count < 5 && top5.Count < Chatters.Count)
        {
            int biggestDefs = 0;
            Chatter bdChatter = Chatters.First().Value;

            foreach (KeyValuePair<string, Chatter> chatterPair in Chatters)
            {
                if (chatterPair.Value.defences > biggestDefs && !top5.Contains(chatterPair.Value))
                {
                    biggestDefs = chatterPair.Value.defences;
                    bdChatter = chatterPair.Value;
                }
            }

            top5.Add(bdChatter);
        }

        return top5;
    }

    public Chatter GetChatter(string name)
    {
        if (!Chatters.ContainsKey(name.ToLower()))
            return null;

        return Chatters[name.ToLower()];
    }

    public bool AddAv(string chatter)
    {
        if (!Chatters.ContainsKey(chatter.ToLower()))
            return false;

        if (Chatters[chatter.ToLower()].spp < sppRequirements[Chatters[chatter.ToLower()].skills.Count])
            return false;

        Chatters[chatter.ToLower()].spp -= sppRequirements[Chatters[chatter.ToLower()].skills.Count];

        Chatters[chatter.ToLower()].av++;

        SaveData();

        return true;
    }

    public bool AddFreeSkill(string chatter, string skill)
    {
        if (!Chatters.ContainsKey(chatter.ToLower()))
            return false;


        List<string> existingSkills = Chatters[chatter.ToLower()].skills;

        if (existingSkills.Contains(skill.ToLower()))
            return false;

        existingSkills.Add(skill.ToLower());

        SaveData();

        return true;

    }

    public bool AddSkill(string chatter, string skill)
    {
        if (!Chatters.ContainsKey(chatter.ToLower()))
            return false;


        if (Chatters[chatter.ToLower()].spp < sppRequirements[Chatters[chatter.ToLower()].skills.Count])
            return false;


        List<string> existingSkills = Chatters[chatter.ToLower()].skills;

        if (existingSkills.Contains(skill.ToLower()))
            return false;


        Chatters[chatter.ToLower()].spp -= sppRequirements[Chatters[chatter.ToLower()].skills.Count];

        existingSkills.Add(skill.ToLower());

        SaveData();

        return true;

    }

    public void SetChatter(string name, int def, int spp, int av)
    {
        if (!Chatters.ContainsKey(name.ToLower()))
            Chatters[name.ToLower()] = new Chatter();

        Chatters[name.ToLower()].defences = def;
        Chatters[name.ToLower()].spp = spp;
        Chatters[name.ToLower()].av = av;

        Chatters[name.ToLower()].skills = new List<string>();

        SaveData();
    }


    public void AddChallenger(string challengerName, int numDice, bool save = true)
    {
        if (!Chatters.ContainsKey(challengerName.ToLower()))
        {
            Chatters[challengerName.ToLower()] = new Chatter()
            {
                name = challengerName.ToLower(),
                spp = 0,
                defences = 0,
                skills = new List<string>(),
                av = 8
            };
        }

        Challenger challenger = new Challenger()
        {
            chatter = Chatters[challengerName.ToLower()],
            numDice = numDice
        };

        Challengers.Add(challenger);

        if (save)
            SaveData();
    }

    public void ChampWins(int spp)
    {
        if (Challengers.Count <= 0)
        {
            //Error
            return;
        }

        //Add spp
        Challengers[0].chatter.spp += spp;

        //Add defense to champ
        CurrentChamp.defences++;

        //Remove latest challenger from list
        Challengers.RemoveAt(0);

        SaveData();

    }

    public void ChampLoses(int spp)
    {
        if (Challengers.Count <= 0)
        {
            //Error
            return;
        }

        //Add spp
        Challengers[0].chatter.spp += spp;

        //Set the latest challenger as the champ
        CurrentChamp = Challengers[0].chatter;

        //Remove latest challenger from list
        Challengers.RemoveAt(0);

        SaveData();
    }


    public bool SetChamp(string champ)
    {
        if (!Chatters.ContainsKey(champ.ToLower()))
            return false;

        CurrentChamp = Chatters[champ.ToLower()];

        SaveData();

        return true;
    }

    private void Update()
    {
        //If the next challenger in line is the champ, remove it
        if (Challengers.Count > 0 && CurrentChamp != null && Challengers[0].chatter == CurrentChamp)
        {
            Debug.Log("test1");

            Challengers.RemoveAt(0);

            SaveData();
        }
    }
}
