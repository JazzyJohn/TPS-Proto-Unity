using UnityEngine;
using System.Collections;

public class HoldGameFinished : GameFinished
{

	

    protected override void WinnerText()
    {
        winner.text =  GameRule.instance.Winner()  +  TextGenerator.instance.GetSimpleText("WaveCnt");
    }
}
