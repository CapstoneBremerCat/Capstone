using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusObject", menuName = "ScriptableObjects/StatusObject", order = 0)]
public class StatusObject : ScriptableObject
{
    public int idCode;           // StatusObject id code

    public StatusData status;
}
