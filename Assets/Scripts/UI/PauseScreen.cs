using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    
    public class PauseScreen : MonoBehaviour
    {
        [SerializeField] StarterAssetsInputs _input;
        public MainMenuManager mainMenuManager;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {
            if (_input != null)
            {
                if (_input.escape)
                {
                    mainMenuManager.EnablePauseScreen();
                }
            }
        }
    }
}

