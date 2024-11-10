using UnityEngine;
using System;
using System.IO;
using Calibration.AutomaticCalibration;
using RehabImmersive;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SOGameConfiguration gameConfig;
    [SerializeField] private GameObject mainMenuGameObject;
    [SerializeField] private GameObject configurationMenuGameObject;
    [SerializeField] private GameObject positionMenuGO;
    [SerializeField] private GameObject tresholdMenuGO;
    [SerializeField] private TMP_Text txtMore;
    [SerializeField] private TMP_Text txtLess;
    [SerializeField] private TMP_Text txtRigthPosition;
    [SerializeField] private TMP_Text txtLeftPosition;
    [SerializeField] private TMP_Text txtCenterPosition;
    [SerializeField] private TMP_Text txtAllPosition;
    [SerializeField] private TMP_Text txtTreshold;
    [SerializeField] private TMP_Text txtHighRestriction;
    [SerializeField] private GameObject loadingImage;
    [SerializeField] private LoadSceneAsync loadScene;


    public void Start()
    {
        mainMenuGameObject.SetActive(true);
        if (!gameConfig.gameConfiguration.gameOptions.ContainsKey(CalibrationConstants.KeyPosition))
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(CalibrationConstants.KeyPosition,
                CalibrationConstants.ValuePositionAll);
            gameConfig.gameConfiguration.handInteraction = GameConfiguration.HandInteraction.Both;
        }

        if (!gameConfig.gameConfiguration.gameOptions.ContainsKey(CalibrationConstants.KeyTreshold)) 
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(CalibrationConstants.KeyTreshold,
                CalibrationConstants.ValueTresholdDefault);
        }
        if (!gameConfig.gameConfiguration.gameOptions.ContainsKey(CalibrationConstants.KeyHighRestriction)) ;
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(CalibrationConstants.KeyHighRestriction,
                true.ToString());
        }
        
        ActivateMainMenuAndSave();
    }


    public void Quit()
    {
        SaveConfiguration();
        Application.Quit();
    }

    private void ActivateMainMenuAndSave()
    {
        loadingImage.SetActive(false);
        configurationMenuGameObject.SetActive(false);
        positionMenuGO.SetActive(false);
        tresholdMenuGO.SetActive(false);
        mainMenuGameObject.SetActive(true);
        SaveConfiguration();
    }

   

    private void DesactivateAll()
    {
        loadingImage.SetActive(false);
        configurationMenuGameObject.SetActive(false);
        positionMenuGO.SetActive(false);
        tresholdMenuGO.SetActive(false);
        mainMenuGameObject.SetActive(false);
    }


    public void ShowCalibrationMenu()
    {
        DesactivateAll();
        loadingImage.SetActive(true);
        loadScene.StartLoading("BodyCalibration", loadingImage);
    }

    public void PlayTest()
    {
        loadingImage.SetActive(true);
        loadScene.StartLoading("TestCalibration", loadingImage);
        
    }


    public void ShowMenuPosition()
    {
        DesactivateAll();
        configurationMenuGameObject.SetActive(true);
        positionMenuGO.SetActive(true);
        switch (gameConfig.gameConfiguration.gameOptions[CalibrationConstants.KeyPosition])
        {
            case CalibrationConstants.ValuePositionLeft:
                TextPositionLeft();
                break;
            case CalibrationConstants.ValuePositionRigth:
                TextPositionRigth();
                break;
            case CalibrationConstants.ValuePositionCenter:
                TextPositionCenter();
                break;
            case CalibrationConstants.ValuePositionAll:
                TextPositionAll();
                break;
            default:
                TextPositionAll();
                break;
        }
    }


    public void SetPositionCenter()
    {
        gameConfig.gameConfiguration.AddOrUpdateOption(CalibrationConstants.KeyPosition,
            CalibrationConstants.ValuePositionCenter);
        gameConfig.gameConfiguration.handInteraction = GameConfiguration.HandInteraction.Both;
        TextPositionCenter();
    }

    public void SetPositionLeft()
    {
        gameConfig.gameConfiguration.AddOrUpdateOption(CalibrationConstants.KeyPosition,
            CalibrationConstants.ValuePositionLeft);
        gameConfig.gameConfiguration.handInteraction = GameConfiguration.HandInteraction.Left;
        TextPositionLeft();
    }

    public void SetPositionRigth()
    {
        gameConfig.gameConfiguration.AddOrUpdateOption(CalibrationConstants.KeyPosition,
            CalibrationConstants.ValuePositionRigth);
        gameConfig.gameConfiguration.handInteraction = GameConfiguration.HandInteraction.Right;
        TextPositionRigth();
    }

    public void SetPositionAll()
    {
        gameConfig.gameConfiguration.AddOrUpdateOption(CalibrationConstants.KeyPosition,
            CalibrationConstants.ValuePositionAll);
        gameConfig.gameConfiguration.handInteraction = GameConfiguration.HandInteraction.Both;
        TextPositionAll();
    }

    public void ShowTresholdMenu()
    {
        

        tresholdMenuGO.SetActive(true);
        positionMenuGO.SetActive(false);
        txtTreshold.text = gameConfig.gameConfiguration.gameOptions[CalibrationConstants.KeyTreshold];
    }

    public void ShowConfigurationdMenu()

    {
        DesactivateAll();
        configurationMenuGameObject.SetActive(true);
        SetHighRestriction();
    }

    private void SetTresholdValue(int value)
    {
        txtTreshold.text = value.ToString();
        gameConfig.gameConfiguration.AddOrUpdateOption(CalibrationConstants.KeyTreshold, value.ToString());
    }

    public void AddMoreTreshold()
    {
        int value = GetTresholdFromText();
        if (value <= 200)
        {
            SetTresholdValue(value + 5);
        }
    }

    public void LessTreshold()
    {
        int value = GetTresholdFromText();
        if (value >=5 )
        {
            SetTresholdValue(value - 5);
        }
    }

    public void UpdateHighRestriction()
    {
        tresholdMenuGO.SetActive(false);
        positionMenuGO.SetActive(false);

        if (gameConfig.gameConfiguration.gameOptions.TryGetValue(CalibrationConstants.KeyHighRestriction,
                out var option))
        {
            bool updateValue = !bool.Parse(option);
            gameConfig.gameConfiguration.AddOrUpdateOption(CalibrationConstants.KeyHighRestriction,
                updateValue.ToString());
            SetHighRestriction();
        }
    }


    private void SetHighRestriction()
    {
        Color colorRestriction;
        if (gameConfig.gameConfiguration.gameOptions.TryGetValue(CalibrationConstants.KeyHighRestriction,
                out var option))
        {
            colorRestriction =
                option.Equals(true.ToString())
                    ? Color.green
                    : Color.white;
        }
        else
        {
            colorRestriction = Color.white;
        }

        txtHighRestriction.color = colorRestriction;
    }

    private int GetTresholdFromText()
    {
        return int.TryParse(txtTreshold.text, out int tresholdInt) ? tresholdInt : 0;
    }

    public void SavePositionConfiguration()
    {
        
        ActivateMainMenuAndSave();
    }

    

    private void TextPositionCenter()
    {
        txtRigthPosition.color = Color.white;
        txtLeftPosition.color = Color.white;
        txtCenterPosition.color = Color.green;
        txtAllPosition.color = Color.white;
    }

    private void TextPositionRigth()
    {
        txtRigthPosition.color = Color.green;
        txtLeftPosition.color = Color.white;
        txtCenterPosition.color = Color.white;
        txtAllPosition.color = Color.white;
    }

    private void TextPositionLeft()
    {
        txtLeftPosition.color = Color.green;
        txtRigthPosition.color = Color.white;
        txtCenterPosition.color = Color.white;
        txtAllPosition.color = Color.white;
    }

    private void TextPositionAll()
    {
        txtRigthPosition.color = Color.green;
        txtLeftPosition.color = Color.green;
        txtCenterPosition.color = Color.green;
        txtAllPosition.color = Color.green;
    }


    public void MainMenuAndSaveConfiguration()
    {
        ActivateMainMenuAndSave();
    }

    /**
     * Save user configuration
     */
    private void SaveConfiguration()
    {
        try
        {
            String userFileConfiguration = Path.Combine(gameConfig.userPath, RehabConstants.FileConfiguration);

            string dataString = JsonUtility.ToJson(gameConfig.gameConfiguration);
            File.WriteAllText(userFileConfiguration, dataString);
        }
        catch (Exception e)
        {
            Debug.Log("Error saving user configuration " + e.Message);
        }
    }


    private GameObject GetDeskGameObject()
    {
        return GameObject.FindGameObjectWithTag("BoxObject");
    }
}