using System.Collections.Generic;

namespace Calibration.AutomaticCalibration
{
    [System.Serializable]
    public class SerializablePositionList
    {
        public List<ObjectPosition> positions;

        public SerializablePositionList(List<ObjectPosition> positions)
        {
            this.positions = positions;
        }
    }
}