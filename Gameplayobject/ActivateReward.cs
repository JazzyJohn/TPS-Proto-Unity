using UnityEngine;
using System.Collections;

public enum RewardState { NO_ACTIVE,ACTIVE,SELECTED };
public enum SpawnRewardState { BLOCK,TELEPORT,AIR_DROP}
public class ActivateReward : MonoBehaviour {

    public int cost;

    private bool isActive= true;   

    private bool selected;

    public AnnonceType type;

    public string annonceText;
  

    public virtual bool Select(Pawn pawn)
    {
        selected = true;
        return true;
    }
    public RewardState State()
    {
        if (isActive)
        {
            if(selected){
                return RewardState.SELECTED;
            }
            else
            {
                return RewardState.ACTIVE;
            }
        }
        else
        {
            return RewardState.NO_ACTIVE;
        }
    }

    public virtual void Deselect(Pawn pawn)
    {
        selected = false;
    }

    public virtual int Activate(Pawn pawn)
    {
       
        selected = false;
        return cost;
    }

    public virtual void TryOpen(int rating)
    {
        if (rating > cost )
        {
            if (!isActive)
            {
                PlayerMainGui.instance.Annonce(type, AnnonceAddType.NONE, annonceText);
            }
            isActive = true;
        }
        else
        {
            isActive = false;
        }
    }
    public virtual void Reset()
    {
       
    }
}
