using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOCourseProject
{
    //элемент массива - для хранения значений после трансляции
    class ItemInter{
        public string name;
        public int value;

        public ItemInter(){
            name = "";
            value = 0;
        }
    }

    //структура для хранения области памяти, области команд и области меток
    class Result{
        public StackMas<ItemInter> data;
        public StackMas<int> commands;
        public StackMas<int> labels;

        public Result(){
            data = new StackMas<ItemInter>(1000);
            commands = new StackMas<int>(1000);
            labels = new StackMas<int>(1000);
        }
    }

    //Типы атрибутов = входные символы
    enum Token
    {
        end = 0, //маркер дна и концевой маркер
        ident = 1,
        konst,
        condition1,
        condition2,
        cycle1,
        cycle2,
        arifm1,
        arifm2,
        logic1,
        logic2,
        prisv,
        delimiter1,
        delimiter2,
        delimiter3,
        program = 21,
        complex,
        operate,
        endif,
        E,
        Elist,
        T,
        Tlist,
        F,
        logicv,
        logiclist,
        //logico,
        //номера действий
        assign = -1,
        add = -2,
        mov = -3,
        compare = -4, //равно
        compare2 = -5, //не равно
        label = -6,
        condtras = -7, //условный переход по сравнению
        tras0 = -8, //условный переход по нулю
        uncondtras = -9, //безусловный переход
        error = -10 // для инициализации стека - конструктор
    }


    class Scaner
    {
        //таблица разделителей
        public HashTable TableKey;
        //таблица идентификаторов и констант
        public HashTable TableId;
        //таблица ключевых слов
        public HashTable TableWord;
        //количество лексем в таблице идентификаторов и констант
        public int CountId;

        //структура для трансляции и интерпретации
        public Result rez;
        //стек - магазинная память
        public Stack MP;

        //Каретка для перемещения по строке
        public int kar;

        //строка, хранящая текст из редактора кода
        public string Str;




        //конструктор
        public Scaner()
        {
            TableKey = new HashTable(9);
            TableId = new HashTable(100);
            TableWord = new HashTable(4);
            CountId = 0;
            Str = null;
            MP = new Stack();
            rez = new Result();

            //заполнение таблицы TableWord
            //условие
            TableWord.AddT("if", (int)Token.condition1);
            TableWord.AddT("else", (int)Token.condition2);

            //цикл
            TableWord.AddT("repeat", (int)Token.cycle1);
            TableWord.AddT("until", (int)Token.cycle2);

            //заполнение таблицы Table Key
            //разделители
            TableKey.AddT(";", (int)Token.delimiter1);
            TableKey.AddT("(", (int)Token.delimiter2);
            TableKey.AddT(")", (int)Token.delimiter3);

            //логические операции
            TableKey.AddT("=", (int)Token.logic1);
            TableKey.AddT("!=", (int)Token.logic2);

            //оператор присваивания
            TableKey.AddT(":=", (int)Token.prisv);

            //арифметические операции
            TableKey.AddT("+", (int)Token.arifm1);
            TableKey.AddT("*", (int)Token.arifm2);

            //Концевой маркер
            TableKey.AddT("", (int)Token.end);


        }


        //принадлежность грамматике - для обнаружения ошибок
        public bool Affiliation(char symbol)
        {
            if (symbol >= 'a' && symbol <= 'z')
                return true;
            else if (symbol >= '0' && symbol <= '9')
                return true;
            else if (symbol == '(')
                return true;
            else if (symbol == ')')
                return true;
            else if (symbol == '+')
                return true;
            else if (symbol == '*')
                return true;
            else if (symbol == ';')
                return true;
            else if (symbol == '=')
                return true;
            else if (symbol == '!')
                return true;
            else if (symbol == ':')
                return true;
            else if (symbol == ' ')
                return true;
            else if (symbol == '\r' || symbol == '\n')
                return true;
            return false;
        }

        //Проверка на разделительный символ
        public bool Delimiter(char symbol)
        {
            if (symbol == '(')
                return true;
            else if (symbol == ')')
                return true;
            else if (symbol == '+')
                return true;
            else if (symbol == '*')
                return true;
            else if (symbol == ';')
                return true;
            else if (symbol == '=')
                return true;
            else if (symbol == '!')
                return true;
            else if (symbol == ':')
                return true;
            return false;
        }

        //Сканирование строки с возвращением атрибута лексемы. Если лексема не выделена, то вернуть -1. Если конец строки, то вернуть 0
        public Item ScanStr(string str, ref int kar)
        {
            char cur = ' ';
            string num = null;
            string word = null;
            string separator = null;

            if (kar < str.Length)
                cur = str[kar];
            else if (kar > str.Length)
                //return -1;
                return null;
            else if (kar == str.Length)
                return TableKey.SearchT("");

            //пропуск пробелов, табуляции и переход на новую строку
            while ((cur == ' ' || cur == '\t' || cur == '\r' || cur == '\n') && kar < str.Length)
            {
                kar++;
                if (kar < str.Length)
                    cur = str[kar];
                if (kar == str.Length)
                    return TableKey.SearchT("");
            }
            //проверка на число
            if (cur >= '0' && cur <= '9')
            {
                do
                {
                    num += cur;
                    kar++;
                    if (kar >= str.Length)
                        break;
                    cur = str[kar];
                } while (cur >= '0' && cur <= '9');
                if (cur >= 'a' && cur <= 'z')
                    return null;
                if (TableId.SearchT(num).name == "")
                {
                    TableId.AddT(num, (int)Token.konst);
                    CountId++;
                }
                return TableId.SearchT(num);
            }
            //проверка на ключевое слово или идентификатор
            if (cur >= 'a' && cur <= 'z')
            {
                do
                {
                    word += cur;
                    if (kar < str.Length)
                        kar++;
                    else
                        break;
                    cur = str[kar];
                } while (cur >= 'a' && cur <= 'z');
                if (!Affiliation(cur))
                    return null;
                if (String.Compare(word, TableWord.SearchT(word).name) == 0)//(word == TableWord.SearchT(word).name)
                    return TableWord.SearchT(word);
                else
                {
                    if (TableId.SearchT(word).name == "")
                    {
                        TableId.AddT(word, (int)Token.ident);
                        CountId++;
                    }
                    return TableId.SearchT(word);
                }
            }
            //проверка на разделитель
            if (Delimiter(cur))
            {
                do
                {
                    separator += cur;
                    kar++;
                    if (kar < str.Length)
                        cur = str[kar];
                    else
                        break;
                } while (Delimiter(cur));
                if (!Affiliation(cur))
                    return null;
                return TableKey.SearchT(separator);
            }
            else
                return null;
        }

        //МП-автомат, принимает лексему, магазинный символ и структуру с данными
        public bool MPauto(Item lexeme, int ms, ref Result rez){
            bool exit = true;
            int temp, tempmas, ptr;
            Item lex;
            ItemInter item; //для добавления в область данных
            bool flag = false; //для добавления в область данных
            if (lexeme != null)
            {
                switch (ms)
                {
                    case (int)Token.program:
                        switch (lexeme.atribut)
                        {
                            case (int)Token.ident:
                            case (int)Token.condition1:
                            case (int)Token.cycle1:
                                MP.Pop();
                                MP.Push((int)Token.complex);
                                MP.Push((int)Token.operate);
                                temp = MP.Top();
                                exit = MPauto(lexeme, temp, ref rez);
                                break;
                            default:
                                exit = false;
                                break;
                        }
                        break;
                    case (int)Token.complex:
                        switch (lexeme.atribut)
                        {
                            case (int)Token.ident:
                            case (int)Token.condition1:
                            case (int)Token.cycle1:
                                MP.Pop();
                                MP.Push((int)Token.complex);
                                MP.Push((int)Token.operate);
                                temp = MP.Top();
                                exit = MPauto(lexeme, temp, ref rez);
                                break;
                            case (int)Token.cycle2:
                            case (int)Token.end:
                                MP.Pop();
                                temp = MP.Top();
                                exit = MPauto(lexeme, temp, ref rez);
                                break;
                            default:
                                exit = false;
                                break;
                        }
                        break;
                    case (int)Token.operate:
                        switch (lexeme.atribut)
                        {
                            case (int)Token.ident:
                                MP.Pop();
                                MP.Push((int)Token.delimiter1); //;
                                MP.Push(-1); //q2

                                //проверка на наличие идентификатора в области данных
                                for (ptr = 0; ptr < rez.data.Size(); ++ptr)
                                {
                                    if (String.Compare(lexeme.name, rez.data.Mas[ptr].name) == 0)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                if (!flag)
                                {
                                    item = new ItemInter();
                                    item.name = lexeme.name;
                                    rez.data.Push(item);
                                }

                                MP.Push(ptr); //p2
                                MP.Push((int)Token.assign); //действие присвоить
                                MP.Push(MP.Size() - 3); //q1
                                MP.Push((int)Token.E); //E
                                MP.Push((int)Token.prisv); //:=
                                temp = MP.Top();
                                lex = ScanStr(Str, ref kar);
                                exit = MPauto(lex, temp, ref rez);
                                break;
                            case (int)Token.condition1:
                                MP.Pop();
                                rez.labels.Push(-1);
                                MP.Push(rez.labels.Size() - 1); //z2
                                MP.Push((int)Token.endif);
                                MP.Push((int)Token.operate);
                                MP.Push(rez.labels.Size() - 1); //z1
                                MP.Push(-1);
                                MP.Push((int)Token.tras0);
                                MP.Push((int)Token.delimiter3);
                                MP.Push(MP.Size() - 3);
                                MP.Push((int)Token.logicv);
                                MP.Push((int)Token.delimiter2);
                                temp = MP.Top();
                                lex = ScanStr(Str, ref kar);
                                exit = MPauto(lex, temp, ref rez);
                                break;
                            case (int)Token.cycle1:
                                MP.Pop();
                                rez.labels.Push(-1);
                                MP.Push((int)Token.delimiter1);
                                MP.Push(rez.labels.Size() - 1); //z2
                                MP.Push(-1); //p2
                                MP.Push((int)Token.condtras); //переход по сравнению
                                MP.Push(MP.Size() - 2); //p1
                                MP.Push((int)Token.logicv);
                                MP.Push((int)Token.cycle2);
                                MP.Push((int)Token.complex);
                                MP.Push(rez.labels.Size() - 1); //z1
                                MP.Push((int)Token.label);
                                temp = MP.Top();
                                lex = ScanStr(Str, ref kar);
                                exit = MPauto(lex, temp, ref rez);
                                break;
                            default:
                                exit = false;
                                break;
                        }
                        break;
                    case (int)Token.endif:
                        switch (lexeme.atribut)
                        {
                            case (int)Token.ident:
                            case (int)Token.condition1:
                            case (int)Token.cycle1:
                            case (int)Token.end:
                                MP.Pop();
                                MP.Push((int)Token.label);
                                temp = MP.Top();
                                exit = MPauto(lexeme, temp, ref rez);
                                break;
                            case (int)Token.condition2:
                                MP.Pop(); // выталкиваем endif
                                ptr = MP.Pop(); //указатель из z1
                                rez.labels.Push(-1);
                                MP.Push(rez.labels.Size() - 1); //w2
                                MP.Push((int)Token.label);
                                MP.Push((int)Token.operate);
                                MP.Push(ptr); //z2
                                MP.Push((int)Token.label);
                                MP.Push(rez.labels.Size() - 1); //w1
                                MP.Push((int)Token.uncondtras);
                                temp = MP.Top();
                                lex = ScanStr(Str, ref kar);
                                exit = MPauto(lex, temp, ref rez);
                                break;
                            default:
                                exit = false;
                                break;
                        }
                        break;
                    case (int)Token.E:
                        switch (lexeme.atribut)
                        {
                            case (int)Token.ident:
                            case (int)Token.konst:
                                MP.Pop();
                                MP.Push(-1);
                                MP.Push((int)Token.Elist);
                                MP.Push(MP.Size() - 2);
                                MP.Push((int)Token.T);
                                temp = MP.Top();
                                exit = MPauto(lexeme, temp, ref rez);
                                break;
                            default:
                                exit = false;
                                break;
                        }
                        break;
                    case (int)Token.Elist:
                        switch (lexeme.atribut)
                        {
                            case (int)Token.arifm1:
                                MP.Pop();
                                ptr = MP.Pop(); //p1
                                                //новая ячейка
                                item = new ItemInter();
                                rez.data.Push(item);
                                MP.Push(rez.data.Size() - 1); //r2
                                MP.Push((int)Token.Elist);
                                MP.Push(rez.data.Size() - 1);//r1
                                MP.Push(-1); //q2
                                MP.Push(ptr); //p2
                                MP.Push((int)Token.add); //действие сложить
                                MP.Push(MP.Size() - 3); //q1
                                MP.Push((int)Token.T);
                                temp = MP.Top();
                                lex = ScanStr(Str, ref kar);
                                exit = MPauto(lex, temp, ref rez);
                                break;
                            case (int)Token.delimiter1:
                                MP.Pop();
                                ptr = MP.Pop(); //p1
                                tempmas = MP.Pop(); //t2
                                MP.PushIndex(tempmas, ptr);
                                temp = MP.Top();
                                exit = MPauto(lexeme, temp, ref rez);
                                break;
                            default:
                                exit = false;
                                break;
                        }
                        break;
                    case (int)Token.T:
                        switch (lexeme.atribut)
                        {
                            case (int)Token.ident:
                            case (int)Token.konst:
                                MP.Pop();
                                MP.Push(-1);
                                MP.Push((int)Token.Tlist);
                                MP.Push(MP.Size() - 2);
                                MP.Push((int)Token.F);
                                temp = MP.Top();
                                exit = MPauto(lexeme, temp, ref rez);
                                break;
                            default:
                                exit = false;
                                break;
                        }
                        break;
                    case (int)Token.Tlist:
                        switch (lexeme.atribut)
                        {
                            case (int)Token.arifm2:
                                MP.Pop();
                                ptr = MP.Pop(); //p1
                                                //новая ячейка
                                item = new ItemInter();
                                rez.data.Push(item);
                                MP.Push(rez.data.Size() - 1); //r2
                                MP.Push((int)Token.Tlist);
                                MP.Push(rez.data.Size() - 1); //r1
                                MP.Push(-1); //q2
                                MP.Push(ptr); //p2
                                MP.Push((int)Token.mov); //действие умножить
                                MP.Push(MP.Size() - 3); //q1
                                MP.Push((int)Token.F);
                                temp = MP.Top();
                                lex = ScanStr(Str, ref kar);
                                exit = MPauto(lex, temp, ref rez);
                                break;
                            case (int)Token.arifm1:
                            case (int)Token.delimiter1:
                                MP.Pop();
                                ptr = MP.Pop(); //p1
                                tempmas = MP.Pop(); //t2
                                MP.PushIndex(tempmas, ptr);
                                temp = MP.Top();
                                exit = MPauto(lexeme, temp, ref rez);
                                break;
                            default:
                                exit = false;
                                break;
                        }
                        break;
                    case (int)Token.F:
                        switch (lexeme.atribut)
                        {
                            case (int)Token.ident:
                            case (int)Token.konst:
                                MP.Pop();
                                //проверка на наличие идентификатора или числа в области данных
                                for (ptr = 0; ptr < rez.data.Size(); ++ptr)
                                {
                                    if (String.Compare(lexeme.name, rez.data.Mas[ptr].name) == 0)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                if (!flag)
                                {
                                    item = new ItemInter();
                                    item.name = lexeme.name;
                                    if (lexeme.atribut == 2)
                                        item.value = Convert.ToInt32(lexeme.name);
                                    rez.data.Push(item);
                                }
                                tempmas = MP.Pop(); //адрес из ячейки p2
                                MP.PushIndex(tempmas, ptr);
                                temp = MP.Top();
                                lex = ScanStr(Str, ref kar);
                                exit = MPauto(lex, temp, ref rez);
                                break;
                            default:
                                exit = false;
                                break;
                        }
                        break;
                    case (int)Token.logicv:
                        switch (lexeme.atribut)
                        {
                            case (int)Token.ident:
                            case (int)Token.konst:
                                MP.Pop();
                                MP.Push(-1);
                                MP.Push((int)Token.logiclist);
                                MP.Push(MP.Size() - 2);
                                MP.Push((int)Token.F);
                                temp = MP.Top();
                                exit = MPauto(lexeme, temp, ref rez);
                                break;
                            default:
                                exit = false;
                                break;
                        }
                        break;
                    case (int)Token.logiclist:
                        switch (lexeme.atribut)
                        {
                            case (int)Token.logic1:
                            case (int)Token.logic2:
                                MP.Pop();
                                ptr = MP.Pop(); //p1
                                item = new ItemInter();
                                rez.data.Push(item);
                                tempmas = MP.Pop();
                                MP.PushIndex(tempmas, rez.data.Size() - 1); //r1
                                MP.Push(rez.data.Size() - 1);
                                MP.Push(-1); //q2
                                MP.Push(ptr); //p2
                                if (lexeme.atribut == (int)Token.logic1)
                                    MP.Push((int)Token.compare); // равно
                                else
                                    MP.Push((int)Token.compare2); //не равно
                                MP.Push(MP.Size() - 3);
                                MP.Push((int)Token.F);
                                temp = MP.Top();
                                lex = ScanStr(Str, ref kar);
                                exit = MPauto(lex, temp, ref rez);
                                break;
                            default:
                                exit = false;
                                break;
                        }
                        break;
                    case (int)Token.delimiter1:
                        switch (lexeme.atribut)
                        {
                            case (int)Token.delimiter1:
                                MP.Pop();
                                temp = MP.Top();
                                lex = ScanStr(Str, ref kar);
                                exit = MPauto(lex, temp, ref rez);
                                break;
                            default:
                                exit = false;
                                break;
                        }
                        break;
                    case (int)Token.delimiter2:
                        switch (lexeme.atribut)
                        {
                            case (int)Token.delimiter2:
                                MP.Pop();
                                temp = MP.Top();
                                lex = ScanStr(Str, ref kar);
                                exit = MPauto(lex, temp, ref rez);
                                break;
                            default:
                                exit = false;
                                break;
                        }
                        break;
                    case (int)Token.delimiter3:
                        switch (lexeme.atribut)
                        {
                            case (int)Token.delimiter3:
                                MP.Pop();
                                temp = MP.Top();
                                lex = ScanStr(Str, ref kar);
                                exit = MPauto(lex, temp, ref rez);
                                break;
                            default:
                                exit = false;
                                break;
                        }
                        break;
                    case (int)Token.prisv:
                        switch (lexeme.atribut)
                        {
                            case (int)Token.prisv:
                                MP.Pop();
                                temp = MP.Top();
                                lex = ScanStr(Str, ref kar);
                                exit = MPauto(lex, temp, ref rez);
                                break;
                            default:
                                exit = false;
                                break;
                        }
                        break;
                    case (int)Token.cycle2:
                        switch (lexeme.atribut)
                        {
                            case (int)Token.cycle2:
                                MP.Pop();
                                temp = MP.Top();
                                lex = ScanStr(Str, ref kar);
                                exit = MPauto(lex, temp, ref rez);
                                break;
                            default:
                                exit = false;
                                break;
                        }
                        break;
                    case (int)Token.end:
                        switch (lexeme.atribut)
                        {
                            case (int)Token.end:
                                MP.Pop();
                                exit = true;
                                break;
                            default:
                                exit = false;
                                break;
                        }
                        break;

                    // символы действия
                    //присвоить
                    case (int)Token.assign:
                        temp = MP.Pop();
                        rez.commands.Push(temp);
                        for (int i = 0; i < 2; ++i)
                        {
                            temp = MP.Pop();
                            rez.commands.Push(temp);
                        }
                        temp = MP.Top();
                        exit = MPauto(lexeme, temp, ref rez);
                        break;
                    //сложить
                    case (int)Token.add:
                        temp = MP.Pop();
                        rez.commands.Push(temp);
                        for (int i = 0; i < 3; ++i)
                        {
                            temp = MP.Pop();
                            rez.commands.Push(temp);
                        }
                        temp = MP.Top();
                        exit = MPauto(lexeme, temp, ref rez);
                        break;
                    //умножить
                    case (int)Token.mov:
                        temp = MP.Pop();
                        rez.commands.Push(temp);
                        for (int i = 0; i < 3; ++i)
                        {
                            temp = MP.Pop();
                            rez.commands.Push(temp);
                        }
                        temp = MP.Top();
                        exit = MPauto(lexeme, temp, ref rez);
                        break;
                    //Условный переход по нулю
                    case (int)Token.tras0:
                        temp = MP.Pop();
                        rez.commands.Push(temp);
                        for (int i = 0; i < 2; ++i)
                        {
                            temp = MP.Pop();
                            rez.commands.Push(temp);
                        }
                        temp = MP.Top();
                        exit = MPauto(lexeme, temp, ref rez);
                        break;
                    //Условный переход по сравнению
                    case (int)Token.condtras:
                        temp = MP.Pop();
                        rez.commands.Push(temp);
                        for (int i = 0; i < 2; ++i)
                        {
                            temp = MP.Pop();
                            rez.commands.Push(temp);
                        }
                        temp = MP.Top();
                        exit = MPauto(lexeme, temp, ref rez);
                        break;
                    //Безусловный переход
                    case (int)Token.uncondtras:
                        temp = MP.Pop();
                        rez.commands.Push(temp);
                        temp = MP.Pop();
                        rez.commands.Push(temp);
                        temp = MP.Top();
                        exit = MPauto(lexeme, temp, ref rez);
                        break;
                    //Равно
                    case (int)Token.compare:
                        temp = MP.Pop();
                        rez.commands.Push(temp);
                        for (int i = 0; i < 3; ++i)
                        {
                            temp = MP.Pop();
                            rez.commands.Push(temp);
                        }
                        temp = MP.Top();
                        exit = MPauto(lexeme, temp, ref rez);
                        break;
                    //Не равно
                    case (int)Token.compare2:
                        temp = MP.Pop();
                        rez.commands.Push(temp);
                        for (int i = 0; i < 3; ++i)
                        {
                            temp = MP.Pop();
                            rez.commands.Push(temp);
                        }
                        temp = MP.Top();
                        exit = MPauto(lexeme, temp, ref rez);
                        break;
                    //Метка
                    case (int)Token.label:
                        MP.Pop(); //вытолкнули метку
                        tempmas = MP.Pop(); // атрибут метки
                        rez.labels.Mas[tempmas] = rez.commands.Size();
                        temp = MP.Top();
                        exit = MPauto(lexeme, temp, ref rez);
                        break;

                    default:
                        exit = false;
                        break;
                }
            }
            else
                exit = false;
            return exit;
        }

        //Интерпретатор
        public void Interpret(ref Result rez){
            int i = 0;
            int elem = rez.commands.Mas[0];
            while (i < rez.commands.Size()){
                //int elem = rez.commands.Mas[i];
                switch (elem){
                    case (int)Token.assign:
                        rez.data.Mas[rez.commands.Mas[i + 1]].value = rez.data.Mas[rez.commands.Mas[i + 2]].value;
                        i += 3;
                        elem = rez.commands.Mas[i];
                        break;
                    case (int)Token.add:
                        rez.data.Mas[rez.commands.Mas[i + 3]].value = rez.data.Mas[rez.commands.Mas[i + 1]].value + rez.data.Mas[rez.commands.Mas[i + 2]].value;
                        i += 4;
                        elem = rez.commands.Mas[i];
                        break;
                    case (int)Token.mov:
                        rez.data.Mas[rez.commands.Mas[i + 3]].value = rez.data.Mas[rez.commands.Mas[i + 1]].value * rez.data.Mas[rez.commands.Mas[i + 2]].value;
                        i += 4;
                        elem = rez.commands.Mas[i];
                        break;
                    case (int)Token.compare:
                        if (rez.data.Mas[rez.commands.Mas[i + 1]].value == rez.data.Mas[rez.commands.Mas[i + 2]].value)
                            rez.data.Mas[rez.commands.Mas[i + 3]].value = 1;
                        else
                            rez.data.Mas[rez.commands.Mas[i + 3]].value = 0;
                        i += 4;
                        elem = rez.commands.Mas[i];
                        break;
                    case (int)Token.compare2:
                        if (rez.data.Mas[rez.commands.Mas[i + 1]].value != rez.data.Mas[rez.commands.Mas[i + 2]].value)
                            rez.data.Mas[rez.commands.Mas[i + 3]].value = 1;
                        else
                            rez.data.Mas[rez.commands.Mas[i + 3]].value = 0;
                        i += 4;
                        elem = rez.commands.Mas[i];
                        break;
                    case (int)Token.label:
                        i = rez.labels.Mas[rez.commands.Mas[i + 1]];
                        elem = rez.commands.Mas[i];
                        break;
                    case (int)Token.condtras:
                        if (rez.data.Mas[rez.commands.Mas[i + 1]].value == 1){
                            i = rez.labels.Mas[rez.commands.Mas[i + 2]];
                            elem = rez.commands.Mas[i];
                        }
                        else{
                            i += 3;
                            elem = rez.commands.Mas[i];
                        }
                        break;
                    case (int)Token.tras0:
                        if (rez.data.Mas[rez.commands.Mas[i + 1]].value == 0){
                            i = rez.labels.Mas[rez.commands.Mas[i + 2]];
                            elem = rez.commands.Mas[i];
                        }
                        else{
                            i += 3;
                            elem = rez.commands.Mas[i];
                        }
                        break;
                    case (int)Token.uncondtras:
                        i = rez.labels.Mas[rez.commands.Mas[i + 1]];
                        elem = rez.commands.Mas[i];
                        break;
                    default:
                        break;
                }
            }
            return;
        }
    }
}

