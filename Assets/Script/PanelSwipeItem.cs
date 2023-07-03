using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace product_name
{
    public class PanelSwipeItem : MonoBehaviour
    {
        #region PUBLIC_VARS

        public Animator characterAnimator;
        public AudioSource audioSource;

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region PROTECTED_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SelectItem()
        {
            characterAnimator.enabled = true;
            audioSource.Play();
        }
        
        public void DeSelectItem()
        {
            characterAnimator.enabled = false;
            audioSource.Stop();
        }
        
        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region PROTECTED_FUNCTIONS

        #endregion

        #region OVERRIDE_FUNCTIONS

        #endregion

        #region VIRTUAL_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region UI_CALLBACKS

        #endregion
    }
}