using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Net;
using System.Globalization;
using System.Data.SqlClient;
using System.Data;
using System.Net.Sockets;
using System.Security.Cryptography;


namespace CSSample
{
    class Tester
    {
        // ***********************************************************	EDIT THIS :
        string serverProgID = ConfigurationManager.AppSettings["opcID"];         // ProgID of OPC server

        private OpcServer theSrv;
        private OpcGroup theGrp;
        private static float[] currentValues;
        private static string responseStringG = "";
        private static HttpListener listener = new HttpListener();

        private static string consoleOut = ConfigurationManager.AppSettings["consoleOutput"];
        private static string answerType = ConfigurationManager.AppSettings["answerType"];
        private static string portNumb = ConfigurationManager.AppSettings["portNumber"];
        private static int timeref = Int32.Parse(ConfigurationManager.AppSettings["refreshTime"]);
        private static string[] tagsNames = ConfigurationManager.AppSettings["tagsNames"].Split(','); // tags from config
        private static string[] ratios = ConfigurationManager.AppSettings["ratios"].Split(',');

        private static string sqlSend = ConfigurationManager.AppSettings["sqlSend"];
        private static string udpSend = ConfigurationManager.AppSettings["udpSend"];
        private static string webSend = ConfigurationManager.AppSettings["webSend"];
        private static string table_name = ConfigurationManager.AppSettings["table"]; // название таблицы из конфига;
        private static string column_name = ConfigurationManager.AppSettings["column"];
        private static int sendtags = Int32.Parse(ConfigurationManager.AppSettings["tags2send"]);

        private static IPAddress remoteIPAddress = IPAddress.Parse(ConfigurationManager.AppSettings["remoteIP"]); // Ip from config
        private static int remotePort = Convert.ToInt16(ConfigurationManager.AppSettings["remotePort"]); // remote port from config

        public static SqlConnection myConn = new SqlConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString); //строка соединения с SQL которая берется из конфига
        SqlCommand myCommand = new SqlCommand("Command String", myConn);

        public void Work()
        {
            /*	try						// disabled for debugging
                {	*/

            theSrv = new OpcServer();
            theSrv.Connect(serverProgID);
            Thread.Sleep(500);              // we are faster then some servers!

            // add our only working group
            theGrp = theSrv.AddGroup("OPCCSharp-Group", false, timeref);

            string[] tags = ConfigurationManager.AppSettings["tags"].Split(','); // tags from config
            if (sendtags > tags.Length) sendtags = tags.Length;

            var itemDefs = new OPCItemDef[tags.Length];
            for (var i = 0; i < tags.Length; i++)
            {
                itemDefs[i] = new OPCItemDef(tags[i], true, i, VarEnum.VT_EMPTY);
            }

            OPCItemResult[] rItm;
            theGrp.AddItems(itemDefs, out rItm);
            if (rItm == null)
                return;
            if (HRESULTS.Failed(rItm[0].Error) || HRESULTS.Failed(rItm[1].Error))
            {
                Console.WriteLine("OPC Tester: AddItems - some failed"); theGrp.Remove(true); theSrv.Disconnect(); return;

            };

            var handlesSrv = new int[itemDefs.Length];
            for (var i = 0; i < itemDefs.Length; i++)
            {
                handlesSrv[i] = rItm[i].HandleServer;
            }

            currentValues = new Single[itemDefs.Length];

            // asynch read our two items
            theGrp.SetEnable(true);
            theGrp.Active = true;
            theGrp.DataChanged += new DataChangeEventHandler(this.theGrp_DataChange);
            theGrp.ReadCompleted += new ReadCompleteEventHandler(this.theGrp_ReadComplete);


            int CancelID;

            int[] aE;
            theGrp.Read(handlesSrv, 55667788, out CancelID, out aE);

            // some delay for asynch read-complete callback (simplification)
            Thread.Sleep(500);

            while (webSend == "yes")
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                context.Response.AddHeader("Access-Control-Allow-Origin", "*");


                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseStringG);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            }
            // disconnect and close
            Console.WriteLine("************************************** hit <return> to close...");
            Console.ReadLine();
            theGrp.ReadCompleted -= new ReadCompleteEventHandler(this.theGrp_ReadComplete);
            theGrp.RemoveItems(handlesSrv, out aE);
            theGrp.Remove(false);
            theSrv.Disconnect();
            theGrp = null;
            theSrv = null;


            /*	}
            catch( Exception e )
                {
                Console.WriteLine( "EXCEPTION : OPC Tester " + e.ToString() );
                return;
                }	*/
        }

        // ------------------------------ events -----------------------------

        public void theGrp_DataChange(object sender, DataChangeEventArgs e)
        {

            foreach (OPCItemState s in e.sts)
            {
                if (HRESULTS.Succeeded(s.Error))
                {
                    if (consoleOut == "yes")
                    {
                        Console.WriteLine(" ih={0} v={1} q={2} t={3}", s.HandleClient, s.DataValue, s.Quality, s.TimeStamp); // выводит данные при изменении в консоль
                    }
                    currentValues[s.HandleClient] = Convert.ToSingle(s.DataValue) * Single.Parse(ratios[s.HandleClient], CultureInfo.InvariantCulture.NumberFormat); //добавляет в массив измененное значение тега
                }
                else
                    Console.WriteLine(" ih={0}    ERROR=0x{1:x} !", s.HandleClient, s.Error);
            }
            string responseString = "{";
            if (answerType == "table")
            {
                responseString = "<HTML><head><meta charset=\"UTF-8\"><meta http-equiv=\"Refresh\" content=\"" + timeref / 1000 + "\"/></head>" +
            "<BODY><table border><tr><td>" + string.Join("<br>", tagsNames) + "</td><td >" + string.Join("<br>", currentValues) + "</td></tr></table></BODY></HTML>";
                responseStringG = responseString;
            }
            else
            {
                for (int i = 0; i < currentValues.Length - 1; i++) responseString = responseString + "\"tag" + i + "\":\"" + currentValues[i] + "\", ";
                responseString = responseString + "\"tag" + (currentValues.Length - 1) + "\":\"" + currentValues[currentValues.Length - 1] + "\"}";
                responseStringG = responseString;
            }
            byte[] byteArray = new byte[sendtags * 4];
            Buffer.BlockCopy(currentValues, 0, byteArray, 0, byteArray.Length);
            if (sqlSend == "yes")
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO " + table_name + " (" + column_name + ") values (@bindata)", myConn);
                    myConn.Open();
                    var param = new SqlParameter("@bindata", SqlDbType.Binary)
                    { Value = byteArray };
                    cmd.Parameters.Add(param);
                    cmd.ExecuteNonQuery();
                    myConn.Close();
                }
                catch (Exception err)
                {
                    Console.WriteLine("SQL-exception: " + err.ToString());
                    return;
                }
            }

            if (udpSend == "yes") UDPsend(byteArray);
        }

        private static void UDPsend(byte[] datagram)
        {
            // Создаем UdpClient
            UdpClient sender = new UdpClient();

            // Создаем endPoint по информации об удаленном хосте
            IPEndPoint endPoint = new IPEndPoint(remoteIPAddress, remotePort);

            try
            {

                sender.Send(datagram, datagram.Length, endPoint);
                //Console.WriteLine("Sended", datagram);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Возникло исключение: " + ex.ToString() + "\n  " + ex.Message);
            }
            finally
            {
                // Закрыть соединение
                sender.Close();
            }
        }
        public void theGrp_ReadComplete(object sender, ReadCompleteEventArgs e)
        {
            Console.WriteLine("ReadComplete event: gh={0} id={1} me={2} mq={3}", e.groupHandleClient, e.transactionID, e.masterError, e.masterQuality);
            foreach (OPCItemState s in e.sts)
            {
                if (HRESULTS.Succeeded(s.Error))
                {
                    Console.WriteLine(" ih={0} v={1} q={2} t={3}", s.HandleClient, s.DataValue, s.Quality, s.TimeStamp);
                }
                else
                    Console.WriteLine(" ih={0}    ERROR=0x{1:x} !", s.HandleClient, s.Error);
            }
        }

        static void Main(string[] args)
        {
            string url = "http://*";
            string port = portNumb;
            string prefix = String.Format("{0}:{1}/", url, port);
            listener.Prefixes.Add(prefix);
            listener.Start();

            Tester tst = new Tester();
            tst.Work();
        }
    }
}