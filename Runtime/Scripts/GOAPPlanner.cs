#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using System;
using System.Collections.Generic;
using CZToolKit.ObjectPool;

namespace CZToolKit.GOAP
{
    public class GOAPPlanner
    {
        /// <summary> 节点对象池，节点对象重复利用 </summary>
        private GOAPNodePool NodePool { get; } = new GOAPNodePool();
        private StackPool<GOAPActionVM> Stack_Pool { get; } = new StackPool<GOAPActionVM>();
        private DictionaryPool<string, bool> DictionaryObjPool { get; } = new DictionaryPool<string, bool>();

        GOAPNode root;
        GOAPNode cheapestNode;
        List<GOAPNode> leaves = new List<GOAPNode>();
        List<GOAPActionVM> usableActions = new List<GOAPActionVM>();

        public GOAPNode CheapestNode { get { return cheapestNode; } }

        /// <summary> 定制最优计划 </summary>
        /// <param name="_agent">代理</param>
        /// <param name="_availableActions">所有可用行为</param>
        /// <param name="_currentStates">当前状态</param>
        /// <param name="_goal">目标状态，想要达到的状态</param>
        public void Plan(GOAPActionVM[] _availableActions,
            Dictionary<string, bool> _currentStates, GOAPGoal _goal, int _maxDepth, ref Queue<GOAPActionVM> _plan)
        {
            if (_currentStates.TryGetValue(_goal.Key, out bool value) && value.Equals(_goal.Value))
                return;

            NodePool.Dispose();

            // 所有可用的行为
            usableActions.Clear();
            foreach (var action in _availableActions)
            {
                if (action.IsUsable())
                {
                    action.DynamicallyEvaluateCost();
                    usableActions.Add(action);
                }
            }

            // 根节点
            root = NodePool.Spawn(null, 0, _currentStates, null);
            // 所有能达到目标的节点
            leaves.Clear();
            // 成本最低的计划节点
            cheapestNode = null;

            // 如果通过构建节点树找到了能够达成目标的计划
            if (BuildGraph(root, usableActions, _goal, 0, _maxDepth, leaves))
            {
                Stack<GOAPActionVM> goapActionStack = Stack_Pool.Spawn();

                foreach (GOAPNode leaf in leaves)
                {
                    if (cheapestNode == null)
                        cheapestNode = leaf;
                    else if (cheapestNode.runningCost > leaf.runningCost)
                        cheapestNode = leaf;
                }

                // 向上遍历并添加行为到栈中，直至根节点，因为从后向前遍历
                while (cheapestNode != null)
                {
                    goapActionStack.Push(cheapestNode.action);

                    if (cheapestNode.parent.action != null)
                        cheapestNode = cheapestNode.parent;
                    else
                        break;
                }

                //goapActions = new Queue<Action>();
                // 再将栈压入到队列中
                while (goapActionStack.Count > 0)
                {
                    _plan.Enqueue(goapActionStack.Pop());
                }
                Stack_Pool.Recycle(goapActionStack);
            }

            // 用完回收所有对象
            DictionaryObjPool.Dispose();
        }

        /// <summary> 构建树并返回所有计划 </summary>
        /// <param name="_parent">父节点，不可为空</param>
        /// <param name="_usableActions">所有可行行为</param>
        /// <param name="_goal">目标计划</param>
        /// <param name="_leaves">已找到的所有计划</param>
        /// <returns>是否找到计划</returns>
        private bool BuildGraph(GOAPNode _parent, List<GOAPActionVM> _usableActions, GOAPGoal _goal, int _depth, int _maxDepth, List<GOAPNode> _leaves)
        {
            if (_maxDepth >= 1 && _depth >= _maxDepth)
                return false;

            foreach (GOAPActionVM action in _usableActions)
            {
                // 不允许出现两个连续的相同行为
                if (_parent == null || action == _parent.action || !action.IsProceduralPrecondition(_parent.state))
                    continue;

                if (InState(_parent.state, action.Preconditions))
                {
                    // 造成效果影响当前状态
                    Dictionary<string, bool> currentState = PopulateState(_parent.state, action.Effects);

                    // 生成动作完成的节点链，成本累加
                    GOAPNode node = NodePool.Spawn(_parent, _parent.runningCost + action.Cost, currentState, action);

                    // 如果当前状态能够达成目标
                    if (currentState.TryGetValue(_goal.Key, out bool value) && value.Equals(_goal.Value))
                        _leaves.Add(node);
                    else
                        BuildGraph(node, _usableActions, _goal, ++_depth, _maxDepth, _leaves);
                }
            }

            return _leaves.Count > 0;
        }

        /// <summary> 返回一个新的修改过的状态 </summary>
        /// <param name="_currentStates">当前状态</param>
        /// <param name="_effects">行为效果</param>
        private Dictionary<string, bool> PopulateState(Dictionary<string, bool> _currentStates, IReadOnlyList<GOAPState> _effects)
        {
            Dictionary<string, bool> newStates = DictionaryObjPool.Spawn();
            newStates.Clear();
            foreach (var state in _currentStates)
            {
                newStates[state.Key] = state.Value;
            }

            foreach (var effect in _effects)
            {
                newStates[effect.Key] = effect.Value;
            }

            return newStates;
        }

        /// <summary> 当前状态是否达成目标(_currentStates是否包含所有的_goals) </summary>
        /// <param name="_currentStates"></param>
        /// <param name="_conditions"></param>
        /// <returns></returns>
        public static bool InState(Dictionary<string, bool> _currentStates, IReadOnlyList<GOAPState> _conditions)
        {
            foreach (GOAPState goal in _conditions)
            {
                // 如果_currentStates不包含_goals，则认为不能达成目标
                if (!_currentStates.TryGetValue(goal.Key, out bool value) && value.Equals(true))
                    return false;
                // 如果包含的值不等于_goal值，则认为不能达成目标
                if (!goal.Value.Equals(value))
                    return false;
            }

            return true;
        }

        public class GOAPNode
        {
            public GOAPNode parent;

            /// <summary> 运行到此节点的总成本 </summary>
            public float runningCost;

            /// <summary> 此节点代表的行为 </summary>
            public GOAPActionVM action;

            /// <summary> 运行到此节点时的当前状态 </summary>
            public Dictionary<string, bool> state;

            public GOAPNode() { }

            public GOAPNode(GOAPNode _parent, float _runningCost, Dictionary<string, bool> _state, GOAPActionVM _action)
            {
                parent = _parent;
                runningCost = _runningCost;
                state = _state;
                action = _action;
            }
        }

        public class GOAPNodePool : ObjectPool<GOAPNode>
        {
            protected override GOAPNode Create()
            {
                return new GOAPNode();
            }

            protected override void OnRecycle(GOAPNode unit)
            {
                unit.parent = null;
                unit.runningCost = 0;
                unit.state = null;
                unit.action = null;
            }

            public GOAPNode Spawn(GOAPNode parent, float runningCost, Dictionary<string, bool> state, GOAPActionVM action)
            {
                GOAPNode unit = base.Spawn();
                unit.parent = parent;
                unit.runningCost = runningCost;
                unit.state = state;
                unit.action = action;
                return unit;
            }
        }

        public class DictionaryPool<K, V> : ObjectPool<Dictionary<K, V>>
        {
            protected override Dictionary<K, V> Create()
            {
                return new Dictionary<K, V>();
            }

            protected override void OnRecycle(Dictionary<K, V> unit)
            {
                unit.Clear();
            }
        }

        public class StackPool<T> : ObjectPool<Stack<T>>
        {
            protected override Stack<T> Create()
            {
                return new Stack<T>(8);
            }

            protected override void OnRecycle(Stack<T> unit)
            {
                unit.Clear();
            }
        }
    }
}