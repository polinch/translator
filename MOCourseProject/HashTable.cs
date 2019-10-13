using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOCourseProject
{
    //элемент таблицы
    class Item
    {
        //имя
        public string name;
        //атрибут
        public int atribut;
        //указатель на следующий
        public Item next;

        //конструктор
        public Item()
        {
            name = "";
            atribut = 0;
            next = null;
        }

    }

    //односвязный список
    class List
    {
        //Голова списка
        private Item Head;

        public Item Dostup
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

        //Конструктор
        public List()
        {
            Head = null;
        }

        //Добавление
        public void Add(string v, int k)
        {
            Item temp = new Item();
            temp.atribut = k;
            temp.name = v;
            if (Head == null)
                Head = temp;
            else
            {
                Item move = Head;
                while (move.next != null)
                    move = move.next;
                move.next = temp;
            }
        }

        //Удаление элемента
        public void Remove(string v)
        {
            Item temp = Head;
            Item pred = new Item();
            bool flag = false;
            if (String.Compare(temp.name, v) == 0)
                flag = true;
            if (!flag && (temp.next != null))
            {
                pred = temp;
                temp = temp.next;
                if (String.Compare(temp.name, v) == 0)
                    flag = true;
                while (!flag && (temp.next != null))
                {
                    pred = temp;
                    temp = temp.next;
                    if (String.Compare(temp.name, v) == 0)
                        flag = true;
                }
                pred.next = temp.next;
            }
            if (flag)
                if (temp == Head)
                    Head = temp.next;
        }

        //Поиск элемента - не выводит результат для несуществующего элемента
        public Item Search(string v)
        {
            Item temp = Head;
            bool flag = false;
            while (!flag && (temp != null))
            {
                if (String.Compare(temp.name, v) == 0)
                    flag = true;
                if (!flag)
                    temp = temp.next;
            }
            if (flag)
                return temp;
            else
                temp = new Item();
            return temp; 
        }

        //Поиск в таблице с возвращением атрибута
        public int Search1(string v)
        {
            Item temp = Head;
            bool flag = false;
            while (!flag && (temp != null))
            {
                if (String.Compare(temp.name, v) == 0)
                    flag = true;
                if (!flag)
                    temp = temp.next;
            }
            if (flag)
                return temp.atribut;
            else
                return 0;

        }

        //Вывод списка на экран
        public void Print()
        {
            Item temp = Head;
            if (temp == null)
                Console.WriteLine();
            else
            {
                while (temp != null)
                {
                    Console.Write(temp.name + " " + temp.atribut + "   ");
                    temp = temp.next;
                }
            }
        }



    }

    //Хеш-таблица
    class HashTable
    {
        //количество элементов в массиве
        private int CountOfElem;

        public int DostupT
        {
            get
            {
                return CountOfElem;
            }
            set
            {
                CountOfElem = value;
            }
        }

        //массив указателей на связанные списки
        public List[] array;



        //конструктор
        public HashTable(int n)
        {
            CountOfElem = n;
            array = new List[CountOfElem];
            for (int i = 0; i < CountOfElem; i++)
                array[i] = new List();
        }

        //хеш-функция: первый символ переводим в целый тип, прибавляем длину строки и берем остаток по модулю от количества элементов таблицы
        public int Hash(string v)
        {
            return v.Length % CountOfElem;
        }

        //Добавление в таблицу
        public void AddT(string str, int n)
        {
            int k = Hash(str);
            array[k].Add(str, n);
        }

        //Удаление из таблицы
        public void RemoveT(string str)
        {
            int k = Hash(str);
            array[k].Remove(str);
        }

        //Поиск в таблице
        public Item SearchT(string str)
        {
            return array[Hash(str)].Search(str);
        }

        //Поиск в таблице с возвращением атрибута
        public int SearchT1(string str)
        {
            return array[Hash(str)].Search1(str);
        }

        //Вывод таблицы на экран
        public void PrintT()
        {
            for (int i = 0; i < CountOfElem; i++)
            {
                array[i].Print();
                Console.WriteLine();
            }
        }

    }
}


