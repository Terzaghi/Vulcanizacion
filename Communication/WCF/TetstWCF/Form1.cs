using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TetstWCF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        WCF_RuleMotorServer.servidor3 servidor;

        private void button1_Click(object sender, EventArgs e)
        {
            servidor = new WCF_RuleMotorServer.servidor3();            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WCF_RuleMotorClient.cliente3 cliente = new WCF_RuleMotorClient.cliente3();
            var datos = cliente.CosasPrueba();
        }
    }
}
