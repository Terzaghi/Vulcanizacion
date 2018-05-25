using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApplicationTest
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            BD_SQL_TEST test = new BD_SQL_TEST();
            //test.inserUsuario();
            //test.getUsuario(1);
            //test.deleteUsuario(12);
            //test.listar();
            //test.Modificar();
            test.insertContramedida();
            //test.getContramedida(2);
            //test.deleteContramedida(3);
            //test.listarContramedida();
            //test.ModificarContramedida();
            //test.insertDispositivo();
            //test.getDispositivo(1);
            //test.insertDispositivo();
            //test.deleteDispositivo(2);
            //test.listarDispositivo();
            //test.ModificarDispositivo();
        }
    }
}
