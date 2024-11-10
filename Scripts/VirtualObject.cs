using UnityEngine;

namespace Calibration.AutomaticCalibration
{
    /// <summary>
    /// Represents a virtual object in a 3D space, typically used in VR or AR environments.
    /// This class stores the position of the object and whether it is correctly positioned
    /// according to certain criteria or conditions.
    /// </summary>
    public class VirtualObject
    {
        /// <summary>
        /// The position of the object in the 3D space.
        /// </summary>
        public Vector3 NewPosition { get; set; }
        public GameObject gameObject { get; set; }
        public Vector3 OriginalPosition { get; set; }

        /// <summary>
        /// Indicates whether the object is correctly positioned.
        /// This can be used to track if the object is in the desired location or state.
        /// </summary>
        public bool IsCorrectlyPositioned { get; set; }

        /// <summary>
        /// Constructor to initialize a virtual object with a specified position.
        /// </summary>
        /// <param name="position">The initial position of the virtual object.</param>
        public VirtualObject(GameObject _gameObject)
        {
            gameObject = _gameObject;
            OriginalPosition = _gameObject.transform.position;
            IsCorrectlyPositioned = false; // Default to not correctly positioned.
        }

        /// <summary>
        /// Updates the position of the virtual object.
        /// </summary>
        /// <param name="newPosition">The new position for the virtual object.</param>
        public void UpdatePosition(Vector3 newPosition)
        {
            NewPosition = newPosition;
        }

        /// <summary>
        /// Sets the position status of the virtual object.
        /// </summary>
        /// <param name="status">True if the object is correctly positioned, false otherwise.</param>
        public void SetPositionStatus(bool status)
        {
            IsCorrectlyPositioned = status;
        }
    }
}