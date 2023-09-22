using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TH.Core
{
    public class BTNode
    {
        public enum NodeState
        {
            RUNNING,
            SUCCESS,
            FAILURE
        }

        #region PUBLIC_VARIABLES
        #endregion

        #region PRIVATE_VARIABLES
        protected List<BTNode> _children;
        #endregion

        #region PUBLIC_METHOD
        #endregion

        #region PRIVATE_METHOD
        #endregion
    }

    public class BTRoot: BTNode
    {
        #region PUBLIC_VARIABLES
        #endregion

        #region PRIVATE_VARIABLES
        #endregion

        #region PUBLIC_METHOD
        #endregion

        #region PRIVATE_METHOD
        #endregion
    }

    public class BTSequenceNode: BTNode
    {
        #region PUBLIC_VARIABLES

        #endregion

        #region PRIVATE_VARIABLES
        #endregion

        #region PUBLIC_METHOD
        #endregion

        #region PRIVATE_METHOD
        #endregion
    }

    public class BTSelectorNode: BTNode
    {

    }

    public class BTLeafNode: BTNode
    {
        #region PUBLIC_VARIABLES
        #endregion

        #region PRIVATE_VARIABLES
        protected Func<bool> _precondition;
        protected Func<IEnumerator> _targetAction;
        #endregion

        #region PUBLIC_METHOD
        public bool Evaluate()
        {
            if (_precondition() == false)
            {
                return false;
            }

            
        }
        #endregion

        #region PRIVATE_METHOD
        #endregion
    }
}
