using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureStore
{
    public interface IDisassembling { void Disassemble(); }//Разбирается на части
    public enum TypeCupboard
    {
        SWardrobe = 1,            //Одностворчатый
        DWardrobe,              //Двухстворчатый
        TWardrobe                //Трехстворчатый
    }
    class Cupboard : Furniture, IDisassembling
    {
        private TypeCupboard f_typeCupboard;//тип хранилища
        private string f_material;//его материал
        public TypeCupboard typeCupboard
        {
            get { return f_typeCupboard; }
            set { f_typeCupboard = value; }
        }
        public string material
        {
            get { return f_material; }
            set { f_material = value; }
        }
        public Cupboard() : //Пустой конструктор 
            base()
        {
            f_typeCupboard = 0;
            f_material = "Unknow";
        }
        public Cupboard(string b_name, int f_cost, TypeCupboard f_typeCupboard, string f_material) //Конструктор 
            : base(b_name, f_cost)
        {
            typeCupboard = f_typeCupboard;
            material = f_material;
        }

        public void Disassemble()
        {
            Console.WriteLine("Хранилище разобрано");
        }
        public override string ToString()
        {
            return "Хранилище  " + base.ToString() + String.Format(" Тип: {0} Материал: {1} ", this.typeCupboard, this.material);
        }
    }
}
