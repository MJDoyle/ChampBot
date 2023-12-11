using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengerListItem : MonoBehaviour
{
    [field: SerializeField]
    public Text NameText { get; private set; }

    [field: SerializeField]
    public Text DiceText { get; private set; }
}
