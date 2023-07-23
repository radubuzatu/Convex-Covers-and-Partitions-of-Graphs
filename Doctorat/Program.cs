using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctorat
{
    class Program
    {
        static void Main(string[] args)
        {
            TreeGraph trg = new TreeGraph(new Set() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18});
            trg.AddEdge(2, 5);
            trg.AddEdge(3, 5);
            trg.AddEdge(1, 4);
            trg.AddEdge(5, 7);
            trg.AddEdge(4, 7);
            trg.AddEdge(6, 7);
            trg.AddEdge(8, 7);
            trg.AddEdge(8, 9);
            trg.AddEdge(9, 10);
            trg.AddEdge(11, 10);
            trg.AddEdge(12, 10);
            trg.AddEdge(11, 13);
            trg.AddEdge(12, 15);
            trg.AddEdge(12, 14);
            trg.AddEdge(16, 14);
            trg.AddEdge(17, 14);
            trg.AddEdge(18, 14);
            
            // List<Graph> lg = trg.GetConnectedComponents();
            Console.WriteLine(trg.GetNontrivialConvexCover());
            Console.WriteLine(trg.GetMaxConvexCoverNumber());

            Console.WriteLine(trg.IsTreeGraph());
            Console.WriteLine(trg.GetTerminalVertices().Count);
            Console.WriteLine(trg.IsConvex(new Set(){1,4,7,8}));

            Graph g = new Graph(new Set() { 1, 2, 3, 4, 5, 6});
            g.AddNeighbors(1, new Set() { 2, 3, 4, 5 });
            g.AddNeighbors(6, new Set() { 2, 3, 4, 5 });
            g.AddNeighbors(2, new Set() { 3 });
            g.AddNeighbors(3, new Set() { 4 });
            g.AddNeighbors(4, new Set() { 5 });

            Graph gr = g;

            Console.WriteLine("////////////////////\\\\\\\\\\\\\\\\\\");
            //Console.WriteLine(g.GetNontrivialConvexCover());
            Console.WriteLine("////////////////////\\\\\\\\\\\\\\\\\\");
          
            ConvexCover cc1 = new ConvexCover(g) { new Set() { 1, 2, 3 }, new Set() { 4, 5, 3 } };
            ConvexCover cc2 = new ConvexCover(gr) { new Set() { 1, 2, 3 }, new Set() { 3,4, 5 } };

            //ConvexCover cc3 = new ConvexCover( { new Set() { 1, 2, 3 }, new Set() { 4, 5, 3 } };
            
            //Console.WriteLine(g.GetGraphType());
            Console.WriteLine(g.GetNontrivialConvexCover());
            Console.WriteLine(g.HasNontrivialConvexCover());
            Console.WriteLine(g.GetGraphType());
            Console.WriteLine(cc1.Equals(cc2));
   
            //ConvexCover convcov = new ConvexCover(grf);
            //convcov.AddSet(new Set() { 4, 5, 7 });
            //convcov.AddSet(new Set() { 2, 1, 6 });
            //convcov.AddSet(new Set() { 3});
            //convcov.AddSet(new Set() { 3,1 });
            //Console.WriteLine( convcov.IsConvexCover());
            //Console.WriteLine(convcov.IsConvexPartition());
            //Console.WriteLine(convcov.ReducibleToConvexCover());
            //convcov.ReduceToConvexCover();
            //Console.WriteLine(convcov);
            //Console.WriteLine(convcov.IsNontrivialConvexCover());
            
            Console.ReadKey();
        }
    }
}
