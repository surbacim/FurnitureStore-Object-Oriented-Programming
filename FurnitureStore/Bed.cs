using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureStore
{
    public interface IUnfolding { void Unfold(); }//Кровать расскладывается
    public enum TypeBed    //Тип кроватей
    {
        Single = 1,            //Односпальная
        Double,              //Двуспальная
        Lorry                //Полуторная
    }
    class Bed : Furniture, IUnfolding
    {
        private TypeBed f_typeBed;//Тип спальной мебели
        private string f_design;//конструкция
        public TypeBed typeBed
        {
            get { return f_typeBed; }
            set { f_typeBed = value; }
        }
        public string design
        {
            get { return f_design; }
            set { f_design = value; }
        }
        public Bed() : //Пустой конструктор 
            base()
        {
            f_design = "Unknow";
            f_typeBed = 0;
        }
        public Bed(string b_name, int f_cost, string f_design, TypeBed f_typeBed) //Конструктор 
            : base(b_name, f_cost)
        {
            design = f_design;
            typeBed = f_typeBed;
        }

        public void Unfold()
        {
            Console.WriteLine("Кровать разложена");
        }

        public override string ToString()
        {
            return "Спальная мебель  " + base.ToString() + String.Format(" Тип: {0} Конструкция: {1} ", this.typeBed, this.design);
        }
    }
}
