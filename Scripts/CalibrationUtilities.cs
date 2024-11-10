using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Calibration.AutomaticCalibration
{
    public class CalibrationUtilities
    {
        private CalibrationConfiguration _generalConfig;

        public CalibrationUtilities(CalibrationConfiguration generalConfig)
        {
            _generalConfig = generalConfig;
        }

        public bool IsInsideOvals(Vector3 objectPosition, bool isRight)
        {
            return IsInsideOvalXY(objectPosition, isRight) && 
                   IsInsideOvalXZ(objectPosition, isRight) && 
                   IsInsideOvalYZ(objectPosition, isRight);
        }

        private bool IsInsideOvalXY(Vector3 objectPosition, bool isRight)
        {
            HandCalibrationConfiguration handConfig = isRight ? _generalConfig.HandCalibrationR : _generalConfig.HandCalibrationL;
            return Mathf.Pow((objectPosition.X - handConfig.OvalCenter.x) / handConfig.OvalRadiusX, 2) + 
                Mathf.Pow((objectPosition.Y - handConfig.OvalCenter.y) / handConfig.OvalRadiusY, 2) <= 1;
        }

        private bool IsInsideOvalXZ(Vector3 objectPosition, bool isRight)
        {
            HandCalibrationConfiguration handConfig = isRight ? _generalConfig.HandCalibrationR : _generalConfig.HandCalibrationL;
            return Mathf.Pow((objectPosition.X - handConfig.OvalCenter.x) / handConfig.OvalRadiusX, 2) + 
                Mathf.Pow((objectPosition.Z - handConfig.OvalCenter.z) / handConfig.OvalRadiusZ, 2) <= 1;
        }

        private bool IsInsideOvalYZ(Vector3 objectPosition, bool isRight)
        {
            HandCalibrationConfiguration handConfig = isRight ? _generalConfig.HandCalibrationR : _generalConfig.HandCalibrationL;
            return Mathf.Pow((objectPosition.Y - handConfig.OvalCenter.y) / handConfig.OvalRadiusY, 2) + 
                Mathf.Pow((objectPosition.Z - handConfig.OvalCenter.z) / handConfig.OvalRadiusZ, 2) <= 1;
        }
    }
}
