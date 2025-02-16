using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RegionOptionCategory
{
    Default,
    ChangeScene,
}
public class RegionOptionPrefab : MonoBehaviour
{
    public UIManager uiManager;
    public UIManager.RegionOptionData optionData;
    public Button optionButton;
    public Text optionName;
    public Transform targetTransform;
    // Start is called before the first frame update
    void Start()
    {
        uiManager = UIManager.instance;
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
    public void InitOption(UIManager.RegionOptionData _optionData)
    {
        optionData = _optionData;
        optionButton = GetComponent<Button>();
        optionName.text = optionData.name;
        targetTransform = optionData.optionCameraTarget;
        RegistEvent();
    }
    public async void SetBuildingOptions()
    {
        switch (optionData.category)
        {
            case RegionOptionCategory.Default:
                uiManager.buildingName.text = optionName.text;
                uiManager.SpawnBuildingOptions(optionData);
                await uiManager.FadeOptionUI(uiManager.regionOptionObject, false);
                uiManager.ChangeOptionUI(uiManager.regionOptionObject, uiManager.buildingOptionObject, uiManager.buildingOptionButtons);
                break;
            case RegionOptionCategory.ChangeScene:
                LevelManager.instance.EnterLevel(optionData.levelID);
                break;
            default:
                break;
        }
    }
    public void RegistEvent()
    {
        optionButton.onClick.AddListener(delegate { Camera.main.GetComponent<CameraTracker>().SetCameraTarget(targetTransform); });
        optionButton.onClick.AddListener(delegate { SetBuildingOptions(); });
        optionButton.onClick.AddListener(delegate { UIManager.instance.EnableButtons(UIManager.instance.regionOptionButtons, false); });
    }
    public void UnRegistEvent()
    {
        optionButton.onClick.RemoveListener(delegate { Camera.main.GetComponent<CameraTracker>().SetCameraTarget(targetTransform); });
        optionButton.onClick.RemoveListener(delegate { SetBuildingOptions(); });
        optionButton.onClick.RemoveListener(delegate { UIManager.instance.EnableButtons(UIManager.instance.regionOptionButtons, false); });
    }
    private void OnDestroy()
    {
        UnRegistEvent();
    }
}
