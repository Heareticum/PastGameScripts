using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    [Serializable]
    public struct RegionInfo
    {
        public string regionName;
        public List<RegionOptionData> options;
    }
    [Serializable]
    public struct RegionOptionData
    {
        public string name;
        public RegionOptionCategory category;
        public LevelID levelID;
        [Header("鏡頭目標")]
        public Transform optionCameraTarget;
        [Header("建築選項")]
        public List<BuildingOptionData> buildingOptions;
    }
    [Serializable]
    public struct BuildingOptionData
    {
        public string name;
        [Header("選項類型")]
        public BuildingOptionCategory category;
        [Header("選項鏡頭目標")]
        public Transform optionCameraTarget;
        [Header("事件觸發訊息")]
        public string eventMessage;
    }

    public static UIManager instance;
    public CameraTracker cameraTracker;
    [Header("地區資訊")]
    public RegionInfo regionInfo;
    public CanvasGroup regionOptionObject;
    public Transform regionOptionSpawnPos;
    public GameObject regionOptionPrefab;
    public List<Button> regionOptionButtons;
    [Header("建築資訊")]
    public Text buildingName;
    public CanvasGroup buildingOptionObject;
    public Transform buildingOptionSpawnPos;
    public GameObject buildingOptionPrefab;
    public List<Button> buildingOptionButtons;
    [Header("船隻升級面板")]
    public GameObject shipUpgradePanel;
    public Button shipPanelBackButton;
    public Button shipPanelUpgradeButton;
    [Header("主城鎮")]
    public Transform maintownDefaultCameraTransform;
    public GameObject playerObject;
    public GameObject playerCamera;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Initialize()
    {
        cameraTracker = Camera.main.GetComponent<CameraTracker>();
        RegistButtonEvents();
        ResetOptions();
        SpawnRegionOptions();
    }
    public void RegistButtonEvents()
    {
        shipPanelBackButton.onClick.AddListener(delegate { shipUpgradePanel.SetActive(false); });
        //shipPanelUpgradeButton.onClick.AddListener(delegate { });
    }
    public void ResetOptions()
    {
        foreach(Transform transform in regionOptionSpawnPos)
        {
            Destroy(transform.gameObject);
        }
        foreach(Transform transform in buildingOptionSpawnPos)
        {
            Destroy(transform.gameObject);
        }
        regionOptionButtons.Clear();
        buildingOptionButtons.Clear();
    }
    public void SpawnRegionOptions()
    {
        foreach (var value in regionInfo.options)
        {
            GameObject obj = Instantiate(regionOptionPrefab, regionOptionSpawnPos);
            RegionOptionPrefab prefab = obj.GetComponent<RegionOptionPrefab>();
            prefab.InitOption(value);
            regionOptionButtons.Add(prefab.optionButton);
        }
    }
    public void SpawnBuildingOptions(RegionOptionData data)
    {
        foreach (Transform transform in buildingOptionSpawnPos)
        {
            Destroy(transform.gameObject);
        }
        buildingOptionButtons.Clear();

        foreach (var value in data.buildingOptions)
        {
            GameObject obj = Instantiate(buildingOptionPrefab, buildingOptionSpawnPos);
            BuildingOptionPrefab prefab = obj.GetComponent<BuildingOptionPrefab>();
            prefab.InitOption(value);
            buildingOptionButtons.Add(prefab.optionButton);
        }
    }
    public async void ChangeOptionUI(CanvasGroup closeGroup, CanvasGroup openGroup, List<Button> openButtons)
    {
        while (cameraTracker.isLockMovement)
        {
            await Task.Yield();
        }
        
        closeGroup?.gameObject.SetActive(false);
        openGroup?.gameObject.SetActive(true);
        await FadeOptionUI(openGroup, true);
        EnableButtons(openButtons, true);
    }
    public async Task FadeOptionUI(CanvasGroup fadeObject, bool enable)
    {
        if(fadeObject == null) { return; }
        if (enable)
        {
            while (fadeObject.alpha < 1)
            {
                fadeObject.alpha += Time.deltaTime;
                await Task.Yield();
            }
        }
        else
        {
            while (fadeObject.alpha > 0)
            {
                fadeObject.alpha -= Time.deltaTime;
                await Task.Yield();
            }
        }
    }
    public void EnableButtons(List<Button> buttons, bool enable)
    {
        if (buttons == null) { return; }
        foreach(var button in buttons)
        {
            button.enabled = enable;
        }
    }
    public void ResetCameraToDefault()
    {
        playerCamera.transform.DetachChildren();
        playerCamera.SetActive(false);
        playerObject.transform.localPosition = Vector3.zero;
        playerObject.transform.localRotation = Quaternion.identity;
        playerObject.SetActive(false);
        ChangeOptionUI(buildingOptionObject, regionOptionObject, regionOptionButtons);
        cameraTracker.SetCameraTarget(maintownDefaultCameraTransform);
        Camera.main.fieldOfView = 39;
    }
}
