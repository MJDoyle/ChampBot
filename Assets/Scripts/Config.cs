using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Config
{
    public static int DeathSPP { get; set; } 

    public static int CasSPP { get; set; }

    public static readonly Dictionary<string, int> SppDict = new Dictionary<string, int> { {string.Empty, 0 }, { "b", 1 }, {"p", 2 }, {"s", 3 }, {"k", 4 }, {"c", 6 }, {"n", 6 }, {"d", 12 } };


}
