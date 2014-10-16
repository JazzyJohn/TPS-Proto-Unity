using UnityEngine;
using System.Collections;

public class DeadGUI : MonoBehaviour {

    public UIPanel Panel;

    public UILabel NameKiller;
    public UILabel LvlKiller;
    public UILabel TimeToResp;

    public UITexture weaponTexture;

    public KillCamera KillCam;

    public void Activate()
    {
        if (Panel.alpha != 1f)
        {
            Panel.alpha = 1f;
            active = true;
            if (KillCam.killpalyer != null)
            {
                NameKiller.text = KillCam.killpalyer.PlayerName;
                weaponTexture.mainTexture = KillCam.killerweapon.HUDIcon;
            }
        }
    }

    public void DeActivate()
    {
        if (Panel.alpha != 0f)
        {
            Panel.alpha = 0f;
            active = false;
        }
    }

	// Use this for initialization
	void Start () 
    {
        StartCoroutine(FindCam());
	}

    IEnumerator FindCam()
    {
        if (Transform.FindObjectOfType<KillCamera>())
        {
            KillCam = Transform.FindObjectOfType<KillCamera>();
            DeActivate();
            yield return null;
        }
        else
        {
            yield return new WaitForEndOfFrame();
            StartCoroutine(FindCam());
        }
    }

	// Update is called once per frame
	void Update () 
    {
        TimeToResp.text = Mathf.Round(KillCam.killCamTime - KillCam.killCamTimer).ToString();
	}
}
