using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Type of the unit's condition
public enum ConditionType
{
    Default,    // normal
    Silence,    // casting unable
    Sturn,      // action unable
}

public enum ItemType
{
    Normal,         // Reset if die
    Permanent       // Permanent apply until exit game
}

