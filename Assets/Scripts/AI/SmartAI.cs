using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Pathfinding;

namespace TH.Core
{

    public class SmartAI : MonoBehaviour
    {
        #region PublicVariables
        public bool HasItem => _hand != null;
        public bool needWork = false;
        #endregion

        #region PrivateVariables
        private const float DANGER_HEALTH = 0.3f;

        private ComponentGetter<Animal> _mainBody =
            new ComponentGetter<Animal>(TypeOfGetter.This);
        private ComponentGetter<PlayerItemGetter> _itemGetter =
            new ComponentGetter<PlayerItemGetter>(TypeOfGetter.This);

        private ItemData _hand = null;

        private bool _isGoingToSalesBox = false;
        private bool _isGoingToTarget = false;
        private float _idleTimeStamp = 0;
        [ShowInInspector] private WorldObject _target;

        private BTNode _BehaviorTree;
        private bool _hashit = false;
        #endregion

        #region PublicMethod
        public void SetHandItem(ItemData item)
        {
            _hand = item;
        }
        #endregion

        #region PrivateMethod
        private void Update()
        {
            _itemGetter.Get(gameObject).PickOne();

            //if (_mainBody.Get(gameObject).HasHit)
            //{
            //    _BehaviorTree.Abort();
            //}

            _BehaviorTree.Evaluate();
        }

        private void Start()
        {
            SetBehavior();
            _mainBody.Get(gameObject).Init("Male", Vector2Int.zero, (x, y) => { });
        }

        private void SetBehavior()
        {
            _BehaviorTree = new BTSelectorNode
                (
                    new List<BTNode>
                    {
                        new BTSequenceNode
                        (
                            new List<BTNode>
                            {
                                new BTLeafNode(()=>true, HasItemOnHand),
                                new BTLeafNode(()=>true, HandleHandItem),
                            }
                        ),

                        new BTSequenceNode
                        (
                            new List<BTNode>
                            {
                                new BTLeafNode(()=>true, IsDangerHealth),
                                new BTSelectorNode
                                (
                                    new List<BTNode>
                                    {
                                        new BTLeafNode(()=>true, EatBerry),
                                        new BTSequenceNode
                                        (
                                            new List<BTNode>
                                            {
                                                new BTLeafNode(()=>true, CheckReachedTarget),
                                                new BTLeafNode(()=>true, Attack),
                                            }
                                        ),
                                        new BTLeafNode(()=>true, GoBerry),
                                    }
                                )
                            }
                        ),

                        new BTSequenceNode
                        (
                            new List<BTNode>
                            {
                                new BTLeafNode(()=>true, HasToWork),
                                new BTSelectorNode
                                (
                                    new List<BTNode>
                                    {
                                        new BTSequenceNode
                                        (
                                            new List<BTNode>
                                            {
                                                new BTLeafNode(()=>true, CheckReachedTarget),
                                                new BTLeafNode(()=>true, Attack),
                                            }
                                        ),
                                        new BTLeafNode(()=>true, GoOre),
                                    }
                                ),
                            }
                        ),

                        new BTLeafNode(()=>true, Idle),
                    }
                );
        }

        private Collider2D[] CheckAround(float scanDistance)
        {
            Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, scanDistance, 1 << LayerMask.NameToLayer("Target"));
            return hit;
        }

        private BTNode.NodeState HasToWork()
        {
            if (needWork)
            {
                return BTNode.NodeState.SUCCESS;
            }

            return BTNode.NodeState.FAILURE;
        }

        private BTNode.NodeState HasItemOnHand()
        {
            if (HasItem)
            {
                Debug.Log(_hand.ItemID);
                return BTNode.NodeState.SUCCESS;
            } else
            {
                return BTNode.NodeState.FAILURE;
            }
        }

        private BTNode.NodeState HandleHandItem()
        {
            if (!HasItem)
            {
                return BTNode.NodeState.FAILURE;
            }

            if (_hand.ItemID == "Berry" &&
                _mainBody.Get(gameObject).HP < _mainBody.Get(gameObject).MaxHP * DANGER_HEALTH)
            {
                _hand = null;
                _mainBody.Get(gameObject).Heal(10);

                return BTNode.NodeState.SUCCESS;
            }

            SalesBox salesBox = FindObjectOfType<SalesBox>();

            if ((transform.position - salesBox.transform.position).magnitude < 1.3f)
            {
                salesBox.Sell(_hand, 1);
                _hand = null;
                _isGoingToSalesBox = false;
                return BTNode.NodeState.SUCCESS;
            }

            if (_isGoingToSalesBox == false)
            {
                _mainBody.Get(gameObject).GoToObject(salesBox.transform);
                _isGoingToSalesBox = true;
            }

            return BTNode.NodeState.RUNNING;
        }

        private BTNode.NodeState HasHit()
        {
            if (_mainBody.Get(gameObject).HasHit)
            {
                return BTNode.NodeState.SUCCESS;
            }
            else
            {
                return BTNode.NodeState.FAILURE;
            }
        }

        private BTNode.NodeState Idle()
        {
            if (_idleTimeStamp < Time.time)
            {
                _mainBody.Get(gameObject).Idle();
                _idleTimeStamp = Time.time + 3f;
            }
            return BTNode.NodeState.SUCCESS;
        }

        private BTNode.NodeState IsDangerHealth()
        {
            Debug.Log("HP" + _mainBody.Get(gameObject).HP + " " + _mainBody.Get(gameObject).MaxHP * DANGER_HEALTH);
            if (_mainBody.Get(gameObject).HP < _mainBody.Get(gameObject).MaxHP * DANGER_HEALTH)
            {
                return BTNode.NodeState.SUCCESS;
            }

            return BTNode.NodeState.FAILURE;
        }

        private BTNode.NodeState CheckReachedTarget()
        {
            if (_isGoingToTarget == true && _target != null)
            {
                if ((transform.position - _target.transform.position).magnitude <= 1.2)
                {
                    _isGoingToTarget = false;
                    return BTNode.NodeState.SUCCESS;
                }
            }

            return BTNode.NodeState.FAILURE;
        }

        private BTNode.NodeState Attack()
        {
            if (_target == null)
            {
                return BTNode.NodeState.FAILURE;
            }

            _mainBody.Get(gameObject).AttackObject(_target);
            return BTNode.NodeState.SUCCESS;
        }
 
        private BTNode.NodeState GoBerry()
        {
            if (_target == null || _target.ObjectID != "BerryBush")
            {
                for (int i = 0; i < 3; i++)
                {
                    Collider2D[] colliders = CheckAround(10f + 10 * i);
                    foreach (var c in colliders)
                    {
                        WorldObject wo = c.GetComponent<WorldObject>();
                        if (wo != null && wo.ObjectID == "BerryBush")
                        {
                            _target = wo;
                            _mainBody.Get(gameObject).GoToObject(_target.transform);

                            _isGoingToTarget = true;
                            return BTNode.NodeState.SUCCESS;
                        }
                    }
                }
            }

            if (_target == null)
            {
                return BTNode.NodeState.SUCCESS;
            }

            if (_isGoingToTarget == false)
            {
                Debug.Log(_target);
                _mainBody.Get(gameObject).GoToObject(_target.transform);
                _isGoingToTarget = true;
            }

            return BTNode.NodeState.SUCCESS;
        }

        private BTNode.NodeState GoOre()
        {
            if (_target == null || _target.ObjectID == "BerryBush")
            {
                for (int i = 0; i < 3; i++)
                {
                    Collider2D[] colliders = CheckAround(10f + 10 * i);
                    foreach (var c in colliders)
                    {
                        WorldObject wo = c.GetComponent<WorldObject>();
                        if (wo != null && wo.ObjectID != "BerryBush" && _mainBody.Get(gameObject) != wo)
                        {
                            _target = wo;
                            _mainBody.Get(gameObject).GoToObject(_target.transform);

                            _isGoingToTarget = true;
                            return BTNode.NodeState.SUCCESS;
                        }
                    }
                }
            }

            if (_isGoingToTarget == false || GetComponent<AIDestinationSetter>().target != _target.transform)
            {
                _mainBody.Get(gameObject).GoToObject(_target.transform);
                _isGoingToTarget = true;
            }

            return BTNode.NodeState.SUCCESS;
        }

        private BTNode.NodeState EatBerry()
        {
            if (_hand == null || _hand.ItemID != "Berry")
            {
                _hand = null;
                return BTNode.NodeState.FAILURE;
            }

            _hand = null;
            _mainBody.Get(gameObject).Heal(10);
            return BTNode.NodeState.SUCCESS;
        }
        #endregion
    }

}