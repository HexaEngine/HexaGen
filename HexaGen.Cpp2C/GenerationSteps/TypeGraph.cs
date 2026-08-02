namespace HexaGen.Cpp2C.GenerationSteps
{
    using HexaGen.CppAst.Model.Declarations;
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class TypeGraph
    {
        private readonly List<TypeNode> nodes = [];
        private readonly Dictionary<CppClass, TypeNode> typeToNode = [];

        public TypeNode GetOrCreate(CppClass type)
        {
            if (!typeToNode.TryGetValue(type, out var node))
            {
                node = new(nodes.Count, type);
                typeToNode.Add(type, node);
                nodes.Add(node);
            }
            return node;
        }

        public bool TryGetNode(CppClass type, [NotNullWhen(true)] out TypeNode? node)
        {
            return typeToNode.TryGetValue(type, out node);
        }

        public bool TryAddDependency(TypeNode target, CppClass type)
        {
            if (TryGetNode(type, out var node))
            {
                target.AddDependency(node);
                return true;
            }
            return false;
        }

        private struct State
        {
            public bool Visited;
            public bool OnStack;
        }

        public IReadOnlyList<TypeNode> Nodes => nodes;

        public List<TypeNode> TopologicalSort()
        {
            State[] states = new State[nodes.Count];
            List<TypeNode> output = new(nodes.Count);

            Stack<(TypeNode node, IEnumerator<TypeNode> deps)> stack = new();

            foreach (var start in nodes)
            {
                if (states[start.Id].Visited)
                    continue;

                stack.Push((start, start.Dependencies.GetEnumerator()));

                while (stack.Count > 0)
                {
                    var (node, it) = stack.Peek();

                    ref var state = ref states[node.Id];

                    if (!state.Visited)
                    {
                        state.Visited = true;
                        state.OnStack = true;
                    }

                    if (it.MoveNext())
                    {
                        var depNode = it.Current;

                        ref var depState = ref states[depNode.Id];

                        if (depState.OnStack)
                            throw new Exception("Circular value-type dependency detected");

                        if (!depState.Visited)
                        {
                            stack.Push((depNode, depNode.Dependencies.GetEnumerator()));
                        }
                    }
                    else
                    {
                        // all deps processed
                        stack.Pop();
                        state.OnStack = false;
                        output.Add(node);
                    }
                }
            }

            return output;
        }
    }
}