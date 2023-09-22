using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TH.Core
{
    public class BTController: MonoBehaviour
    {
        #region PUBLIC_VARIABLES
        #endregion

        #region PRIVATE_VARIABLES
        private BTNode _root;
        #endregion

        #region PUBLIC_METHOD
        /// <summary>
        /// Evaluate target Behavior Tree
        /// </summary>
        /// <returns>true if Behavior Tree completed its behavior</returns>
        public bool Evaluate() {
            if (_root == null) {
                Debug.LogError("BTController.Evaluate() failed: _root is null");
            }
            
            BTNode.NodeState result = _root.Evaluate();

            if (result == BTNode.NodeState.FAILURE) {
                Debug.LogError("BTController.Evaluate() failed");
            }
            return result == BTNode.NodeState.SUCCESS;
        }
        #endregion

        #region PRIVATE_METHOD
        #endregion
    }
}