using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;



public class SpinWheelWithRandomizedRewards : MonoBehaviour
{
    public RectTransform firstWheel;
    public RectTransform secondWheel;
    public Button spinButton;
    public Text rewardText;
    public Text totalPointText;
    public Text item1_valueText;
    public Text item2_valueText;
   
    private bool isSpinning = false;
    private float firstWheelDuration = 3.5f; // Duration for the first wheel's spin
    private float secondWheelDuration = 3.5f; // Duration for the second wheel's spin


    int totalPoint = 1000;
    private int[] firstWheelRewards = { 100, 150, 200, 250, 300, -300, 350, 400};
    //private int[] secondWheelRewards = { 50, 100, 150, 200, 250, 300 ,350,400};

    [SerializeField] private List<Image> wheel1_item;
    [SerializeField] private List<Image> wheel2_item;
    [SerializeField] private Image stop_result_wheel1;
    [SerializeField] private Image stop_result_wheel2;

    [SerializeField] private GameObject light1, light1_2, light2, light2_1,settingPanel,back;

    private void Awake()
    {

        GameObject wheel1_at_start = GameObject.Find("wheel1-spin-game");
        GameObject wheel2_at_start = GameObject.Find("wheel2-spin-game");
        LeanTween.rotateAround(wheel1_at_start, Vector3.forward, 360f, 2f)
                    .setEase(LeanTweenType.easeInOutElastic).setLoopClamp();
        LeanTween.rotateAround(wheel2_at_start, Vector3.forward, -360f, 1f)
                    .setEase(LeanTweenType.easeOutQuad).setLoopClamp();
    }

    void Start()
    {
        totalPointText.text = totalPoint > 0 ? $"+{totalPoint}" : $"{totalPoint}";
        spinButton.onClick.AddListener(SpinWheels);
       
    }

    public void SpinWheels()
    {
        if (!isSpinning)
        {
            isSpinning = true;
            AudioController.Instance.PlaySFX("point");
            totalPoint -= 100;
            totalPointText.text = totalPoint > 0 ? $"+{totalPoint}" : $"{totalPoint}";

            spinButton.interactable = false;

           
            int firstWheelSpins = Random.Range(4, 7);
            int secondWheelSpins = Random.Range(4, 7);

            
            float firstWheelRandomOffset = Random.Range(-30f, 30f); 
            float secondWheelRandomOffset = Random.Range(-30f, 30f); 

            // Additional small random factor to ensure different stopping points
            float smallRandomFactor = Random.Range(-15f, 15f);

            // First Wheel Spin
            LeanTween.rotateAround(firstWheel, Vector3.forward, -360f * firstWheelSpins + firstWheelRandomOffset, firstWheelDuration)
                     .setEase(LeanTweenType.easeOutQuad)
                     .setOnComplete(() => SnapToSegment(firstWheel, 8, firstWheelRewards, "first"));

            // Second Wheel Spin with additional small random factor
            LeanTween.rotateAround(secondWheel, Vector3.forward, -360f * secondWheelSpins + secondWheelRandomOffset + smallRandomFactor, secondWheelDuration)
                     .setEase(LeanTweenType.easeOutQuad)
                     .setOnComplete(() => SnapToSegment(secondWheel, 8, firstWheelRewards, "second"));
        }
    }

     void SnapToSegment(RectTransform wheel, int segments, int[] rewards, string wheelName)
    {
        int rand = Random.Range(0, 8);
        float zRotation = wheel.eulerAngles.z;
        float segmentAngle = 360f / segments;
        float snappedRotation =  segmentAngle * rand;
       
       // wheel.eulerAngles = new Vector3(0, 0, snappedRotation);
        LeanTween.rotateZ(wheel.gameObject,  snappedRotation-720, 0.5f);
        //int segmentIndex = Mathf.RoundToInt(snappedRotation / segmentAngle) % segments;
        int rewardAmount = rewards[rand];
        
        // Store the reward from the first and second wheels separately
        if (wheelName == "first")
        {
            LeanTween.rotateZ(wheel.gameObject, snappedRotation - 720, firstWheelDuration-1f);
            Debug.Log($"First wheel reward - {rewardAmount}-{rand}");
            PlayerPrefs.SetInt("firstWheelReward", rewardAmount);

            //stop_result_wheel1.sprite = wheel1_item[rand].sprite;
            //LeanTween.scale(stop_result_wheel1.gameObject, new Vector3(1, 1, 1), 1f).setEaseInOutQuart().setLoopPingPong().setRepeat(2);

            StartCoroutine(ResultWheelShow(rand, wheelName));
        }
        else if (wheelName == "second")
        {
            LeanTween.rotateZ(wheel.gameObject, snappedRotation - 720, secondWheelDuration-1f);
            Debug.Log($"Second wheel reward - {rewardAmount}-{rand}");
            PlayerPrefs.SetInt("secondWheelReward", rewardAmount);

            //stop_result_wheel2.sprite = wheel2_item[rand].sprite;
            //LeanTween.scale(stop_result_wheel2.gameObject, new Vector3(1, 1, 1), 1f).setEaseInOutQuart().setLoopPingPong().setRepeat(2);

            StartCoroutine(ResultWheelShow(rand,wheelName));
        }

        
    }

    IEnumerator ResultWheelShow(int rand,string wheelName)
    {
        yield return new WaitForSeconds(3f);
        AudioController.Instance.PlaySFX("win");
        if (wheelName == "first")
        {
           

            stop_result_wheel1.sprite = wheel1_item[rand].sprite;
            light1.SetActive(true);
            light1_2.SetActive(true);
            LeanTween.rotateAround(light1, Vector3.forward, 360f, 1f).setEaseOutQuad().setLoopCount(10);
            LeanTween.rotateAround(light1_2, Vector3.forward, -360f, 1f).setEaseOutQuad().setLoopCount(10); ;
            LeanTween.scale(stop_result_wheel1.gameObject, new Vector3(1, 1, 1), 0.5f).setEaseInOutQuart().setLoopPingPong().setRepeat(8);
        }
        else if (wheelName == "second")
        {
            light2.SetActive(true);
            light2_1.SetActive(true);
            LeanTween.rotateAround(light2, Vector3.forward, 360f, 1f).setEaseOutQuad().setLoopCount(10); ;
            LeanTween.rotateAround(light2_1, Vector3.forward, -360f, 1f).setEaseOutQuad().setLoopCount(10); 
            stop_result_wheel2.sprite = wheel2_item[rand].sprite;
            LeanTween.scale(stop_result_wheel2.gameObject, new Vector3(1, 1, 1), 0.5f).setEaseInOutQuart().setLoopPingPong().setRepeat(8);
        }
        yield return new WaitForSeconds(1f);
        // Check if both wheels have finished spinning
        if (!isSpinning)
        {
            AudioController.Instance.PlaySFX("win");
            item1_valueText.text = PlayerPrefs.GetInt("firstWheelReward") > 0 ? $"+{PlayerPrefs.GetInt("firstWheelReward")} point" : $"{PlayerPrefs.GetInt("firstWheelReward")} point";
            item2_valueText.text = PlayerPrefs.GetInt("secondWheelReward") > 0 ? $"+{PlayerPrefs.GetInt("secondWheelReward")} point" : $"{PlayerPrefs.GetInt("secondWheelReward")} point";
            LeanTween.scale(item1_valueText.gameObject, new Vector3(2, 2, 2), 0.5f).setEaseLinear().setLoopPingPong().setRepeat(6);
            LeanTween.scale(item2_valueText.gameObject, new Vector3(2, 2, 2), 0.5f).setEaseLinear().setLoopPingPong().setRepeat(6);

            int totalReward = PlayerPrefs.GetInt("firstWheelReward") + PlayerPrefs.GetInt("secondWheelReward");

            totalPoint += totalReward;
            totalPointText.text = totalPoint > 0  ?  $"+{totalPoint}" : $"{totalPoint}";
            DisplayReward(totalReward);
            

            // Re-enable the spin button after both wheels stop
            spinButton.interactable = true;
        }

        // Set the spinning flag to false after the first wheel finishes
        isSpinning = false;
        

        yield return new WaitForSeconds(3f);
        //rewardText.gameObject.SetActive(false);
        light1.SetActive(false);
        light1_2.SetActive(false);
        light2.SetActive(false);
        light2_1.SetActive(false);
    }


    private void DisplayReward(int reward)
    {
        if(reward < 0) rewardText.text = "You lose: " + reward + "!";
        else rewardText.text = "You won: +" + reward + " !";

        rewardText.gameObject.SetActive(true);
        LeanTween.scale(rewardText.gameObject, new Vector3(2, 2, 2), 0.5f).setEaseLinear().setLoopPingPong().setRepeat(6);
    }


    public void OnClickOpenSetting()
    {
        AudioController.Instance.PlaySFX("point");
        LeanTween.move(settingPanel, new Vector3(0, 0, 0), 0.7f).setEaseInQuart().setOnComplete(
            () =>
            {

                back.SetActive(true);   
            }
            ); 

    }

    public void OnClickCloseSetting()
    {
        AudioController.Instance.PlaySFX("point");
        LeanTween.move(settingPanel, new Vector3(0, 10, 0), 0.7f).setEaseInQuart().setOnComplete(
            () =>
            {
                GameObject parent = GameObject.Find("setting - panel");
                parent.SetActive(false);

                back.SetActive(false);
            }
            );
    }

    public void onOpenRulePanel()
    {
        AudioController.Instance.PlaySFX("point");
        GameObject rulePanel = GameObject.Find("rule - panel");
        LeanTween.scale(rulePanel, new Vector3(1, 1, 1), 0.5f).setEaseSpring();
    }



    public void onCloseRulePanel()
    {
        AudioController.Instance.PlaySFX("point");
        GameObject rulePanel = GameObject.Find("rule - panel");
        LeanTween.scale(rulePanel, new Vector3(0, 0, 0), 0.5f).setEaseSpring();
    }



    public void onClickStart()
    {
        AudioController.Instance.PlaySFX("point");
        GameObject homeGameobject = GameObject.Find("HomeScreen");
        LeanTween.scale(homeGameobject, new Vector3(0, 0, 0), 0.5f).setEaseSpring();
    }



    public void onClickHome()
    {
        AudioController.Instance.PlaySFX("point");
        GameObject homeGameobject = GameObject.Find("HomeScreen");
        LeanTween.scale(homeGameobject, new Vector3(1, 1, 1), 0.5f).setEaseSpring();
    }



    public void onClickQuit()
    {
        AudioController.Instance.PlaySFX("point");
        Application.Quit();
    }
}
