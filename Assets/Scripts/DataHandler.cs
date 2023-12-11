using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class Chatter
{
    public string name;

    public List<string> skills;

    public int spp;

    public int defences;

    public int av;

    public int niggles;
}

public struct Challenger
{
    public Chatter chatter;

    public int numDice;
}

public class DataHandler : MonoBehaviour
{
    private List<int> sppRequirements = new List<int>();

    public List<string> PossibleSkills { get; private set; } = new List<string>(); 

    public Dictionary<string, Chatter> Chatters { get; private set; }

    public Chatter CurrentChamp { get; private set; }

    public List<Challenger> Challengers { get; private set; }

    private UIHandler uiHandler;




    private void Start()
    {
        uiHandler = GetComponent<UIHandler>();

        //LoadGoogleCredentials();

        //LoadDataFromGoogleSheet();

        Chatters = new Dictionary<string, Chatter>();

        Challengers = new List<Challenger>();

        LoadData();
    }

    private void ClearData()
    {
        //Chatters
        Chatters.Clear();

        //Challengers
        Challengers.Clear();

        //Current champ
        CurrentChamp = null;

        //Spp
        sppRequirements.Clear();

    }

    public void LoadData()
    {
        ClearData();

        Debug.Log("Loading from file");

        //READ ALL CHATTERS

        StreamReader chatterReader = new StreamReader("Data/chatters.txt");

        string line;
        while ((line = chatterReader.ReadLine()) != null)
        {
            string[] chatterStringElements;

            chatterStringElements = line.Split(',');

            if (chatterStringElements.Length >= 5)
            {
                Chatter chatter = new Chatter()
                {
                    name = chatterStringElements[0].ToLower(),
                    defences = int.Parse(chatterStringElements[1]),
                    spp = int.Parse(chatterStringElements[2]),
                    av = int.Parse(chatterStringElements[3]),
                    niggles = int.Parse(chatterStringElements[4]),
                    skills = new List<string>()
                };


                for (int i = 5; i < chatterStringElements.Length; i++)
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

        StreamReader champReader = new StreamReader("Data/currentChamp.txt");

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

        StreamReader challengerReader = new StreamReader("Data/currentChallengers.txt");

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

        //READ SPP

        StreamReader sppReader = new StreamReader("Config/spp.txt");

        while ((line = sppReader.ReadLine()) != null)
        {
            if (int.TryParse(line, out int spp))
                sppRequirements.Add(spp);
        }


        sppReader.Close();


        //READ SKILLS

        StreamReader skillReader = new StreamReader("Config/skills.txt");

        while ((line = skillReader.ReadLine()) != null)
        {
            PossibleSkills.Add(line.ToLower());
        }

        skillReader.Close();

        uiHandler.SetChallengerListItems();

        uiHandler.ShowChallengerListItems();
    }



    private void SaveData(string path)
    {
        Debug.Log("Saving to file");

        //SAVE CHATTERS
        StreamWriter chatterWriter = new StreamWriter(path + "chatters.txt", false);

        foreach (KeyValuePair<string, Chatter> chatterPair in Chatters)
        {
            string stringToWrite = "";

            stringToWrite += chatterPair.Value.name + ",";
            stringToWrite += chatterPair.Value.defences + ",";
            stringToWrite += chatterPair.Value.spp + ",";
            stringToWrite += chatterPair.Value.av + ",";
            stringToWrite += chatterPair.Value.niggles + ",";

            foreach (string skill in chatterPair.Value.skills)
            {
                stringToWrite += skill + ",";
            }

            stringToWrite = stringToWrite.Remove(stringToWrite.Length - 1, 1);

            chatterWriter.WriteLine(stringToWrite);
        }        

        chatterWriter.Close();



        //SAVE CHAMP



        StreamWriter champWriter = new StreamWriter(path + "currentChamp.txt", false);

        if (CurrentChamp != null)
            champWriter.WriteLine(CurrentChamp.name);

        champWriter.Close();

        //SAVE CHALLENGERS

        StreamWriter challengerWriter = new StreamWriter(path + "currentChallengers.txt", false);

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

    public bool SetSPP(string chatter, int spp)
    {
        if (!Chatters.ContainsKey(chatter.ToLower()))
            return false;

        if (spp < 0)
            return false;

        Chatters[chatter.ToLower()].spp = spp;

        return true;
    }

    public bool AddAv(string chatter)
    {
        if (!Chatters.ContainsKey(chatter.ToLower()))
            return false;

        if (Chatters[chatter.ToLower()].spp < sppRequirements[Chatters[chatter.ToLower()].skills.Count])
            return false;

        Chatters[chatter.ToLower()].spp -= sppRequirements[Chatters[chatter.ToLower()].skills.Count];

        Chatters[chatter.ToLower()].av++;

        SaveData("Data/");
        SaveData("Backup/");

        return true;
    }

    public bool AddFreeSkill(string chatter, string skill)
    {
        if (!Chatters.ContainsKey(chatter.ToLower()))
            return false;

        if (!PossibleSkills.Contains(skill.ToLower()))
            return false;


        List<string> existingSkills = Chatters[chatter.ToLower()].skills;

        if (existingSkills.Contains(skill.ToLower()))
            return false;

        existingSkills.Add(skill.ToLower());

        SaveData("Data/");
        SaveData("Backup/");

        return true;

    }

    public bool AddNiggles(string chatter, int numNiggles)
    {
        if (!Chatters.ContainsKey(chatter))
            return false;

        Chatters[chatter].niggles += numNiggles;

        Chatters[chatter].niggles = Mathf.Max(Chatters[chatter].niggles, 0);

        return true;
    }

    public bool AddSkill(string chatter, string skill)
    {
        if (!Chatters.ContainsKey(chatter.ToLower()))
            return false;


        if (Chatters[chatter.ToLower()].skills.Count >= sppRequirements.Count)
            return false;


        if (Chatters[chatter.ToLower()].spp < sppRequirements[Chatters[chatter.ToLower()].skills.Count])
            return false;

        if (!PossibleSkills.Contains(skill.ToLower()))
            return false;


        List<string> existingSkills = Chatters[chatter.ToLower()].skills;

        if (existingSkills.Contains(skill.ToLower()))
            return false;


        Chatters[chatter.ToLower()].spp -= sppRequirements[Chatters[chatter.ToLower()].skills.Count];

        existingSkills.Add(skill.ToLower());

        SaveData("Data/");
        SaveData("Backup/");

        return true;

    }

    public void SetChatter(string name, int def, int spp, int av, int niggles)
    {
        if (!Chatters.ContainsKey(name.ToLower()))
            Chatters[name.ToLower()] = new Chatter();

        if (def < 0)
            return;

        if (spp < 0)
            return;

        if (av < 0)
            return;

        Chatters[name.ToLower()].defences = def;
        Chatters[name.ToLower()].spp = spp;
        Chatters[name.ToLower()].av = av;
        Chatters[name.ToLower()].niggles = niggles;

        Chatters[name.ToLower()].skills = new List<string>();

        SaveData("Data/");
        SaveData("Backup/");
    }


    public void AddChallenger(string challengerName, int numDice, bool save = true)
    {
        if (numDice < 1 || numDice > 3)
            return;

        if (!Chatters.ContainsKey(challengerName.ToLower()))
        {
            Chatters[challengerName.ToLower()] = new Chatter()
            {
                name = challengerName.ToLower(),
                spp = 0,
                defences = 0,
                skills = new List<string>(),
                av = 8,
                niggles = 0
            };
        }

        Challenger challenger = new Challenger()
        {
            chatter = Chatters[challengerName.ToLower()],
            numDice = numDice
        };

        Challengers.Add(challenger);

        if (save)
        {
            SaveData("Data/");
            SaveData("Backup/");
        }
    }

    private void ResetChatter(string chatterName)
    {
        if (!Chatters.ContainsKey(chatterName))
            return;

        Chatters[chatterName].av = 8;
        Chatters[chatterName].defences = 0;
        Chatters[chatterName].niggles = 0;
        Chatters[chatterName].skills.Clear();
        Chatters[chatterName].spp = 0;
    }

    public void ChampWins(int spp, string challengerInjury, string champInjury)
    {
        if (spp < 0)
            return;

        if (Challengers.Count <= 0)
        {
            //Error
            return;
        }

        //CHALLENGER

        //If dead
        if (challengerInjury.Contains("dea"))
        {
            ResetChatter(Challengers[0].chatter.name);

            CurrentChamp.spp += 3;
        }

        else
        {
            //If niggled
            if (challengerInjury.Contains("gl"))
                Challengers[0].chatter.niggles++;

            //Add spp
            Challengers[0].chatter.spp += spp;
        }

        //Remove latest challenger from list
        Challengers.RemoveAt(0);

        //CHAMP

        //Add defense to champ
        CurrentChamp.defences++;

        //Add niggle to champ if niggled
        if (champInjury.Contains("gl"))
            CurrentChamp.niggles++;

        SaveData("Data/");
        SaveData("Backup/");

    }

    public void ChallengerWins(int spp, string challengerInjury, string champInjury)
    {
        if (spp < 0)
            return;

        if (Challengers.Count <= 0)
        {
            //Error
            return;
        }

        //CHAMP

        //Add niggle to champ if niggled
        if (champInjury.Contains("gl"))
            CurrentChamp.niggles++;

        //Kill champ if dead
        else if (champInjury.Contains("dea"))
            ResetChatter(CurrentChamp.name);

        //CHALLENGER

        //If niggled
        if (challengerInjury.Contains("gl"))
            Challengers[0].chatter.niggles++;

        //Add spp
        Challengers[0].chatter.spp += spp;

        //Set the latest challenger as the champ
        CurrentChamp = Challengers[0].chatter;

        //Remove latest challenger from list
        Challengers.RemoveAt(0);

        SaveData("Data/");
        SaveData("Backup/");
    }

    public void DimmyWins()
    {
        if (Challengers.Count <= 0)
        {
            //Error
            return;
        }

        ResetChatter(CurrentChamp.name);

        ResetChatter(Challengers[0].chatter.name);

        if (!Chatters.ContainsKey("dimmy_gee"))
        {
            Chatters["dimmy_gee"] = new Chatter()
            {
                name = "dimmy_gee",
                spp = 0,
                defences = 0,
                skills = new List<string>(),
                av = 8,
                niggles = 0
            };
        }

        SetChamp("dimmy_gee");

    }

    public bool SetChamp(string champ)
    {
        if (!Chatters.ContainsKey(champ.ToLower()))
            return false;

        CurrentChamp = Chatters[champ.ToLower()];

        SaveData("Data/");
        SaveData("Backup/");

        return true;
    }

    private void Update()
    {
        //If the next challenger in line is the champ, swap with the next other challenger in the queue
        if (Challengers.Count > 0 && CurrentChamp != null && Challengers[0].chatter == CurrentChamp)
        {
            //Find differnt challenger that's closest to the start

            for (int i = 0; i < Challengers.Count; i++)
            {
                if (Challengers[i].chatter != Challengers[0].chatter)
                {
                    Challenger replacement = Challengers[i];

                    Challengers[i] = Challengers[0];

                    Challengers[0] = replacement;

                    SaveData("Data/");
                    SaveData("Backup/");

                    break;
                }
            }
        }
    }
}
