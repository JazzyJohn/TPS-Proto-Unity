//--------------------------------------------
//            NGUI: HUD Text
// Copyright © 2012 Tasharen Entertainment
//--------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// HUD text creates temporary on-screen text entries that are perfect for damage, effects, and messages.
/// </summary>

[AddComponentMenu("NGUI/Examples/HUD Text")]
public class HUDText : MonoBehaviour
{


	public enum TypeMessage {Texts, Sprites, Perfab};

	public class Entry
	{
		public TypeMessage Type;
		public UISprite Sprite;
        public UISprite ArrowSprite;
        public Transform ArrowTransform;
		public float time;			// Timestamp of when this entry was added
		public float stay = 0f;		// How long the text will appear to stay stationary on the screen
		public float offset = 0f;	// How far the object has moved based on time
		public float val = 0f;		// Optional value (used for damage)
        public bool constant = false;
		public UILabel label;		// Label on the game object
        public Vector3 startpos;
        public bool isShow= true;
        public bool withArrow = false;
        public string defSpriteName;
      
		public Transform Perfab;
		public bool OnTarget = false;
        
		public PerfabMessageInfo PerfabInfo;

		public float movementStart { get { return time + stay; } }
	}



	/// <summary>
	/// Sorting comparison function.
	/// </summary>

	static int Comparison (Entry a, Entry b)
	{
		if (a.movementStart < b.movementStart) return -1;
		if (a.movementStart > b.movementStart) return 1;
		return 0;
	}

	/// <summary>
	/// Font that will be used to create labels.
	/// </summary>

	public Camera gameCamera;
	public Camera uiCamera;
	Transform mTrans;

	void Awake () { mTrans = transform; }


	public UIFont font;

    public string[] arrowName;

	/// <summary>
	/// Effect applied to the text.
	/// </summary>

	public UILabel.Effect effect = UILabel.Effect.None;

	/// <summary>
	/// Curve used to move entries with time.
	/// </summary>

	public AnimationCurve offsetCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(3f, 40f) });

	/// <summary>
	/// Curve used to fade out entries with time.
	/// </summary>

	public AnimationCurve alphaCurve = new AnimationCurve(new Keyframe[] { new Keyframe(1f, 1f), new Keyframe(3f, 0f) });

	/// <summary>
	/// Curve used to scale the entries.
	/// </summary>

	public AnimationCurve scaleCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) });

	List<Entry> mList = new List<Entry>();
	List<Entry> mUnused = new List<Entry>();

	int counter = 0;

    public float speedUp = 1f;
	/// <summary>
	/// Whether some HUD text is visible.
	/// </summary>

	public bool isVisible { get { return mList.Count != 0; } }

	/// <summary>
	/// Create a new entry, reusing an old entry if necessary.
	/// </summary>
	/// 
	Entry Create (string namePerfab)
	{


		Entry ne = new Entry();
		ne.time = Time.realtimeSinceStartup;
		ne.Perfab = (Instantiate(Resources.Load<GameObject>("GUIPerfab/"+namePerfab))as GameObject).transform;

		ne.Perfab.parent = mTrans;
        
		ne.Perfab.name = counter.ToString();
		ne.PerfabInfo = ne.Perfab.GetComponent<PerfabMessageInfo>();
		ne.Type = TypeMessage.Perfab;
        ne.label = ne.PerfabInfo.Label;
        ne.label.keepCrispWhenShrunk = UILabel.Crispness.Never;
        ne.Sprite = ne.PerfabInfo.Sprite;
        ne.ArrowSprite = ne.PerfabInfo.ArrowSprite;
        if (ne.ArrowSprite != null)
        {
            ne.ArrowTransform = ne.ArrowSprite.transform.parent;
        }
		mList.Add(ne);
		++counter;
		return ne;
	}

	Entry Create (TypeMessage Type)
	{
		// See if an unused entry can be reused
		if (mUnused.Count > 0)
		{
			switch(mUnused[mUnused.Count - 1].Type)
			{
			case TypeMessage.Texts:
				Entry ent_Text = mUnused[mUnused.Count - 1];
				mUnused.RemoveAt(mUnused.Count - 1);
				ent_Text.time = Time.realtimeSinceStartup;
				ent_Text.label.depth = NGUITools.CalculateNextDepth(gameObject);
				NGUITools.SetActive(ent_Text.label.gameObject, true);
				ent_Text.offset = 0f;
				mList.Add(ent_Text);
				return ent_Text;
				break;
			case TypeMessage.Sprites:
				Entry ent_Sprite = mUnused[mUnused.Count - 1];
				mUnused.RemoveAt(mUnused.Count - 1);
				ent_Sprite.time = Time.realtimeSinceStartup;
				ent_Sprite.Sprite.depth = NGUITools.CalculateNextDepth(gameObject);
				NGUITools.SetActive(ent_Sprite.Sprite.gameObject, true);
				ent_Sprite.offset = 0f;
				mList.Add(ent_Sprite);
				return ent_Sprite;
				break;
			}
		}
		
		// New entry
		Entry ne = new Entry();
		ne.time = Time.realtimeSinceStartup;

		switch(Type)
		{
		case TypeMessage.Texts:
			ne.label = NGUITools.AddWidget<UILabel>(gameObject);
			ne.label.name = counter.ToString();
			ne.label.effectStyle = effect;
			ne.label.bitmapFont = font;
            ne.label.keepCrispWhenShrunk = UILabel.Crispness.Never;
			ne.Type = TypeMessage.Texts;

			ne.label.cachedTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			break;
		case TypeMessage.Sprites:
			ne.Sprite = NGUITools.AddWidget<UISprite>(gameObject);
			ne.Sprite.name = counter.ToString();
			ne.Type = TypeMessage.Sprites;

			ne.Sprite.cachedTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			break;
		}



		// Make it small so that it's invisible to start with

		mList.Add(ne);
		++counter;
		return ne;
	}

	/// <summary>
	/// Delete the specified entry, adding it to the unused list.
	/// </summary>

	public void Delete (Entry ent)
	{
		mList.Remove(ent);
		if(ent==null||ent.label==null||ent.label.gameObject==null){
			return;
		}
		switch(ent.Type)
		{
		case TypeMessage.Texts:
			mUnused.Add(ent);
			NGUITools.SetActive(ent.label.gameObject, false);
			break;
		case TypeMessage.Sprites:
			mUnused.Add(ent);
			NGUITools.SetActive(ent.Sprite.gameObject, false);
			break;
		case TypeMessage.Perfab:
			Destroy(ent.Perfab.gameObject);
			break;
		}
	}

	/// <summary>
	/// Add a new scrolling text entry.
	/// </summary>

	void Start()
	{
        gameCamera = Camera.mainCamera;
		gameCamera.gameObject.GetComponent<PlayerMainGui>().P1Hud = this;
	}



	
	public Entry Add (object obj, Color c,  Vector3 pos,bool constant)
	{
		if (!enabled) return null;
		
		float time = Time.realtimeSinceStartup;
	   float val = 0f;
		
		if (obj is float)
		{
			
			val = (float)obj;
		}
		else if (obj is int)
		{
			
			val = (int)obj;
		}
		
		/*if (isNumeric)
		{
			if (val == 0f) 
				return null;
			
			for (int i = mList.Count; i > 0; )
			{
				Entry ent = mList[--i];
				if (ent.time + 1f < time) continue;
				
				if (ent.val != 0f)
				{
					if (ent.val < 0f && val < 0f)
					{
						ent.val += val;
                        ent.label.transform.position = uiCamera.ViewportToWorldPoint(Camera.main.WorldToViewportPoint(pos));
                        ent.startpos = pos;
						pos = mTrans.localPosition;
						pos.x = Mathf.FloorToInt(pos.x);
						pos.y = Mathf.FloorToInt(pos.y);
						pos.z = 0f;
						mTrans.localPosition = pos;
						ent.label.text = Mathf.RoundToInt(ent.val).ToString();
						return null;
					}
					else if (ent.val > 0f && val > 0f)
					{
						ent.val += val;
                        ent.label.transform.position = uiCamera.ViewportToWorldPoint(Camera.main.WorldToViewportPoint(pos));
                        ent.startpos = pos;
						pos = mTrans.localPosition;
						pos.x = Mathf.FloorToInt(pos.x);
						pos.y = Mathf.FloorToInt(pos.y);
						pos.z = 0f;
						mTrans.localPosition = pos;
						ent.label.text = Mathf.RoundToInt(ent.val).ToString();
						return null;
					}
				}
			}
		}*/
		
		// Create a new entry
		Entry ne = Create(TypeMessage.Texts);
		ne.label.color = c;
        ne.label.transform.position = uiCamera.ViewportToWorldPoint(Camera.main.WorldToViewportPoint(pos));
        ne.startpos = pos;
        ne.constant = constant;
		pos = mTrans.localPosition;
		pos.x = Mathf.FloorToInt(pos.x);
		pos.y = Mathf.FloorToInt(pos.y);
		pos.z = 0f;
		mTrans.localPosition = pos;
		ne.val = val;
		
		
		//if (isNumeric) ne.label.text = (val < 0f ? Mathf.RoundToInt(ne.val).ToString() : "+" + Mathf.RoundToInt(ne.val));
        //    Debug.Log( (string)obj);
		ne.label.text = (string)obj;
		
		// Sort the list
		mList.Sort(Comparison);
        return ne;
	}

	public Entry Add (string AtlasPath, string NameSprite, Vector3 pos, bool constant)
	{
		if (!enabled) return null;
		
		float time = Time.realtimeSinceStartup;

		Entry ne = Create(TypeMessage.Sprites);
		ne.Sprite.transform.position = uiCamera.ViewportToWorldPoint(Camera.main.WorldToViewportPoint(pos));
		pos = mTrans.localPosition;
		pos.x = Mathf.FloorToInt(pos.x);
		pos.y = Mathf.FloorToInt(pos.y);
		pos.z = 0f;
		mTrans.localPosition = pos;
		ne.startpos = pos;
		ne.constant = constant;

		//ne.Sprite.atlas = Указать надо как-то атлас!
		ne.Sprite.spriteName = NameSprite;
		mList.Sort(Comparison);
		return ne;
	}


	public Entry Add (string NamePerfab, string NameSprite, string Text, Vector3 pos, bool constant)
	{
		float time = Time.realtimeSinceStartup;
		
		Entry ne = Create(NamePerfab);
		ne.Perfab.position = uiCamera.ViewportToWorldPoint(Camera.main.WorldToViewportPoint(pos));
		
		ne.startpos = pos;
        pos = mTrans.localPosition;
        pos.x = Mathf.FloorToInt(pos.x);
        pos.y = Mathf.FloorToInt(pos.y);
        pos.z = 0f;
        mTrans.localPosition = pos;
		ne.constant = constant;
        ne.Perfab.localRotation = Quaternion.identity;
        if (ne.PerfabInfo.OnSprite && NameSprite!="")
        {
            ne.PerfabInfo.Sprite.spriteName = NameSprite;
           
        }
        ne.defSpriteName = ne.PerfabInfo.Sprite.spriteName  ;
		if(ne.PerfabInfo.OnLabel)
			ne.PerfabInfo.Label.text = Text;

		mList.Sort(Comparison);
		return ne;
	}
	/// <summary>
	/// Disable all labels when this script gets disabled.
	/// </summary>

	void OnDisable ()
	{
		for (int i = mList.Count; i > 0; )
		{
			Entry ent = mList[--i];
			switch(ent.Type)
			{
			case TypeMessage.Texts:
				if (ent.label != null) ent.label.enabled = false;
				else mList.RemoveAt(i);
				break;
			case TypeMessage.Sprites:
				if (ent.Sprite != null) ent.Sprite.enabled = false;
				else mList.RemoveAt(i);
				break;
			case TypeMessage.Perfab:
				if (ent.PerfabInfo.OnSprite)
				{
					if(ent.PerfabInfo.Sprite != null) ent.PerfabInfo.Sprite.enabled = false;
					else
					{
						mList.RemoveAt(i);
						break;
					}
				}
				if (ent.PerfabInfo.OnLabel)
				{
					if(ent.PerfabInfo.Label != null) ent.PerfabInfo.Label.enabled = false;
					else
					{
						mList.RemoveAt(i);
						break;
					}
				}
				break;
			}
		}
	}

	/// <summary>
	/// Update the position of all labels, as well as update their size and alpha.
	/// </summary>

	public void ReDraw ()
	{
		float time = Time.realtimeSinceStartup;

		Keyframe[] offsets = offsetCurve.keys;
		Keyframe[] alphas = alphaCurve.keys;
		Keyframe[] scales = scaleCurve.keys;

		float offsetEnd = offsets[offsets.Length - 1].time;
		float alphaEnd = alphas[alphas.Length - 1].time;
		float scalesEnd = scales[scales.Length - 1].time;
		float totalEnd = Mathf.Max(scalesEnd, Mathf.Max(offsetEnd, alphaEnd));

		// Adjust alpha and delete old entries
		for (int i = mList.Count; i > 0; )
		{
			Entry ent = mList[--i];
			float currentTime = time - ent.movementStart;
            Vector3 pos = ent.startpos;
            //Debug.Log("vector" + Camera.main.WorldToViewportPoint(pos));
            Vector3 viewport = Camera.main.WorldToViewportPoint(pos);
			switch(ent.Type)
			{
			case TypeMessage.Texts:
                    if (ent.isShow && viewport.z > 0)
                {
                    ent.label.enabled = true;
                }
                else
                {
                    ent.label.enabled = false;
                }
                    ent.label.transform.position = uiCamera.ViewportToWorldPoint(viewport);
				if (!ent.constant)
				{
					ent.offset = offsetCurve.Evaluate(currentTime);
					ent.label.alpha = alphaCurve.Evaluate(currentTime);
				}
				float s_Text = scaleCurve.Evaluate(time - ent.time);
				if (s_Text < 0.001f) s_Text = 0.001f;
				ent.label.cachedTransform.localScale = new Vector3(s_Text, s_Text, s_Text);
               
				break;
			case TypeMessage.Sprites:
                if (ent.isShow && viewport.z > 0)
                {
                    if (ent.Sprite.spriteName != ent.defSpriteName)
                    {
                        ent.Sprite.spriteName = ent.defSpriteName;
                    }
                  
                    ent.Sprite.enabled = true;
                }
                else
                {
                    if (ent.isShow)
                    {
                        if (ent.isShow && viewport.x < 0.5)
                        {
                            if (ent.Sprite.spriteName != arrowName[0])
                            {
                                ent.Sprite.spriteName = arrowName[0];
                            }
                        }
                        else
                        {
                            if (ent.Sprite.spriteName != arrowName[1])
                            {
                                ent.Sprite.spriteName = arrowName[1];
                            }
                        }
                        viewport = NormalizeOffScreen(viewport);

                        ent.Sprite.enabled = true;
                    }
                    else
                    {
                        ent.Sprite.enabled = false;
                    }

                }
                ent.Sprite.transform.position = uiCamera.ViewportToWorldPoint(viewport);
				if (!ent.constant)
				{
					ent.offset = offsetCurve.Evaluate(currentTime);
					ent.Sprite.alpha = alphaCurve.Evaluate(currentTime);
				}
				float s_Sprite = scaleCurve.Evaluate(time - ent.time);
				if (s_Sprite < 0.001f) s_Sprite = 0.001f;
				ent.Sprite.cachedTransform.localScale = new Vector3(s_Sprite, s_Sprite, s_Sprite);
             
				break;
			case TypeMessage.Perfab:
            
				if (ent.isShow && (!IsOffscreen(viewport) || ent.OnTarget))
                {
                    if (ent.Sprite.spriteName != ent.defSpriteName)
                    {
                        ent.Sprite.spriteName = ent.defSpriteName;
                    }
					if(ent.PerfabInfo.OnSprite)
                    	ent.Sprite.enabled = true;
					if(ent.PerfabInfo.Label)
                    	ent.label.enabled = true;
                    if (ent.PerfabInfo.OnProgressBar)
                    {
                        ent.PerfabInfo.Foreground.enabled = true;
                    }
                    if (ent.ArrowSprite != null)
                    {
                        ent.ArrowSprite.enabled = false;
                    }
                 
                }
                else
                {
                   
                    if (ent.isShow && ent.withArrow)
                    {

                        if (ent.ArrowSprite != null)
                        {
                            ent.ArrowSprite.enabled = true;
                            viewport = NormalizeOffScreen(viewport,ent.ArrowTransform);
                        }
                        else
                        {
                            viewport = NormalizeOffScreen(viewport);
                        }
                      
                     
                        ent.Sprite.enabled = true;
                    }
					else
                    {
						if (ent.PerfabInfo.OnSprite)
						{
                        	ent.Sprite.enabled = false;
							if(ent.PerfabInfo.OnProgressBar)
								ent.PerfabInfo.Foreground.enabled = false;
						}
                        if (ent.PerfabInfo.OnLabel)
                            ent.label.enabled = false;
                    }
					
                }
                ent.Perfab.position = uiCamera.ViewportToWorldPoint(viewport);
				if (!ent.constant)
				{
					ent.offset = offsetCurve.Evaluate(currentTime);
					if(ent.PerfabInfo.OnLabel)
						ent.PerfabInfo.Label.alpha = alphaCurve.Evaluate(currentTime);
					if(ent.PerfabInfo.OnSprite)
						ent.PerfabInfo.Sprite.alpha = alphaCurve.Evaluate(currentTime);
				}
				float s_Perfab = scaleCurve.Evaluate(time - ent.time);
				if (s_Perfab < 0.001f) s_Perfab = 0.001f;
				ent.Perfab.localScale = new Vector3(s_Perfab, s_Perfab, s_Perfab);
               
				break;
			}
           
            pos = mTrans.localPosition;
            pos.x = Mathf.FloorToInt(pos.x);
            pos.y = Mathf.FloorToInt(pos.y);
            pos.z = 0f;
            mTrans.localPosition = pos;		
			
		}

		

		// Move the entries
		for (int i = mList.Count; i > 0; )
		{
			Entry ent = mList[--i];
            if (!ent.constant)
            {
                ent.startpos += new Vector3(0f, speedUp * Time.deltaTime, 0f);
            }
			
		}
	}
    private bool IsOffscreen(Vector3 viewport)
    {
        return( viewport.x < 0f || viewport.x > 1f || viewport.y < 0f || viewport.y > 1f|| viewport.z<0);

    }
    private Vector3 NormalizeOffScreen(Vector3 viewport, Transform transform=null)
    {
        if (viewport.z > 0)
        {
            if (viewport.x > 0.9)
            {
                viewport.x = 0.9f;
                if (transform != null)
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            }
            if (viewport.x < 0.1)
            {
                viewport.x = 0.1f;
                if (transform != null)
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
            }
            if (viewport.y > 0.9)
            {
                viewport.y = 0.9f;
                if (transform != null)
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
            }
            if (viewport.y < 0.1)
            {
                viewport.y = 0.1f;
                if (transform != null)
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }
        else
        {
            if (viewport.y > 0 && viewport.y<1)
            {
                if (viewport.x > 0.5)
                {
                    viewport.x = 0.1f;
                    if (transform != null)
                        transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                }
                else
                {
                    viewport.x = 0.9f;
                    if (transform != null)
                        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                }

            }
            else
            {
                viewport.x = Mathf.Clamp(viewport.x, 0.1f, 0.9f);
                if (viewport.y > 0.5)
                {
                    viewport.y = 0.1f;
                    if (transform != null)
                        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }
                else
                {
                    viewport.y = 0.9f;
                    if (transform != null)
                        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                }
            }
        }
        viewport.z = 10.0f;
        return viewport;
    }
}
