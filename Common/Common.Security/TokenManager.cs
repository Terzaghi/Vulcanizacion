using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Security
{
    public class TokenManager
    {
        //ILogger log = BSHP.LoggerManager.LogFactory.GetLogger(typeof(TokenManager).ToString());

        /// <summary>
        /// Genera un nuevo token de usuario, devolviendo el string cifrado del mismo
        /// </summary>
        /// <param name="Id_Usuario"></param>
        /// <param name="Id_Idioma"></param>
        /// <param name="PermisoZonas"></param>
        /// <param name="TrickHoraCaducidad"></param>
        /// <param name="ClavePrivada"></param>
        /// <returns></returns>
        public string GenerateStringToken(int Id_Usuario, int Id_Idioma, List<Funcionalidad> PermisoZonas, List<int> Grupos, long? TrickHoraCaducidad, string ClavePrivada)
        {
            string strTokenCifrado = "";

            try
            {
                // Creamos nuestra trama
                string strToken = Id_Usuario.ToString() + "/";

                // Guardamos el idioma
                strToken += Id_Idioma + "/";

                // Guardamos los grupos
                if (Grupos == null) Grupos = new List<int>();                
                foreach (int grupo in Grupos)
                {
                    strToken += grupo + "|";
                }
                if (strToken.EndsWith("|"))
                    strToken = strToken.Substring(0, strToken.Length - 1);                
                strToken += "/";

                // Guardamos los permisos
                if (PermisoZonas != null)
                {
                    foreach (Funcionalidad zona in PermisoZonas)
                    {
                        // Almacenamos el id_funcionalidad 
                        strToken += zona.Id_Feature.ToString() + "-";
                        // y separado por un guion ponemos la lista de permisos de esta funcionalidad
                        foreach (int permiso in zona.Permissions)
                        {
                            strToken += permiso + ",";
                        }
                        // Para separar entre una funcionalidad con permisos y otra ponemmos una barra |
                        strToken += "|";
                    }
                }
                if (strToken.EndsWith("|"))
                    strToken = strToken.Substring(0, strToken.Length - 1);
                // Guardamos la caducidad
                strToken += "/" + ((TrickHoraCaducidad == null) ? 0 : TrickHoraCaducidad);

                // Ciframos la trama
                string pass = string.Empty;
                if (ClavePrivada != null)
                    pass = ClavePrivada;
                                
                Cifrado.AES cifrado = new Cifrado.AES();
                strTokenCifrado = cifrado.Encrypt(strToken, pass, "inicializa cadena", "SHA1", 1, "BSHP-3512-BACRFA", 128);
            }
            catch (Exception er)
            {
                strTokenCifrado = "";
                var exception = er;
                //log.Error("GenerateStringToken()", er);
            }
            return strTokenCifrado;
        }

        /// <summary>
        /// Genera un token de usuario, devolviendo el objeto contenedor del mismo
        /// </summary>
        /// <param name="Id_Usuario"></param>
        /// <param name="Id_Idioma"></param>
        /// <param name="PermisoZonas"></param>
        /// <param name="TrickHoraCaducidad"></param>
        /// <param name="ClavePrivada"></param>
        /// <returns></returns>
        public Token GenerateToken(int Id_Usuario, int Id_Idioma, List<int> Grupos, List<Funcionalidad> PermisoZonas, long? TrickHoraCaducidad, string ClavePrivada)
        {
            Token tok = null;
            try
            {
                string strToken = GenerateStringToken(Id_Usuario, Id_Idioma, PermisoZonas, Grupos, TrickHoraCaducidad, ClavePrivada);
                tok = ReadToken(strToken, ClavePrivada);
            }
            catch (Exception er)
            {
                var exception = er;
                //log.Error("GenerateToken()", er);
            }
            return tok;
        }

        /// <summary>
        /// Lee un token en string y devuelve su objeto
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="ClavePrivada"></param>
        /// <returns></returns>
        public Token ReadToken(string Token, string ClavePrivada)
        {
            Token tokUsuario = null;

            try
            {
                // Desciframos y cargamos la trama
                tokUsuario = new Token(Token, ClavePrivada);
            }
            catch (Exception er)
            {
                var exception = er;
                tokUsuario = new Token();
                //log.Error("ReadToken()", er);
            }
            return tokUsuario;
        }
    }
}
