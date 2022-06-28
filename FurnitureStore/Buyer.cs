using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FurnitureStore
{
    public class Buyer
    {
        public string Name;//Имя
        public bool Active { get; set; }
        private static int ID = 0;
        private int Id_buyer;
        public int IDBuyer { get { return Id_buyer; } }
        public Buyer(string n)//Конструктор 
        {
            this.Name = n;
            Active = true;
            ID++;
            Id_buyer = ID;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
