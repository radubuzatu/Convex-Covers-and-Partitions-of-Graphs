using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctorat
{
    public class Set : HashSet<int>, IEquatable<Set>, ICloneable
    {
        public Set() : base() { }

        public Set(IEnumerable<int> set) : this()
        {
            foreach (var el in set)
                this.Add(el);
        }

        public Set(Set set) : this(set.AsEnumerable()) { }
    
        public Set Intersect(Set set)
        {
            return new Set(this.Intersect(set));
        }

        public Set Union(Set set)
        {
            return new Set(this.Union(set));
        }

        public Set Except(Set set)
        {
            return new Set(this.Except(set));
        }

        public bool Equals(Set set) 
        {
            return this.IsSubsetOf(set) && this.IsSupersetOf(set);
        }

        public object Clone()
        {
            return new Set(this);
        }

        public override string ToString()
        {
            string str = "";
            foreach (var el in this)
            {
                str += " " + el;
            }
            return str;
        }
    }

    public class ConvexCover : HashSet<Set>, IEquatable<ConvexCover>, ICloneable
    {
        public Graph graph;

        public ConvexCover(Graph graph) : base()
        {
            this.graph = graph;
        }

        public ConvexCover(ConvexCover convexCover) : this(convexCover.graph)
        {
            foreach (var set in convexCover)
                this.Add((Set)set.Clone());
        }

        public void AddSet(Set set)
        {
            Add(set);
        }

        public void RemoveSet(Set set)
        { 
            Remove(set);
        }

        public void ReduceToConvexCover()
        {
            Set allVertices = graph.GetAllVertices();
            //remove set of all vertices
            if (Contains(allVertices))
                RemoveSet(allVertices);
            //remove all sets contained in union of other sets
            while (!IsEverySetIsNotContainedInUnionOfOtherSets())
            {
                Set setToRemove = new Set();
                int flag = 0;
                foreach (var set1 in this)
                {
                    Set localSet = new Set();
                    foreach (var set2 in this)
                        if (set1 != set2)
                            localSet = localSet.Union(set2);
                    if (localSet.IsSupersetOf(set1))
                    {
                        setToRemove = set1;
                        flag = 1;
                        break;
                    }
                }
                if (flag == 1)
                    Remove(setToRemove);
            }
        }

        public bool IsEverySetIsNotContainedInUnionOfOtherSets()
        {
            foreach (var set1 in this)
            {
                Set localSet = new Set();
                foreach (var set2 in this)
                    if (set1 != set2)
                        localSet = localSet.Union(set2);
                if (localSet.IsSupersetOf(set1))
                    return false;
            }
            return true;
        }

        public bool IsUnionEqualsToAllVertices()
        {
            Set localSet = new Set();
            foreach (var set in this)
                localSet = localSet.Union(set);
            if (!graph.GetAllVertices().Equals(localSet))
                return false;
            else return true;
        }

        public bool IsEverySetConvex()
        {
            foreach (var set in this)
                if (!graph.IsConvex(set))
                    return false;
            return true;
        }

        public bool IsReducibleToConvexCover()
        {
            ConvexCover convexCover = new ConvexCover(this);
            Set allVertices = graph.GetAllVertices();
            Set localSet = new Set();
            //remove set of all vertices
            if (convexCover.Contains(allVertices))
                convexCover.RemoveSet(allVertices);
            //verify if every set in convex
            if (!convexCover.IsEverySetConvex())
                return false;
            //verify if X(G) is equal with union of all sets
            if (!convexCover.IsUnionEqualsToAllVertices())
                return false;
            return true;
        }

        public bool IsConvexCover()
        {
            //verify if every set in convex
            if (!IsEverySetConvex())
                return false;
            //verify if X(G) is equal with union of all sets
            if (!IsUnionEqualsToAllVertices())
                return false;
            //verify if every set is not contained in union of other sets
            if (!IsEverySetIsNotContainedInUnionOfOtherSets())
                return false;
            else return true;
        }

        public bool IsConvexPartition()
        {
            //verify if family of sets is a convex cover
            if (!IsConvexCover())
                return false;
            //verify if all sets are disjoin
            foreach (var set1 in this)
                foreach (var set2 in this)
                    if (set1.Intersect(set2).Count >= 1)
                        return false;
            return true;
        }

        public bool IsEverySetNontrivial()
        {
            //determine if all sets have at least 3 elements
            foreach (var set in this)
                if (set.Count <= 2)
                    return false;
            return true;
        }

        public bool IsNontrivialConvexCover()
        {
            return IsConvexCover() && IsEverySetNontrivial(); 
        }

        public bool IsnontrivialConvexPartition()
        {
           return IsConvexPartition() && IsEverySetNontrivial();
        }

        public bool Equals(Set set)
        {
            return this.IsSubsetOf(set) && this.IsSupersetOf(set);
        }

        public object Clone()
        {
            return new Set(this);
        }

        public override string ToString()
        {
            string str = "";
            foreach (var el in this)
            {
                str += " " + el;
            }
            return str;
        }
    }

    public enum GraphType
    {
        FGraph, JGraph, HPrimGraph, HPrimPrimGraph, OtherGraph
    }

    public class Graph : Dictionary<int, Set>, IEquatable<Graph>
    {
        public Graph() : base() { }

        public Graph(Set vertexSet) : this()
        {
            foreach (var vertex in vertexSet)
                Add(vertex, new Set());
        }

        public Graph(Graph graf) : this()
        {
            foreach (var vertex in graf.Keys)
                Add(vertex, (Set)graf[vertex].Clone());
        }

        public void AddVertex(int vertex)
        {
            this.Add(vertex, new Set());
        }

        public void RemoveVertex(int vertex)
        {
            this.Remove(vertex);
            //remove all references
            foreach (var el in this[vertex])
                this[el].Remove(vertex);
            this.Remove(vertex);
        }

        public bool HasVertex(int vertex)
        {
            return Keys.Contains(vertex);
        }

        public void AddVertices(Set vertices)
        {
            foreach (var el in vertices)
                this.AddVertex(el);
        }

        public void RemoveVertices(Set vertices)
        {
            foreach (var el in vertices)
                this.RemoveVertex(el);
        }

        public void AddEdge(int from, int to)
        {
            this[from].Add(to);
            this[to].Add(from);
        }

        public void RemoveEdge(int from, int to)
        {
            this[from].Remove(to);
            this[to].Remove(from);
        }

        public void AddNeighbors(int vertex, Set neighbors)
        {
            this[vertex].UnionWith(neighbors);
        }

        public void RemoveNeighbors(int vertex, Set neighbors)
        {
            this[vertex].Except(neighbors);
        }

        public Set GetNeighbors(int vertex)
        {
            return new Set(this[vertex]);
        }

        public Set GetAllVertices()
        {
            return new Set(this.Keys);
        }       

        public Graph GetInducedSubGraph(Set vertices)
        {
            Graph localGraf = new Graph(vertices);
            foreach (var vertex in vertices)
                localGraf[vertex] = this[vertex].Intersect(vertices);
            return localGraf;
        }

        public int GetDiameter()
        {
            Set vertices = GetAllVertices();
            int diameter = 0;
            foreach (var vertex in vertices)
            {
                Set neighbors = GetNeighbors(vertex);
                foreach (var el in neighbors)
                {
                    int distance = GetDistance(vertex, el);
                    if (diameter < distance)
                        diameter = distance;
                }
            }
            return diameter;
        }

        public int GetDistance(int from, int to)
        {
            //use dijkstra algorithm
            int distance;
            MetricSegment(from, to, out distance);
            return distance;
        }

        public ConvexCover GetNontrivialConvexCover()
        {
            Set vertices = GetAllVertices();
            Set localSet = new Set();
            ConvexCover convexCover = new ConvexCover(this);
            int flag = 0;
            foreach (var vertex in vertices)
            {
                if (!localSet.Contains(vertex))
                {
                    flag = 0;
                    foreach (var x in vertices)
                        foreach (var y in vertices)
                            if (x != y && x != vertex && y != vertex)
                            {
                                Set convexSet = Dconv(new Set() { x, y, vertex };
                                if (convexSet != vertices)
                                    convexCover.Add(convexSet);
                                localSet = localSet.Union(convexSet);
                                flag = 1;
                            }
                    if (flag == 0)
                        return convexCover;
                    else
                    { convexCover.ReduceToConvexCover();
                        return convexCover;
                    }
                }
            }
        }
        
        public GraphType WhatGraphType()
        {
            if (IsFGraph())
                return GraphType.FGraph;
            Set vertices = GetAllVertices();
            if (vertices.Count <= 4)
                return GraphType.OtherGraph;
            //verufy if there is at least one simplicial vertex
            foreach (var vertex in vertices)
                if (IsCliqueSubGraph(GetNeighbors(vertex)))
                    return GraphType.JGraph;
            //generate all (2,t)-convex covers
            HashSet<ConvexCover> setConvexCovers = new HashSet<ConvexCover>();
            Set setT= new Set();
            Set setNt = new Set();
            foreach (var vertex1 in vertices)
                foreach (var vertex2 in vertices)
                {
                    if (vertex1 != vertex2)
                    {
                        setT = new Set() { vertex1, vertex2 };
                        setNt = vertices.Except(setT);
                        if (IsConvex(setNt))
                            setConvexCovers.Add(new ConvexCover(this) { setT, setNt });
                    }
                }
            if (setConvexCovers.Count == 0)
                return GraphType.OtherGraph;
            if (setConvexCovers.Count >= 0)
                return GraphType.JGraph;
            //there is one (2,t)-convex cover
            int x = setT.First();
            int y = setT.Last();
            Set aSet = GetNeighbors(x).Except(new Set() { y });
            Set bSet = GetNeighbors(y).Except(new Set() { x });
            if (aSet.Intersect(bSet).Count >= 1)
                return GraphType.HPrimPrimGraph;
            if (Suplimentary(aSet, bSet) == GraphType.HPrimPrimGraph && Suplimentary(bSet, aSet) == GraphType.HPrimPrimGraph)
                return GraphType.HPrimPrimGraph;
            if (Dconv(aSet.Union(bSet)) != setNt)
                return GraphType.HPrimPrimGraph;
            if (aSet.Union(bSet) == setNt)
                return GraphType.HPrimPrimGraph;
            return GraphType.HPrimGraph;
        }

        private GraphType Suplimentary(Set aSet, Set bSet)
        {
            foreach (var vertex1 in aSet)
            {
                int flag = 0;
                foreach (var vertex2 in bSet)
                    if (GetNeighbors(vertex1).Contains(vertex2))
                        flag = 1;
                if (flag == 1)
                    return GraphType.HPrimPrimGraph;
            }
            return GraphType.OtherGraph;
        }

        public bool HasNontrivialConvexCover()
        {
            ConvexCover convexCover = GetNontrivialConvexCover();
            if (convexCover.Count >= 2)
                return true;
            else return false;
        }

        public bool IsEdge(int from, int to)
        {
            return this[from].Contains(to);
        }

        public bool IsFGraph()
        {
            if (Keys.Count <= 3)
                return false;
            //verify if a graph is a cycle of length 4
            if (Keys.Count == 4)
            {
                int[] vertices = Keys.ToArray();
                if (this[vertices[0]].Count != 2 || this[vertices[1]].Count != 2 ||
                    this[vertices[2]].Count != 2 || this[vertices[3]].Count != 2)
                    return false;
            }
            int flag = 0;
            int bivertex = 0;
            //verify bivertex of a graph
            foreach (var vertex in Keys)
                if (this[vertex].Count == 2)
                {
                    flag++;
                    bivertex = vertex;
                }
            if (flag != 1)
                return false;
            Set localSet1 = GetAllVertices();
            Set localSet2 = GetAllVertices();
            localSet1.Remove(bivertex);
            localSet1.Remove(this[bivertex].First());
            localSet1.Remove(bivertex);
            localSet1.Remove(this[bivertex].Last());
            //verify if there are two special cliques  
            if (IsCliqueSubGraph(localSet1) && IsCliqueSubGraph(localSet2))
                return true;
            else return false;
        }

        public bool IsHPrinCaseA()
        {
            if (WhatGraphType() != GraphType.HPrimGraph)
                return false;
            Set vertices = GetAllVertices();
            Set setT = new Set();
            Set setNt = new Set();
            Set localSet;
            foreach (var x in vertices)
            {
                int flag = 0;
                foreach (var y in vertices)
                {
                    setT = new Set() { x, y };
                    setNt = vertices.Except(setT);
                    if (IsConvex(setNt))
                    {
                        flag = 1;
                        break;
                    }

                }
                if (flag == 1)
                    break;
            }
            foreach (var vertex in vertices)
                if (Dconv(setT.Union(new Set() { vertex })) != vertices)
                    return true;
            return false;
        }

        public bool IsConnectedGraph()
        {
            if (TraverseBFS().Equals(GetAllVertices()))
                return true;
            else return false;
        }

        public bool IsCliqueSubGraph(Set set)
        {
            foreach (var x in set)
                foreach (var y in set)
                    if (x != y)
                        if (!IsEdge(x, y))
                            return false;
            return true;
        }        

        public List<Graph> GetConnectedComponents()
        {
            Graph localGraph = new Graph(this);
            List<Graph> connectedComponents = new List<Graph>();
            //traverse all vertices of a graph and generate subgraphs for connected components
            do
            {
                Set localSet = localGraph.TraverseBFS();
                connectedComponents.Add(localGraph.GetInducedSubGraph(localSet));
                localGraph.RemoveVertices(localSet);
            }
            while (localGraph.GetAllVertices().Count >= 1);
            return connectedComponents;
        }

        public Set TraverseBFS()
        {
            Queue<int> queue = new Queue<int>();
            Set set = new Set();
            queue.Enqueue(Keys.First());
            //traverse all vertices of a graph using queue
            while (queue.Count > 0)
            {
                int tempVertex = queue.Dequeue();
                set.Add(tempVertex);
                //Console.WriteLine("Vertex: " + tempVertex);
                Set tempSet = this[tempVertex].Except(set);
                foreach (var item in tempSet)
                    queue.Enqueue(item);
            }
            return set;
        }

        public Set TraverseDFS()
        {
            Stack<int> stack = new Stack<int>();
            Set set = new Set();
            stack.Push(Keys.First());
            //traverse all vertices of a graph using stack
            while (stack.Count > 0)
            {
                int tempVertex = stack.Pop();
                set.Add(tempVertex);
                //Console.WriteLine("Vertex: " + tempVertex);
                Set tempSet = this[tempVertex].Except(set);
                foreach (var vertex in tempSet)
                    stack.Push(vertex);
            }
            return set;
        }

        public Set MetricSegment(int x, int y, out int distance)
        {
            Set visitedVertices = new Set();
            List<Set> stages = new List<Set> { new Set() { x } };
            Set metricSegment = new Set { x, y };
            int current = 0;
            bool presentce = true;
            //BFS to find y
            while (!stages[current].Contains(y))
            {
                visitedVertices = visitedVertices.Union(stages[current]);
                Set tempSet = new Set();
                foreach (var vertex in stages[current])
                    tempSet = tempSet.Union(GetNeighbors(vertex).Except(visitedVertices));                
                stages.Add(tempSet);
                current++;
            }
            //back to find set of all paths
            distance = current;
            stages[current] = new Set { y };
            while (current >= 2)
            {
                Set tempSet = new Set();
                foreach (var vertex in stages[current])
                    tempSet = tempSet.Union(GetNeighbors(vertex).Intersect(stages[current - 1]));
                stages[current - 1] = tempSet;
                metricSegment = metricSegment.Union(tempSet);
                current--;
            }
            return metricSegment;
        }

        public Set Dconv(Set dConvSet)
        {
            int dist;
            foreach (var vertex1 in dConvlSetFin)
            {
                foreach (var vertex2 in dConvlSetFin)
                {
                    Set localSet = MetricSegment(vertex1, vertex2, out dist);
                    if(dConvlSetFin.IsSupersetOf(localSet))

                }
            }
            return dConvlSetFin;
        }

        public bool IsConvex(Set convSet)
        {
            if (convSet.Equals(Dconv(convSet)))
                return true;
            else return false;
        }

        public bool Equals(Graph graph)
        {
            return true;
        }

        public override string ToString()
        {
            string str = "Vertces: ";
            foreach (var el in this.Keys)
                str += " " + el.ToString();
            return str;
        }
    }

    public enum TreeGraphType
    {
        AGraph, BGraph, OtherTreeGraph
    }
    public class TreeGraph : Graph
    {
        public TreeGraph() : base() { }

        public TreeGraph(Set vertexSet) : this()
        {
            foreach (var vertex in vertexSet)
                Add(vertex, new Set());
        }

        public TreeGraph(Graph graf) : this()
        {
            foreach (var vertex in graf.Keys)
                Add(vertex, (Set)graf[vertex].Clone());
        }

        public bool IsTreeGraph()
        {
            TreeGraph localGraph = new TreeGraph(this);
            Set terminalVertices = localGraph.GetTerminalVertices();
            //repeat removing all terminal vertices of localGraph
            while (terminalVertices.Count >= 1)
            {
                localGraph.RemoveVertices(terminalVertices);
                terminalVertices = localGraph.GetTerminalVertices();
            }
            //if in localGraph remain some nonterminal vertices, then it is not a tree  
            if (localGraph.GetAllVertices().Count >= 1)
                return false;
            else return true;
        }

        public TreeGraphType WhatTreeGraphType()
        {
            Set vertices = GetAllVertices();
            if (vertices.Count <= 5)
                return TreeGraphType.OtherTreeGraph;
            int diam = GetDiameter();
            if (diam <= 2 || diam >= 5)
                return TreeGraphType.OtherTreeGraph;
            if (diam == 3)
            {
                int flag = 0;
                foreach (var vertex in vertices)
                    if (GetNeighbors(vertex).Count >= 3)
                        flag++;
                if (flag == 2)
                    return TreeGraphType.AGraph;
                else return TreeGraphType.OtherTreeGraph;
            }
            else //diam==4
            {
                foreach (var x in vertices)
                    foreach (var y in vertices)
                        foreach (var z in vertices)
                            if (x != y && x != z && y != z)
                                if (GetDistance(x, z) == diam && GetDistance(y, z) == diam)
                                    if (GetNeighbors(x).Intersect(GetNeighbors(y)).Count == 1)
                                        return TreeGraphType.BGraph;
                return TreeGraphType.OtherTreeGraph;   
            }
        }

        public Set GetTerminalVertices()
        {
            Set locaSet = new Set();
            Set vertices = GetAllVertices();
            foreach (var el in vertices)
                if (GetNeighbors(el).Count <= 1)
                    locaSet.Add(el);
            return locaSet;
        }

        public int GetMaxConvexCoverNumer()
        {
            Set vertices = GetAllVertices();
            Set terminalVertices = GetTerminalVertices();
            Set neigborsOfTermianVertices = new Set();
            Set articulationVertices = new Set();
            if (vertices.Count <= 2)
                return 0;
            //determine articulation set 
            foreach (var vertex in vertices)
            {
                int localVertex = GetNeighbors(vertex).First();
                if (!terminalVertices.Contains(localVertex))
                    neigborsOfTermianVertices.Add(localVertex);
            }
            articulationVertices = vertices.Except(terminalVertices.Union(neigborsOfTermianVertices));
            //verify diameter
            int diam = GetDiameter();
            if (diam <= 1)
                return 0;
            if (diam == 2)
                return terminalVertices.Count - 1;
            if ((diam >= 3 && diam <= 5) || (diam >= 6 && articulationVertices.Count == 0))
                return terminalVertices.Count;
            //determine recursve
            int maxConvexCoverNumber = 0;
            foreach (var articVertex in articulationVertices)
            {
                TreeGraph treeGraph = new TreeGraph(this);
                treeGraph.RemoveVertex(articVertex);
                List<Graph> connectedComponents = treeGraph.GetConnectedComponents();
                Set neigborsOfArticVertex = GetNeighbors(articVertex);
                int maxLocal1 = 0;
                foreach (var vertex in neigborsOfArticVertex)
                {
                    foreach (var graph in connectedComponents)
                    {
                        if (graph.HasVertex(vertex))
                        {
                            graph.AddVertex(vertex);
                            graph.AddEdge(vertex, articVertex);
                            break;
                        }
                    }
                    int maxLocal2 = 0;
                    foreach (var graph in connectedComponents)
                        maxLocal2 += ((TreeGraph)graph).GetMaxConvexCoverNumer();
                    if (maxLocal1 < maxLocal2)
                        maxLocal1 = maxLocal2;
                }
                if (maxConvexCoverNumber < maxLocal1)
                    maxLocal1 = maxConvexCoverNumber;
            }
            return maxConvexCoverNumber;
        }

        public int GetMaxConvexPartitionNumer()
        {
            Set vertices = GetAllVertices();
            //trivial case
            if (vertices.Count <= 2)
                return 0;
            //determine terminal sets
            HashSet<Set> terminalSets = GetTerminalSets();
            //remove all terminal sets
            foreach (var terminalSet in terminalSets)
                RemoveVertices(terminalSet);
            int maxConvexPartitioin = terminalSets.Count;
            //determine max convex cover number for every connected subtree 
            List<Graph> connectedTrees = GetConnectedComponents();
            foreach (var connectedTree in connectedTrees)
                maxConvexPartitioin += ((TreeGraph)connectedTree).GetMaxConvexPartitionNumer();
            return maxConvexPartitioin;
        }

        public HashSet<Set> GetTerminalSets()
        {
            HashSet<Set> termianlSets = new HashSet<Set>();
            Set terminalVertices = GetTerminalVertices();
            Set vertices = GetAllVertices().Except(terminalVertices);
            foreach (var vertex in vertices)
            {
                int flag = 0;
                Set neigbors = GetNeighbors(vertex);
                Set neigborsI = new Set();
                Set neigborsIAll = new Set();
                Set neigborsV = new Set();
                //verify if there is V relation
                neigborsV = neigbors.Intersect(terminalVertices);
                if (neigborsV.Count >= 2)
                    flag = 1;
                //verify if there is I relation
                foreach (var el in neigbors)
                {
                    Set neigborsEl = GetNeighbors(el);
                    neigborsI = neigbors.Intersect(terminalVertices);
                    if (neigborsEl.Count == 2 && neigborsI.Count == 1)
                    {
                        neigborsIAll = neigborsIAll.Union(neigborsI);
                        neigborsIAll.Add(el);
                        flag = 1;
                    }
                }
                if (flag == 1)
                    termianlSets.Add(neigborsV.Union(neigborsIAll));
            }
            return termianlSets;
        }
    }
}
