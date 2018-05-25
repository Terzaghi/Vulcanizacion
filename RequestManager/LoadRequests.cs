using LoggerManager;
using Model.BL;
using Model.BL.DTO;
using RequestManager.DTO;
using System;
using System.Collections.Generic;

namespace RuleManager.Clases
{
    /// <summary>
    /// Conversor y carga datos de BL conviertiendolos a objetos del rule
    /// </summary>
    public class LoadRequests
    {
        ILogger log = LogFactory.GetLogger(typeof(LoadRequests));        
       

        public List<Request> Cargar()
        {
            List<Request> result = null;

            try
            {
                result = new List<Request>();

                log.Debug("Cargando listado de solicitudes...");
                Solicitudes modSolicitudes = new Solicitudes(null);
                var lstSolicitudes = modSolicitudes.Listar();               

                result = CargarObjetosRegla(lstSolicitudes);
            }
            catch (Exception ex)
            {
                log.Error("Cargar()", ex);
            }
            return result;
        }

        public List<Request> CargarSolicitud(int Id_Request)
        {
            List<Request> result = null;

            try
            {
                result = new List<Request>();

                log.Debug("Cargando solicitud...");
                Solicitudes modSolicitudes = new Solicitudes(null);
                var solicitud = modSolicitudes.Detalles(Id_Request);

                if (solicitud != null)
                {
                    List<Solicitud> lst = new List<Solicitud>();
                    lst.Add(solicitud);

                    result = CargarObjetosRegla(lst);
                }
                else
                {
                    log.Warning("CargarSolicitud(Id_Request: {0}). Solicitud no encontrada");
                }

            }
            catch (Exception ex)
            {
                log.Error("Cargar()", ex);
            }
            return result;
        }

        private List<Request> CargarObjetosRegla(List<Solicitud> lstSolicitudesBL)
        {
            List<Request> result = null;

            if (lstSolicitudesBL != null)
            {
                result = new List<Request>();

                log.Debug("Número de solicitudes configuradas en el sistema: {0}", lstSolicitudesBL.Count);

                
                foreach (var solicitudBL in lstSolicitudesBL)
                {
                    // Cargamos los datos vinculados a las mismas
                    log.Debug("Cargando configuración de la solicitud (Id_Request: {0})", solicitudBL.Id);
                
                    // Generamos la solicitud
                    Request request = new Request()
                    {
                        Id_Request = Convert.ToInt32(solicitudBL.Id),
                      
                    };
                    result.Add(request);
                }
            }

            return result;
        }
        

    }
}
