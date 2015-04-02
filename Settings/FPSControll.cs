using UnityEngine;
using System.Collections;


public class FPSControll : MonoBehaviour{
	//minimal framerate (if current FPS is lower, quality should decrease immediately)
	public float minAcceptableFramerate = 30;
	//current quality (as text, visible in inspector)
	public string currentQuality;
	//current framerate (calculated while component is running)
	public float currentFramerate;
	//disable component if user changed quality manually (for example in menu)
	public bool forceBestQualityOnStart=false;
	public bool disableAfterManualQualityChange=false;
	public bool disabled=false;
	public bool verbose=false;
	//how many times per second framerate should be checked
	public float updateRate = 1.0f;  // how much updates per second.
	//Guard avoiding changing quality backwards and forwards
	//If threshold is set to X, it means that quality won't increase until framerate
	//will be higher than minAcceptableFramerate+X
	public float threshold = 5;
	//current quality number
	private int currQuality;
	private float minThreshold;
	private float maxThreshold;
	private int previousQuality=-1;
	private int frameCount = 0;
	private float nextUpdate = 0.0f;
	private bool ignoreOneIteration=true;
	private bool testIteration=false;

    public static FPSControll instance;

	public void afterQualityChange(){
        NGUI_setting.setQuality(QualitySettings.GetQualityLevel());
      
		//Does nothing by default. 
		//If you have menu allowing user to choose quality, you can set it's active value value here.
	}
	
    public void Awake(){
        instance = this;
    }
    public bool dontCount = true;

    public void SetDelay(float delay)
    {
        nextUpdate = Time.realtimeSinceStartup + 1.0f / updateRate + delay;
    }
	public void Start () {
        
		minThreshold=threshold;
		if(forceBestQualityOnStart){
			QualitySettings.SetQualityLevel(QualitySettings.names.Length-1);
			currQuality = QualitySettings.GetQualityLevel();
			currentQuality=""+currQuality+" ("+QualitySettings.names[currQuality]+")";
			if(verbose)Debug.Log("Quality on start: "+currentQuality);
		}else{
			aproxQuality();
		}
		restartComponent();
		nextUpdate = Time.realtimeSinceStartup + 1.0f/updateRate;
	}
	
	private void aproxQuality(){ // simplified function from Bootcamp demo
		var fillrate = SystemInfo.graphicsPixelFillrate;
		var shaderLevel = SystemInfo.graphicsShaderLevel;
		var videoMemory = SystemInfo.graphicsMemorySize;
		var processors = SystemInfo.processorCount;
		if (fillrate < 0){
			if (shaderLevel < 10) fillrate = 1000;
			else if (shaderLevel < 20) fillrate = 1300;
			else if (shaderLevel < 30) fillrate = 2000;
			else fillrate = 3000;
			if (processors >= 6) 	fillrate *= 3;
			else if (processors >= 3) fillrate *= 2;
			if (videoMemory >= 512) 	fillrate *= 2;
			else if (videoMemory <= 128) fillrate /= 2;
		}
		float fillneed  = (Screen.width*Screen.height + 400*300) * (minAcceptableFramerate / 1000000.0f);
		float[] levelmult  =new float[]{5.0f, 30.0f, 80.0f, 130.0f, 200.0f, 320.0f};
		int level = 0;
		while ((level < QualitySettings.names.Length-1) && fillrate > fillneed * levelmult[level+1]) ++level;
		QualitySettings.SetQualityLevel(level);
		currQuality = QualitySettings.GetQualityLevel();
		currentQuality=""+currQuality+" ("+QualitySettings.names[currQuality]+")";
		if(verbose)Debug.Log("Quality on start: "+currentQuality);
	}
	
	public void restartComponent(){
		threshold=minThreshold;
		maxThreshold=minThreshold;
		currQuality = QualitySettings.GetQualityLevel();
	}
	
	public void  Update () {

     
        if (dontCount)
        {
            nextUpdate = Time.realtimeSinceStartup + 1.0f / updateRate;
            return;
        }
		frameCount++;
	    if (Time.realtimeSinceStartup > nextUpdate){
	    	nextUpdate = Time.realtimeSinceStartup + 1.0f/updateRate;
	        currentFramerate = frameCount * updateRate;
	        frameCount = 0;
          

	    	if(threshold>minThreshold)threshold--;
	    	if(currQuality != QualitySettings.GetQualityLevel()){
	    		currQuality = QualitySettings.GetQualityLevel();
	    		currentQuality=""+currQuality+" ("+QualitySettings.names[currQuality]+")";
	    		if(disableAfterManualQualityChange){
	    			disabled=true;
	    			return;
	    		}
	    	}
	    	if(disabled){
				ignoreOneIteration=true;
				return;
			}
	    	currQuality = QualitySettings.GetQualityLevel();
	        if(ignoreOneIteration){
	        	ignoreOneIteration=false;
	        	return;
	        }
	        if(testIteration){
	        	testIteration=false;
	        	if(currentFramerate<minAcceptableFramerate){ //failed
	        		decreaseQuality();
	        		return;
	        	}else{
	        		//...
	        	}
	        }
	        
	        if(currentFramerate<minAcceptableFramerate){
	        	decreaseQuality();
	        }else if(currentFramerate-threshold>minAcceptableFramerate){
	        	increaseQuality();
	      	}
	    }
	}
	
	public void increaseQuality(){
		changeQuality(1);   
	}
	
	public void decreaseQuality(){
		changeQuality(-1);
	}
	
	public void  changeQuality(int amount){
		if(amount>0){
			if(currQuality+amount>=QualitySettings.names.Length)return;
		}else{
			if(currQuality+amount<0)return;
		}
		if(currQuality+amount==previousQuality){
			maxThreshold*=2;
	    	threshold=maxThreshold;
	    	minThreshold+=0.5f;
		}
		previousQuality=currQuality;
		QualitySettings.SetQualityLevel(currQuality+amount);

		currQuality = QualitySettings.GetQualityLevel();
		currentQuality=""+currQuality+" ("+QualitySettings.names[currQuality]+")";
	    ignoreOneIteration=true;
	    if(amount>0){
	    	testIteration=true;
	    	if(verbose)Debug.Log("Quality increased to "+currQuality+", framerate: "+currentFramerate);
	    }else{
	    	if(verbose)Debug.Log("Quality decreased to "+currQuality+", framerate: "+currentFramerate);
	    }
	    afterQualityChange();
	}
	
}

public static class DisplayDetector
{
    private static Camera main;

    private static Transform camTransform;
    public static bool IsOnScreen<T>(this T obj,Vector3 vect) where T : Component
    {
        if(main==null){
            main =Camera.main;
           camTransform = main.transform;
        }
        Vector3 result = Camera.main.WorldToViewportPoint(vect);

        if (Physics.Linecast(vect, main.transform.position, LayerMask.GetMask("Default")))
        {
            //Debug.Log("offScreen");
            return false;
        }
//        Debug.Log("offScreen");
        return result.z > 0 && result.x > 0f && result.x < 1.0f && result.y > 0 && result.y < 1.0f;
    }

}