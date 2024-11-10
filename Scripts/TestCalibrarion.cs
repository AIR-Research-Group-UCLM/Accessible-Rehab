using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Samples;
using RehabImmersive;
using TMPro;
using Tracking;
using UnityEngine;


namespace Calibration.AutomaticCalibration
{
    public class TestCalibrarion : MonoBehaviour
    {
        
        [SerializeField] private GameObject buttons;
        [SerializeField] private Material materialOK;
        [SerializeField] private Material materialKO;
        [SerializeField] private GameObject cubePrefab;
        [SerializeField] private List<Vector3> _objectsPositions;
        [SerializeField] private OVRCameraRig _ovrCameraRig;
        [SerializeField] private HandGrabInteractor _handGrabR;
        [SerializeField] private HandGrabInteractor _handGrabL;
        [SerializeField] private GoalBox goalBox;
        private AutomaticCalibration _automaticCalibrationDefault, _automaticCalibrationCurrent;
        [SerializeField] private SOGameConfiguration _gameConfigSo;
        [SerializeField] private LineRenderer lineRenderer1;
        [SerializeField] private LineRenderer lineRenderer2;
        [SerializeField] private LineRenderer lineRenderer3;
        [SerializeField] private LineRenderer lineRenderer4;
        [SerializeField] private LineRenderer lineRenderer5;
        [SerializeField] private LineRenderer lineRenderer6;
        [SerializeField] private BodyCalibrationUtils utils;
        [SerializeField] private TrackingDataExecutorHand handExecutor;
        [SerializeField] private SceneLoader loader;
        [SerializeField] private PlaneCenterArea planeCenter;
        [SerializeField] private TMP_Text _textMesh;
        [SerializeField] private ParticleSystem _particleFinish;
        [SerializeField] private AudioSource _soundWind;
        [SerializeField] private AutomaticCalibrationConf _automaticCalibrationConf;
        private CalibrationConfiguration _caliConfDefaultUser, _caliConfCurrentUser;
        private List<VirtualObject> _virtualObjectsDefaultUser, _virtualObjectsCurrentUser;
        private HistoricalJSon historicalJson;
        private HistoricalWritterJSon _historicalWritter;
        private Historical historical;
        private HistoricalWritter _historicalW;
        private bool _saveHMD;
        private bool _showTime;
        private float initialTime;
        private float currentTime;
        private List<GameObject> _objects;
        private bool isDefault = true;
        private string defaultRelocatedPosition;
        private float timeRelocateDefault;
        private float timeRelocate;
        private float maxTime = 60.0f;

        void Start()
        {
            _gameConfigSo.grabIdentier = "";
            _saveHMD = true;
            _showTime = false;
            timeRelocate = timeRelocateDefault = currentTime = 0;
            ObtainLastConfiguration();
            SetRelocatedValues();
            GenerateBlocksInPosition(false);

            //initialize historical writter
            _historicalWritter = new HistoricalWritterJSon(_gameConfigSo.userPath);
            _gameConfigSo.gameStarts = true;
            _historicalW = new HistoricalWritter(_gameConfigSo.userPath);
            historical = new Historical(_gameConfigSo.gameName);

            historical.TrackingFile = TrackingDataWriter.Instance.FilePath;
           
            historicalJson = new HistoricalJSon(_gameConfigSo.gameName, GetExerciceHand());


            GetExerciceHand();
            historicalJson.trackingFile = TrackingDataWriter.Instance.FilePath;
            string originalPositions = GetVirtualObjectsHistoricalAsJson(_objects);
            historicalJson.AddOrUpdateGameOption(CalibrationConstants.KeyOriginalVObjectPosition, originalPositions);
        }


        /**
          * Generates blocks in specified positions.
         */
        private void GenerateBlocksInPosition(bool defaultUser)
        {
            //reseat goal
            goalBox.ResetGoal();

            if (_objects != null)
            {
                foreach (GameObject objectBlock in _objects)
                {
                    objectBlock.SetActive(false);
                }
            }

            _objects = new List<GameObject>();
            foreach (Vector3 cubePosition in _objectsPositions)
            {
                var c = Instantiate(cubePrefab);
                BlockAutroGrasp blockAutoGrasp = c.GetComponentInChildren<BlockAutroGrasp>();
                blockAutoGrasp.Initialize(_handGrabR, _handGrabL);
                c.transform.position = cubePosition;
                _objects.Add(c);
            }

            _virtualObjectsDefaultUser = new List<VirtualObject>();
            _virtualObjectsCurrentUser = new List<VirtualObject>();
            if (defaultUser)
            {
                foreach (GameObject obj in _objects)
                {
                    VirtualObject virtualObj = new VirtualObject(obj);
                    _virtualObjectsDefaultUser.Add(virtualObj);
                }

                _automaticCalibrationDefault = new AutomaticCalibration(_caliConfDefaultUser,
                    _virtualObjectsDefaultUser, _automaticCalibrationConf);
            }
            else
            {
                foreach (GameObject obj in _objects)
                {
                    VirtualObject virtualObj = new VirtualObject(obj);
                    _virtualObjectsCurrentUser.Add(virtualObj);
                }

                _automaticCalibrationCurrent = new AutomaticCalibration(_caliConfCurrentUser,
                    _virtualObjectsCurrentUser, _automaticCalibrationConf);
            }

            SetBlocksWithAonfigurationCalibration();
        }

        private void TimeStart()
        {
            initialTime = Time.time;
            _showTime = true;
        }

        private void TimeStop()
        {
            _showTime = false;
        }

        public void Update()
        {
            if (_saveHMD)
            {
                historical.HmdPosition = _ovrCameraRig.centerEyeAnchor.transform.position;
                historical._hmdRotation = _ovrCameraRig.centerEyeAnchor.transform.rotation.eulerAngles;
                _saveHMD = false;
            }

            if (_showTime)
            {
                currentTime = Time.time - initialTime;
                _textMesh.text = "Time: " + currentTime.ToString("F0");
                if (currentTime > maxTime)
                {
                    _showTime = false;
                    FinishLevel();
                }
            }


           
        }

        /**
         * Return exercice hand as string
         */
        private string GetExerciceHand()
        {
            if (_automaticCalibrationConf.CurrentExerciseArea is AutomaticCalibrationConf.ExerciseArea.Right)
            {
                historical.hand = Historical.HandInteraction.Right;
                return GameConfiguration.HandInteraction.Right.ToString();
            }
            else if (_automaticCalibrationConf.CurrentExerciseArea is AutomaticCalibrationConf.ExerciseArea.Left)
            {
                historical.hand = Historical.HandInteraction.Left;
                return GameConfiguration.HandInteraction.Left.ToString();
            }
            else
            {
                historical.hand = Historical.HandInteraction.Both;
                return GameConfiguration.HandInteraction.Both.ToString();
            }
        }


        private void VObjectPositionToHistorical(List<GameObject> lobject, string key)
        {
            int i = 0;
            foreach (GameObject obj in lobject)
            {
                i++;
                historical.AddOrUpdateGameOption(key + "_" + obj.name,
                    obj.transform.position.ToString("f0", CultureInfo.InvariantCulture));
            }
        }

        private string GetVirtualObjectsHistoricalAsJson(List<GameObject> lobject)
        {
            List<ObjectPosition> positions = new List<ObjectPosition>();
            foreach (GameObject obj in lobject)
            {
                positions.Add(new ObjectPosition(obj.transform.position));
            }

            SerializablePositionList positionList = new SerializablePositionList(positions);
            return JsonUtility.ToJson(positionList);
        }

        /**
         * Set ExerciseArea
         */
        private void SetRelocatedValues()
        {
            AutomaticCalibrationConf.ExerciseArea exerciseArea = AutomaticCalibrationConf.ExerciseArea.Global;
            if (_gameConfigSo.gameConfiguration.gameOptions.ContainsKey(CalibrationConstants.KeyPosition))
            {
                switch (_gameConfigSo.gameConfiguration.gameOptions[CalibrationConstants.KeyPosition])
                {
                    case CalibrationConstants.ValuePositionLeft:
                        exerciseArea = AutomaticCalibrationConf.ExerciseArea.Left;
                        break;
                    case CalibrationConstants.ValuePositionRigth:
                        exerciseArea = AutomaticCalibrationConf.ExerciseArea.Right;
                        break;
                    case CalibrationConstants.ValuePositionCenter:
                        exerciseArea = AutomaticCalibrationConf.ExerciseArea.Central;
                        break;
                    case CalibrationConstants.ValuePositionCenterAndElbowFlexed:
                        exerciseArea = AutomaticCalibrationConf.ExerciseArea.CentralAndDown;
                        break;
                    case CalibrationConstants.ValuePositionAll:
                        exerciseArea = AutomaticCalibrationConf.ExerciseArea.Global;
                        break;
                    default:
                        exerciseArea = AutomaticCalibrationConf.ExerciseArea.Global;
                        break;
                }
            }

            if (_gameConfigSo.gameConfiguration.gameOptions.TryGetValue(
                    CalibrationConstants.KeyTreshold, out var treshold))
            {
                _automaticCalibrationConf.Threshold = float.Parse(treshold);
            }
            else
            {
                _automaticCalibrationConf.Threshold = 100f;
            }

            _automaticCalibrationConf.CurrentExerciseArea = exerciseArea;
            bool hRestricted = _gameConfigSo.gameConfiguration.gameOptions.TryGetValue(
                CalibrationConstants.KeyHighRestriction,
                out var option) && bool.Parse(option);
            _automaticCalibrationConf.ConsiderHeightLimits = hRestricted;
        }


        private void SetBlocksWithAonfigurationCalibration()
        {
            foreach (GameObject obj in _objects)
            {
                // get et BlockAutoGrasp from the parent of the object
                BlockAutroGrasp block = obj.GetComponentInChildren<BlockAutroGrasp>();

                if (block != null)
                {
                    // L SetCalibration 
                    block.SetCalibration(_automaticCalibrationCurrent);
                }
                else
                {
                    Debug.LogWarning("No se encontró el componente BlockAutoGrasp en el padre de: " + obj.name);
                }
            }
        }


        //Relocate objects with calibration data from standar user
        public void RelocatebjectsDefaultUser()
        {
           
            isDefault = true;
            GenerateBlocksInPosition(true);
            _gameConfigSo.gameStarts = false;

            DrawOvalsAndCenterAreaOriginal();
            ApplyAutomaticCalibrationDefaultUser();
            //set new position for each virtual object
            foreach (var vObject in _virtualObjectsDefaultUser)
            {
                if (!vObject.IsCorrectlyPositioned)
                {
                    vObject.gameObject.transform.position = vObject.NewPosition;
                }
            }

            string relocatedPostion = GetVirtualObjectsHistoricalAsJson(_objects);
            
            historicalJson.AddOrUpdateGameOption(CalibrationConstants.KeyVReloactedDefaultObjectPosition, relocatedPostion);
            historical.AddOrUpdateGameOption(CalibrationConstants.KeyVReloactedDefaultObjectPosition, relocatedPostion);
            //reset executor and set new file name (Relocated)
            handExecutor.ResetAndSetNewName(CalibrationConstants.FileNameRelocation + "Default");
            _gameConfigSo.gameStarts = true;
            TimeStart();
            buttons.SetActive(false);
        }


        //End the test, display results and save data in the history.
        private void FinishLevel()
        {
            buttons.SetActive(true);
            _gameConfigSo.gameStopped = true;
            TimeStop();
            _textMesh.text = "Congratulations!\n" + goalBox.BlocksMoved() + " blocks moved at " + currentTime.ToString("F2");
            
            SaveTimeToFinish();
            currentTime = 0;
            if (isDefault)
            {
                historical.AddOrUpdateGameOption(CalibrationConstants.KeyVODefaulMoved, goalBox.BlocksMoved().ToString());
            }
            else
            {
                historical.AddOrUpdateGameOption(CalibrationConstants.KeyVOCurrentMoved, goalBox.BlocksMoved().ToString());
            }
        }
        public void LevelCompleted()
        {
            if (_particleFinish != null)
            {
                _particleFinish.gameObject.SetActive(true);
                _particleFinish.Play();
            }

            if (_soundWind != null)
            {
                _soundWind.Play(5);
            }

            FinishLevel();
        }

        private void SaveTimeToFinish()
        {
            if (isDefault)
            {
                timeRelocateDefault = currentTime;
            }
            else
            {
                timeRelocate = currentTime;
            }
            
           
        }
        
        
        //It draws the ovals that delineate the ROM, applies automatic calibration, detects incorrectly positioned objects and assigns the new position in accordance with the ROM.
        public void Relocatebjects()
        {
           
            isDefault = false;
            GenerateBlocksInPosition(false);
            TimeStart();
            _gameConfigSo.gameStarts = false;
            DrawOvalsAndCenterAreaOriginal();
            ApplyAutomaticCalibration();
            //set new position for each virtual object
            foreach (var vObject in _virtualObjectsCurrentUser)
            {
                if (!vObject.IsCorrectlyPositioned)
                {
                    vObject.gameObject.transform.position = vObject.NewPosition;
                }
            }

            string relocatedPostion = GetVirtualObjectsHistoricalAsJson(_objects);
        
            historicalJson.AddOrUpdateGameOption(CalibrationConstants.KeyVReloactedObjectPosition, relocatedPostion);
            historical.AddOrUpdateGameOption(CalibrationConstants.KeyVReloactedObjectPosition, relocatedPostion);


            //reset executor and set new file name (Relocated)
            handExecutor.ResetAndSetNewName(CalibrationConstants.FileNameRelocation);
            _gameConfigSo.gameStarts = true;
            buttons.SetActive(false);
        }


        public void Exit()
        {
            _gameConfigSo.gameStarts = false;

            historicalJson.AddOrUpdateGameOption(CalibrationConstants.KeyRelocatedTrackingFile,
                TrackingDataWriter.Instance.FilePath);

            _historicalWritter.WriteHistorical(historicalJson);

            historical.AddOrUpdateGameOption(CalibrationConstants.KeyRelocatedTrackingFile,
                TrackingDataWriter.Instance.FilePath);
            historical.AddOrUpdateGameOption(CalibrationConstants.KeyTimeRelocation, timeRelocate.ToString("f0"));
            historical.AddOrUpdateGameOption(CalibrationConstants.KeyTimeRelocationDefault, timeRelocateDefault.ToString("f0"));
            _historicalW.WriteHistorical(historical);
            loader.Load("StartMenu");
        }


        private void DrawOvalsAndCenterAreaOriginal()
        {
            HandCalibrationConfiguration handL = _caliConfCurrentUser.HandCalibrationL;
            HandCalibrationConfiguration handR = _caliConfCurrentUser.HandCalibrationR;
       
            if (_automaticCalibrationConf.CurrentExerciseArea is AutomaticCalibrationConf.ExerciseArea.Central)
            {
                //  planeCenter.StartAnimation(_caliConf.handCalibrationR.palmCenter,_caliConf.handCalibrationR.palmCenter, _caliConf.handCalibrationL.palmCenter);
                utils.DrawCenter(_caliConfCurrentUser.handCalibrationR.palmCenter,
                    _caliConfCurrentUser.handCalibrationL.palmCenter, lineRenderer1);
            }
            else
            {
                if (_automaticCalibrationConf.CurrentExerciseArea is AutomaticCalibrationConf.ExerciseArea.Right
                    or AutomaticCalibrationConf.ExerciseArea.Global)
                {
                    //XZ oval
                    utils.DrawOval(handR.ovalXZCenter, handR.ovalXZRadiusX,
                        handR.ovalXZRadiusZ, BodyCalibrationUtils.Plane.XZ, lineRenderer1);
                    //XY oval
                    utils.DrawOval(handR.OvalCenter, handR.OvalRadiusX,
                        handR.OvalRadiusY, BodyCalibrationUtils.Plane.XY, lineRenderer2);
                    //YZ oval
                    utils.DrawOval(handR.OvalCenter, handR.OvalRadiusY,
                        handR.OvalRadiusZ, BodyCalibrationUtils.Plane.YZ, lineRenderer3);
                }

                if (_automaticCalibrationConf.CurrentExerciseArea is AutomaticCalibrationConf.ExerciseArea.Left
                    or AutomaticCalibrationConf.ExerciseArea.Global)
                {
                    //XZ oval
                    utils.DrawOval(handL.ovalXZCenter, handL.ovalXZRadiusX,
                        handR.ovalXZRadiusZ, BodyCalibrationUtils.Plane.XZ, lineRenderer4);
                    //XY oval
                    utils.DrawOval(handL.OvalCenter, handL.OvalRadiusX,
                        handL.OvalRadiusY, BodyCalibrationUtils.Plane.XY, lineRenderer5);
                    //YZ oval
                    utils.DrawOval(handL.OvalCenter, handL.OvalRadiusY,
                        handL.OvalRadiusZ, BodyCalibrationUtils.Plane.YZ, lineRenderer6);
                }
            }
        }

       
        private void ApplyAutomaticCalibration()

        {
            _automaticCalibrationCurrent.CheckAndMarkObjectsPositions();
            _automaticCalibrationCurrent.RelocateObjects();
        }

        private void ApplyAutomaticCalibrationDefaultUser()

        {
            _automaticCalibrationDefault.CheckAndMarkObjectsPositions();
            _automaticCalibrationDefault.RelocateObjects();
        }


        //Gets the user's most recent calibration history.
        private void ObtainLastConfiguration()
        {
            CalibrationHistoryManager calibrationHistoryManagerCurrentUser =
                new CalibrationHistoryManager(_gameConfigSo.userPath);
            string parentPath = Path.GetDirectoryName(_gameConfigSo.userPath);
            CalibrationHistoryManager calibrationHistoryDefaultUser =
                new CalibrationHistoryManager(parentPath, "CalibrationDefault.json");
            //configuration from default user
            _caliConfDefaultUser =
                calibrationHistoryDefaultUser.CalibrationHistory.GetLastCalibrationConfiguration();
            //configuration form current user

            _caliConfCurrentUser =
                calibrationHistoryManagerCurrentUser.CalibrationHistory.GetLastCalibrationConfiguration();
        }
    }
}