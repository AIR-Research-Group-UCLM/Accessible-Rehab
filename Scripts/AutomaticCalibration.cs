using System;
using System.Collections.Generic;
using UnityEngine;

namespace Calibration.AutomaticCalibration
{
    /// <summary>
    /// Manages the automatic calibration of virtual objects in a VR environment.
    /// </summary>
    public class AutomaticCalibration
    {
        private readonly CalibrationConfiguration _calibrationConfig;
        private readonly AutomaticCalibrationConf _automaticCalibrationConf;
        private Vector3 _centerR, _centerL, _centerXZR, _centerXZL;
        private float _radiusXR, _radiusYR, _radiusZR;
        private float _radiusXHorizontalR, _radiusZHorizontalR;
        private float _radiusXL, _radiusYL, _radiusZL;
        private float _radiusXHorizontalL, _radiusZHorizontalL;
        private Vector3 _palmCenterR, _palmCenterL;
        private List<VirtualObject> _virtualObjects;
        private float _thresholdFactor;
        private float _heigthLimit;
        public HandCalibrationConfiguration handRModified;
        public HandCalibrationConfiguration handLModified;


        /// <summary>
        /// Initializes a new instance of AutomaticCalibration with calibration configurations,
        /// current HMD position, and a list of virtual objects.
        /// </summary>
        /// <param name="config">The calibration configuration settings.</param>
        /// <param name="hmdPosition">The current position of the HMD.</param>
        /// <param name="objects">List of virtual objects for calibration.</param>
        /// <param name="automaticCalibrationConf">Additional calibration settings.</param>
        public AutomaticCalibration(CalibrationConfiguration config, List<VirtualObject> objects,
            AutomaticCalibrationConf automaticCalibrationConf)
        {
            _calibrationConfig = config;
            _automaticCalibrationConf = automaticCalibrationConf;
            _virtualObjects = objects;
            _thresholdFactor = _automaticCalibrationConf.Threshold != 0 ? _automaticCalibrationConf.Threshold / 100f : 1;
            AdjustToHMDAndTreshold();
        }
        
        public AutomaticCalibration(CalibrationConfiguration config, List<GameObject> objects,
            AutomaticCalibrationConf automaticCalibrationConf)
        {
            _virtualObjects = new List<VirtualObject>();
            foreach (GameObject obj in objects)
            {
                VirtualObject virtualObj = new VirtualObject(obj);
                _virtualObjects.Add(virtualObj);
            }
            _calibrationConfig = config;
            _automaticCalibrationConf = automaticCalibrationConf;
            
            _thresholdFactor = _automaticCalibrationConf.Threshold != 0 ? _automaticCalibrationConf.Threshold / 100f : 1;
            AdjustToHMDAndTreshold();
        }
        public AutomaticCalibration(CalibrationConfiguration config,
            AutomaticCalibrationConf automaticCalibrationConf)
        {
            _calibrationConfig = config;
            _automaticCalibrationConf = automaticCalibrationConf;
            
            _thresholdFactor = _automaticCalibrationConf.Threshold != 0 ? _automaticCalibrationConf.Threshold / 100f : 1;
            AdjustToHMDAndTreshold();
        }

        public void SetVirtualObjects(List<VirtualObject> objects)
        {
            _virtualObjects = objects;
        }

        public void AdjustToHMDAndTreshold()
        {
            AdjustAreaToHmd(_calibrationConfig.hmdInitialPosition);
            ApplyThresholdToArea();
            handRModified = new HandCalibrationConfiguration(_calibrationConfig.HandCalibrationR.maxPosition,
                _calibrationConfig.HandCalibrationR.minPosition, _calibrationConfig.HandCalibrationR.maxPosition,
                _calibrationConfig.HandCalibrationR.minRotation, _palmCenterR, _radiusXR, _radiusYR, _radiusZR,
                _centerR, _radiusXHorizontalR, _radiusZHorizontalR,
                _centerXZR);

            handLModified = new HandCalibrationConfiguration(_calibrationConfig.HandCalibrationL.maxPosition,
                _calibrationConfig.HandCalibrationL.minPosition, _calibrationConfig.HandCalibrationL.maxPosition,
                _calibrationConfig.HandCalibrationL.minRotation, _palmCenterL, _radiusXL, _radiusYL, _radiusZL,
                _centerL, _radiusXHorizontalL, _radiusZHorizontalL,
                _centerXZL);
           
            Debug.Log($"CenterR: {_centerR}");
            Debug.Log($"RadiusXR: {_radiusXR}");
            Debug.Log($"RadiusYR: {_radiusYR}");
            Debug.Log($"RadiusZR: {_radiusZR}");
            Debug.Log($"PalmCenterL: {_palmCenterL}");
            Debug.Log($"CenterL: {_centerL}");
            Debug.Log($"RadiusXL: {_radiusXL}");
            Debug.Log($"RadiusYL: {_radiusYL}");
            Debug.Log($"RadiusZL: {_radiusZL}");
            Debug.Log($"HeigthLimit: {_heigthLimit}");
        }

        public HandCalibrationConfiguration HandRModified
        {
            get => handRModified;
            set => handRModified = value;
        }

        public HandCalibrationConfiguration HandLModified
        {
            get => handLModified;
            set => handLModified = value;
        }

        /// <summary>
        /// Checks the positions of all virtual objects against the defined exercise area and height limits.
        /// Marks objects as incorrectly positioned if they do not comply with the criteria.
        /// </summary>
        public void CheckAndMarkObjectsPositions()
        {
            foreach (var virtualObject in _virtualObjects)
            {
                // Check if the object is within the specified exercise area and meets height requirements
                bool isCorrectlyPositioned = IsObjectInArea(virtualObject.OriginalPosition);

                // Mark the object as incorrectly positioned if it does not meet the criteria
                virtualObject.IsCorrectlyPositioned = isCorrectlyPositioned;
            }
        }

        /// <summary>
        /// Applies the threshold factor to the calibration area.
        /// This method adjusts the palm centers, ellipse centers, and radii according to the threshold.
        /// </summary>
        private void ApplyThresholdToArea()
        {
            // Adjust palm centers
          //  _palmCenterR *= _thresholdFactor;
          //  _palmCenterL *= _thresholdFactor;

            // Adjust centers and radii
            _centerR *= _thresholdFactor;
            _centerL *= _thresholdFactor;
            _centerXZL *= _thresholdFactor;
            _centerXZR *= _thresholdFactor;
            _radiusXR *= _thresholdFactor;
            _radiusYR *= _thresholdFactor;
            _radiusZR *= _thresholdFactor;
            _radiusXL *= _thresholdFactor;
            _radiusYL *= _thresholdFactor;
            _radiusZL *= _thresholdFactor;
            _radiusXHorizontalR *= _thresholdFactor;
            _radiusZHorizontalR *= _thresholdFactor;
            _radiusXHorizontalL *= _thresholdFactor;
            _radiusZHorizontalL *= _thresholdFactor;
        }

        /// <summary>
        /// Determines if an object is inside an elliptical area in 3D space.
        /// Adjusts for the calibration threshold and uses global variables for ellipse parameters.
        /// </summary>
        /// <param name="objectPosition">3D position of the object to check.</param>
        /// <param name="isRightEllipse">Indicates if the right or left ellipse should be used for calculation.</param>
        /// <returns>True if the object is inside the specified elliptical area; otherwise, false.</returns>
        private bool IsInsideEllipse(Vector3 objectPosition, bool isRightEllipse)
        {
            Vector3 center = isRightEllipse ? _centerR : _centerL;
            Vector3 centerHorizontal = isRightEllipse ? _centerXZR : _centerXZL;
            float radiusX = isRightEllipse ? _radiusXR : _radiusXL;
            float radiusY = isRightEllipse ? _radiusYR : _radiusYL;
            float radiusZ = isRightEllipse ? _radiusZR : _radiusZL;
            float radiusXHorizontal = isRightEllipse ? _radiusXHorizontalR : _radiusXHorizontalL;
            float radiusZHorizontal = isRightEllipse ? _radiusZHorizontalR : _radiusZHorizontalL;

           
            bool insideXZ = Math.Round(Mathf.Pow((objectPosition.x - centerHorizontal.x) / radiusXHorizontal, 2) +
                Mathf.Pow((objectPosition.z - centerHorizontal.z) / radiusZHorizontal, 2), 4) <= 1;
            bool insideXY = Math.Round(Mathf.Pow((objectPosition.x - center.x) / radiusX, 2) +
                Mathf.Pow((objectPosition.y - center.y) / radiusY, 2), 4) <= 1;
            bool insideYZ = Math.Round(Mathf.Pow((objectPosition.y - center.y) / radiusY, 2) +
                Mathf.Pow((objectPosition.z - center.z) / radiusZ, 2), 4) <= 1;


            return (insideXZ && !(objectPosition.y > center.y)) || (insideXY && insideYZ);
        }

        /// <summary>
        /// Adjusts the calibration area based on the current position of the HMD (Head-Mounted Display).
        /// This method recalculates the centers and radii of the calibration ellipses for both hands
        /// by applying the displacement between the initial HMD position and the current HMD position.
        /// </summary>
        /// <param name="hmdPositionActual">The current position of the HMD.</param>
        private void AdjustAreaToHmd(Vector3 hmdPositionActual)
        {
            try
            {
                Vector3 displacement = hmdPositionActual - _calibrationConfig.HmdInitialPosition;
                _palmCenterR = _calibrationConfig.HandCalibrationR.PalmCenter + displacement;
                _palmCenterL = _calibrationConfig.HandCalibrationL.PalmCenter + displacement;

                _centerR = _calibrationConfig.HandCalibrationR.ovalCenter + displacement;
                _radiusXR = _calibrationConfig.HandCalibrationR.ovalRadiusX + displacement.x;
                _radiusYR = _calibrationConfig.HandCalibrationR.ovalRadiusY + displacement.y;
                _radiusZR = _calibrationConfig.HandCalibrationR.ovalRadiusZ + displacement.z;
                _palmCenterL = _calibrationConfig.HandCalibrationL.PalmCenter + displacement;

                _centerL = _calibrationConfig.HandCalibrationL.ovalCenter + displacement;
                _radiusXL = _calibrationConfig.HandCalibrationL.ovalRadiusX + displacement.x;
                _radiusYL = _calibrationConfig.HandCalibrationL.ovalRadiusY + displacement.y;
                _radiusZL = _calibrationConfig.HandCalibrationL.ovalRadiusZ + displacement.z;


                _centerXZL = _calibrationConfig.HandCalibrationL.ovalXZCenter + displacement;
                _centerXZR = _calibrationConfig.HandCalibrationR.ovalXZCenter + displacement;
                _radiusXHorizontalR = _calibrationConfig.HandCalibrationR.ovalXZRadiusX + displacement.x;
                _radiusZHorizontalR = _calibrationConfig.HandCalibrationR.ovalXZRadiusZ + displacement.z;
                _radiusXHorizontalL = _calibrationConfig.HandCalibrationL.ovalXZRadiusX + displacement.x;
                _radiusZHorizontalL = _calibrationConfig.HandCalibrationL.ovalXZRadiusZ + displacement.z;
                _heigthLimit = _calibrationConfig.handCalibrationR.palmCenter.y + displacement.y;


             
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error recalibrating area");
            }
        }


        /// <summary>
        /// Determines whether an object is within a specified exercise area.
        /// </summary>
        /// <param name="objectPosition">The 3D position of the object being checked.</param>
        /// <param name="exerciseArea">The exercise area to check against (Left, Right, Central, Global).</param>
        /// <returns>True if the object is within the specified area, otherwise false.</returns>
        public bool IsObjectInArea(Vector3 objectPosition)
        {
            bool isObjectInArea = false;

            switch (_automaticCalibrationConf.CurrentExerciseArea)
            {
                case AutomaticCalibrationConf.ExerciseArea.Left:
                {
                    isObjectInArea = IsInsideEllipse(objectPosition, false);
                    break;
                }
                case AutomaticCalibrationConf.ExerciseArea.Right:
                {
                    isObjectInArea = IsInsideEllipse(objectPosition, true);
                    break;
                }
                case AutomaticCalibrationConf.ExerciseArea.Central:
                {
                    isObjectInArea = IsInsideCentral(objectPosition);
                    break;
                }
                case AutomaticCalibrationConf.ExerciseArea.CentralAndDown:
                {
                    isObjectInArea = IsInsideCentralAndDown(objectPosition);
               
                    break;
                }
                
                case AutomaticCalibrationConf.ExerciseArea.Global:
                {
                    isObjectInArea = IsInsideOneEllipse(objectPosition);
                    break;
                }

                default:
                {
                    isObjectInArea = false;
                    break;
                }
            }

            isObjectInArea = isObjectInArea && _automaticCalibrationConf.ConsiderHeightLimits
                ? _heigthLimit <= objectPosition.y
                : isObjectInArea;
        
        


            return isObjectInArea;
        }


        private bool IsInsideOneEllipse(Vector3 objectPosition)
        {
            return IsInsideEllipse(objectPosition, true) ||
                   IsInsideEllipse(objectPosition, false);
        }

        /// <summary>
        /// Determines whether the given object position is within the central area, adjusted for the calibration threshold.
        /// The central area is defined by the X range between the adjusted right and left palm centers.
        /// </summary>
        /// <param name="objectPosition">The 3D position of the object to check.</param>
        /// <returns>True if the object is within the adjusted central area, otherwise false.</returns>
        public bool IsInsideCentral(Vector3 objectPosition)
        {
            // Aplicar el umbral al rango X de los centros de palma
            float minX = Mathf.Min(_palmCenterR.x, _palmCenterL.x);
            float maxX = Mathf.Max(_palmCenterR.x, _palmCenterL.x);

            bool isWithinXRange = objectPosition.x >= minX && objectPosition.x <= maxX;

            if (isWithinXRange)
            {
                return IsInsideOneEllipse(objectPosition);
            }

            return false;
        }
    
        public bool IsInsideCentralAndDown(Vector3 objectPosition)
        {
            return IsInsideCentral(objectPosition) && objectPosition.y == ((_palmCenterR.y + _palmCenterL.y)/2);
        }


        /// <summary>
        /// Relocates virtual objects that are not correctly positioned within the defined workspace.
        /// This method calculates new positions for objects based on the recalibrated ellipses and the defined threshold.
        /// </summary>
        public void RelocateObjects()
        {
            foreach (var vObject in _virtualObjects)
            {
                if (!vObject.IsCorrectlyPositioned)
                {
                    Vector3 newPosition = CalculateNewPosition(vObject.OriginalPosition);
                    vObject.UpdatePosition(newPosition);
                }
            }
        }

        private Vector3 CalculateNewPosition(Vector3 originalPosition)
        {
            Vector3 newPosition = originalPosition;

            switch (_automaticCalibrationConf.CurrentExerciseArea)
            {
                case AutomaticCalibrationConf.ExerciseArea.Left:
                    newPosition = FindNearestPointInEllipse(
                        originalPosition, false
                    );
                    break;
                case AutomaticCalibrationConf.ExerciseArea.Right:
                    newPosition = FindNearestPointInEllipse(
                        originalPosition, true
                    );
                    break;
                case AutomaticCalibrationConf.ExerciseArea.Central:
                    newPosition = FindNearestPointInCentralArea(originalPosition);
                    break;
                case AutomaticCalibrationConf.ExerciseArea.CentralAndDown:
                    newPosition =  FindNearestPointInCentralAreaAndDown(originalPosition);
                    break;
                case AutomaticCalibrationConf.ExerciseArea.Global:
                    newPosition = FindNearestPointInGlobalArea(originalPosition);
                    break;
            }


            if (_automaticCalibrationConf.ConsiderHeightLimits && _heigthLimit >  originalPosition.y)
            {
                newPosition.y = _heigthLimit;
            }

            return newPosition;
        }


        private bool IsInsideEllipseOnXYPlane(Vector3 point, Vector3 center, float radiusX, float radiusY)
        {
            return Math.Round(Mathf.Pow((point.x - center.x) / radiusX, 2) + Mathf.Pow((point.y - center.y) / radiusY, 2), 4) <= 1;
        }

        private bool IsInsideEllipseOnYZPlane(Vector3 point, Vector3 center, float radiusY, float radiusZ)
        {
            return Math.Round(Mathf.Pow((point.y - center.y) / radiusY, 2) + Mathf.Pow((point.z - center.z) / radiusZ, 2), 4) <= 1;
        }

        private bool IsInsideEllipseOnXZPlane(Vector3 point, Vector3 center, float radiusXHorizontal,
            float radiusZHorizontal)
        {
            return Math.Round(Mathf.Pow((point.x - center.x) / radiusXHorizontal, 2) +
                Mathf.Pow((point.z - center.z) / radiusZHorizontal, 2), 4) <= 1;
        }

        private Vector3 FindNearestPointInEllipse1(Vector3 point, bool isRightEllipse)
        {
            Vector3 centerXY_YZ = isRightEllipse ? _centerR : _centerL;
            Vector3 centerXZ = isRightEllipse ? _centerXZR : _centerXZL;
            float radiusX = isRightEllipse ? _radiusXR : _radiusXL;
            float radiusY = isRightEllipse ? _radiusYR : _radiusYL;
            float radiusZ = isRightEllipse ? _radiusZR : _radiusZL;
            float radiusXHorizontal = isRightEllipse ? _radiusXHorizontalR : _radiusXHorizontalL;
            float radiusZHorizontal = isRightEllipse ? _radiusZHorizontalR : _radiusZHorizontalL;

            Vector3 nearestPoint = point;


            // Ajustar solo las coordenadas que están fuera del óvalo
            if (!IsInsideEllipseOnXYPlane(point, centerXY_YZ, radiusX, radiusY))
            {
                Vector3 nearestPointXY = FindNearestPointOnXYPlane(point, centerXY_YZ, radiusX, radiusY);
                nearestPoint.x = nearestPointXY.x;
                nearestPoint.y = nearestPointXY.y;
            }

            if (!IsInsideEllipseOnYZPlane(point, centerXY_YZ, radiusY, radiusZ))
            {
                Vector3 nearestPointYZ = FindNearestPointOnYZPlane(point, centerXY_YZ, radiusY, radiusZ);
                nearestPoint.y = nearestPointYZ.y; // Solo ajustar Y si es necesario
                nearestPoint.z = nearestPointYZ.z;
            }

            if (!IsInsideEllipseOnXZPlane(point, centerXZ, radiusXHorizontal, radiusZHorizontal))
            {
                Vector3 nearestPointXZ = FindNearestPointOnXZPlane(point, centerXZ, radiusXHorizontal, radiusZHorizontal);
                nearestPoint.x = nearestPointXZ.x; // Solo ajustar X si es necesario
                nearestPoint.z = nearestPointXZ.z;
            }

            return nearestPoint;
        }

        private Vector3 FindNearestPointInEllipse(Vector3 point, bool isRightEllipse)

        {
            // Definir el centro y los radios de los óvalos
            Vector3 centerXY_YZ = isRightEllipse ? _centerR : _centerL;
            Vector3 centerXZ = isRightEllipse ? _centerXZR : _centerXZL;
            float radiusX = isRightEllipse ? _radiusXR : _radiusXL;
            float radiusY = isRightEllipse ? _radiusYR : _radiusYL;
            float radiusZ = isRightEllipse ? _radiusZR : _radiusZL;
            float radiusXHorizontal = isRightEllipse ? _radiusXHorizontalR : _radiusXHorizontalL;
            float radiusZHorizontal = isRightEllipse ? _radiusZHorizontalR : _radiusZHorizontalL;


            // Nearest point to original 
            Vector3 nearestPoint = point;

            if (point.y > centerXY_YZ.y)
            {
                // Verificar y ajustar --> si no está dentro del óvalo en el eje Y del plano XY
                if (!IsInsideEllipseOnXYPlane(point, centerXY_YZ, radiusX, radiusY))
                {
                    nearestPoint = FindNearestPointOnXYPlane(nearestPoint, centerXY_YZ, radiusX, radiusY);
                }


                // verificar y ajustar Z --> si no está dentro del óvalo en el eje Z
                if (!IsInsideEllipseOnYZPlane(nearestPoint, centerXY_YZ, radiusY, radiusZ))
                {
                    nearestPoint = FindNearestPointOnYZPlane(nearestPoint, centerXY_YZ, radiusY, radiusZ);
                }
            }
            else
            {
                // Verificar y ajustar X --> si no está dentro del óvalo en el eje X
                if (!IsInsideEllipseOnXZPlane(point, centerXZ, radiusXHorizontal, radiusZHorizontal))
                {
                    nearestPoint = FindNearestPointOnXZPlane(point, centerXZ, radiusXHorizontal, radiusZHorizontal);
                }
            }


            return nearestPoint;
        }


        private Vector3 FindNearestPointOnXAxis1(Vector3 point, Vector3 center, float radiusX, float radiusY, float radiusZ)
        {
            // Asumimos que estamos trabajando con el óvalo en el plano XY
            // Mantenemos Y y Z constantes
            float nearestX;
            if (Mathf.Pow((point.y - center.y) / radiusY, 2) + Mathf.Pow((point.z - center.z) / radiusZ, 2) <= 1)
            {
                // Si el punto está dentro del óvalo en Y y Z, ajustamos solo X
                float deltaX = point.x - center.x;
                nearestX = center.x + Mathf.Clamp(deltaX, -radiusX, radiusX);
            }
            else
            {
                // Si el punto está fuera del óvalo, lo colocamos en el borde más cercano en el eje X
                nearestX = (point.x > center.x) ? center.x + radiusX : center.x - radiusX;
            }

            return new Vector3(nearestX, point.y, point.z);
        }

        private Vector3 FindNearestPointOnYAxis1(Vector3 point, Vector3 center, float radiusX, float radiusY, float radiusZ)
        {
            // Asumimos que estamos trabajando con el óvalo en el plano XY
            // Mantenemos X y Z constantes
            float nearestY;
            if (Mathf.Pow((point.x - center.x) / radiusX, 2) + Mathf.Pow((point.z - center.z) / radiusZ, 2) <= 1)
            {
                // Si el punto está dentro del óvalo en X y Z, ajustamos solo Y
                float deltaY = point.y - center.y;
                nearestY = center.y + Mathf.Clamp(deltaY, -radiusY, radiusY);
            }
            else
            {
                // Si el punto está fuera del óvalo, lo colocamos en el borde más cercano en el eje Y
                nearestY = (point.y > center.y) ? center.y + radiusY : center.y - radiusY;
            }

            return new Vector3(point.x, nearestY, point.z);
        }

        private Vector3 FindNearestPointOnZAxis1(Vector3 point, Vector3 center, float radiusX, float radiusY, float radiusZ)
        {
            // Asumimos que estamos trabajando con el óvalo en el plano XZ
            // Mantenemos X y Y constantes
            float nearestZ;
            if (Mathf.Pow((point.x - center.x) / radiusX, 2) + Mathf.Pow((point.y - center.y) / radiusY, 2) <= 1)
            {
                // Si el punto está dentro del óvalo en X y Y, ajustamos solo Z
                float deltaZ = point.z - center.z;
                nearestZ = center.z + Mathf.Clamp(deltaZ, -radiusZ, radiusZ);
            }
            else
            {
                // Si el punto está fuera del óvalo, lo colocamos en el borde más cercano en el eje Z
                nearestZ = (point.z > center.z) ? center.z + radiusZ : center.z - radiusZ;
            }

            return new Vector3(point.x, point.y, nearestZ);
        }

        private Vector3 FindNearestPointOnXAxis1(Vector3 point, Vector3 center, float radiusX)
        {
            float deltaX = point.x - center.x;
            float nearestX = center.x + Mathf.Clamp(deltaX, -radiusX, radiusX);
            return new Vector3(nearestX, point.y, point.z);
        }

        private Vector3 FindNearestPointOnYAxis1(Vector3 point, Vector3 center, float radiusY)
        {
            float deltaY = point.y - center.y;
            float nearestY = center.y + Mathf.Clamp(deltaY, -radiusY, radiusY);
            return new Vector3(point.x, nearestY, point.z);
        }

        private Vector3 FindNearestPointOnZAxis1(Vector3 point, Vector3 center, float radiusZ)
        {
            float deltaZ = point.z - center.z;
            float nearestZ = center.z + Mathf.Clamp(deltaZ, -radiusZ, radiusZ);
            return new Vector3(point.x, point.y, nearestZ);
        }

        private Vector3 FindNearestPointInEllipseIterativo(Vector3 point, bool isRightEllipse)
        {
            Vector3 centerXY_YZ = isRightEllipse ? _centerR : _centerL;
            Vector3 centerXZ = isRightEllipse ? _centerXZR : _centerXZL;
            float radiusX = isRightEllipse ? _radiusXR : _radiusXL;
            float radiusY = isRightEllipse ? _radiusYR : _radiusYL;
            float radiusZ = isRightEllipse ? _radiusZR : _radiusZL;
            float radiusXHorizontal = isRightEllipse ? _radiusXHorizontalR : _radiusXHorizontalL;
            float radiusZHorizontal = isRightEllipse ? _radiusZHorizontalR : _radiusZHorizontalL;

            // Comienza con el punto original
            Vector3 nearestPoint = point;

            // Ajusta cada coordenada individualmente si es necesario
            if (!IsInsideEllipseOnXYPlane(nearestPoint, centerXY_YZ, radiusX, radiusY))
            {
                nearestPoint = FindNearestPointOnXYPlane(nearestPoint, centerXY_YZ, radiusX, radiusY);
                if (!IsInsideEllipseOnYZPlane(nearestPoint, centerXY_YZ, radiusY, radiusZ) ||
                    !IsInsideEllipseOnXZPlane(nearestPoint, centerXZ, radiusXHorizontal, radiusZHorizontal))
                {
                    nearestPoint.y = point.y; // Vuelve a la coordenada Y original si ajustar X y Z no es suficiente
                }
            }

            if (!IsInsideEllipseOnYZPlane(nearestPoint, centerXY_YZ, radiusY, radiusZ))
            {
                nearestPoint = FindNearestPointOnYZPlane(nearestPoint, centerXY_YZ, radiusY, radiusZ);
                if (!IsInsideEllipseOnXZPlane(nearestPoint, centerXZ, radiusXHorizontal, radiusZHorizontal) ||
                    !IsInsideEllipseOnXYPlane(nearestPoint, centerXY_YZ, radiusX, radiusY))
                {
                    nearestPoint.z = point.z; // Vuelve a la coordenada Z original si ajustar X y Y no es suficiente
                }
            }

            if (!IsInsideEllipseOnXZPlane(nearestPoint, centerXZ, radiusXHorizontal, radiusZHorizontal))
            {
                nearestPoint = FindNearestPointOnXZPlane(nearestPoint, centerXZ, radiusXHorizontal, radiusZHorizontal);
                if (!IsInsideEllipseOnXYPlane(nearestPoint, centerXY_YZ, radiusX, radiusY) ||
                    !IsInsideEllipseOnYZPlane(nearestPoint, centerXY_YZ, radiusY, radiusZ))
                {
                    nearestPoint.x = point.x; // Vuelve a la coordenada X original si ajustar Y y Z no es suficiente
                }
            }

            return nearestPoint;
        }

        private Vector3 FindNearestPointOnXYPlane(Vector3 point, Vector3 center, float radiusX, float radiusY)
        {
            float angle = Mathf.Atan2(point.y - center.y, point.x - center.x);
            return new Vector3(center.x + radiusX * Mathf.Cos(angle), center.y + radiusY * Mathf.Sin(angle), point.z);
        }

        private Vector3 FindNearestPointOnYZPlane(Vector3 point, Vector3 center, float radiusY, float radiusZ)
        {
            float angle = Mathf.Atan2(point.z - center.z, point.y - center.y);
            return new Vector3(point.x, center.y + radiusY * Mathf.Cos(angle), center.z + radiusZ * Mathf.Sin(angle));
        }

        private Vector3 FindNearestPointOnXZPlane(Vector3 point, Vector3 center, float radiusXHorizontal,
            float radiusZHorizontal)
        {
            float angle = Mathf.Atan2(point.z - center.z, point.x - center.x);
            return new Vector3(center.x + radiusXHorizontal * Mathf.Cos(angle), point.y,
                center.z + radiusZHorizontal * Mathf.Sin(angle));
        }


// Métodos adicionales para calcular la posición más cercana en cada eje

        /// <summary>
        /// Calculates the nearest point on the border of an ellipse to a given point, considering whether it's the right or left ellipse.
        /// The method determines the appropriate ellipse (XY, YZ, or XZ) based on the object's Y-axis position relative to the ellipse's center.
        /// </summary>
        /// <param name="point">The original point from which to find the nearest point on the ellipse.</param>
        /// <param name="isRightEllipse">Indicates whether the calculation is for the right ellipse (true) or left ellipse (false).</param>
        /// <returns>The nearest point on the border of the specified ellipse to the given point.</returns>

        /*   private Vector3 FindNearestPointInEllipse(Vector3 point,bool isRightEllipse)
       {



               Vector3 centerXY_YZ = isRightEllipse ? _centerR : _centerL;
               Vector3 centerXZ = isRightEllipse ? _centerXZR : _centerXZL;
               float radiusX = isRightEllipse ? _radiusXR : _radiusXL;
               float radiusY = isRightEllipse ? _radiusYR : _radiusYL;
               float radiusZ = isRightEllipse ? _radiusZR : _radiusZL;
               float radiusXHorizontal = isRightEllipse ? _radiusXHorizontalR : _radiusXHorizontalL;
               float radiusZHorizontal = isRightEllipse ? _radiusZHorizontalR : _radiusZHorizontalL;


           Vector3 relativePosition = point - centerXY_YZ;

           if (point.y > centerXY_YZ.y)
           {
               float angleXY = Mathf.Atan2(relativePosition.y, relativePosition.x);
               float angleYZ = Mathf.Atan2(relativePosition.y, relativePosition.z);

               Vector3 nearestPointXY = new Vector3(centerXY_YZ.x + radiusX * Mathf.Cos(angleXY),
                   centerXY_YZ.y + radiusY * Mathf.Sin(angleXY),
                   point.z);

               Vector3 nearestPointYZ = new Vector3(point.x,
                   centerXY_YZ.y + radiusY * Mathf.Cos(angleYZ),
                   centerXY_YZ.z + radiusZ * Mathf.Sin(angleYZ));

               return (Vector3.Distance(point, nearestPointXY) < Vector3.Distance(point, nearestPointYZ))
                   ? nearestPointXY
                   : nearestPointYZ;
           }
           else
           {
               relativePosition = point - centerXZ;
               float angleXZ = Mathf.Atan2(relativePosition.z, relativePosition.x);
               return new Vector3(centerXZ.x + radiusXHorizontal * Mathf.Cos(angleXZ),
                   point.y,
                   centerXZ.z + radiusZHorizontal * Mathf.Sin(angleXZ));
           }
       }

   */
        // <summary>
        /// Calculates the nearest point within either the right or left ellipse areas for a given object in the global exercise area.
        /// This method finds the nearest points on both the right and left ellipses and determines which one is closer to the object's original position.
        /// It is used when the object is to be positioned within a global area where both right and left sides are considered.
        /// </summary>
        /// <param name="originalPosition">The original position of the object in the 3D space.</param>
        /// <returns>The nearest point within the global area (either in the right or left ellipse) to the original position.</returns>
        private Vector3 FindNearestPointInGlobalArea(Vector3 originalPosition)
        {
            // Calculate nearest points to ellipses
            Vector3 nearestPointRight = FindNearestPointInEllipse(
                originalPosition, true
            );

            Vector3 nearestPointLeft = FindNearestPointInEllipse(
                originalPosition, false
            );

            // Determine which point is closest to the original position.
            float distanceToRight = Vector3.Distance(originalPosition, nearestPointRight);
            float distanceToLeft = Vector3.Distance(originalPosition, nearestPointLeft);

            return distanceToRight < distanceToLeft ? nearestPointRight : nearestPointLeft;
        }


        /// <summary>
        /// Calculates the nearest point within the central area for a given object.
        /// The central area is defined by the X range between the adjusted right and left palm centers.
        /// This method finds the nearest points on both the right and left ellipses and determines which one is closer to the object's original position.
        /// It is used when the object is to be positioned within the central area where both right and left sides are considered.
        /// </summary>
        /// <param name="originalPosition">The original position of the object in the 3D space.</param>
        /// <returns>The nearest point within the central area to the original position, or the original position if it's already within the central area.</returns>
        private Vector3 FindNearestPointInCentralArea(Vector3 originalPosition)
        {
      
            Vector3 nearestPositionX = originalPosition;

            Vector3 nearestPointRight = FindNearestPointInEllipse(
                originalPosition, true
            );

            Vector3 nearestPointLeft = FindNearestPointInEllipse(
                originalPosition, false
            );

            //calculate nearest position X

            nearestPositionX.x = GetNearestPositionX(nearestPositionX);

            if (!IsInsideOneEllipse(nearestPositionX))
            {
                float distanceToRight = Vector3.Distance(originalPosition, nearestPointRight);
                float distanceToLeft = Vector3.Distance(originalPosition, nearestPointLeft);

                nearestPositionX = distanceToRight < distanceToLeft ? nearestPointRight : nearestPointLeft;
            }


            return nearestPositionX;
        }

        private float GetNearestPositionX( Vector3 position)
        {
            float minX = Mathf.Min(_palmCenterR.x, _palmCenterL.x) * _thresholdFactor;
            float maxX = Mathf.Max(_palmCenterR.x, _palmCenterL.x) * _thresholdFactor;
            if (position.x < minX)
            {
                return  minX;
            }
            else if (position.x > maxX)
            {
                return  maxX;
            }
            else
            {
                return position.x;
            }
        }
    
        private Vector3 FindNearestPointInCentralAreaAndDown(Vector3 originalPosition)
        {
       
            Vector3 nearestPositionX = originalPosition;
            nearestPositionX.x = GetNearestPositionX(nearestPositionX);

            nearestPositionX.y = (_palmCenterR.y + _palmCenterL.y) / 2;
      

            return nearestPositionX;
        }
    }
}