using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Calibration.AutomaticCalibration
{
    [Serializable]
    public class CalibrationHistory
    {
        public List<CalibrationConfiguration> historyList;

        public CalibrationConfiguration GetLastCalibrationConfiguration()
        {
            return (historyList != null) ?  historyList[historyList.Count - 1] : new CalibrationConfiguration();
        }
        
    
        public CalibrationHistory()
        {
            historyList = new List<CalibrationConfiguration>();
        }
    }



    public class CalibrationHistoryManager
    {
        private CalibrationHistory calibrationHistory;
        private string filePath;

        public CalibrationHistory CalibrationHistory => calibrationHistory;


   
        public CalibrationHistoryManager(string filePath, string nameFile)
        {
            
            this.filePath = Path.Combine(filePath, nameFile);
            LoadCalibrationHistory();
        }
        
        public CalibrationHistoryManager(string filePath)
        {
            
            this.filePath = filePath;
            LoadCalibrationHistory();
        }

        public void AddCalibrationToHistory(CalibrationConfiguration calibration)
        {
            calibrationHistory.historyList.Add(calibration);
        }

        public void SaveCalibrationHistory()
        {
            string json = JsonUtility.ToJson(calibrationHistory);
            System.IO.File.WriteAllText(filePath, json);
        }

        private void LoadCalibrationHistory()
        {
            if (System.IO.File.Exists(filePath))
            {
                string json = System.IO.File.ReadAllText(filePath);
                calibrationHistory = JsonUtility.FromJson<CalibrationHistory>(json);
            }
            else
            {
                calibrationHistory = new CalibrationHistory();
            }
        }
    }
}