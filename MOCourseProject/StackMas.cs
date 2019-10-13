using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOCourseProject
{
    //стек на основе массива - работа с данными, метками и командами
    class StackMas<T>{
        //стек
        public T[] Mas;
        //голова стека - изначально -1, потом - размер стека
        private int Head;
        
        public int HeadDostup{
            get{
                return Head;
            }
            set{
                Head = value;
            }
        }

        public StackMas(int N){
            Mas = new T[N];
            Head = -1;
        }

        public void Push(T elem){
            Mas[++HeadDostup] = elem;
        }

        public int Size(){
            return HeadDostup + 1; 
        }

        //вернуть значение из стека по индексу
        public T Masindex(int i){
            return Mas[i];
        }

        public T Pop(){
            return Mas[HeadDostup--];
        }

        public T Top(){
            return Mas[HeadDostup];
        }

        public bool Empty(){
            if (HeadDostup == -1)
                return true;
            else
                return false;
        }
    }
}
