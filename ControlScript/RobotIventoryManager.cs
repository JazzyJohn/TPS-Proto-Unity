using UnityEngine;
using System.Collections;

public class RobotIventoryManager : InventoryManager
{

    public override void Init()
    {
        if (owner == null)
        {
            owner = GetComponent<Pawn>();
            

            GenerateBag();
            GenerateInfo();

            
        }
    }
}
