using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace WebApi_TestClient
{
    public partial class Form1 : Form
    {
        private readonly string _host = string.Empty;

        public Form1()
        {
            InitializeComponent();

            _host = ConfigurationManager.AppSettings["HostWebAPI"];

            //Prueba1(); //Funciona
            //Prueba2(); //Functiona
        }

        #region TraceLog

        public class TraceList
        {
            public Brush Color { get; set; }
            public string Trace { get; set; }
        }

        private void AddTrace(string pMessage)
        {
            Action update = () =>
            {
                lstTraceLog.Items.Add(new TraceList
                {
                    Color = Brushes.Black,
                    Trace = string.Format("{0} - {1}", DateTime.Now.ToString("HH:mm:ss"), pMessage)
                });
                lstTraceLog.TopIndex = lstTraceLog.Items.Count - 1;
            };
            lstTraceLog.BeginInvoke(update);
        }

        private void lstTraceLog_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            var lstTrace = sender as ListBox;

            var item = (lstTrace != null) ? lstTrace.Items[e.Index] as TraceList : null;

            if (item != null)
            {
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    e = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index,
                                                e.State ^ DrawItemState.Selected,
                                                e.ForeColor, Color.White);

                    e.DrawBackground();
                }

                e.Graphics.DrawString(item.Trace, e.Font, item.Color, e.Bounds, StringFormat.GenericDefault);

                e.DrawFocusRectangle();
            }
        }

        #endregion

        #region Pruebas básicas

        private void btnTest1_Click(object sender, EventArgs e)
        {
            Prueba1();
        }

        private void btnTest2_Click(object sender, EventArgs e)
        {
            Prueba2();
        }

        private static async void Prueba2()
        {
            var api = new WebApi.Client.RuleMotorClient("http://psmnt96/ECCO/webapi/");
            var response = await api.PostTest();

            var a = 0;
        }

        private static void Prueba1()
        {

            try
            {
                string url = string.Format("http://psmnt96/ECCO/webapi/api/RuleMotor/PostTest");

                JObject pAttributes = new JObject();
                pAttributes.Add("parameter1", JToken.FromObject(new List<int>() { 1, 2, 3 }));
                pAttributes.Add("parameter2", "Hola Mundo");

                string jsonText = pAttributes.ToString();

                using (var client = new HttpClient())
                {
                    var httpContent = new StringContent(jsonText);
                    httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    //var response = client.PostAsync(url, httpContent);
                    //response.GetAwaiter();
                    ////response.ConfigureAwait(false);
                    //var asd = response.Result;

                    //Funciona:
                    HttpResponseMessage response = client.PostAsync(url, httpContent).Result;
                    response.EnsureSuccessStatusCode();
                    var json = response.Content.ReadAsStringAsync().Result;


                    //using (var response = await client.PostAsync(url, httpContent))
                    //{
                    //    if (response.IsSuccessStatusCode)
                    //    {
                    //        var result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(bool));
                    //    }
                    //}
                }
            }
            catch (System.Exception er)
            {
                var a = 0;
            }
        }

        #endregion

        #region Device Client

        private async void txtGetDeviceByIp_Click(object sender, EventArgs e)
        {
            var ip = txtDeviceIp.Text.Trim();

            var api = new WebApi.Client.DeviceClient(_host);

            var device = await api.GetDeviceByIp(ip);

            string msg = string.Format("{0} {1}", "GetDeviceByIp()", device != null ? device.Id_Device + " " + device.Description : "null");
            AddTrace(msg);
        }

        private async void btnGetGroupsByIp_Click(object sender, EventArgs e)
        {
            var ip = txtGroupsIp.Text.Trim();

            var api = new WebApi.Client.DeviceClient(_host);

            var groups = await api.GetGroupsByIp(ip);

            string msg = string.Format("{0} {1}", "GetGroupsByIp()", groups != null ? string.Join(",", groups) : "null");
            AddTrace(msg);
        }

        #endregion

        #region Group Client

        private async void btnGetGroupNames_Click(object sender, EventArgs e)
        {
            //List<int> ids = new List<int>();
            //var strIds = txtGetGroupNames.Text.Trim().Split(';');

            //foreach (var item in strIds)
            //{
            //    ids.Add(Convert.ToInt32(item));
            //}

            //var api = new WebApi.Client.GroupClient(_host);
            //var names = await api.GetGroupNames(ids);

            //var str = "null";
            //if (names != null)
            //{
            //    str = "";

            //    foreach (var item in names)
            //    {
            //        str += item.Key + "-" + item.Value + " ";
            //    }
            //}

            //string msg = string.Format("{0} {1}", "GetGroupNames()", str);

            //AddTrace(msg);
        }

        #endregion

        #region Historic Client

        private async void btnListar_Click(object sender, EventArgs e)
        {
            string strUser = txtListarUser.Text.Trim();
            int[] users = null;

            if (!string.IsNullOrEmpty(strUser))
            {
                int user = 0;
                if (int.TryParse(strUser, out user))
                    users = new int[] { user };
            }

            string strDevice = txtListarDevice.Text.Trim();
            int[] devices = null;

            if (!string.IsNullOrEmpty(strDevice))
            {
                int device = 0;
                if (int.TryParse(strDevice, out device))
                    devices = new int[] { device };
            }

            WebApi.Client.RuleMotorClient ruleMotor = new WebApi.Client.RuleMotorClient(_host);
            var pending = await ruleMotor.ListPendingNotificationsWithState(0, 5, new List<int> { 4 }, 50);

            var api = new WebApi.Client.HistoricClient(_host);

            //var historics = await api.Listar(null, null, users, devices, null, 0, 50);
            //var historics = await api.Listar(null, null, null, new int[] { 2 }, new int[] { 1, 2, 3}, null, 0, 200);
            var historics = await api.Listar(null, null, new int[] { 0 }, new int[] { 5 }, new int[] { 4 }, null, null, 0, 50, false);

            string msg = string.Format("Listar() Número de elementos obtenidos: {0}", historics != null ? historics.Count.ToString() : "null");

            AddTrace(msg);
        }

        #endregion

        #region RuleMotor Client

        private async void btnMarkActionStateAs_Click(object sender, EventArgs e)
        {
            var user = txtMarkActionUser.Text.Trim();
            var device = txtMarkActionDevice.Text.Trim();
            var state = txtMarkActionState.Text.Trim();

            var api = new WebApi.Client.RuleMotorClient(_host);
            await api.MarkActionStateAs(1, Model.BL.DTO.Enums.Notification_State.Enviado, 1, 1);
        }

        private async void btnListActiveRules_Click(object sender, EventArgs e)
        {
            var api = new WebApi.Client.RuleMotorClient(_host);
            await api.ListActiveRules(new int[] { 16,18,50,51,52 });
        }

        private async void btnListPending_Click(object sender, EventArgs e)
        {
            var api = new WebApi.Client.RuleMotorClient(_host);
            //await api.ListPendingNotificationsWithState(5, 0, new List<int> { 1, 2, 3 }, 50);
            await api.ListPendingNotificationsWithState(0, 5, new List<int> { 1 }, 50);
        }

        #endregion

        private async void btnGetTest_Click(object sender, EventArgs e)
        {
            var api = new WebApi.Client.DeviceClient(_host);

            var message = await api.GetTest("HolaMundo");

            string msg = string.Format("{0} {1}", "GetTest()", message != null ? message : "null");

            AddTrace(msg);
        }
    }
}
