using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DeepCrawlSims.BattleControl
{

    /// <summary>
    /// Topological wizard is present in every scene and calculates a topological order of StatusEffect elements
    /// according to their requirements.
    /// </summary>
    public class TopologicalWizard
    {
        private List<Component> components;
        private Dictionary<Type, List<Type>> adj = new Dictionary<Type, List<Type>>();
        private List<Type> answers;

        /// <summary>
        /// Given a list of all possible StatusEffects and their requirements for ordering, calculate a possible topological order for them.
        /// </summary>
        private void CalculateTopologicalOrder()
        {
            foreach (var eff in components)
            {
                adj[eff.GetType()] = new List<Type>();
                var ret = eff.GetRequirements();
                if (ret != null)
                {
                    foreach (var item in ret)
                    {
                        if (!adj[item.Item1].Contains(item.Item2))
                        {
                            adj[item.Item1].Add(item.Item2);
                        }
                    }
                }
            }
            answers = TopologicalSort();
        }

        /// <summary>
        /// The function to do Topological Sort.
        /// It uses recursive topologicalSortUtil()
        /// </summary>
        /// <returns>Sorted list of Status Effects</returns>
        List<Type> TopologicalSort()
        {
            List<Type> stack = new List<Type>();

            // Mark all the vertices as not visited
            var visited = new Dictionary<Type, bool>();
            foreach (var item in components)
            {
                visited.Add(item.GetType(), false);
            }

            // Call the recursive helper function
            // to store Topological Sort starting
            // from all vertices one by one
            foreach (var eff in components)
            {
                if (visited[eff.GetType()] == false)
                    TopologicalSortUtil(eff.GetType(), visited, stack);
            }
            stack.Reverse();

            return stack;
        }

        /// <summary>
        /// A recursive function used by TopologicalSort.
        /// </summary>
        /// <param name="v">Current graph vertex</param>
        /// <param name="visited">All visited vertices </param>
        /// <param name="stack">All the remaining vertices to sort</param>
        void TopologicalSortUtil(Type v, Dictionary<Type, bool> visited, List<Type> stack)
        {

            // Mark the current node as visited.
            visited[v] = true;

            // Recur for all the vertices
            // adjacent to this vertex
            foreach (var vertex in adj[v])
            {
                if (!visited[vertex])
                    TopologicalSortUtil(vertex, visited, stack);
            }

            // Push current vertex to
            // stack which stores result
            stack.Add(v);
        }

        public int Compare(Type a, Type b)
        {
            return (answers.IndexOf(a).CompareTo(answers.IndexOf(b)));
        }
    }

}
