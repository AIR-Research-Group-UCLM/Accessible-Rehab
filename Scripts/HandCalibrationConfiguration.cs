using System;
using UnityEngine;

namespace Calibration.AutomaticCalibration
{
    [Serializable]
    public class HandCalibrationConfiguration
    {
        public Vector3 maxPosition;
        public Vector3 minPosition;
        public Vector3 maxRotation;
        public Vector3 minRotation;
        public Vector3 palmCenter;
        public Vector3 legHeight;
        public float ovalRadiusX;
        public float ovalRadiusY;
        public float ovalRadiusZ;
        public Vector3 ovalCenter;
        public float ovalXZRadiusX;
        public float ovalXZRadiusZ;
        public Vector3 ovalXZCenter;

        public HandCalibrationConfiguration(Vector3 maxPosition, Vector3 minPosition, 
            Vector3 maxRotation, Vector3 minRotation, Vector3 palmCenter, 
            float ovalRadiusX, float ovalRadiusY, float ovalRadiusZ, Vector3 ovalCenter,  float ovalXZRadiusX, float ovalXZRadiusZ, Vector3 ovalXZCenter)
        {
            this.maxPosition = maxPosition;
            this.minPosition = minPosition;
            this.maxRotation = maxRotation;
            this.minRotation = minRotation;
            this.palmCenter = palmCenter;
            this.ovalRadiusX = ovalRadiusX;
            this.ovalRadiusY = ovalRadiusY;
            this.ovalRadiusZ = ovalRadiusZ;
            this.ovalCenter = ovalCenter;
            this.ovalXZRadiusX = ovalXZRadiusX;
            this.ovalXZRadiusZ = ovalXZRadiusZ;
            this.ovalXZCenter = ovalXZCenter;
        }
        public HandCalibrationConfiguration()
            : this(Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, 
                Vector3.zero, 0.0f, 0.0f, 0.0f, Vector3.zero,0.0f, 0.0f, Vector3.zero)
        {
        }
        public Vector3 MaxPosition
        {
            get => maxPosition;
            set => maxPosition = value;
        }

        public Vector3 LegHeight => legHeight;

        public Vector3 MinPosition
        {
            get => minPosition;
            set => minPosition = value;
        }

        public Vector3 MaxRotation
        {
            get => maxRotation;
            set => maxRotation = value;
        }

        public Vector3 MinRotation
        {
            get => minRotation;
            set => minRotation = value;
        }

        public Vector3 PalmCenter
        {
            get => palmCenter;
            set => palmCenter = value;
        }

        public float OvalRadiusX
        {
            get => ovalRadiusX;
            set => ovalRadiusX = value;
        }

        public float OvalRadiusY
        {
            get => ovalRadiusY;
            set => ovalRadiusY = value;
        }

        public float OvalRadiusZ
        {
            get => ovalRadiusZ;
            set => ovalRadiusZ = value;
        }

        public Vector3 OvalCenter
        {
            get => ovalCenter;
            set => ovalCenter = value;
        }
    }
}