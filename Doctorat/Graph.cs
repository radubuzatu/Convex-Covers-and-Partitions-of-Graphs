using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctorat
{
    // clasa mulțime
    public class Set : HashSet<int>, IEquatable<Set>
    {
        public Set() : base() { }

        public Set(IEnumerable<int> set) : base(set) { }

        public Set(Set set) : this(set.AsEnumerable()) { }

        public Set Intersect(Set set)
        {
            Set localset = new Set(this);
            localset.IntersectWith(set);
            return localset;
        }

        public Set Union(Set set)
        {
            Set localset = new Set(this);
            localset.UnionWith(set);
            return localset;
        }

        public Set Except(Set set)
        {
            Set localset = new Set(this);
            localset.ExceptWith(set);
            return localset;
        }

        public bool Equals(Set set)
        {
            if (set == null)
                return false;
            else return GetHashCode().Equals(set.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj is Set)
                return Equals((Set)obj);
            else return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            List<int> vector = new List<int>(this);
            vector.Sort();
            string str = "Set: ";
            if (vector.Count == 0)
                return str + ".";
            for (int i = 0; i < vector.Count - 1; i++)
                str += vector[i] + ", ";
            str += vector[vector.Count - 1] + ".";
            return str;
        }
    }

    // clasa acoperire d-convexă
    public class ConvexCover : HashSet<Set>, IEquatable<ConvexCover>
    {
        public Graph graph;

        public ConvexCover(Graph graph)
            : base()
        {
            this.graph = graph;
        }

        public ConvexCover(ConvexCover convexCover)
            : this(convexCover.graph)
        {
            foreach (var set in convexCover)
                Add(new Set(set));
        }

        public void AddSet(Set set)
        {
            Add(set);
        }

        public void RemoveSet(Set set)
        {
            Remove(set);
        }

        // verifică dacă fiecare mulțime nu se conține în reuniune celorlalate
        public bool IsEverySetIsNotContainedInUnionOfOtherSets()
        {
            foreach (var set1 in this)
            {
                Set localSet = new Set();
                foreach (var set2 in this)
                    if (!set1.Equals(set2))
                        localSet = localSet.Union(set2);
                if (localSet.IsSupersetOf(set1))
                    return false;
            }
            return true;
        }

        // verifică dacă reuniunea mulțimilor formează mulțime de vârfuri a grafului
        public bool IsUnionEqualsAllVertices()
        {
            Set localSet = new Set();
            foreach (var set in this)
                localSet = localSet.Union(set);
            if (graph.GetAllVertices().Equals(localSet))
                return true;
            else return false;
        }

        // verifică d-convxitatea fiecărei mulțimi
        public bool IsEverySetConvex()
        {
            foreach (var set in this)
                if (!graph.IsConvex(set))
                    return false;
            return true;
        }

        // verifică dacă familie de mulțimi se reduce la acoperire d-convexă
        public bool IsReducibleToConvexCover()
        {
            ConvexCover convexCover = new ConvexCover(this);
            Set allVertices = graph.GetAllVertices();
            Set localSet = new Set();
            if (convexCover.Contains(allVertices))
                convexCover.RemoveSet(allVertices);
            if (!convexCover.IsEverySetConvex())
                return false;
            if (!convexCover.IsUnionEqualsAllVertices())
                return false;
            return true;
        }

        // reduce familie de mulțimi la acoperire d-convexă
        public void ReduceToConvexCover()
        {
            if (IsReducibleToConvexCover())
            {
                Set allVertices = graph.GetAllVertices();
                if (Contains(allVertices))
                    RemoveSet(allVertices);
                while (!IsEverySetIsNotContainedInUnionOfOtherSets())
                {
                    Set setToRemove = new Set();
                    int flag = 0;
                    foreach (var set1 in this)
                    {
                        Set localSet = new Set();
                        foreach (var set2 in this)
                            if (!set1.Equals(set2))
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
        }

        // verifică dacă familie de mulțimi formează acoperire d-convexă
        public bool IsConvexCover()
        {
            if (!IsEverySetConvex())
                return false;
            if (!IsUnionEqualsAllVertices())
                return false;
            if (!IsEverySetIsNotContainedInUnionOfOtherSets())
                return false;
            else return true;
        }

        // verifică dacă familie de mulțimi formează divizare d-convexă
        public bool IsConvexPartition()
        {
            if (!IsConvexCover())
                return false;
            foreach (var set1 in this)
                foreach (var set2 in this)
                    if (!set1.Equals(set2))
                        if (set1.Intersect(set2).Count >= 1)
                            return false;
            return true;
        }

        // verifică netrivialitatea fiecărei mulțimi
        public bool IsEverySetNontrivial()
        {
            foreach (var set in this)
                if (set.Count <= 2)
                    return false;
            return true;
        }

        // verifică dacă se obține acoperire d-convexă netrivială 
        public bool IsNontrivialConvexCover()
        {
            return IsConvexCover() && IsEverySetNontrivial();
        }

        // verifică dacă se obține divizare d-convexă netrivială 
        public bool IsnontrivialConvexPartition()
        {
            return IsConvexPartition() && IsEverySetNontrivial();
        }

        public bool Equals(ConvexCover convCov)
        {
            if (convCov == null)
                return false;
            else return GetHashCode().Equals(convCov.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj is ConvexCover)
                return Equals((ConvexCover)obj);
            else return false;
        }

        public override int GetHashCode()
        {
            string str = "";
            List<int> vector = new List<int>();
            foreach (var set in this)
                vector.Add(set.GetHashCode());
            vector.Sort();
            foreach (var code in vector)
                str += code + " ";
            str = graph.GetHashCode().ToString() + ":" + str;
            return str.GetHashCode();
        }

        public override string ToString()
        {
            string str = "Convex Cover: ";
            List<Set> listSets = new List<Set>(this);
            if (listSets.Count == 0)
                return str + ".";
            for (int i = 0; i < listSets.Count - 1; i++)
                str += listSets[i] + "; ";
            str += listSets[listSets.Count - 1] + ".";
            return str;
        }
    }

    // familii de grafuri
    public enum GraphType
    {
        FGraph, JGraph, HPrimGraph, HPrimPrimGraph, OtherGraph
    }

    // clasa graf
    public class Graph : Dictionary<int, Set>
    {
        public Graph() : base() { }

        public Graph(Set vertices)
            : this()
        {
            foreach (var x in vertices)
                Add(x, new Set());
        }

        public Graph(Graph graf)
            : this()
        {
            foreach (var x in graf.Keys)
                Add(x, new Set(graf[x]));
        }

        public void AddVertex(int vertex)
        {
            this.Add(vertex, new Set());
        }

        public void RemoveVertex(int vertex)
        {
            foreach (var x in this[vertex])
                this[x].Remove(vertex);
            this.Remove(vertex);
        }

        public void AddVertices(Set vertices)
        {
            foreach (var x in vertices)
                this.AddVertex(x);
        }

        public void RemoveVertices(Set vertices)
        {
            foreach (var x in vertices)
                this.RemoveVertex(x);
        }

        public bool ContainsVertex(int vertex)
        {
            return Keys.Contains(vertex);
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

        public bool IsEdge(int from, int to)
        {
            if (this[from].Contains(to))
                return true;
            return false;
        }

        public void AddNeighbors(int vertex, Set neighbors)
        {
            foreach (var vertexLoc in neighbors)
                AddEdge(vertex, vertexLoc);
        }

        public Set GetNeighbors(int vertex)
        {
            return new Set(this[vertex]);
        }

        public Set GetAllVertices()
        {
            return new Set(Keys);
        }

        // determină subgraf indus
        public Graph GetInducedSubGraph(Set vertices)
        {
            Graph localGraf = new Graph(vertices);
            foreach (var x in vertices)
                localGraf[x] = this[x].Intersect(vertices);
            return localGraf;
        }

        // determină diametrul grafului
        public int GetDiameter()
        {
            int diameter = 0;
            int distance;
            foreach (var x in Keys)
            {
                foreach (var y in Keys)
                {
                    if (x != y)
                    {
                        distance = GetDistance(x, y);
                        if (diameter < distance)
                            diameter = distance;
                    }
                }
            }
            return diameter;
        }

        // determină distanța dintre vârfurile 
        public int GetDistance(int from, int to)
        {
            int distance;
            MetricSegment(from, to, out distance);
            return distance;
        }

        // determină segmentul metric și distanța dintre două vârfuri
        public Set MetricSegment(int x, int y, out int distance)
        {
            Set visitedVertices = new Set();
            List<Set> stages = new List<Set> { new Set() { x } };
            Set metricSegment = new Set { x, y };
            int current = 0;
            while (!stages[current].Contains(y))
            {
                visitedVertices = visitedVertices.Union(stages[current]);
                Set tempSet = new Set();
                foreach (var vertex in stages[current])
                    tempSet = tempSet.Union(GetNeighbors(vertex).Except(visitedVertices));
                stages.Add(tempSet);
                current++;
            }
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

        // generează o acoperire d-convexă netrivială
        public ConvexCover GetNontrivialConvexCover()
        {
            Set vertices = GetAllVertices();
            Set localSet = new Set();
            ConvexCover convexCover = new ConvexCover(this);
            foreach (var x in vertices)
            {
                if (!localSet.Contains(x))
                {
                    int flag = 0;
                    foreach (var y in vertices)
                        foreach (var z in vertices)
                            if (y != z && y != x && z != x)
                            {
                                Set convexSet = ConvexHull(new Set() { x, y, z });
                                if (!convexSet.Equals(vertices))
                                {
                                    convexCover.Add(convexSet);
                                    localSet = localSet.Union(convexSet);
                                    flag = 1;
                                }
                            }
                    if (flag == 0)
                        return null;
                }
            }
            convexCover.ReduceToConvexCover();
            return convexCover;
        }

        // verifică dacă graf poate fi acoperit cu mulțimi d-convexe netriviale
        public bool HasNontrivialConvexCover()
        {
            ConvexCover convexCover = GetNontrivialConvexCover();
            if (convexCover == null)
                return false;
            if (convexCover.Count >= 2)
                return true;
            else return false;
        }

        // verifică dacă graf aparține familiei F 
        public bool IsFGraph()
        {
            if (Keys.Count <= 3)
                return false;
            if (Keys.Count == 4)
                foreach (var vertex in Keys)
                    if (this[vertex].Count != 2)
                        return false;
            int flag = 0;
            int bivertex = 0;
            foreach (var vertex in Keys)
                if (this[vertex].Count == 2)
                {
                    flag++;
                    bivertex = vertex;
                }
            if (flag != 1)
                return false;
            if (this[this[bivertex].First()].Contains(this[bivertex].Last()))
                return false;
            Set localSet1 = GetAllVertices();
            Set localSet2 = GetAllVertices();
            localSet1.Remove(this[bivertex].First());
            localSet1.Remove(bivertex);
            localSet2.Remove(this[bivertex].Last());
            localSet2.Remove(bivertex);
            if (IsCliqueSubGraph(localSet1) && IsCliqueSubGraph(localSet2))
                return true;
            else return false;
        }

        // determină tipul grafului
        public GraphType GetGraphType()
        {
            if (IsFGraph())
                return GraphType.FGraph;
            Set vertices = GetAllVertices();
            if (vertices.Count <= 4)
                return GraphType.OtherGraph;
            foreach (var vertex in vertices)
                if (IsCliqueSubGraph(GetNeighbors(vertex)))
                    return GraphType.JGraph;
            HashSet<ConvexCover> setConvexCovers = new HashSet<ConvexCover>();
            Set setT = new Set();
            Set setNt = new Set();
            foreach (var x in vertices)
                foreach (var y in vertices)
                {
                    if (x != y)
                    {
                        setT = new Set() { x, y };
                        setNt = vertices.Except(setT);
                        if (IsConvex(setNt))
                            setConvexCovers.Add(new ConvexCover(this) { setT, setNt });
                    }
                }
            if (setConvexCovers.Count == 0)
                return GraphType.OtherGraph;
            if (setConvexCovers.Count >= 2)
                return GraphType.JGraph;
            int x1 = setT.First();
            int y1 = setT.Last();
            Set aSet = GetNeighbors(x1);
            aSet.Remove(y1);
            Set bSet = GetNeighbors(y1);
            bSet.Remove(x1);
            if (aSet.Intersect(bSet).Count >= 1)
                return GraphType.HPrimPrimGraph;
            foreach (var vertex in aSet)
                if (GetNeighbors(vertex).Intersect(bSet).Count == 0)
                    return GraphType.HPrimPrimGraph;
            foreach (var vertex in bSet)
                if (GetNeighbors(vertex).Intersect(aSet).Count == 0)
                    return GraphType.HPrimPrimGraph;
            if (!ConvexHull(aSet.Union(bSet)).Equals(setNt))
                return GraphType.HPrimPrimGraph;
            if (aSet.Union(bSet).Equals(setNt))
                return GraphType.HPrimPrimGraph;
            return GraphType.HPrimGraph;
        }

        // verifică dacă graf aparține familiei HPrim și respectă condiția a)
        public bool IsHPrinCaseA()
        {
            if (GetGraphType() != GraphType.HPrimGraph)
                return false;
            Set vertices = GetAllVertices();
            Set setT = new Set();
            Set setNt = new Set();
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
            foreach (var vertex in setNt)
                if (!ConvexHull(setT.Union(new Set() { vertex })).Equals(vertices))
                    return true;
            return false;
        }

        // verifică dacă graf este conex
        public bool IsConnectedGraph()
        {
            if (TraverseBFS().Equals(GetAllVertices()))
                return true;
            else return false;
        }

        // verifică dacă mulțimea de vârfuri formează o clică
        public bool IsCliqueSubGraph(Set set)
        {
            foreach (var x in set)
                foreach (var y in set)
                    if (x != y)
                        if (!IsEdge(x, y))
                            return false;
            return true;
        }

        // determină componentele conexe ale grafului
        public List<Graph> GetConnectedComponents()
        {
            Graph localGraph = new Graph(this);
            List<Graph> connectedComponents = new List<Graph>();
            do
            {
                Set localSet = localGraph.TraverseBFS();
                connectedComponents.Add(localGraph.GetInducedSubGraph(localSet));
                localGraph.RemoveVertices(localSet);
            }
            while (localGraph.GetAllVertices().Count >= 1);
            return connectedComponents;
        }

        // parcurgere în lățime
        public Set TraverseBFS()
        {
            Queue<int> queue = new Queue<int>();
            Set set = new Set();
            queue.Enqueue(Keys.First());
            while (queue.Count > 0)
            {
                int tempVertex = queue.Dequeue();
                set.Add(tempVertex);
                Set tempSet = this[tempVertex].Except(set);
                foreach (var vertex in tempSet)
                    queue.Enqueue(vertex);
            }
            return set;
        }

        // parcurgere în adâncime
        public Set TraverseDFS()
        {
            Stack<int> stack = new Stack<int>();
            Set set = new Set();
            stack.Push(Keys.First());
            while (stack.Count > 0)
            {
                int tempVertex = stack.Pop();
                set.Add(tempVertex);
                Set tempSet = this[tempVertex].Except(set);
                foreach (var vertex in tempSet)
                    stack.Push(vertex);
            }
            return set;
        }

        // determină învelitoare d-convexă
        public Set ConvexHull(Set set)
        {
            Set localSet1;
            Set localSet2 = new Set(set);
            int dist;
            do
            {
                localSet1 = new Set(localSet2);
                foreach (var x in localSet1)
                    foreach (var y in localSet1)
                        if (x != y)
                            localSet2 = localSet2.Union(MetricSegment(x, y, out dist));
            }
            while (!localSet2.Equals(localSet1));
            return localSet2;
        }

        // verifică d-convexitatea mulțimii
        public bool IsConvex(Set set)
        {
            if (set.Equals(ConvexHull(set)))
                return true;
            else return false;
        }

        public override string ToString()
        {
            string str = "Graph: ";
            List<int> vertices = new List<int>(Keys);
            vertices.Sort();
            if (vertices.Count == 0)
                return str + ".";
            for (int i = 0; i < vertices.Count - 1; i++)
                str += vertices[i] + "- " + this[vertices[i]] + "; ";
            str += vertices[vertices.Count - 1] + "- " + this[vertices[vertices.Count - 1]] + ".";
            return str;
        }
    }

    // familii de arbori
    public enum TreeGraphType
    {
        ATreeGraph, BTreeGraph, OtherTreeGraph
    }

    // clasa arbore
    public class TreeGraph : Graph
    {
        public TreeGraph() : base() { }

        public TreeGraph(Set vertexSet)
            : this()
        {
            foreach (var vertex in vertexSet)
                Add(vertex, new Set());
        }

        public TreeGraph(Graph graf)
            : this()
        {
            foreach (var vertex in graf.Keys)
                Add(vertex, new Set(graf[vertex]));
        }

        // determină vârfurile terminale 
        public Set GetTerminalVertices()
        {
            Set terminalSet = new Set();
            foreach (var vertex in GetAllVertices())
                if (GetNeighbors(vertex).Count <= 1)
                    terminalSet.Add(vertex);
            return terminalSet;
        }

        // verifică dacă graf este un arbore 
        public bool IsTreeGraph()
        {
            TreeGraph localGraph = new TreeGraph(this);
            Set terminalVertices = localGraph.GetTerminalVertices();
            while (terminalVertices.Count >= 1)
            {
                localGraph.RemoveVertices(terminalVertices);
                terminalVertices = localGraph.GetTerminalVertices();
            }
            if (localGraph.GetAllVertices().Count >= 1)
                return false;
            else return true;
        }

        // determină tipul arborelui
        public TreeGraphType GetTreeGraphType()
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
                    return TreeGraphType.ATreeGraph;
                else return TreeGraphType.OtherTreeGraph;
            }
            else
            {
                foreach (var x in vertices)
                    foreach (var y in vertices)
                        foreach (var z in vertices)
                            if (x != y && x != z && y != z)
                                if (GetDistance(x, z) == diam && GetDistance(y, z) == diam)
                                    if (GetNeighbors(x).Intersect(GetNeighbors(y)).Count == 1)
                                        return TreeGraphType.BTreeGraph;
                return TreeGraphType.OtherTreeGraph;
            }
        }

        // determină numărul de acoperire d-convexă netrivială maximă
        public int GetMaxConvexCoverNumber()
        {
            Set vertices = GetAllVertices();
            Set terminalVertices = GetTerminalVertices();
            Set neigborsOfTermianVertices = new Set();
            int diam = GetDiameter();
            if (diam <= 1)
                return 0;
            if (diam == 2)
                return terminalVertices.Count - 1;
            foreach (var vertex in terminalVertices)
                neigborsOfTermianVertices = neigborsOfTermianVertices.Union(GetNeighbors(vertex));
            Set articulationVertices = vertices.Except(terminalVertices.Union(neigborsOfTermianVertices));
            if ((diam >= 3 && diam <= 5) || (diam >= 6 && articulationVertices.Count == 0))
                return terminalVertices.Count;
            int maxConvexCoverNumber = 0;
            foreach (var vertexArtic in articulationVertices)
            {
                TreeGraph treeGraph = new TreeGraph(this);
                Set neigborsOfVertexArtic = GetNeighbors(vertexArtic);
                treeGraph.RemoveVertex(vertexArtic);
                List<Graph> connectedComponents = treeGraph.GetConnectedComponents();
                int maxLocal1 = 0;
                foreach (var vertex in neigborsOfVertexArtic)
                {
                    Graph localGraph = new Graph();
                    foreach (var graph in connectedComponents)
                    {
                        if (graph.ContainsVertex(vertex))
                        {
                            graph.AddVertex(vertexArtic);
                            graph.AddEdge(vertex, vertexArtic);
                            localGraph = graph;
                            break;
                        }
                    }
                    int maxLocal2 = 0;
                    foreach (var graph in connectedComponents)
                        maxLocal2 += (new TreeGraph(graph)).GetMaxConvexCoverNumber();
                    if (maxLocal1 < maxLocal2)
                        maxLocal1 = maxLocal2;
                    localGraph.RemoveEdge(vertex, vertexArtic);
                    localGraph.RemoveVertex(vertexArtic);
                }
                if (maxConvexCoverNumber < maxLocal1)
                    maxConvexCoverNumber = maxLocal1;
            }
            if (maxConvexCoverNumber > terminalVertices.Count)
                return maxConvexCoverNumber;
            else return terminalVertices.Count;
        }

        // determină toate mulțimile terminale netriviale
        public HashSet<Set> GetTerminalSets()
        {
            HashSet<Set> terminalSets = new HashSet<Set>();
            Set terminalVertices = GetTerminalVertices();
            Set vertices = GetAllVertices().Except(terminalVertices);
            foreach (var vertex1 in vertices)
            {
                int flag = 0;
                Set neigbors = GetNeighbors(vertex1);
                Set neigborsV = neigbors.Intersect(terminalVertices);
                if (neigborsV.Count >= 2)
                    flag = 1;
                Set neigborsIAll = new Set();
                foreach (var vertex2 in neigbors.Except(terminalVertices))
                {
                    Set neigborsVertex2 = GetNeighbors(vertex2);
                    Set neigborsI = neigborsVertex2.Intersect(terminalVertices);
                    if (neigborsVertex2.Count == 2 && neigborsI.Count == 1)
                    {
                        neigborsIAll = neigborsIAll.Union(neigborsI);
                        neigborsIAll.Add(vertex2);
                        flag = 1;
                    }
                }
                if (flag == 1)
                    terminalSets.Add(neigborsV.Union(neigborsIAll).Union(new Set() { vertex1 }));
            }
            return terminalSets;
        }

        // determină numărul de dvizare d-convexă netrivială maximă
        public int GetMaxConvexPartitionNumer()
        {
            if (GetAllVertices().Count <= 2)
                return 0;
            HashSet<Set> terminalSets = GetTerminalSets();
            TreeGraph localTree = new TreeGraph(this);
            foreach (var terminalSet in terminalSets)
                localTree.RemoveVertices(terminalSet);
            int maxConvexPartitionNumber = terminalSets.Count;
            if (localTree.GetAllVertices().Count == 0)
                return maxConvexPartitionNumber;
            List<Graph> connectedComponents = localTree.GetConnectedComponents();
            foreach (var connectedGraph in connectedComponents)
                maxConvexPartitionNumber += (new TreeGraph(connectedGraph)).GetMaxConvexPartitionNumer();
            return maxConvexPartitionNumber;
        }
    }
}
