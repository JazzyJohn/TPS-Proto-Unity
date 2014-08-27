using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DispenserInvisibility : Building {

    //public Material invisibleMaterial;
    // Use this for initialization

    private List<Pawn> inHere = new List<Pawn>();

    void OnTriggerEnter(Collider other)
    {
        Pawn pawn = other.GetComponent<Pawn>();
        if (pawn != null && pawn.team ==team)
        {
            inHere.Add(pawn);
            if (pawn.foxView.isMine)
            {
                pawn.SetMaterial();
            }
            else
            {
                pawn.SetInvisible(true);
            }
        }
    }
    void OnTriggerLeave(Collider other)
    {
        Pawn pawn = other.GetComponent<Pawn>();
        if (pawn != null && pawn.team == team)
        {
            inHere.Remove(pawn);
            Leave(pawn);
        }
    }
    void OnDestroy()
    {
        foreach (Pawn pawn in inHere)
        {
            Leave(pawn);
        }
    }
    void Leave(Pawn pawn)
    {
        if (pawn.foxView.isMine)
        {
            pawn.SetNormalMaterial();
        }
        else
        {
            pawn.SetInvisible(false);
        }
    }
}
