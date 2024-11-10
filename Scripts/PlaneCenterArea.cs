using UnityEngine;

namespace Calibration.AutomaticCalibration
{
    public class PlaneCenterArea : MonoBehaviour
    {
        public bool isVisibleOnStart = false; 
        private Vector3 initialScale = Vector3.one;
        private Vector3 finalScale = Vector3.one;
        public float duration = 2.0f; 

        private float startTime;
        private bool animationActive;

        void Start()
        {
            // Set initial scale and visibility
            transform.localScale = initialScale;
            animationActive = false;
            gameObject.SetActive(isVisibleOnStart);
        }

        // Function to start the animation
        public void StartAnimation(Vector3 position, Vector3 initial, Vector3 end)
        {
            
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    
            // Calcular el centro en X pero mantener Y y Z de uno de los puntos, si deseas que no cambien
            Vector3 centerPosition1 = new Vector3((initial.x + end.x) / 2, initial.y, initial.z);
            cube.transform.position = centerPosition1;

            Vector3 direction1 = initial - end;
            cube.transform.localScale = new Vector3(direction1.magnitude, cube.transform.localScale.y, cube.transform.localScale.z);

            
            
            gameObject.SetActive(true);
            Vector3 centerPosition = (initial + end) / 2;
            transform.position = centerPosition;

            Vector3 direction = initial - end;
            transform.localScale = new Vector3(direction.magnitude, transform.localScale.y, transform.localScale.z);
            Debug.DrawLine(initial, end, Color.red, 1000);
            // Ajustar la rotación del cubo para alinearlo con los puntos
            transform.up = direction.normalized;
            
         /*   transform.position = position;
            initialScale = initial;
            finalScale = end;
            // Initialize the animation
            startTime = Time.time;
            animationActive = true;

            // Ensure the object is visible when animation starts
            gameObject.SetActive(true);*/
        }

        // Update is called once per frame
        void Update()
        {
            if (animationActive)
            {
                float timeElapsed = Time.time - startTime;
                float normalizedTime = Mathf.Clamp01(timeElapsed / duration);

                // Interpolate only the scale on the X axis
                float scaleX = Mathf.Lerp(initialScale.x, finalScale.x, normalizedTime);

                // Apply the resulting scale
                transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);

                // If the duration has passed, stop the animation
                if (timeElapsed >= duration)
                {
                    animationActive = false;

                   
                   
                }
            }
        }
    }
}