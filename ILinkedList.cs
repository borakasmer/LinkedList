using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomLinkedList
{
    public interface ILinkedList
    {
        void Push(Person person);
        void InsertAfter(Person personData, Node<Person> backNode);
        void InsertBefore(Person personData, Node<Person> headNode);
        Person Pop();
        Person PopFromBack();
        Person PopAfter(Node<Person> headNode);
        Person PopBefore(Node<Person> headNode);
        void Append(Person personData);
        void Undo();
        void Redo();
        void PrintList(Node<Person> head);
    }
}
