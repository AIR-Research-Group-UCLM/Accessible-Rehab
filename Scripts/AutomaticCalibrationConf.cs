using System;
using UnityEngine;

namespace Calibration.AutomaticCalibration
{
    [Serializable]
    public class AutomaticCalibrationConf: MonoBehaviour
    {
        
        [SerializeField]
        private bool considerHeightLimits;

        [SerializeField]
        private ExerciseArea currentExerciseArea;

        [SerializeField]
        private float threshold;
        /// <summary>
        /// Enumerates the possible exercise areas. These areas define specific zones in the VR space
        /// where activities or interactions are expected to take place.
        /// </summary>
        public enum ExerciseArea
        {
            Left,
            Right,
            Central,
            Global,
            CentralAndDown
        }
        
        // <summary>
        /// Indicates whether to consider height limits in the calibration process,
        /// such as ensuring objects are not positioned below a certain height (e.g., below the knees).
        /// </summary>
        public bool ConsiderHeightLimits { get; set; }
        
        
        public ExerciseArea CurrentExerciseArea { get; set; }
        /// <summary>
        /// The threshold for the workspace area in percentage.
        /// A value of 100% represents the limits set during the initial calibration.
        /// Values above or below 100% proportionally adjust the limits in the X, Y, and Z axes,
        /// allowing for flexible workspace configuration.
        /// </summary>
        public float Threshold { get; set; }

        /// <summary>
        /// Constructor to initialize the configuration with a specific threshold.
        /// </summary>
        /// <param name="threshold">The initial threshold percentage for workspace adjustment.
        /// Defaults to 100% if not specified.</param>
        public AutomaticCalibrationConf(float threshold = 100)
        {
            SetThreshold(threshold);
        }

        /// <summary>
        /// Sets the threshold for the workspace area. This method allows the threshold to be
        /// adjusted to values either greater or smaller than 100%, providing flexibility in
        /// workspace configuration.
        /// </summary>
        /// <param name="threshold">The threshold percentage for workspace adjustment.</param>
        public void SetThreshold(float threshold)
        {
            Threshold = threshold;
        }

        // Additional configurations and methods can be added here as needed...
    }

}