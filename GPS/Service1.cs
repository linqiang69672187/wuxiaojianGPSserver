using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GPS
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }
        private log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       // private DataTable dt;
        protected override void OnStart(string[] args)
        {
            try
            {
               // dt = GetTableSchema();
                Thread cz_thread = new Thread(new ThreadStart(CZreceiver));
                cz_thread.Start();
                log.Info("车载视频GPS监听启动:" );
            }
            catch (Exception ex)
            {

               log.Error(ex.ToString());
            }

        }
        public static void CZreceiver()
        {
            UdpClient client = null;
            string receiveString = null;
            byte[] receiveData = null;
            //实例化一个远程端点，IP和端口可以随意指定，等调用client.Receive(ref remotePoint)时会将该端点改成真正发送端端点 
            IPEndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);
           log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            while (true)
            {
                client = new UdpClient(9588);
                receiveData = client.Receive(ref remotePoint);//接收数据 
                receiveString = Encoding.Default.GetString(receiveData);
                ServiceResult rsdevice = ServiceResult.Parse(receiveString);
                if (rsdevice.Command == "GPS")
                {
                    StringBuilder sbSQL = new StringBuilder("");
                    sbSQL.Append("INSERT INTO HistoryGps_CZSP ([DevType] ,[PDAID],[SendTime],[Lo],[La])  VALUES (1,'" + rsdevice.DeviceID + "','" + rsdevice.Time + "'," + rsdevice.Latitude + "," + rsdevice.Longitude + ")");

                    try
                    {
                        DbComponent.SQLHelper.ExecuteNonQuery(CommandType.Text, sbSQL.ToString());
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.ToString());
                    }
                   //log.Info("设备ID:" + rsdevice.DeviceID + ";时间:" + rsdevice.Time + ";经度:" + rsdevice.Longitude + ";纬度:" + rsdevice.Latitude + ";经度:" + rsdevice.Longitude + ";类型:车载视频");
                }
                client.Close();//关闭连接 
            }

        }




        #region DataTABLE
        static DataTable GetTableSchema()
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] {   
        new DataColumn("DevType",typeof(int)),  
        new DataColumn("PDAID",typeof(string)),  
        new DataColumn("SendTime",typeof(DateTime)),  
        new DataColumn("Lo",typeof(float)),  
        new DataColumn("La",typeof(float))
            
            });
            return dt;
        }




         //for (int i = 0; i < totalRow; i++)
         //       {
         //           DataRow dr = dt.NewRow();
         //           dr[0] = Guid.NewGuid();
         //           dr[1] = string.Format("商品", i);
         //           dr[2] = (decimal)i;
         //           dt.Rows.Add(dr);
         //       }
        #endregion





        protected override void OnStop()
        {

        }
    }
}
