using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace TH.Core
{
    public abstract class BTNode
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
        public abstract NodeState Evaluate();

        public abstract void Abort();
        #endregion

        #region PRIVATE_METHOD
        #endregion
    }

    public abstract class BTInternalNode: BTNode {
        protected bool _isRunningChild = false;
        protected int _runningIndex = 0;

        public override void Abort()
        {
            Debug.Log("abort");
            _isRunningChild = false;
            _runningIndex = 0;

            foreach (var n in _children)
            {
                n.Abort();
            }
        }
    }

    public class BTSequenceNode: BTInternalNode
    {
        public BTSequenceNode(List<BTNode> children) {
            _children = children;
        }

        #region PUBLIC_VARIABLES
        #endregion

        #region PRIVATE_VARIABLES
        #endregion

        #region PUBLIC_METHOD
        public override NodeState Evaluate()
        {
            if (_children == null || _children.Count == 0) {
                return NodeState.FAILURE;
            }

            if (_isRunningChild == false) {
                _runningIndex = 0;
                _isRunningChild = true;
            }

            if (_runningIndex >= _children.Count) {
                _isRunningChild = false;
                _runningIndex = 0;
                return NodeState.SUCCESS;
            }

            Debug.Log("run sequence " + _runningIndex);
            BTNode child = _children[_runningIndex];
            NodeState result = child.Evaluate();

            if (result == NodeState.RUNNING) {
                return NodeState.RUNNING;
            }

            if (result == NodeState.SUCCESS) {
                _runningIndex++;
                Debug.Log("onsuccess sequence " + _runningIndex);
                return NodeState.RUNNING;
            }

            if (result == NodeState.FAILURE) {
                _isRunningChild = false;
                _runningIndex = 0;
                return NodeState.FAILURE;
            }

            _isRunningChild = false;
            _runningIndex = 0;
            return NodeState.FAILURE;
        }
        #endregion

        #region PRIVATE_METHOD
        #endregion
    }

    public class BTSelectorNode: BTInternalNode
    {
        public enum SelectionPolicy {
            RANDOM,
            SEQUENTIAL
        }

        public BTSelectorNode(List<BTNode> children, SelectionPolicy policy=SelectionPolicy.SEQUENTIAL) {
            _children = children;
            _policy = policy;
        }

        #region PUBLIC_VARIABLES
        #endregion

        #region PRIVATE_VARIABLES
        protected List<BTNode> _runningSequence;
        protected SelectionPolicy _policy;
        #endregion

        #region PUBLIC_METHOD
        public override NodeState Evaluate() {
            Debug.Log("selector call");
            if (_children == null || _children.Count == 0) {
                Debug.Log("selector fail");
                return NodeState.FAILURE;
            }

            if (_isRunningChild == false) {
                if (_policy == SelectionPolicy.SEQUENTIAL)
                    _runningSequence = _children;
                else if (_policy == SelectionPolicy.RANDOM)
                    _runningSequence = _children.OrderBy(a => Guid.NewGuid()).ToList();
                _runningIndex = 0;
                _isRunningChild = true;
            }

            if (_runningIndex >= _children.Count) {
                _isRunningChild = false;
                return NodeState.FAILURE;
            }

            Debug.Log("run selector " + _runningIndex);
            BTNode child = _runningSequence[_runningIndex];
            NodeState result = child.Evaluate();

            if (result == NodeState.RUNNING) {
                return NodeState.RUNNING;
            }

            if (result == NodeState.SUCCESS) {
                _isRunningChild = false;
                _runningIndex = 0;
                return NodeState.SUCCESS;
            }

            if (result == NodeState.FAILURE) {
                _runningIndex++;
                Debug.Log("onfail selector " + _runningIndex);
                return NodeState.RUNNING;
            }

            _isRunningChild = false;
            return NodeState.FAILURE;
        }
        #endregion

        #region PRIVATE_METHOD
        #endregion
    }

    public class BTLeafNode: BTNode
    {
        public BTLeafNode(Func<bool> preCondition, Func<NodeState> targetAction) {
            _preCondition = preCondition;
            _targetAction = targetAction;

            _children = null;
        }

        #region PUBLIC_VARIABLES
        #endregion

        #region PRIVATE_VARIABLES
        protected Func<bool> _preCondition;
        protected Func<NodeState> _targetAction;
        #endregion

        #region PUBLIC_METHOD
        public override NodeState Evaluate()
        {
            if (_preCondition() == false)
            {
                return NodeState.FAILURE;
            }

            if (_targetAction == null) {
                return NodeState.FAILURE;
            }

            NodeState result = _targetAction();
            Debug.Log(_targetAction.Method.Name + " " + result);
            return result;
        }

        public override void Abort() { }
        #endregion

        #region PRIVATE_METHOD
        #endregion
    }
}
