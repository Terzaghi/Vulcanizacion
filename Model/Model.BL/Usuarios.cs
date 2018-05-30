using Common.Cache;
using LoggerManager;
using Model.BL.DTO;
using Model.BL.Utils;
using Model.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.BL
{
    public class Usuarios
    {
        ILogger log = LogFactory.GetLogger(typeof(Usuarios));

        private string _connectionString;

        public Usuarios(string connectionString = null)
        {
            this._connectionString = connectionString;
        }
        public Usuario Detalles(int Id_User)
        {
            Usuario user = null;

            try
            {
                string nombreCache = string.Format("user{0}", Id_User);

                if (!CacheData.Exist(nombreCache))
                {
                    UsuarioDAL model = new UsuarioDAL(_connectionString);

                    var userDAL = model.Detalles(Id_User);

                    user = Converter.ConvertToBL(userDAL);

                    CacheData.Add(nombreCache, user);
                }
                else
                {
                    user = (Usuario)CacheData.Get(nombreCache);
                }
            }
            catch (Exception ex)
            {
                log.Error("Detalles()", ex);
            }

            return user;
        }

        public Usuario GetUserByIdentityCode(string identityCode)
        {
            Usuario user = null;

            try
            {
                string nombreCache = string.Format("user_identitycode{0}", identityCode);

                if (!CacheData.Exist(nombreCache))
                {
                    UsuarioDAL model = new UsuarioDAL(_connectionString);

                    var userDAL = model.GetUserByIdentityCode(identityCode);

                    user = Converter.ConvertToBL(userDAL);
                                        

                    CacheData.Add(nombreCache, user);
                }
                else
                {
                    user = (Usuario)CacheData.Get(nombreCache);
                }
            }
            catch (Exception ex)
            {
                log.Error("GetUserByIdentityCode()", ex);
            }

            return user;
        }

        public List<Usuario> Listar()
        {
            List<Usuario> users = null;

            try
            {
                UsuarioDAL model = new UsuarioDAL(_connectionString);
                IList<DAL.DTO.Usuario> usersDAL;

                usersDAL = model.Listar();

                if (usersDAL != null)
                {
                    users = Converter.ConvertToBL(usersDAL);
                }



            }
            catch (Exception ex)
            {
                log.Error("Listar()", ex);
            }

            return users;
        }

        public int ValidateCredentials(string usuario, string password)
        {
            int id_Usuario = -1;

            try
            {
                UsuarioDAL model = new UsuarioDAL(_connectionString);

                // Las passwords se almacenan cifradas                
                string strPwdCifrada = CifrarPassword(password);

                id_Usuario = model.ValidateCredentials(usuario, strPwdCifrada);
            }
            catch (Exception ex)
            {
                log.Error("ValidateCredentials()", ex);
            }

            return id_Usuario;
        }

        public int Agregar(Usuario usuario)
        {
            int id = -1;

            try
            {
                log.Debug("Agregar(). Se va a agregar un nuevo usuario");

                UsuarioDAL model = new UsuarioDAL(_connectionString);

                // Las passwords se almacenan cifradas                
                string strPwdCifrada = CifrarPassword(usuario.Password);

                DAL.DTO.Usuario usrDal = Converter.ConvertToDAL(usuario);
                usrDal.Password = strPwdCifrada;

                bool permitidoCrear = true;
                if (usuario.Identity_Code.Trim().Length > 0)
                {
                    int idUserIC = model.ValidarIdentityCode_Libre(usuario.Identity_Code);
                    if (idUserIC < 0 || idUserIC == usuario.Id)
                    {
                        // Permitimos que se mantenga el identityCode o se use porque está libre                            
                    }
                    else
                    {
                        // Ese identity code está en uso
                        log.Debug("El identityCode que intenta asignar y este ya está en uso por otro usuario (IdentityCode: {0}", usuario.Identity_Code);
                        id = -2;
                        permitidoCrear = false;
                    }
                }

                if (permitidoCrear)
                {
                    id = model.Agregar(usrDal);
                }
            }
            catch (Exception er)
            {
                log.Error("Agregar()", er);
            }

            return id;
        }

        public int Modificar(Usuario usuario)
        {
            int result = -1;

            try
            {
                if (usuario != null)
                {
                    log.Debug("Modificar(). Se va a modificar un usuario");

                    bool permititidoModificar = true;

                    UsuarioDAL model = new UsuarioDAL(_connectionString);
                    // Comprobamos si han cambiado el identityCode por otro que ya existía (y no era nuestro)
                    if (usuario.Identity_Code.Trim().Length > 0)
                    {
                        int idUserIC = model.ValidarIdentityCode_Libre(usuario.Identity_Code);

                        if (idUserIC < 0 || idUserIC == usuario.Id)
                        {
                            // Permitimos que se mantenga el identityCode o se use porque está libre                            
                        }
                        else
                        {
                            // Ese identity code está en uso
                            log.Debug("Se ha cambiado el identityCode, y este ya está en uso por otro usuario (Id_User: {0}, Usuario: {1}", usuario.Id, usuario.Nombre);
                            result = -2;
                            permititidoModificar = false;
                        }
                    }

                    if (permititidoModificar)
                    {
                        DAL.DTO.Usuario usrDal = Converter.ConvertToDAL(usuario);
                        
                        if (usuario.Password != null)
                        {
                            // Las passwords se almacenan cifradas                
                            string strPwdCifrada = CifrarPassword(usuario.Password);
                            usrDal.Password = strPwdCifrada;
                        }

                        bool sw = model.Modificar(usrDal);

                        if (sw)
                        {
                            // Como respuesta devolvemos el identificador del usuario
                            result = usuario.Id;

                            // Si había una caché para el usuario la borramos
                            // Borramos la caché para ese usuario
                            string nombreCache = string.Format("user{0}", usuario.Id);
                            CacheData.Remove(nombreCache);
                        }
                    }

                }
            }
            catch (Exception er)
            {
                log.Error("Modificar()", er);
            }

            return result;
        }

        public bool Eliminar(int Id_Usuario)
        {
            bool sw = false;

            try
            {
                log.Debug("Eliminar(). Id_Usuario: {0}", Id_Usuario);

                UsuarioDAL model = new UsuarioDAL(_connectionString);
                sw = model.Eliminar(Id_Usuario);


                // Borramos la caché para ese usuario
                string nombreCache = string.Format("user{0}", Id_Usuario);
                CacheData.Remove(nombreCache);
            }
            catch (Exception er)
            {
                log.Error("Eliminar()", er);
            }

            return sw;
        }

        #region Métodos privados
        private string CifrarPassword(string password)
        {
            string strPwdCifrada = string.Empty;

            try
            {
                // Ciframos las credenciales
                Common.Security.Cifrado.AES cifrado = new Common.Security.Cifrado.AES();
                if (password.Trim().Length > 0)
                {
                    // Ponemos un cifrado reversible por si necesitamos el acceso, podríamos poner una simétrica con la misma clave 
                    // pero nos dificultaría el soporte a los usuarios si fuera necesario
                    strPwdCifrada = cifrado.Encrypt(password, "pwdCred@1!", "inicializa cadena", "SHA1", 1, "BSHP-3512-BACRFA", 128);
                }
            }
            catch (Exception er)
            {
                log.Error("CifrarPassword()", er);
            }
            return strPwdCifrada;
        }
        #endregion

        public bool Vincular(int Id_User, int Id_Prensa)
        {
            bool sw = false;

            try
            {
                DAL.UsuarioDAL mod = new DAL.UsuarioDAL(_connectionString);
                sw = mod.Vincular(Id_User, Id_Prensa);
            }
            catch (Exception er)
            {
                log.Error("Vincular()", er);
            }

            return sw;
        }

        public bool Desvincular(int Id_User, int Id_Prensa)
        {
            bool sw = false;

            try
            {
                DAL.UsuarioDAL mod = new DAL.UsuarioDAL(_connectionString);
                sw = mod.Desvincular(Id_User, Id_Prensa);
            }
            catch (Exception er)
            {
                log.Error("Desvincular()", er);
            }

            return sw;
        }
        public IList<Tuple<int,int>> ListarPrensasUsuarios()
        {
            IList<Tuple<int, int>> lst = new List<Tuple<int, int>>();
            try
            {
                DAL.UsuarioDAL mod = new DAL.UsuarioDAL(_connectionString);
                lst= mod.ListarPrensasUsuarios();

            }
            catch (Exception er)
            {
                log.Error("ListarPrensasDeUsuario()", er);
            }
            return lst;
        }
    }
}
