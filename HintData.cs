using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintData
{
    public string hintName = null;

    public List<SubHintData> subHintData = new List<SubHintData>();

    public bool active = false;

    public bool completed = false;
}
