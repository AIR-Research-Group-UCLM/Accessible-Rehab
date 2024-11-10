using Calibration.AutomaticCalibration;
using UnityEngine;


namespace RehabImmersive
{
    public class GoalBox : MonoBehaviour
    {
        [SerializeField] private SOGameConfiguration gameConfig;
      
      
        [SerializeField] private AudioSource audioGoal;
        [SerializeField] private TestCalibrarion test;
        private float timeInside = 0f;
        private bool inside = false;
        
        private GameObject objetInTrigger;
        private int totalBlocks = 5;
        private int blocksMoved = 0;


        public void ResetGoal()
        {
            totalBlocks = 5;
            blocksMoved = 0;
            timeInside = 0;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(RehabConstants.BlockMesh))
            {
                inside = true;
                timeInside = 0f; // reset
                objetInTrigger = other.gameObject; 
            }
        }

        
        private void OnTriggerExit(Collider other)
        {
            if (objetInTrigger != null && other.gameObject == objetInTrigger)
            {
                inside = false;
                timeInside = 0f; // reset
            }
        }

        private void Update()
        {
            if (inside)

            {
                timeInside += Time.deltaTime;
                //block has moved into goal
                BlockAutroGrasp block;
                if (timeInside >= 1.0f) // 
                {
                    
                    block = objetInTrigger.GetComponentInParent<BlockAutroGrasp>();
                    if (block != null)
                    {
                        block.HasEntered = true;
                        //release block
                        block.ReleaseObjectBlock();
                        UpdateGoalBoxCorrectly(block.gameObject);
                        objetInTrigger.SetActive(false);
                      

                    }

                    timeInside = 0;

                }
            }
        }

        /**
         * Update the number of blocks moved correctly and play a sound
         */
        private void UpdateGoalBoxCorrectly(GameObject block)
        {
            gameConfig.moveCorrectly = true;
            audioGoal.Play();
            gameConfig.grabIdentier = "";
            UpdateBlocksGoal(block);
        }


        private void UpdateBlocksGoal(GameObject block)
        {
            
            block.SetActive(false);
            blocksMoved++;
            if (totalBlocks == blocksMoved)
            {
                test.LevelCompleted();
            }
        }

        public int BlocksMoved()
        {
            return blocksMoved;
        }

       
    }
}