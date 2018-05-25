using Common.Security;
using LoggerManager;
using Model.BL.DTO.Enums;
using Model.BL.Utils;
using Model.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.BL
{
    public class LoginsDispositivos
    {
        ILogger log = LogFactory.GetLogger(typeof(LoginsDispositivos));

        private string _connectionString;

        #region Securización

        public Token SecurityToken { get; set; }

        public LoginsDispositivos(Token token=null, string connectionString = null)
        {
            this.SecurityToken = token;
            this._connectionString = connectionString;
        }

        #endregion

        public bool Agregar(int Id_Usuario, int Id_Dispositivo, Tipo_Evento EventType,string ConnectionId)
        {
            bool result = false;

            try
            {
                if (Id_Usuario != 0 || Id_Dispositivo != 0)
                {
                    Login_DispositivoDAL modLogin = new Login_DispositivoDAL(_connectionString);

                    var deviceLogin = new DAL.DTO.Login_Dispositivo
                    {
                        Id_Dispositivo = Id_Dispositivo,
                        Id_Usuario = Id_Usuario,
                        Fecha = DateTime.Now,
                        Id_Evento = (int)EventType,
                        Connection_Id = (ConnectionId != null) ? ConnectionId : ""
                    };

                    result = modLogin.Agregar(deviceLogin) > 0;
                }
            }
            catch (Exception ex)
            {
                log.Error("Agregar()", ex);
            }

            return result;
        }

        public bool AgregarMultiple(List<BL.DTO.Login_Dispositivo> lst)
        {
            bool result = false;

            try
            {
                Login_DispositivoDAL modLogin = new Login_DispositivoDAL(_connectionString);

                List<DAL.DTO.Login_Dispositivo> dal = Converter.ConvertToDAL(lst);

                var incorrectos = new List<DAL.DTO.Login_Dispositivo>();

                foreach (var item in dal)
                {
                    if (!(modLogin.Agregar(item) > 0))
                    {
                        incorrectos.Add(item);
                    }
                }

                result = incorrectos.Count == 0;
            }
            catch (Exception er)
            {
                log.Error("AgregarMultiple", er);
            }
            return result;
        }

        public List<DTO.Login_Dispositivo> ListarEventosFinales(int lastIdDeviceLogin)
        {
            List<DTO.Login_Dispositivo> lst = null;

            try
            {
                Login_DispositivoDAL dal = new Login_DispositivoDAL(_connectionString);

                var logins = dal.Listar(lastIdDeviceLogin, (int)Tipo_Evento.login);

                lst = Converter.ConvertToBL(logins);
            }
            catch (Exception er)
            {
                log.Error(string.Format("ListarEventosFinales({0}). ", lastIdDeviceLogin), er);
            }

            return lst;
        }

        public List<DTO.Login_Dispositivo> Listar()
        {
            List<DTO.Login_Dispositivo> lst = null;

            try
            {
                Login_DispositivoDAL dal = new Login_DispositivoDAL(_connectionString);

                var logins = dal.Listar();

                lst = Converter.ConvertToBL(logins);
            }
            catch (Exception er)
            {
                log.Error("Listar(). ", er);
            }
            return lst;
        }
    }
}
