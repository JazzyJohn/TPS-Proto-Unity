
using UnityEngine;
using CodeWriter.Acting;

public class LotteryGUI : MonoBehaviour
{
	[SerializeField] UIButton rotateButton;
	[SerializeField] UILabel countLabel;
	[SerializeField] GameObject cellLighting;

	[Space(10)]
	[SerializeField] int rotateCircles;
	[SerializeField] float rotateDuration;

    [Space(10)]
    [SerializeField] MainMenuGUI mainMenu;

     [Space(10)]
    [SerializeField]  UILabel amountLabel;
     [SerializeField] UILabel offerLabel;
     [SerializeField] UIRect askWindow;
	private bool isPlaying;

	public void Open()
	{
		GetComponent<UIPanel>().alpha = 1f;

		LotteryManager.Instance.OnReplaysCountChanged += HandleEngineReplayAdded;
		
		this.UpdateReplaysCount();
	}

	public void Close()
	{
		LotteryManager.Instance.OnReplaysCountChanged -= HandleEngineReplayAdded;

		GetComponent<UIPanel>().alpha = 0f;
	}

	private void HandleEngineReplayAdded (object sender, System.EventArgs e)
	{
		this.UpdateReplaysCount();
	}

	private void UpdateReplaysCount()
	{
		UpdatePlayButtonActive();

		countLabel.text = "" + LotteryManager.Instance.AvailableReplays;
	}

	private void UpdatePlayButtonActive()
	{
		bool isActive = !isPlaying && LotteryManager.Instance.CanPlay;

		if(rotateButton.enabled == isActive)
			return;

		rotateButton.enabled = isActive;

		rotateButton.gameObject.CompleteActions();
		rotateButton.gameObject.PerformAction(new Easing {
			From = isActive ? 0.5f : 1f,
			To = isActive ? 1f : 0.5f,
			Duration = 0.1f,
			ExternUpdate = val => rotateButton.GetComponent<UISprite>().alpha = val
		});
	}

	public void Play()
	{
		if(isPlaying)
			return;

		if(!LotteryManager.Instance.CanPlay)
		{
			Debug.Log("попытки кончились, купи ещё!");
			return;
		}

		isPlaying = true;
		UpdatePlayButtonActive();

		int winItemIndex = LotteryManager.Instance.Play();

		int to = (-30 * winItemIndex + 15) - (360 * rotateCircles);

		var easing = new Easing
		{
			From = cellLighting.transform.rotation.eulerAngles.z,
			To = to,
			Duration = rotateDuration,
			Method = Easings.QuartEaseOut,
			ExternUpdate = val => cellLighting.transform.rotation = Quaternion.Euler(0, 0, val),

			ExternComplete = () => {
				isPlaying = false;
				UpdatePlayButtonActive();

				LotteryManager.Instance.ApplyLastPlay();
			}
		};

		cellLighting.gameObject.PerformAction(easing);
	}
    int amount;
	public void BuyReplay()
	{
		if(LotteryManager.Instance.CanBuyReplay)
		{
            LotteryManager.Instance.BuyReplay(amount);
            CloseAsk();
		}
		else
		{
            mainMenu.MoneyError();
		}
	}

    public void CahngeBuyAmount(UIScrollBar ScrollArg) 
    {
       
        amount = Mathf.RoundToInt( ScrollArg.value * (float)ScrollArg.numberOfSteps)+1;
        amountLabel.text = (amount * LotteryManager.LOTTERY_PRICE).ToString();
        offerLabel.text = TextGenerator.instance.GetMoneyText("buyLottery", amount);


    }
    public void CloseAsk()
    {
        askWindow.alpha = 0.0f;
    }

    public void OpenAsk()
    {
        askWindow.alpha = 1.0f;
    }
}
