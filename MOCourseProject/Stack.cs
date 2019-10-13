using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOCourseProject
{
    //элемент стека
    class Node
    {
        //магазинный символ
        public int state;
        public int number; //порядковый номер
        public Node next;

        public Node()
        {
            state = (int)Token.end; //Маркер дна
            number = 0;
            next = null;
        }
    }

    //стек для магазинной памяти
    class Stack
    {
        private Node Head;
        public Node Dostup
        {
            get
            {
                return Head;
            }
            set
            {
                Head = value;
            }
        }

        public Stack()
        {
            Head = new Node();
            //Head.number = 0;
        }

        //добавить в стек
        public void Push(int x)
        {
            if (Dostup == null){
                Node p = new Node();
                p.next = Dostup;
                Dostup = p;

            }
            else{
                Node p = new Node();
                p.state = x;
                p.number = Dostup.number + 1;
                p.next = Head;
                Head = p;
            } 
        }

        //извлечь из стека
        public int Pop()
        {
            Node p = Head;
            Head = Head.next;
            return p.state;
        }

        //считать верхушку стека
        public int Top()
        {
            int x = Head.state;
            return x;
        }

        public int Size(){
            return Dostup.number + 1;
        }

        public void PushIndex(int index, int token){
            Node temp = Dostup;
            bool flag = false;
            while (!flag && temp != null){
                if (temp.number == index)
                    flag = true;
                else
                    temp = temp.next;
            }
            if (flag)
                temp.state = token;
        }

        //проверка на пустоту - истина, если стек пуст
        public bool IsEmpty()
        {
            if (Head == null)
                return true;
            else
                return false;
        }
    }

}
