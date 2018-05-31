using LoggerManager;
using PrensaCatalog.DTO;
using System;
using System.Collections.Generic;

namespace PrensaCatalog
{
    public class Prensas
    {
        ILogger log = LogFactory.GetLogger(typeof(Prensas));

        public Dictionary<int, Prensa> _caracteristicasPrensa { get; set; }  // Key: id_Prensa; Value: RM.DTO.Prensa
        public Dictionary<int, List<int>> _prensasUsuario { get; set; } //Key:Id_User; Value: List<Id_Prensa>

        public Prensas()
        {
            LoadPrensas();
            LoadPrensasUsuario();
        }
        private bool LoadPrensas()
        {
            bool sw = false;

            try
            {
                log.Debug("Cargando catálogo de prensas");

                this._caracteristicasPrensa = new Dictionary<int, Prensa>();
                List<Prensa> lstPrensas = CargarPrensasSystem();

                if (lstPrensas != null)
                {
                    foreach (var p in lstPrensas)
                    {
                        // Agregamos la configuración de prensa en el diccionario
                        _caracteristicasPrensa.Add(p.prensa.Id, p);

                    }
                    sw = true;
                }
                else
                {
                    log.Warning("LoadPrensasCatalog(). No se pudo cargar el catálogo de prensas");
                }
            }
            catch (Exception er)
            {
                log.Error("LoadPrensasCatalog()", er);
            }

            return sw;
        }

        private bool LoadPrensasUsuario()
        {
            bool sw = false;

            try
            {
                log.Debug("Cargando catálogo de prensas de Usuario");

                this._prensasUsuario = new Dictionary<int, List<int>>();
                Model.BL.Usuarios model = new Model.BL.Usuarios();
                var prensasUsuario = model.ListarPrensasUsuarios();
            }
            catch (Exception er)
            {
                log.Error("LoadPrensasCatalog()", er);
            }

            return sw;
        }
        private List<Prensa> CargarPrensasSystem()
        {
            List<Prensa> lstPrensas = new List<Prensa>();

            try
            {
                Model.BL.Prensas model = new Model.BL.Prensas();
                var prensas = model.Listar();

                if (prensas != null)
                {
                   foreach(Model.BL.DTO.Prensa p in prensas)
                    {
                        Prensa Rm_Dto_Prensa = new Prensa();
                        Rm_Dto_Prensa.prensa = p;
                        //Falta barcode_cubierta
                        //Falta barcode_siguiente_cubierta
                        //Falta prioridad
                        lstPrensas.Add(Rm_Dto_Prensa);
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("CargarCatalogoPrensas()", er);
            }

            return lstPrensas;
        }

   
        public bool AddTag(Prensa Rm_Dto_Prensa)
        {
            bool sw = false;

            try
            {         
                    if (!_caracteristicasPrensa.ContainsKey(Rm_Dto_Prensa.prensa.Id))
                    {
                        _caracteristicasPrensa.Add(Rm_Dto_Prensa.prensa.Id, Rm_Dto_Prensa);
                    }
                    sw = true;
                
            }
            catch (Exception er)
            {
                log.Error("AddTag", er);
            }

            return sw;
        }

        public List<int> GetUserPrensas(int Id_User)
        {
            List<int> lstPrensasOfUser = new List<int>();
            if (_prensasUsuario.ContainsKey(Id_User))
            {
                lstPrensasOfUser = _prensasUsuario[Id_User];
            }
            return lstPrensasOfUser;
        }
        public void SetUserPrensas(int Id_User, List<int> prensasUser)
        {
          
            if (!_prensasUsuario.ContainsKey(Id_User))
            {
                _prensasUsuario.Add(Id_User, prensasUser);
            }
            else {
                _prensasUsuario[Id_User] = prensasUser;

            }
        }
        public void RemoveUser(int Id_User)
        {
            if (!_prensasUsuario.ContainsKey(Id_User))
            {
                _prensasUsuario.Remove(Id_User);

            }
        }
        public void EnablePrensa(int Id_Prensa)
        {
            if (_caracteristicasPrensa.ContainsKey(Id_Prensa))
            {
                Prensa valuePrensa = _caracteristicasPrensa[Id_Prensa];
                valuePrensa.prensa.Prensa_Activa = Convert.ToInt16(true);
                _caracteristicasPrensa[Id_Prensa] = valuePrensa;
            }
        }
        public void DisablePrensa(int Id_Prensa)
        {
            if (_caracteristicasPrensa.ContainsKey(Id_Prensa))
            {
                Prensa valuePrensa = _caracteristicasPrensa[Id_Prensa];
                valuePrensa.prensa.Prensa_Activa = Convert.ToInt16(false);
                _caracteristicasPrensa[Id_Prensa] = valuePrensa;
            }
        }


    }
}
