using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;


namespace FurnitureStore
{
    class Program
    {
        public static void OnBuying(string message)
        {
            Console.WriteLine("Event {0} {1}", message, DateTime.Now);
        }
        static void Main(string[] args)
        {


            #region TestFurniturestore
            Furniture[] fs = { new Bed("Тахта", 6499, "Емкая, угловая", TypeBed.Single),
            new Cupboard("Сервант", 13000, TypeCupboard.DWardrobe, "Кедр глянец"),
            new Bed("Кровать", 9980, "С подъемным механизмом", TypeBed.Double),
            new Cupboard("Буфет", 22999, TypeCupboard.TWardrobe, "Ольха"),
           new Bed("Кровать", 24799, "Подиум без спинок", TypeBed.Double),
           new Bed("Кушетка", 10999, "Ложе с одной спинкой", TypeBed.Lorry),
           new Cupboard("Комод", 4480, TypeCupboard.TWardrobe, "Венге"),
           new Cupboard("Бар", 2930, TypeCupboard.SWardrobe, "Старое дерево"),
           new Bed("Софа", 23999, "Деревянная, обшитая тканью", TypeBed.Lorry),
           new Bed("Диван", 18000, "Диван-книжка", TypeBed.Double),
            new Cupboard("Трюмо", 5540, TypeCupboard.SWardrobe, "Махагон"),
           new Cupboard("Секретер", 15590, TypeCupboard.DWardrobe, "Дуб светлый"),
            };
            Console.WriteLine();
            #endregion
            #region TestFurnitureStoreWithEvents
            FurnitureStoreWithEvents fse = new FurnitureStoreWithEvents();

            Buyer[] br =
            {
             new Buyer("Светлана"),
             new Buyer("Роман"),
             new Buyer("Георгий"),
             new Buyer("Елена"),
             new Buyer("Марина")
         };
            fse.InitDB();
            Random rnd = new Random();
            Thread[] thArray = new Thread[br.Length];
            ActionsBuyer[] acts = new ActionsBuyer[br.Length];
            for (int i = 0; i < br.Length; i++)
            {
                acts[i] = new ActionsBuyer(br[i], fs[rnd.Next(fs.Length)], fse, 1200 + (i % 2) * 300);
                acts[i].InitEvent();
                thArray[i] = new Thread(acts[i].DoActions);
                thArray[i].Start();
            }
            bool b = true;
            while (b)
            {
                b = false;
                foreach (var thread in thArray)
                {
                    b = thread.IsAlive || b;
                }
            }
            Console.WriteLine("All threads end work!");
            Console.WriteLine();
            Console.WriteLine("-----------------------------");
            Console.WriteLine();
            Console.WriteLine("Journal");

            for (int i = 0; i < br.Length; i++)
            {
                foreach (Operation op in fse.GetJournalForBuyer(br[i]))
                    Console.WriteLine(op);
            }
            Console.WriteLine();
            Console.WriteLine("-----------------------------");
            Console.WriteLine();
            fse.WriteToXML("out.xml");
            // fse.ReadXML_Journal("out.xml");
            Console.WriteLine();
            Console.WriteLine("---------ReportDb--------------------");
            Console.WriteLine();
            fse.ListDB();
            Console.WriteLine();
            Console.WriteLine("----------ReportDbForBuyer-------------------");
            Console.WriteLine();

            for (int i = 0; i < br.Length; i++)
            {
                fse.ListDBForBuyer(br[i]);
            }
            // очистка последних N элементов таблицы Operations
            //  fse.CleanOperations(1000);
            fse.QuitDB();
            Console.ReadKey();
        }
        #endregion
    }

}
