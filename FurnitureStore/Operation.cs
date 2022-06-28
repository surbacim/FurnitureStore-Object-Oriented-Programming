using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureStore
{

    //Тип событий
    public enum TypeOperation
    {
        Buying = 1,         //Покупка мебели
        Delivery,            //Доставка мебели
        StartSetting,    //Начало установки мебели
        FinishSetting,        //Конец установки мебели
        Using,              //Использование мебели
    }
    public class Operation
    {
        public Buyer br;
        public TypeOperation to;
        public Furniture fr;
        public DateTime timeop;
        public string Message;
        public Operation() { fr = null; br = null; timeop = DateTime.Now; }
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4}", br, to, fr, timeop, Message);
        }
    }
}
