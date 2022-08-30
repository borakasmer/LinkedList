using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CustomLinkedList
{
    public enum LogType
    {
        Add = 1,
        Remove = 2
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            LinkedList list = new LinkedList();
            list.Push(new Person() { Name = "Bora", Surname = "Kasmer", Age = 44, TC = "45678912345" });
            list.Push(new Person() { Name = "Secil", Surname = "Kasmer", Age = 40, TC = "3453453455" });
            list.Append(new Person() { Name = "Arya", Surname = "Kasmer", Age = 5, TC = "3823438283" });
            list.InsertAfter(new Person() { Name = "Duru", Surname = "Kasmer", Age = 10, TC = "77734535345" }, list.Head.Next);
            //list.Push(new Person() { Name = "Test", Surname = "TestSoyad", Age = 404, TC = "404404404040" });
            //list.Append(new Person() { Name = "Test", Surname = "TestSoyad", Age = 404, TC = "404404404040" });
            //list.InsertAfter(new Person() { Name = "Test", Surname = "TestSoyad", Age = 404, TC = "404404404040" }, list.Head.Next.Next);

            //list.InsertBefore(new Person() { Name = "Test", Surname = "TestSoyad", Age = 10, TC = "77734535345" }, list.Head);

            //var person = list.PopFromBack();
            //Console.WriteLine("Person Removed From Back: " + person.Name + "-" + person.Surname);
            //var person2 = list.PopBefore(list.Head);
            //Console.WriteLine("Person Removed From  After Head: " + person2.Name + "-" + person2.Surname);

            var person2 = list.Pop();
            Console.WriteLine("Person Removed From Header: " + person2.Name + "-" + person2.Surname);        
            var person3 = list.Pop();
            Console.WriteLine("Person Removed From Header: " + person3.Name + "-" + person3.Surname);


            list.PrintList(list.Head);

            list.Undo();
            list.PrintList(list.Head);
            list.Undo();
            list.PrintList(list.Head);
            list.Undo();
            list.PrintList(list.Head);

            list.Redo();
            list.PrintList(list.Head);
            list.Redo();
            list.PrintList(list.Head);
            list.Redo();
            list.PrintList(list.Head);

            Console.ReadLine();
        }
    }

    public class LogCard<T> where T : class
    {
        public LogType LogType { get; set; }
        public Node<T> Node { get; set; }
    }
    public class Person
    {
        public string TC { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
    }
    public class LinkedList : ILinkedList
    {
        public Node<Person> Head { get; set; }
        public Stack<LogCard<Person>> UndoList { get; set; }
        public Stack<LogCard<Person>> RedoList { get; set; }

        public LinkedList()
        {
            UndoList = new Stack<LogCard<Person>>();
            RedoList = new Stack<LogCard<Person>>();
        }
        public void Push(Person person)
        {
            Node<Person> new_node = new Node<Person>() { Data = person };
            new_node.Back = null;
            new_node.Next = Head;
            if (Head != null)
            {
                Head.Back = new_node;
            }
            Head = new_node;
            UndoList.Push(new LogCard<Person>() { LogType = LogType.Add, Node = new_node });
        }

        public void InsertAfter(Person personData, Node<Person> backNode)
        {
            Node<Person> insertedNode = new Node<Person>() { Data = personData };
            if (backNode.Next != null)
            {
                backNode.Next.Back = insertedNode;
            }

            insertedNode.Next = backNode.Next;
            backNode.Next = insertedNode;
            insertedNode.Back = backNode;
            UndoList.Push(new LogCard<Person>() { LogType = LogType.Add, Node = insertedNode });
        }

        public void InsertBefore(Person personData, Node<Person> headNode)
        {
            Node<Person> insertedNode = new Node<Person>() { Data = personData };
            if (headNode.Back != null)
            {
                headNode.Back.Next = insertedNode;
            }
            else
            {
                Head = insertedNode;
            }
            insertedNode.Next = headNode;

            insertedNode.Back = headNode.Back;
            headNode.Back = insertedNode;

            UndoList.Push(new LogCard<Person>() { LogType = LogType.Add, Node = insertedNode });
        }
        public void Append(Person personData)
        {
            Node<Person> appendPerson = new Node<Person> { Data = personData };
            Node<Person> last = Head;
            if (Head == null)
            {
                appendPerson.Back = null;
                Head = appendPerson;
                return;
            }
            appendPerson.Next = null;
            while (last.Next != null)
            {
                last = last.Next;
            }
            last.Next = appendPerson;
            appendPerson.Back = last;
            UndoList.Push(new LogCard<Person>() { LogType = LogType.Add, Node = appendPerson });
        }

        public void Undo()
        {
            if (UndoList.Count > 0)
            {
                LogCard<Person> undoLog = UndoList.Pop();
                Node<Person> undoNode = undoLog.Node;
                switch (undoLog.LogType)
                {
                    case LogType.Add:
                        {
                            //HeadRemoved
                            if (undoNode.Back == null)
                            {
                                undoNode.Next.Back = null;
                                Head = undoNode.Next;
                            }
                            //LastRemoved
                            else if (undoNode.Next == null)
                            {
                                undoNode.Back.Next = null;
                            }
                            //RemoveAfter
                            else
                            {
                                undoNode.Back.Next = undoNode.Next;
                                undoNode.Next.Back = undoNode.Back;
                            }
                            RedoList.Push(undoLog);
                            break;
                        }
                    case LogType.Remove:
                        {
                            //HeadInsert
                            if (undoNode.Back == null)
                            {
                                undoNode.Next.Back = undoNode;
                                Head = undoNode;
                            }
                            //LastInsert
                            else if (undoNode.Next == null)
                            {
                                undoNode.Back.Next = undoNode;
                            }
                            //InsertAfter
                            else
                            {
                                undoNode.Back.Next = undoNode;
                                undoNode.Next.Back = undoNode;
                            }
                            RedoList.Push(new LogCard<Person>() { LogType = LogType.Remove, Node = undoNode });
                            break;
                        }
                }
            }
        }

        public void Redo()
        {
            if (RedoList.Count > 0)
            {
                LogCard<Person> redoLog = RedoList.Pop();
                Node<Person> redoNode = redoLog.Node;
                switch (redoLog.LogType)
                {
                    case LogType.Add:
                        {
                            //HeadInsert
                            if (redoNode.Back == null)
                            {
                                redoNode.Next.Back = redoNode;
                                Head = redoNode;
                            }
                            else if (redoNode.Next == null)
                            {
                                redoNode.Back.Next = redoNode;
                            }
                            //InsertAfter
                            else
                            {
                                redoNode.Back.Next = redoNode;
                                redoNode.Next.Back = redoNode;
                            }
                            UndoList.Push(redoLog);
                            break;
                        }
                    case LogType.Remove:
                        {
                            //HeadRemove
                            if (redoNode.Back == null)
                            {
                                redoNode.Next.Back = null;
                                Head = redoNode.Next;
                            }
                            else if (redoNode.Next == null)
                            {
                                redoNode.Back.Next = null;
                            }
                            //RemoveAfter
                            else
                            {
                                redoNode.Back.Next = redoNode.Next;
                                redoNode.Next.Back = redoNode.Back;
                            }
                            UndoList.Push(redoLog);
                            break;
                        }
                }
            }
        }

        public void PrintList(Node<Person> head)
        {
            Console.WriteLine("Ileri Say");
            Node<Person> last = head;
            while (last.Next != null)
            {
                Console.Write($"[{last.Data.Name}  {last.Data.Surname}], ");
                last = last.Next;
            }
            Console.WriteLine($"[{last.Data.Name}  {last.Data.Surname}]");
            Console.WriteLine("Geri Say");
            while (last.Back != null)
            {
                Console.Write($"[{last.Data.Name}  {last.Data.Surname}], ");
                last = last.Back;
            }
            Console.WriteLine($"[{last.Data.Name}  {last.Data.Surname}]");
        }

        public Person Pop()
        {
            var headNode = Head;
            UndoList.Push(new LogCard<Person>() { LogType = LogType.Remove, Node = headNode });
            headNode.Next.Back = null;
            Head = headNode.Next;
            return headNode.Data;
        }
        public Person PopFromBack()
        {
            var back = Head;
            while (back.Next != null)
            {
                back = back.Next;
            }
            UndoList.Push(new LogCard<Person>() { LogType = LogType.Remove, Node = back });
            back.Back.Next = null;
            return back.Data;
        }

        public Person PopAfter(Node<Person> headNode)
        {
            Node<Person> takenNode;
            if (headNode.Next != null)
            {
                takenNode = headNode.Next;
            }
            else
            {
                takenNode = headNode;
            }
            UndoList.Push(new LogCard<Person>() { LogType = LogType.Remove, Node = takenNode });

            if (takenNode.Next != null)
            {
                takenNode.Back.Next = takenNode.Next;
                takenNode.Next.Back = takenNode.Back;
            }
            else
            {
                takenNode.Back.Next = null;
            }
            return takenNode.Data;
        }
        public Person PopBefore(Node<Person> headNode)
        {
            Node<Person> takenNode;
            if (headNode.Back != null)
            {
                takenNode = headNode.Back;
            }
            else
            {
                takenNode = headNode;
            }
            UndoList.Push(new LogCard<Person>() { LogType = LogType.Remove, Node = takenNode });

            if (takenNode.Back != null)
            {
                takenNode.Back.Next = takenNode.Next;
                takenNode.Next.Back = takenNode.Back;
            }
            else
            {
                takenNode.Next.Back = null;
                Head = takenNode.Next;
            }
            return takenNode.Data;
        }
    }
    public class Node<T>
    {
        Random random;
        public static HashSet<int> hset = new HashSet<int>();
        public Node()
        {
            random = new Random();
            Id = RandTo20();
        }
        public int Id { get; set; }
        public Node<T> Next { get; set; }
        public Node<T> Back { get; set; }
        public T Data { get; set; }

        public int RandTo20()
        {
            int rnd = 0;
            while (hset.Contains(rnd))
            {
                rnd = random.Next(20);
            }
            hset.Add(rnd);
            return rnd;
        }
    }
}
