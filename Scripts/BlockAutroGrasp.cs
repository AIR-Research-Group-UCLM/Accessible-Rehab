using System;
using Calibration.AutomaticCalibration;
using Oculus.Interaction.HandGrab;

using UnityEngine;
using UnityEngine.SceneManagement;


namespace RehabImmersive
{
    public class BlockAutroGrasp : MonoBehaviour
    {
        [SerializeField] private SOGameConfiguration gameConfig;


        protected bool hasEntered;

      //  [SerializeField] private HandGrabInteractor _handGrabR;
       // [SerializeField] private HandGrabInteractor _handGrabL;
      private HandGrabInteractor _handGrabR;
      private HandGrabInteractor _handGrabL;
        [SerializeField] private string identifier;
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private AudioSource audioGrab;
        [SerializeField] private AudioSource audioRelease;
        public AutomaticCalibration calibration;
        [SerializeField] private  Material materialOK;
        [SerializeField] private  Material materialKO;
        
        private bool soundGrabbed;
        private bool IsRight { get; set; }


        void Start()
        {
            identifier = Guid.NewGuid().ToString();
            HasEntered = false;
            soundGrabbed = true;
            IsRight = true;
         
            
        }

       public void Initialize(HandGrabInteractor handGrabR, HandGrabInteractor handGrabL)
        {
            _handGrabR = handGrabR;
            _handGrabL = handGrabL;
        }
        public void SetCalibration(AutomaticCalibration autoCalibration)
        {
            calibration = autoCalibration;
        }
        public void Update()
        {
            if (gameConfig.grabIdentier.Equals(identifier)
                && !HasEntered)
            {
                Vector3 palm = IsRight ? _handGrabR.HandGrabApi.GetPalmCenter() :_handGrabL.HandGrabApi.GetPalmCenter();
                var transform1 = transform;
                transform1.position = palm;
                transform1.rotation = _handGrabR.HandGrabApi.transform.rotation;
                
                //check
               
            }

            if (calibration != null)
            {
                //GetComponentInChildren<Renderer>().material = materialKO;
                if (!calibration.IsObjectInArea(transform.position))
               {
                    calibration.IsObjectInArea(transform.position);
                   Debug.Log("-------------------" + transform.position); 
               }
                GetComponentInChildren<Renderer>().material = calibration.IsObjectInArea(transform.position) ? materialOK : materialKO;
            }

        }


        public void GrabObjectBlock()
        {
            //set grab identifer
            gameConfig.grabIdentier = identifier;
        }

        public void ReleaseObjectBlock()
        {
            audioRelease.Play();
            gameConfig.grabIdentier = "";
            particle.transform.position = gameObject.transform.position; 
            particle.Play();
            
        }


        private void OnTriggerEnter(Collider other)
        {
           
            if (_handGrabL != null && _handGrabR != null &&
                gameConfig.grabIdentier.Equals("") &&
                other != null && other.gameObject != null
                && other.gameObject.name.EndsWith("CapsuleCollider")
                && !HasEntered)
            {
                if (soundGrabbed)
                {
                    audioGrab.Play();
                    soundGrabbed = false;
                }

                IsRight = GetScenePathFromCollider(other).Contains("Right");
                //set the actual block is grabbed
                gameConfig.grabIdentier = identifier;
            }
        }


        string GetScenePathFromCollider(Collider collider)
        {
            Transform parentTransform = collider.transform.parent;
            string scenePath = "";

            while (parentTransform != null)
            {
                scenePath = parentTransform.name + "/" + scenePath;
                parentTransform = parentTransform.parent;
            }

            scenePath = SceneManager.GetActiveScene().name + "/" + scenePath;

            return scenePath;
        }
        public bool HasEntered
        {
            get => hasEntered;
            set => hasEntered = value;
        }
    }


}