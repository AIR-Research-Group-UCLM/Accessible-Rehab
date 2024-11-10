using UnityEngine;

namespace Calibration.AutomaticCalibration
{
    [System.Serializable]
    public class ObjectPosition
    {
        public float x;
        public float y;
        public float z;

        public ObjectPosition(Vector3 position)
        {
            x = position.x;
            y = position.y;
            z = position.z;
        }
    }

}