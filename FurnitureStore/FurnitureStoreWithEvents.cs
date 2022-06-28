using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace FurnitureStore
{


    class FurnitureStoreWithEvents : Furniturestore
    {
        public event EventBuyer BuyerEvents;
        public List<Operation> listops;
        //объекты бд
        public SqlConnection con;
        public SqlDataAdapter daEv, daTF, daFr, daBr, daOp;
        public SqlCommandBuilder cmdEv, cmdTF, cmdFr, cmdBr, cmdOp;
        public DataSet DS;
        public DataTable dtEv, dtTF, dtFr, dtBr, dtOp;
        int idop = 0;
        public FurnitureStoreWithEvents()
        {
            listops = new List<Operation>();
            BuyerEvents += OnEventFurniture;
        }
        //Обработчик событий
        public virtual void OnEventFurniture(Operation opr)
        {
            lock (this)
            {
                if (opr == null) { Console.WriteLine("opr is null"); return; }
                if (opr.br == null) { Console.WriteLine("br is null"); return; }
                try
                {
                    listops.Add(opr);
                    //DB
                    Furniture curfr = opr.fr;
                    Buyer curbr = opr.br;

                    int idfr = curfr.IDFurniture;
                    int idbr = curbr.IDBuyer;

                    DataRow[] selectfr = dtFr.Select(string.Format("IdFr={0}", idfr));
                    DataRow[] selectbr = dtBr.Select(string.Format("IdBr={0}", idbr));

                    if (selectfr.Length == 0 && selectbr.Length == 0)
                    {
                        DataRow drfr = dtFr.NewRow();
                        DataRow drbr = dtBr.NewRow();

                        drfr["IdFr"] = idfr;
                        drfr["Name"] = curfr.name;
                        drfr["Cost"] = curfr.cost;
                        drfr["IdTF"] = ID_TypeFurniture(curfr);
                        dtFr.Rows.Add(drfr);

                        drbr["IdBr"] = idbr;
                        drbr["Name"] = curbr.Name;
                        dtBr.Rows.Add(drbr);
                    }
                    //Операции

                    DataRow dropr = dtOp.NewRow();
                    idop++;

                    dropr["IdOp"] = idop;
                    dropr["IdFr"] = idfr;
                    dropr["IdBr"] = idbr;
                    dropr["IdEv"] = opr.to;
                    dropr["DateTime"] = opr.timeop;
                    dropr["Message"] = opr.Message;

                    dtOp.Rows.Add(dropr);
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.Message);
                    Console.WriteLine("Event={0} Furniture={1} Buyer={2}", opr.to, opr.fr, opr.br);
                }
            }
        }
        //Отчет по времени
        public IEnumerable<Operation> GetOperationsWithTimeInterval(DateTime beginDT, DateTime endDT)
        {
            IEnumerable<Operation> query = from oj in listops
                                           where (oj.timeop >= beginDT && oj.timeop <= endDT)
                                           select oj;
            return query;
        }
        //Отчет по читателям
        public IEnumerable<Operation> GetJournalForBuyer(Buyer curbuyer)
        {
            IEnumerable<Operation> query = from oj in listops
                                           where (oj.br == curbuyer)
                                           select oj;
            return query;
        }
        //Журнал XML
        public void WriteToXML(string filename)
        {
            Console.WriteLine("Start to XML");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;//Использование отступов
            settings.IndentChars = (" ");//Символы для отступов
            XmlWriter writer = XmlWriter.Create(filename, settings);
            int id = 1;
            writer.WriteStartElement("Operations");
            foreach (var op in listops)
            {
                writer.WriteStartElement("Operation");
                writer.WriteAttributeString("ID", id.ToString());
                writer.WriteElementString("Type", op.to.ToString());
                writer.WriteStartElement("DateTime");
                writer.WriteElementString("Date", op.timeop.ToLongDateString());
                writer.WriteElementString("Time", op.timeop.ToShortTimeString());
                writer.WriteEndElement();
                writer.WriteStartElement("Buyer");
                writer.WriteElementString("Name", op.br.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("Furniture");
                if (op.fr is Bed)
                {
                    writer.WriteElementString("TypeFurniture", "Bed");
                }
                else
                {
                    writer.WriteElementString("TypeFurniture", "Cupboard");
                }
                writer.WriteElementString("Name", op.fr.name);
                writer.WriteElementString("Cost", op.fr.cost.ToString());
                writer.WriteEndElement();
                id++;
            }
            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
            Console.WriteLine("Finish to XML");
        }
        //Вывод журнала XML
        public void ReadXML_Journal(string namefile)
        {
            XmlReader xmlreader = XmlReader.Create(namefile);
            while (xmlreader.Read())
            {
                if (xmlreader.IsStartElement())
                {
                    Console.WriteLine("<{0}> ", xmlreader.Name);
                    if (xmlreader.HasAttributes)
                    {
                        Console.WriteLine("Attributes of <" + xmlreader.Name + ">");
                        while (xmlreader.MoveToNextAttribute())
                        {
                            Console.WriteLine(" {0}={1}", xmlreader.Name, xmlreader.Value);
                        }
                        xmlreader.MoveToElement();
                    }
                    if (xmlreader.HasValue) Console.WriteLine(xmlreader.Value);
                }
                if (xmlreader.HasValue) Console.WriteLine(xmlreader.Value);
                if (xmlreader.NodeType == XmlNodeType.EndElement) Console.WriteLine("</{0}>", xmlreader.Name);
            }
            xmlreader.Close();
        }
        /* public void DBTest()
         {
             Console.WriteLine("Start");
             try
             {
                 SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Юрий\source\repos\FurnitureStore\FurnitureStore\FurnitureStoreDataBase.mdf;Integrated Security=True;Connect Timeout=30");
                 con.Open();
                 SqlCommand com = new SqlCommand("select * from Events", con);
                 SqlDataReader reader = com.ExecuteReader();
                 if (reader.HasRows)
                 {
                     while (reader.Read())
                     {
                         Console.WriteLine(" first data {0} second data {1}", reader[0], reader[1]);
                     }
                 }
                 reader.Close();
                 con.Close();
             }
             catch (Exception e)
             {
                 Console.WriteLine(e.Message);
             }
             Console.WriteLine("Finish");
         }*/
        public int ID_TypeFurniture(Furniture fr)
        {
            if (fr is Bed) return 1;
            if (fr is Cupboard) return 2;
            return 0;
        }
        public void InitDB()
        {
            con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Юрий\source\repos\FurnitureStore\FurnitureStore\FurnitureStoreDataBase.mdf;Integrated Security=True;Connect Timeout=30");
            con.Open();
            DS = new DataSet("DB");

            daEv = new SqlDataAdapter("select * from Events", con);
            daTF = new SqlDataAdapter("select * from TypeFurniture", con);
            daFr = new SqlDataAdapter("select * from Furniture", con);
            daBr = new SqlDataAdapter("select * from Buyer", con);
            daOp = new SqlDataAdapter("select * from Operations", con);

            cmdEv = new SqlCommandBuilder(daEv);
            cmdTF = new SqlCommandBuilder(daTF);
            cmdFr = new SqlCommandBuilder(daFr);
            cmdBr = new SqlCommandBuilder(daBr);
            cmdOp = new SqlCommandBuilder(daOp);

            daEv.Fill(DS, "Events");
            daTF.Fill(DS, "TypeFurniture");
            daFr.Fill(DS, "Furniture");
            daBr.Fill(DS, "Buyer");
            daOp.Fill(DS, "Operations");

            dtEv = DS.Tables["Events"];
            dtTF = DS.Tables["TypeFurniture"];
            dtFr = DS.Tables["Furniture"];
            dtBr = DS.Tables["Buyer"];
            dtOp = DS.Tables["Operations"];

            idop = dtOp.Rows.Count;

            ViewDS(DS);
        }
        public void QuitDB()
        {
            daFr.Update(DS, "Furniture");
            daBr.Update(DS, "Buyer");
            daOp.Update(DS, "Operations");
            con.Close();
        }

        public void ViewDS(DataSet DS)
        {
            Console.WriteLine("DataSet is named: {0}", DS.DataSetName);
            // Вывести каждую таблицу.
            foreach (DataTable dt in DS.Tables)
            {
                ViewDataTable(dt);
            }
        }

        public void ViewDataTable(DataTable dt)
        {
            Console.WriteLine("\n----------------------------------");
            Console.WriteLine("Table =>  {0}", dt.TableName);
            // Вывести имена столбцов.
            for (int curCol = 0; curCol < dt.Columns.Count; curCol++)
            {
                Console.Write(dt.Columns[curCol].ColumnName + "\t");
            }
            Console.WriteLine();
            // Вывести DataTable.
            for (int curRow = 0; curRow < dt.Rows.Count; curRow++)
            {
                for (int curCol = 0; curCol < dt.Columns.Count; curCol++)
                {
                    Console.Write(dt.Rows[curRow][curCol].ToString() + "\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n----------------------------------");
        }
        //Просмотр БД
        public void ListDB()
        {
            var query = from tj in dtOp.AsEnumerable()
                        join tf in dtFr.AsEnumerable() on tj.Field<int>("IdFr") equals tf.Field<int>("IdFr")
                        join tr in dtBr.AsEnumerable() on tj.Field<int>("IdBr") equals tr.Field<int>("IdBr")
                        join te in dtEv.AsEnumerable() on tj.Field<int>("IdEv") equals te.Field<int>("IdEv")
                        select new
                        {
                            namef = tf.Field<string>("Name"),
                            costf = tf.Field<int>("Cost"),
                            nameb = tr.Field<string>("Name"),
                            dataevent = tj.Field<DateTime>("DateTime"),
                            nevents = te.Field<string>("NameEvent"),
                            idtf = tf.Field<int>("IdTF"),
                            mes = tj.Field<string>("Message")
                        };
            foreach (var op in query)
            {
                DataRow[] tfr = dtTF.Select(string.Format("IdTF={0}", op.idtf));
                Console.WriteLine("{0}  {1}  {2}  {3}  {4}  {5}  {6}  ", op.namef, op.costf, op.nameb, tfr[0]["Type"], op.nevents, op.dataevent, op.mes);
            }
        }
        //Просмотр БД для покупателя
        public void ListDBForBuyer(Buyer curbr)
        {
            var query = from tj in dtOp.AsEnumerable()
                        join tf in dtFr.AsEnumerable() on tj.Field<int>("IdFr") equals tf.Field<int>("IdFr")
                        join tb in dtBr.AsEnumerable() on tj.Field<int>("IdBr") equals tb.Field<int>("IdBr")
                        join te in dtEv.AsEnumerable() on tj.Field<int>("IdEv") equals te.Field<int>("IdEv")
                        select new
                        {
                            namef = tf.Field<string>("Name"),
                            costf = tf.Field<int>("Cost"),
                            nameb = tb.Field<string>("Name"),
                            dataevent = tj.Field<DateTime>("DateTime"),
                            nevents = te.Field<string>("NameEvent"),
                            idtf = tf.Field<int>("IdTF"),
                            mes = tj.Field<string>("Message"),
                            idBr = tb.Field<int>("IdBr")
                        };

            foreach (var op in query)
            {
                if (curbr.IDBuyer == op.idBr)
                {
                    DataRow[] tfr = dtTF.Select(string.Format("IdTF={0}", op.idtf));
                    Console.WriteLine("Operations for {0}  {1}  {2}  {3}  {4}  {5}  {6}  ", op.namef, op.nameb, op.costf, tfr[0]["Type"], op.nevents, op.dataevent, op.mes);
                }
            }
        }
        //Очистка таблицы операций
        public void CleanOperations(int id)
        {
            try
            {

                string sql = "Delete from Operations where IdOp < @id";

                SqlCommand cmd = new SqlCommand();

                cmd.Connection = con;
                cmd.CommandText = sql;

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

                int rowCount = cmd.ExecuteNonQuery();
                DS.AcceptChanges();

                Console.WriteLine("Очистил " + rowCount);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                Console.WriteLine(e.StackTrace);
            }


        }
    }
}
