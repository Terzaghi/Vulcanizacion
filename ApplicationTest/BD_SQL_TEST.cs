using Model.DAL;
using Model.DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ApplicationTest
{
    public class BD_SQL_TEST
    {

        #region "Usuario"
        public void inserUsuario()
        {
            UsuarioDAL dalUsuario = new UsuarioDAL(ConfigurationManager.ConnectionStrings[1].ConnectionString);
            Usuario miUsuario = new Usuario();
            miUsuario.Nombre = "Cristina";
            miUsuario.Password = "XXXXX";
            miUsuario.Identity_Code = "1234CDFE";
            dalUsuario.Agregar(miUsuario);
        }
        public void getUsuario(int id)
        {
            UsuarioDAL dalUsuario = new UsuarioDAL(ConfigurationManager.ConnectionStrings[1].ConnectionString);
            Usuario user= dalUsuario.Detalles(id);

        }
        public void deleteUsuario(int id)
        {
            UsuarioDAL dalUsuario = new UsuarioDAL(ConfigurationManager.ConnectionStrings[1].ConnectionString);
            dalUsuario.Eliminar(id);

        }
        public void listar()
        {
            UsuarioDAL dalUsuario = new UsuarioDAL(ConfigurationManager.ConnectionStrings[1].ConnectionString);
            IList<Usuario> list=dalUsuario.Listar();
        }
        public void Modificar()
        {
            Usuario miUsuario = new Usuario();
            miUsuario.Nombre = "Cris";
            miUsuario.Password = "YYYY";
            miUsuario.Identity_Code = "4444";
            miUsuario.Id = 4;

            UsuarioDAL dalUsuario = new UsuarioDAL(ConfigurationManager.ConnectionStrings[1].ConnectionString);
            dalUsuario.Modificar(miUsuario);

        }
        #endregion
        #region "Contramedida"
        public void insertContramedida()
        {
            ContramedidaDAL dalcontramedida = new ContramedidaDAL(ConfigurationManager.ConnectionStrings[1].ConnectionString);
            Contramedida contramedida = new Contramedida();
            contramedida.Nombre = "contramedida xxx";
            dalcontramedida.Agregar(contramedida);
        }
        public void getContramedida(int id)
        {
            ContramedidaDAL dalcontramedida = new ContramedidaDAL(ConfigurationManager.ConnectionStrings[1].ConnectionString);
            Contramedida contramedida = dalcontramedida.Detalles(id);

        }
        public void deleteContramedida(int id)
        {
            ContramedidaDAL dalcontramedida = new ContramedidaDAL(ConfigurationManager.ConnectionStrings[1].ConnectionString);
            dalcontramedida.Eliminar(id);

        }
        public void listarContramedida()
        {
            ContramedidaDAL dalcontramedida = new ContramedidaDAL(ConfigurationManager.ConnectionStrings[1].ConnectionString);
            IList<Contramedida> list = dalcontramedida.Listar();
        }
        public void ModificarContramedida()
        {
            Contramedida contramedida = new Contramedida();
            contramedida.Nombre = "Contramedida Modificada";
            contramedida.Id_Contramedida = 4;

            ContramedidaDAL dalUsuario = new ContramedidaDAL(ConfigurationManager.ConnectionStrings[1].ConnectionString);
            dalUsuario.Modificar(contramedida);

        }
        #endregion
        #region "Dispositivo"
        public void insertDispositivo()
        {
            DispositivoDAL dalDispositivo= new DispositivoDAL(ConfigurationManager.ConnectionStrings[1].ConnectionString);
            Dispositivo d = new Dispositivo();
            d.Serial_Number = "111111111";
            d.IP = "XXX.XXX.XXX";
            d.Descripcion = "Descripcion";
            dalDispositivo.Agregar(d);
        }
        public void getDispositivo(int id)
        {
            DispositivoDAL daldispositivo = new DispositivoDAL(ConfigurationManager.ConnectionStrings[1].ConnectionString);
            Dispositivo d = daldispositivo.Detalles(id);

        }
        public void deleteDispositivo(int id)
        {
            DispositivoDAL daldispositivo = new DispositivoDAL(ConfigurationManager.ConnectionStrings[1].ConnectionString);
            daldispositivo.Eliminar(id);

        }
        public void listarDispositivo()
        {
            DispositivoDAL daldispositivo = new DispositivoDAL(ConfigurationManager.ConnectionStrings[1].ConnectionString);
            IList<Dispositivo> list = daldispositivo.Listar();
        }
        public void ModificarDispositivo()
        {
            Dispositivo d = new Dispositivo();
            d.Serial_Number = "Serial Modificado";
            d.IP = "IP Modificado";
            d.Descripcion = "Descripcion Modificada";
            d.Id_Disposito = 1;

            DispositivoDAL dalDispositivo = new DispositivoDAL(ConfigurationManager.ConnectionStrings[1].ConnectionString);
            dalDispositivo.Modificar(d);

        }
#endregion
    }
}
