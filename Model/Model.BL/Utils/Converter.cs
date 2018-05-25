using System;
using System.Collections.Generic;
using System.Text;
using Model.BL.DTO;
using Model.DAL.DTO;
using Model.BL.DTO.Enums;

namespace Model.BL.Utils
{
    internal static class Converter
    {
      
        

        #region Usuario

        internal static DTO.Usuario ConvertToBL(DAL.DTO.Usuario item)
        {
            DTO.Usuario user = null;

            if (item != null)
            {
                user = new DTO.Usuario
                {
                    Id = item.Id,
                    Nombre = item.Nombre,
                    Identity_Code = item.Identity_Code,
                    Password = item.Password,
                  };
            }
            return user;
        }

       

        internal static List<DTO.Usuario> ConvertToBL(IList<DAL.DTO.Usuario> usersDAL)
        {
            List<DTO.Usuario> result = null;

            if (usersDAL != null)
            {
                result = new List<DTO.Usuario>();

                foreach (var item in usersDAL)
                {
                    var user = ConvertToBL(item);

                    if (user != null)
                        result.Add(user);
                }
            }
            return result;
        }

    

        internal static DAL.DTO.Usuario ConvertToDAL(DTO.Usuario item)
        {
            DAL.DTO.Usuario user = null;

            if (item != null)
            {
                user = new DAL.DTO.Usuario
                {
                    Id = item.Id,
                    Nombre = item.Nombre,
                    Identity_Code = item.Identity_Code,
                    Password = item.Password
                };
            }
            return user;
        }

        #endregion

        #region Dispositivo

        internal static DTO.Dispositivo ConvertToBL(DAL.DTO.Dispositivo item)
        {
            DTO.Dispositivo device = null;

            if (item != null)
            {
                device = new DTO.Dispositivo
                {
                    Id_Disposito = item.Id_Disposito,
                    Serial_Number = item.Serial_Number,
                    IP = item.IP,
                    Descripcion = item.Descripcion
                };
            }
            return device;
        }

        internal static List<DTO.Dispositivo> ConvertToBL(IList<DAL.DTO.Dispositivo> devices)
        {
            List<DTO.Dispositivo> result = null;

            if (devices != null)
            {
                result = new List<DTO.Dispositivo>();

                foreach (var item in devices)
                {
                    var device = ConvertToBL(item);

                    if (device != null)
                        result.Add(device);
                }
            }
            return result;
        }

        #endregion

        #region Prensa

        internal static DTO.Prensa ConvertToBL(DAL.DTO.Prensa item)
        {
            DTO.Prensa prensa = null;

            if (item != null)
            {
                prensa = new DTO.Prensa
                {
                    Id = item.Id,
                    Nombre = item.Nombre,
                    Barcode_Prensa = item.Barcode_Prensa,
                    Barcode_Pintado = item.Barcode_Pintado,
                    Barcode_Pinchado = item.Barcode_Pinchado,
                    Prensa_Activa = item.Prensa_Activa,
                    Id_Zona = item.Id_Zone
    };
            }
            return prensa;
        }
        internal static DTO.PrensaDato ConvertToBL(DAL.DTO.PrensaDato item)
        {
            DTO.PrensaDato prensaDato = null;

            if (item != null)
            {
                prensaDato = new DTO.PrensaDato
                {
                    Fecha = item.Fecha,
                    PrensaId=item.PrensaId,
                    TagActivaValue=item.TagActivaValue,
                    TagCVValue=item.TagCVValue,
                    TagTempValue=item.TagTempValue,
                    TagCicloValue=item.TagCicloValue,
                    TagProdValue=item.TagProdValue,
                    Cavidad=item.Cavidad
                 };
            }
            return prensaDato;
        }
        internal static List<DTO.PrensaDato> ConvertToBL(IList<DAL.DTO.PrensaDato> prensasDatos)
        {
            List<DTO.PrensaDato> result = null;

            if (prensasDatos != null)
            {
                result = new List<DTO.PrensaDato>();

                foreach (var item in prensasDatos)
                {
                    var device = ConvertToBL(item);

                    if (device != null)
                        result.Add(device);
                }
            }
            return result;
        }

        internal static List<DTO.Prensa> ConvertToBL(IList<DAL.DTO.Prensa> prensas)
        {
            List<DTO.Prensa> result = null;

            if (prensas != null)
            {
                result = new List<DTO.Prensa>();

                foreach (var item in prensas)
                {
                    var device = ConvertToBL(item);

                    if (device != null)
                        result.Add(device);
                }
            }
            return result;
        }
        internal static DAL.DTO.Prensa ConvertToDAL(DTO.Prensa prensa)
        {
            DAL.DTO.Prensa result = null;

            if (prensa != null)
            {
                result = new DAL.DTO.Prensa
                {
                    Id = prensa.Id,
                    Nombre = prensa.Nombre,
                    Barcode_Prensa = prensa.Barcode_Prensa,
                    Barcode_Pintado = prensa.Barcode_Pintado,
                    Barcode_Pinchado = prensa.Barcode_Pinchado,
                    Prensa_Activa = prensa.Prensa_Activa
                };
            }
            return result;
        }

        #endregion

        #region Permiso

        internal static DTO.Permiso ConvertToBL(DAL.DTO.Permiso item)
        {
            DTO.Permiso permiso = null;

            if (item != null)
            {
                permiso = new DTO.Permiso
                {
                    tipoPermiso= (Model.BL.DTO.Enums.Tipo_Permiso) Enum.Parse(typeof(Model.BL.DTO.Enums.Tipo_Permiso),item.Id_Permiso.ToString()),
                    Nombre = item.Nombre,
                   
                };
            }
            return permiso;
        }

        internal static List<DTO.Permiso> ConvertToBL(IList<DAL.DTO.Permiso> permisos)
        {
            List<DTO.Permiso> result = null;

            if (permisos != null)
            {
                result = new List<DTO.Permiso>();

                foreach (var item in permisos)
                {
                    var permiso = ConvertToBL(item);

                    if (permiso != null)
                        result.Add(permiso);
                }
            }
            return result;
        }
        internal static DAL.DTO.Permiso ConvertToDAL(DTO.Permiso item)
        {
            DAL.DTO.Permiso permiso = null;

            if (item != null)
            {
                permiso = new DAL.DTO.Permiso
                {
                    Id_Permiso = (int)item.tipoPermiso,
                    Nombre = item.Nombre,
                };
            }
            return permiso;
        }

        #endregion

        #region Rol

        internal static DTO.Rol ConvertToBL(DAL.DTO.Rol item)
        {
            DTO.Rol rol = null;

            if (item != null)
            {
                rol = new DTO.Rol
                {
                    rol = (Model.BL.DTO.Enums.Tipo_Rol)Enum.Parse(typeof(Model.BL.DTO.Enums.Tipo_Rol), item.Id.ToString()),
                    Nombre = item.Nombre,

                };
            }
            return rol;
        }

        internal static List<DTO.Rol> ConvertToBL(IList<DAL.DTO.Rol> roles)
        {
            List<DTO.Rol> result = null;

            if (roles != null)
            {
                result = new List<DTO.Rol>();

                foreach (var item in roles)
                {
                    var rol = ConvertToBL(item);

                    if (rol != null)
                        result.Add(rol);
                }
            }
            return result;
        }
        internal static DAL.DTO.Rol ConvertToDAL(DTO.Rol item)
        {
            DAL.DTO.Rol rol = null;

            if (item != null)
            {
                rol = new DAL.DTO.Rol
                {
                    Id = (int)item.rol,
                    Nombre = item.Nombre,
                };
            }
            return rol;
        }

        #endregion

        #region Solicitud

        internal static DTO.Solicitud ConvertToBL(DAL.DTO.Solicitud item)
        {
            DTO.Solicitud solicitud = null;

            if (item != null)
            {
                solicitud = new DTO.Solicitud
                {
                   Id=item.Id,
                   Fecha_Generacion=item.Fecha_Generacion,
                   Id_Prensa=item.Id_Prensa

                 };
            }
            return solicitud;
        }

        internal static List<DTO.Solicitud> ConvertToBL(IList<DAL.DTO.Solicitud> solicitudes)
        {
            List<DTO.Solicitud> result = null;

            if (solicitudes != null)
            {
                result = new List<DTO.Solicitud>();

                foreach (var item in solicitudes)
                {
                    var rol = ConvertToBL(item);

                    if (rol != null)
                        result.Add(rol);
                }
            }
            return result;
        }
        internal static DAL.DTO.Solicitud ConvertToDAL(DTO.Solicitud item)
        {
            DAL.DTO.Solicitud solicitud = null;

            if (item != null)
            {
                solicitud = new DAL.DTO.Solicitud
                {
                    Id = item.Id,
                    Fecha_Generacion = item.Fecha_Generacion,
                    Id_Prensa = item.Id_Prensa
                };
            }
            return solicitud;
        }

        #endregion


        #region Zona

        internal static DTO.Zona ConvertToBL(DAL.DTO.Zona item)
        {
            DTO.Zona zona = null;

            if (item != null)
            {
                zona = new DTO.Zona
                {
                     Id=item.Id,
                     Nombre=item.Nombre

                };
            }
            return zona;
        }

        internal static List<DTO.Zona> ConvertToBL(IList<DAL.DTO.Zona> zonas)
        {
            List<DTO.Zona> result = null;

            if (zonas != null)
            {
                result = new List<DTO.Zona>();

                foreach (var item in zonas)
                {
                    var zona = ConvertToBL(item);

                    if (zona != null)
                        result.Add(zona);
                }
            }
            return result;
        }
        internal static DAL.DTO.Zona ConvertToDAL(DTO.Zona item)
        {
            DAL.DTO.Zona zona = null;

            if (item != null)
            {
                zona = new DAL.DTO.Zona
                {
                    Id = item.Id,
                    Nombre = item.Nombre
                };
            }
            return zona;
        }

        #endregion

        #region "Especificacion"
        internal static DTO.Especificacion ConvertToBL(DAL.DTO.Especificacion item)
        {
            DTO.Especificacion especificacion = null;

            if (item != null)
            {
                especificacion = new DTO.Especificacion
                {
                    CV = item.CV,
                    Minutos_Limite_Vulcanizado = item.Minutos_Limite_Vulcanizado,
                  
                };
            }
            return especificacion;
        }



        internal static List<DTO.Especificacion> ConvertToBL(IList<DAL.DTO.Especificacion> especificacionesDAL)
        {
            List<DTO.Especificacion> result = null;

            if (especificacionesDAL != null)
            {
                result = new List<DTO.Especificacion>();

                foreach (var item in especificacionesDAL)
                {
                    var especificacion = ConvertToBL(item);

                    if (especificacion != null)
                        result.Add(especificacion);
                }
            }
            return result;
        }


        #endregion

        #region "Historico_Contramedidas"
        internal static DAL.DTO.Historico_Contramedidas ConvertToDAL(DTO.Historico_Contramedidas item)
        {
            DAL.DTO.Historico_Contramedidas historico = null;

            if (item != null)
            {
                historico = new DAL.DTO.Historico_Contramedidas
                {
                   Id=item.Id,
                   Expiracion=item.Expiracion,
                   CV=item.CV,
                   Lote=item.Lote,
                   Id_Contramedida=(int)item.Contramedida,
                   Id_Prensa=item.Id_Prensa
                };
            }
            return historico;
        }
        internal static DTO.Historico_Contramedidas ConvertToBL(DAL.DTO.Historico_Contramedidas item)
        {
            DTO.Historico_Contramedidas historico = null;

            if (item != null)
            {
                historico = new DTO.Historico_Contramedidas
                {
                    Id = item.Id,
                    Expiracion = item.Expiracion,
                    CV = item.CV,
                    Lote = item.Lote,
                    Contramedida = (Tipo_Contramedidas) Enum.Parse(typeof(Tipo_Contramedidas),item.Id_Contramedida.ToString()),
                    Id_Prensa = item.Id_Prensa

                };
            }
            return historico;
        }

        internal static List<DTO.Historico_Contramedidas> ConvertToBL(IList<DAL.DTO.Historico_Contramedidas> historicos)
        {
            List<DTO.Historico_Contramedidas> result = null;

            if (historicos != null)
            {
                result = new List<DTO.Historico_Contramedidas>();

                foreach (var item in historicos)
                {
                    var historico = ConvertToBL(item);

                    if (historico != null)
                        result.Add(historico);
                }
            }
            return result;
        }
        #endregion

        #region "Historico_Deshabilitacion"
        internal static DAL.DTO.Historico_Deshabilitacion ConvertToDAL(DTO.Historico_Deshabilitacion item)
        {
            DAL.DTO.Historico_Deshabilitacion historico = null;

            if (item != null)
            {
                historico = new DAL.DTO.Historico_Deshabilitacion
                {
                    Id_Deshabilitacion = item.Id_Deshabilitacion,
                    Fecha = item.Fecha,
                    Comentario = item.Comentario,
                    Id_Motivo = (int)item.Motivo,
                    Id_Permiso = item.Id_Permiso,
                    Id_Prensa = item.Id_Prensa,
                    Id_Usuario = item.Id_Usuario,
                    Id_Dispositivo = item.Id_Dispositivo
                };
            }
            return historico;
        }
        internal static DTO.Historico_Deshabilitacion ConvertToBL(DAL.DTO.Historico_Deshabilitacion item)
        {
            DTO.Historico_Deshabilitacion historico = null;

            if (item != null)
            {
                historico = new DTO.Historico_Deshabilitacion
                {
                    Id_Deshabilitacion = item.Id_Deshabilitacion,
                    Fecha = item.Fecha,
                    Comentario = item.Comentario,
                    Motivo = (DTO.Enums.Motivo_Deshabilitacion)Enum.Parse(typeof(DTO.Enums.Motivo_Deshabilitacion), item.Id_Motivo.ToString()),
                    Id_Permiso = item.Id_Permiso,
                    Id_Prensa = item.Id_Prensa,
                    Id_Usuario = item.Id_Usuario,
                    Id_Dispositivo = item.Id_Dispositivo

                };
            }
            return historico;
        }

        internal static List<DTO.Historico_Deshabilitacion> ConvertToBL(IList<DAL.DTO.Historico_Deshabilitacion> historicos)
        {
            List<DTO.Historico_Deshabilitacion> result = null;

            if (historicos != null)
            {
                result = new List<DTO.Historico_Deshabilitacion>();

                foreach (var item in historicos)
                {
                    var historico = ConvertToBL(item);

                    if (historico != null)
                        result.Add(historico);
                }
            }
            return result;
        }
        #endregion

        #region "Historico_Solicitud"
        internal static DAL.DTO.Historico_Solicitud ConvertToDAL(DTO.Historico_Solicitud item)
        {
            DAL.DTO.Historico_Solicitud historico = null;

            if (item != null)
            {
                historico = new DAL.DTO.Historico_Solicitud
                {
                   Id_Historico=item.Id_Historico,
                   Fecha=item.Fecha,
                   Id_Solicitud=item.Id_Solicitud,
                   Id_Estado=(int)item.Estado,
                   Id_Usuario=item.Id_Usuario,
                   Id_Dispositivo=item.Id_Dispositivo
    };
            }
            return historico;
        }
        internal static DTO.Historico_Solicitud ConvertToBL(DAL.DTO.Historico_Solicitud item)
        {
            DTO.Historico_Solicitud historico = null;

            if (item != null)
            {
                historico = new DTO.Historico_Solicitud
                {
                    Id_Historico = item.Id_Historico,
                    Fecha = item.Fecha,
                    Id_Solicitud = item.Id_Solicitud,
                    Estado= (Estado_Solicitud) Enum.Parse(typeof(Estado_Solicitud), item.Id_Estado.ToString()),
                    Id_Usuario = item.Id_Usuario,
                    Id_Dispositivo = item.Id_Dispositivo

                };
            }
            return historico;
        }

        internal static List<DTO.Historico_Solicitud> ConvertToBL(IList<DAL.DTO.Historico_Solicitud> historicos)
        {
            List<DTO.Historico_Solicitud> result = null;

            if (historicos != null)
            {
                result = new List<DTO.Historico_Solicitud>();

                foreach (var item in historicos)
                {
                    var historico = ConvertToBL(item);

                    if (historico != null)
                        result.Add(historico);
                }
            }
            return result;
        }
        #endregion

        #region Tag Operation

        //internal static DTO.TagOperation ConvertToBL(DAL.DTO.TagOperation operationsDAL)
        //{
        //    DTO.TagOperation operation = null;

        //    if (operationsDAL != null)
        //    {
        //        operation = new DTO.TagOperation
        //        {
        //            Value = operationsDAL.Value
        //        };
        //    }
        //    return operation;
        //}

        #endregion

        #region Tag Provider

        internal static DTO.Tag_Provider ConvertToBL(DAL.DTO.Tag_Provider item)
        {
            DTO.Tag_Provider provider = null;

            if (item != null)
            {
                provider = new DTO.Tag_Provider
                {
                    Id_Proveedor = item.Id_Proveedor,
                    Nombre = item.Nombre,
                    OPCServer=item.OPCServer,
                    PollingInterval=item.PollingInterval,
                    RequestConnection=item.RequestConnection
                };
            }
            return provider;
        }

        internal static List<DTO.Tag_Provider> ConvertToBL(IList<DAL.DTO.Tag_Provider> providers)
        {
            List<DTO.Tag_Provider> result = null;

            if (providers != null)
            {
                result = new List<DTO.Tag_Provider>();

                foreach (var item in providers)
                {
                    var provider = ConvertToBL(item);

                    if (provider != null)
                        result.Add(provider);
                }
            }
            return result;
        }

        #endregion

        #region Tag

        internal static DAL.DTO.Tag ConvertToDAL(DTO.Tag objBL)
        {
            DAL.DTO.Tag result = new DAL.DTO.Tag()
            {
                Id= objBL.Id,
                Descripcion = objBL.Descripcion,
                Id_Proveedor = objBL.Id_Proveedor
            };

            return result;
        }

        internal static DTO.Tag ConvertToBL(DAL.DTO.Tag objDAL)
        {
            DTO.Tag result = null;

            if (objDAL != null)
            {
                result = new DTO.Tag()
                {
                    Id = objDAL.Id,
                    Descripcion = objDAL.Descripcion,
                    Id_Proveedor = objDAL.Id_Proveedor
                };
            }

            return result;
        }

        internal static List<DTO.Tag> ConvertToBL(List<DAL.DTO.Tag> tags)
        {
            List<DTO.Tag> result = null;

            if (tags != null)
            {
                result = new List<DTO.Tag>();

                foreach (var item in tags)
                {
                    var tag = ConvertToBL(item);

                    if (tag != null)
                        result.Add(tag);
                }
            }
            return result;
        }
        #endregion

        

        #region Login_Dispositivo

        internal static DAL.DTO.Login_Dispositivo ConvertToDAL(DTO.Login_Dispositivo login)
        {
            DAL.DTO.Login_Dispositivo result = null;

            if (login != null)
            {
                result = new DAL.DTO.Login_Dispositivo
                {
                    Id_Login = login.Id_Login,
                    Id_Dispositivo = login.Id_Dispositivo,
                    Id_Evento = (int)login.Evento,
                    Id_Usuario= login.Id_Usuario,
                    Fecha = login.Fecha,
                    Connection_Id = login.Connection_Id
                };
            }
            return result;
        }

        internal static List<DAL.DTO.Login_Dispositivo> ConvertToDAL(List<DTO.Login_Dispositivo> logins)
        {
            List<DAL.DTO.Login_Dispositivo> result = null;

            if (logins != null)
            {
                result = new List<DAL.DTO.Login_Dispositivo>();

                foreach (var item in logins)
                {
                    var tag = ConvertToDAL(item);

                    if (tag != null)
                        result.Add(tag);
                }
            }
            return result;
        }

        internal static DTO.Login_Dispositivo ConvertToBL(DAL.DTO.Login_Dispositivo login)
        {
            DTO.Login_Dispositivo result = null;

            if (login != null)
            {
                result = new DTO.Login_Dispositivo
                {
                    Id_Login = login.Id_Login,
                    Id_Dispositivo = login.Id_Dispositivo,
                    Evento = (DTO.Enums.Tipo_Evento)login.Id_Evento,
                    Id_Usuario = login.Id_Usuario,
                    Fecha = login.Fecha,
                    Connection_Id = login.Connection_Id
                };
            }
            return result;
        }

        internal static List<DTO.Login_Dispositivo> ConvertToBL(IList<DAL.DTO.Login_Dispositivo> logins)
        {
            List<DTO.Login_Dispositivo> result = null;

            if (logins != null)
            {
                result = new List<DTO.Login_Dispositivo>();

                foreach (var item in logins)
                {
                    var tag = ConvertToBL(item);

                    if (tag != null)
                        result.Add(tag);
                }
            }
            return result;
        }

        #endregion
    
        #region Byte Array Conversions

        internal static object GetObjectFromBytes(Common.Enums.TagType type, byte[] value)
        {
            object result = null;

            if (value != null)
            {
                switch (type)
                {
                    case Common.Enums.TagType.Numeric:
                        result = BitConverter.ToInt32(value, 0);
                        break;
                    case Common.Enums.TagType.Decimal:
                        result = BitConverter.ToDouble(value, 0);
                        break;
                    case Common.Enums.TagType.Text:
                        result = Encoding.Default.GetString(value);
                        break;
                    case Common.Enums.TagType.Boolean:
                        result = BitConverter.ToBoolean(value, 0);
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        internal static byte[] GetBytesFromObject(Common.Enums.TagType type, object value)
        {
            byte[] result = null;

            if (value != null)
            {
                switch (type)
                {
                    case Common.Enums.TagType.Numeric:
                        result = BitConverter.GetBytes(Convert.ToInt32(value));
                        break;
                    case Common.Enums.TagType.Decimal:
                        result = BitConverter.GetBytes(Convert.ToDouble(value));
                        break;
                    case Common.Enums.TagType.Text:
                        result = Encoding.Default.GetBytes(value.ToString());
                        break;
                    case Common.Enums.TagType.Boolean:
                        result = BitConverter.GetBytes(Convert.ToBoolean(value));
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        #endregion



    }
}
