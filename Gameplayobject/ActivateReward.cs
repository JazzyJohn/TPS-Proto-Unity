using UnityEngine;
using System.Collections;

public enum RewardState { NO_ACTIVE,ACTIVE,SELECTED };
public enum SpawnRewardState { BLOCK,TELEPORT,AIR_DROP}
public class ActivateReward : MonoBehaviour {

    public int cost;

    private bool isActive= false;

    private bool used;

    private bool selected;

    public virtual void Select(Pawn pawn)
    {
        selected = true;
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

    public virtual void Activate(Pawn pawn)
    {
        used = true;
        selected = false;
        isActive = false;
    }

    public virtual void TryOpen(int rating)
    {
        if (rating > cost && !used)
        {
            isActive = true;
        }
    }
    public virtual void Reset()
    {
        used = false;
    }
}
