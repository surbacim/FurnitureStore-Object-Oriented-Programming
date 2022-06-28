using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureStore
{
    public abstract class Furniture : IComparable
    {
        public string f_name;//Название
        public int f_cost;//Цена
        private static int ID = 0;
        private int Id_furniture;
        public bool Active { get; set; }
        public string name
        {
            get { return f_name; }
            set { f_name = value; }
        }
        public int cost
        {
            get { return f_cost; }
            set { f_cost = value; }
        }
        public int IDFurniture { get { return Id_furniture; } }
        public Furniture() //Пустой конструктор 
        {
            f_name = "No name";
            f_cost = 0;
            ID++;
            Id_furniture = ID;
            Active = true;
        }
        public Furniture(string f_name, int f_cost) //Конструктор
        {
            name = f_name;
            cost = f_cost;
            ID++;
            Id_furniture = ID;
            Active = true;
        }
        public override string ToString()
        {
            return String.Format("Название: {0}  Цена: {1}  ", this.name, this.cost);
        }
        int IComparable.CompareTo(object obj)
        {
            Furniture temp = obj as Furniture;
            if (temp != null)
                return this.name.CompareTo(temp.name);
            else
                throw new ArgumentException("Parameter is not a Furniture!");
        }

    }
}
