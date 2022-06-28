using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FurnitureStore
{
    public delegate void EventBuyer(Operation evBuyer);
    class ActionsBuyer
    {
        public Buyer br; // Источник события - покупатель
        public event EventBuyer event_actions;
        public FurnitureStoreWithEvents fse;
        public Furniture fr; // Мебель, с которою взаимодействует покупатель
        public int timing;
        static Random rnd = new Random();


        public ActionsBuyer() { br = null; fr = null; fse = null; timing = 0; } //Пустой конструктор 
        public ActionsBuyer(Buyer b, Furniture f, FurnitureStoreWithEvents fe, int t) //Конструктор 
        {
            br = b; fr = f; fse = fe; timing = t;
        }
        // Регистрация обработчика событий 
        public void InitEvent()
        {
            if (fse != null) event_actions += fse.OnEventFurniture;
        }

        // Действе - установка
        public void SettingFurniture(Buyer buyer, Furniture curfurniture, int intervalSetting)
        {
            if (curfurniture == null) { return; }
            if (buyer == null) { return; }
            Operation ops = new Operation();
            ops.to = TypeOperation.StartSetting;
            ops.br = buyer;
            ops.fr = curfurniture;
            ops.Message = "Начали установку мебели";
            buyer.Active = false;
            //  curfurniture.Active = false;
            if (event_actions != null) event_actions(ops);
            Thread.Sleep(intervalSetting);
            ops = new Operation();
            ops.to = TypeOperation.FinishSetting;
            ops.br = buyer;
            ops.fr = curfurniture;
            ops.Message = "Закончили установку мебели";
            if (event_actions != null) event_actions(ops);
            buyer.Active = true;
            //  curfurniture.Active = true;
        }
        //Действие - Покупка мебели
        public void BuyingFurniture(Buyer buyer, Furniture curfurniture)
        {
            if (buyer == null) { return; }
            curfurniture.Active = false;
            Operation ops = new Operation();
            ops.to = TypeOperation.Buying;
            ops.br = buyer;
            ops.fr = curfurniture;
            ops.Message = "Купили мебель";
            if (event_actions != null) event_actions(ops);
            fse.DeleteFurniture(curfurniture);
        }
        //Действие - доставка мебели
        public void DeliveryFromStore(Buyer buyer, Furniture curfurniture)
        {
            if (curfurniture == null) { return; }
            curfurniture.Active = false;
            Operation ops = new Operation();
            ops.to = TypeOperation.Delivery;
            ops.br = buyer;
            ops.fr = curfurniture;
            ops.Message = "Доставили мебель";
            if (event_actions != null) event_actions(ops);
            
        }

        //Действие - пользование мебелью после установки
        public void UsingFurniture(Buyer buyer, Furniture curfurniture)
        {
            if (curfurniture == null) { return; }
            curfurniture.Active = false;
            Operation ops = new Operation();
            ops.to = TypeOperation.Using;
            ops.br = buyer;
            ops.fr = curfurniture;
            ops.Message = "Мебелью начали пользоваться";
            if (event_actions != null) event_actions(ops);

        }

        //Генерация событий
        public void DoActions()
        {
            Thread.Sleep(2000);
            if (br != null && fr != null && timing > 0)
            {
                fr.Active = false;
                Thread.Sleep(rnd.Next(timing));
                BuyingFurniture(br, fr);
                Thread.Sleep(rnd.Next(timing));
                DeliveryFromStore(br, fr);
                Thread.Sleep(rnd.Next(timing));
                SettingFurniture(br, fr, timing);
                Thread.Sleep(rnd.Next(timing));
                UsingFurniture(br, fr);
                fr.Active = true;
            }
        }
    }
}
