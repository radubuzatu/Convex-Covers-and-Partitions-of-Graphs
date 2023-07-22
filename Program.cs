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
            Node<int> node1 = new Node<int>(10);
            Node<int> node2 = new Node<int>(10);
            Console.WriteLine(node1==node2);
            Graph<int> graph1 = new Graph<int>();
            graph1.AddNode(1);
            graph1.AddNode(2);
            graph1.AddNode(3);
            HashSet<Node<int>> a = new HashSet<Node<int>> { new Node<int>(10), new Node<int>(20) };
            HashSet<Node<int>> b = new HashSet<Node<int>> { new Node<int>(10), new Node<int>(20) };
            Console.WriteLine(graph1.ToString());
            a.Add(new Node<int>(10));
            Console.WriteLine(a.Count);
            Console.WriteLine(a.Contains(node1));
            int v = 10, u = 10;
            HashSet<int> set = new HashSet<int> { 1, 2, 3 };
            bool b = set.Add(1);
            bool a = set.Add(4);
            Dictionary<int, int> deci = new Dictionary<int, int>();
            deci.Add(1, 1);
            deci.Add(3, 4);
            deci.Add(1, 2);
            Console.WriteLine(b+" "+a+ " "+deci[1].ToString());
            Console.ReadKey();
        }
    }
}
