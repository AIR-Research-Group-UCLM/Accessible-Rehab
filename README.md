# Creating Adapted Environments: Enhancing Accessibility in Virtual Reality for Upper Limb Rehabilitation through Automated Element Adjustment

## Table of Contents
1. [Summary](#summary)
2. [Data Folder](#data-folder)
   - [Data Participants Folder](#-dataparticipants-folder)
   - [CalibrationDefault.json](#-calibrationdefaultjson)
   - [Resume.md](#-resumemd)
3. [Detailed Explanation of Files](#detailed-explanation-of-files)
   - [Historical.csv](#-historicalcsv)
   - [Calibrationhistory.json](#-calibrationhistoryjson)
   - [Configuration.json](#-configurationjson)
   - [CalibrationDefault.json](#-calibrationdefaultjson)
## Summary

In the last decade, Virtual Reality (VR) has emerged as a promising tool for upper limb rehabilitation, effectively complementing conventional therapies. However, one of the main challenges lies in designing virtual environments that adapt to the specific needs of each patient, considering their unique motor limitations. An inadequately adapted environment can result in overexertion and the inability to perform exercises, negatively affecting both the patient's motivation and their recovery.

In this article, we present an innovative calibration method that individually identifies and maps motor limitations on the left and right sides of the body. As a result, an irregular volume, formed by the interconnection of three elliptical shapes, is generated that envelops the patient and represents their safe range of movements. Furthermore, a second method is introduced that automatically readjusts the location of objects within the virtual environment to the safe space generated, optimizing the patient's accessibility and interaction with therapy elements.

To test the results, an immersive VR environment was designed in which the aforementioned methods were applied for the automatic placement of virtual elements in the peripersonal space (PPS) of the participants. Testing has been carried out at the Hospital Nacional de Parapléjicos in Toledo (HNPT) with patients suffering from spinal cord injuries (SCI) and healthy participants who are SCI specialists.

The quantitative results obtained demonstrate that this dynamic adjustment of the environment allows for adaptation that leads to a 100% success rate in task completion after the automatic adjustment, compared to a 62.5% success rate when using a configuration with virtual elements adapted to the motor capabilities of a healthy person (for both healthy participants and patients). This adjustment not only facilitates a greater number of exercise repetitions, but also reduces the time needed to access each object, with an average reduction in time of 47.94% across the entire sample. This reduction is even more significant when considering only the group of SCI patients, with a reduction of 53.78%. Additionally, the qualitative evaluation complements the study with a perception of ease of use for the calibration (mean = 1.29 ± 0.46) and low complexity in accessing the interactive objects after the automatic adjustment (mean = 1.12 ± 0.45). These results demonstrate the effectiveness of the proposed algorithms and the improved user experience.

## Organization

### 📁 Data Folder

Contains data collected from the sessions conducted at HNPT, including the file obtained after applying the calibration to a user of median height (166 cm). This file is used in the test for the standard configuration.

#### 📁 Data Participants Folder

Contains a subfolder for each participant identifier. "P" indicates a SCI patient, and "H" indicates a healthy participant. Within each subfolder:

- **📄 Historical.csv**: contains the historical data related to the detection and auto-adjustment test. Includes initial position and rotation of the HMD, tracking file paths, position of elements before and after applying the detection and auto-adjustment algorithm, number of blocks moved in standard and automatic configurations, and the time elapsed until the last block was moved.
- **📄 Calibrationhistory.json**: cumulative JSON historical data collected after calibration.
- **📄 Configuration.json**: resulting file from the calibration process of a healthy user with a height of 166 cm, used for the standard configuration.
- **📁 Tracking Data Folder**: 
  - **📄OculusTracking_XXX**: tracking file stored at the beginning of the test (fixed configuration).
  - **📄 Relocation_XXX.csv**: file with data obtained during the automatic mode execution.
  - **📄 RelocationDefault_XXX.csv**: file with data obtained during the standard mode execution.
#### 📄 CalibrationDefault.json

Contains the file obtained after applying the calibration to a user of median height (166 cm). This file is used in the test for the standard configuration.

#### 📊 Resume.md
File containing summary tables of the data obtained after the experimental sessions.

## Detailed Explanation of Files

### 📄 Historical.csv

- **Game**: contains the name of the application (Calibration).
- **TRACKING_FILE**: path to the generic tracking file, the one used at the beginning of the test (fixed configuration).
- **HMD_POSITION_X, HMD_POSITION_Y, HMD_POSITION_Z**: initial position of the HMD.
- **HMD_ROTATION_X, HMD_ROTATION_Y, HMD_ROTATION_Z**: initial rotation of the HMD.
- **RELOCATEDDEFAULTPOSITION_**: posición de los cinco bloques con la configuración estándar.
- **VODEFAULTMOVED_**: number of blocks moved in the standard configuration.
- **RELOCATEDPOSITION_**: position of the blocks after applying the detection and auto-adjustment algorithm.
- **VOCURRENTMOVED_**: number of blocks moved after the automatic adjustment.
- **RELOCATEDTRACKINGFILE**: path to the tracking file used during the automatic adjustment.
- **TIMERELOCATION**: time elapsed until the last block was moved in the automatic mode.
- **TIMERELOCATIONDEFAULT**: time elapsed until the last block was moved in the standard mode.



### 📄 Calibrationhistory.json
The asymmetric calibration generates needed data for the accurate replication of the workspace tailored to each patient. This data includes the position and rotation of the HMD at the start of calibration (HMD position and rotation), the coordinates of the right (right palm center) and left (left palm center) palms when the elbows are bent at 90 degrees centered to the patient (palm center position). Furthermore, the parameters of the ellipses defined for the three principal planes are included: the horizontal plane (horizontal ellipse) is described by the center (horizontal ellipse center) and the radii (horizontal ellipse radii); the frontal and sagittal planes (frontal and sagittal ellipses) are characterized by the center (frontal and sagittal ellipse center) and the radii (frontal and sagittal ellipse radii), where right and left sides are indicated.

### 📄 Configuration.json
Configuration file used for the execution of the test. For all participants, "gameOptions" is used with the following settings: 
```json
{
  "keys": ["Position", "Treshold", "HighRestriction"],
  "values": ["All", "90", "True"]
}
```
  - Position: entire area (right, left, and central).
  - Threshold: 90% of the participant's ROM.
  - Height restriction enabled.
### 📄 CalibrationDefault.json

```json
{
  "historyList": [
    {
      "date": "20240503_133821",
      "hmdInitialPosition": {"x": 0.0, "y": 0.0, "z": 0.0},
      "hmdInitialRotation": {"x": 0.0, "y": 0.0, "z": 0.0},
      "hmdCalibrationPosition": {"x": -0.004234354943037033, "y": 1.1844514608383179, "z": -0.033591628074645999},
      "hmdCalibrationRotation": {"x": 353.571533203125, "y": 3.091718912124634, "z": 357.5124206542969},
      "trunkDisplacementThreshold_X": 0.0,
      "trunkDisplacementThreshold_Z": 0.0,
      "neckFlexionThreshold_X": 0.0,
      "neckFlexionThreshold_Y": 0.0,
      "neckFlexionThreshold_Z": 0.0,
      "armSupinationThreshold": 0.0,
      "heightLimit": 0.0,
      "handCalibrationR": {
        "maxPosition": {"x": 0.778315544128418, "y": 1.6023156642913819, "z": 0.538240909576416},
        "minPosition": {"x": -0.10157419741153717, "y": 0.0, "z": 0.0},
        "maxRotation": {"x": 0.0, "y": 0.0, "z": 0.0},
        "minRotation": {"x": 0.0, "y": 0.0, "z": 0.0},
        "palmCenter": {"x": 0.12055854499340058, "y": 0.865615963935852, "z": 0.30123358964920046},
        "legHeight": {"x": 0.0, "y": 0.0, "z": 0.0},
        "ovalRadiusX": 0.439944863319397,
        "ovalRadiusY": 0.41582006216049197,
        "ovalRadiusZ": 0.538240909576416,
        "ovalCenter": {"x": 0.38795045018196108, "y": 1.1864956617355347, "z": 0.0},
        "ovalXZRadiusX": 0.3527584671974182,
        "ovalXZRadiusZ": 0.5688069462776184,
        "ovalXZCenter": {"x": 0.38795045018196108, "y": 1.047489881515503, "z": 0.0}
      },
      "handCalibrationL": {
        "maxPosition": {"x": 0.3439410924911499, "y": 1.5047670602798463, "z": 0.5346383452415466},
        "minPosition": {"x": -0.6854338645935059, "y": 0.0, "z": 0.0},
        "maxRotation": {"x": 0.0, "y": 0.0, "z": 0.0},
        "minRotation": {"x": 0.0, "y": 0.0, "z": 0.0},
        "palmCenter": {"x": -0.08009471744298935, "y": 0.8881590962409973, "z": 0.315422385931015},
        "legHeight": {"x": 0.0, "y": 0.0, "z": 0.0},
        "ovalRadiusX": 0.5146874785423279,
        "ovalRadiusY": 0.46288806200027468,
        "ovalRadiusZ": 0.5346383452415466,
        "ovalCenter": {"x": -0.12112224102020264, "y": 1.0418790578842164, "z": 0.0},
        "ovalXZRadiusX": 0.49084949493408205,
        "ovalXZRadiusZ": 0.5330077409744263,
        "ovalXZCenter": {"x": -0.12112224102020264, "y": 1.0444214344024659, "z": 0.0}
      }
    }
  ]
}
```
Note: The fields `trunkDisplacementThreshold_X`, `trunkDisplacementThreshold_Z`, `neckFlexionThreshold_X`, `neckFlexionThreshold_Y`, `neckFlexionThreshold_Z`, `armSupinationThreshold`, `maxRotation`, `minRotation`, `legHeight`, `hmdInitialPosition`, and `hmdInitialRotation` are being stored for future research purposes and are not currently utilized in the calibration process.


