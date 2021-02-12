using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Bingo
{
    class Program
    {
        private static RelationshipGraph rg;

        // Read RelationshipGraph whose filename is passed in as a parameter.
        // Build a RelationshipGraph in RelationshipGraph rg
        private static void ReadRelationshipGraph(string filename)
        {
            rg = new RelationshipGraph();                           // create a new RelationshipGraph object

            string name = "";                                       // name of person currently being read
            int numPeople = 0;
            string[] values;
            Console.Write("Reading file " + filename + "\n");
            try
            {
                string input = System.IO.File.ReadAllText(filename);// read file
                input = input.Replace("\r", ";");                   // get rid of nasty carriage returns 
                input = input.Replace("\n", ";");                   // get rid of nasty new lines
                string[] inputItems = Regex.Split(input, @";\s*");  // parse out the relationships (separated by ;)
                foreach (string item in inputItems) 
		{
                    if (item.Length > 2)                            // don't bother with empty relationships
                    {
                        values = Regex.Split(item, @"\s*:\s*");     // parse out relationship:name
                        if (values[0] == "name")                    // name:[personname] indicates start of new person
                        {
                            name = values[1];                       // remember name for future relationships
                            rg.AddNode(name);                       // create the node
                            numPeople++;
                        }
                        else
                        {               
                            rg.AddEdge(name, values[1], values[0]); // add relationship (name1, name2, relationship)

                            // handle symmetric relationships -- add the other way
                            if (values[0] == "hasSpouse" || values[0] == "hasFriend")
                                rg.AddEdge(values[1], name, values[0]);

                            // for parent relationships add child as well
                            else if (values[0] == "hasParent")
                                rg.AddEdge(values[1], name, "hasChild");
                            else if (values[0] == "hasChild")
                                rg.AddEdge(values[1], name, "hasParent");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write("Unable to read file {0}: {1}\n", filename, e.ToString());
            }
            Console.WriteLine(numPeople + " people read");
        }

        // Show the relationships a person is involved in
        private static void ShowPerson(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
                Console.Write(n.ToString());
            else
                Console.WriteLine("{0} not found", name);
        }

        // Show a person's friends
        private static void ShowFriends(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                Console.Write("{0}'s friends: ",name);
                List<GraphEdge> friendEdges = n.GetEdges("hasFriend");
                foreach (GraphEdge e in friendEdges) {
                    Console.Write("{0} ",e.To());
                }
                Console.WriteLine();
            }
            else
                Console.WriteLine("{0} not found", name);     
        }

        // Show all the orphans
        private static void ShowOrphans()
        {
            Console.Write("Orphan(s): ");
            // look for people with no parent
            foreach (GraphNode n in rg.nodes)
            {
                if (n.GetEdges("hasParent").Count == 0)
                    Console.Write(n.Name + " ");
            }
        }

        // Show a person's siblings
        private static void ShowSiblings(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                Console.Write("{0}'s sibling(s): ", name);
                // look for person's parent(s)
                List<GraphEdge> parentEdges = n.GetEdges("hasParent");
                foreach (GraphEdge e in parentEdges)
                {
                    // look for parent's children other than self
                    List<GraphEdge> siblingEdges = e.ToNode().GetEdges("hasChild");
                    foreach (GraphEdge s in siblingEdges)
                    {
                        if (s.To() != name)
                            Console.Write("{0} ", s.To());
                    }
                }
                Console.WriteLine();
            }
            else
                Console.WriteLine("{0} not found", name);
        }

        // Show a person's descendants
        private static void ShowDescendants(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                Console.Write("{0}'s descendant(s): ", name);
                List<GraphEdge> descendantEdges = n.GetEdges("hasChild");
                if (descendantEdges.Count == 0)
                    Console.Write("none");
               
                int count = 0;
                while (descendantEdges.Count != 0)
                {
                    List<GraphEdge> descendantEdges2 = new List<GraphEdge>();
                    // title of descendants
                    if (count == 0)
                        Console.Write("\n\tchildren: ");
                    else if (count == 1)
                        Console.Write("\n\tgrandchildren: ");
                    else
                        Console.Write("\n\t" + string.Concat(Enumerable.Repeat("great ", (count-1))) + "grandchildren: ");

                    foreach (GraphEdge e in descendantEdges)
                    {
                        Console.Write("{0} ", e.To());
                        // update descendantEdges with the children
                        foreach (GraphEdge d in e.ToNode().GetEdges("hasChild"))
                            descendantEdges2.Add(d);
                    }
                    descendantEdges = descendantEdges2;
                    count++;
                }
                Console.WriteLine();
            }
            else
                Console.WriteLine("{0} not found", name);
        }

        // bingo finds and prints the shortest path of relationship between two people
        private static void Bingo(string name1, string name2)
        {
            GraphNode n1 = rg.GetNode(name1);
            GraphNode n2 = rg.GetNode(name2);
            if (n1 != null && n2 != null && n1 != n2)
            {
                // label all nodes as "unvisited"
                foreach (GraphNode n in rg.nodes)
                    n.Label = "Unvisited";
                // create queue for BFS and a reversed (for future output) BFS spanning tree
                Queue<GraphNode> queue = new Queue<GraphNode>();
                RelationshipGraph bfs = new RelationshipGraph(); 
                // starts at root node
                n1.Label = "Visited";
                queue.Enqueue(n1);
                // BFS
                while (queue.Count != 0)
                {
                    GraphNode node = queue.Dequeue();
                    foreach (GraphEdge e in node.GetEdges())
                    {
                        GraphNode to = e.ToNode();
                        if (to.Label != "Visited")
                        {
                            to.Label = "Visited";
                            queue.Enqueue(to);
                            bfs.AddEdge(e.To(), e.From(), e.Relationship());
                        }
                        // breaks if successfully found
                        if (to == n2)
                            queue.Clear();
                    }
                }
                // output if relationship is not found
                if (bfs.GetNode(name2) == null)
                    Console.WriteLine("No relationship found between {0}, {1}", name1, name2);
                else
                // output relationship
                {
                    // use stack to reverse the order in the tree
                    Stack<string> relationships = new Stack<string>();
                    GraphNode child = bfs.GetNode(name2);
                    while (child != bfs.GetNode(name1))
                    {
                        foreach (GraphEdge e in child.GetEdges())
                        {
                            relationships.Push(e.Relationship());
                            child = e.ToNode();
                        }
                    }
                    Console.Write(name2 + " is " + name1);
                    foreach (string s in relationships)
                        Console.Write("'s " + s.Remove(0, 3));
                }
            }
            else
            {
                if (n1 == null)
                    Console.WriteLine("{0} not found ", name1);
                if (n2 == null)
                    Console.WriteLine("{0} not found", name2);
                if (n1 == n2)
                    Console.WriteLine("Please enter two different persons.");
            }
            Console.WriteLine();
        }


        // accept, parse, and execute user commands
        private static void CommandLoop()
        {
            string command = "";
            string[] commandWords;
            Console.Write("Welcome to Harry's Dutch Bingo Parlor!\n");

            while (command != "exit")
            {
                Console.Write("\nEnter a command: ");
                command = Console.ReadLine();
                commandWords = Regex.Split(command, @"\s+");        // split input into array of words
                command = commandWords[0];

                if (command == "exit")
                    return;                                               // do nothing

                // read a relationship graph from a file
                else if (command == "read" && commandWords.Length > 1)
                    ReadRelationshipGraph(commandWords[1]);

                // show information for one person
                else if (command == "show" && commandWords.Length > 1)
                    ShowPerson(commandWords[1]);

                else if (command == "friends" && commandWords.Length > 1)
                    ShowFriends(commandWords[1]);

                // dump command prints out the graph
                else if (command == "dump")
                    rg.Dump();

                // show all orphans
                else if (command == "orphans")
                    ShowOrphans();

                // show siblings of a person
                else if (command == "siblings" && commandWords.Length > 1)
                    ShowSiblings(commandWords[1]);

                // show descendants of a person
                else if (command == "descendants" && commandWords.Length > 1)
                    ShowDescendants(commandWords[1]);

                // bingo
                else if (command == "bingo" && commandWords.Length > 1)
                    Bingo(commandWords[1], commandWords[2]);

                // illegal command
                else
                    Console.Write("\nLegal commands: read [filename], dump, show [personname],\n  friends [personname], orphans, siblings [personname], descendants [personname], bingo [personname personname], exit\n");
            }
        }

        static void Main(string[] args)
        {
            CommandLoop();
        }
    }
}
