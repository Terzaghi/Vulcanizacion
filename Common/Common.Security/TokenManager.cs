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
        public string GenerateStringToken(int Id_Usuario,int Id_Device, long? TrickHoraCaducidad)
        {
            string strTokenCifrado = "";

            try
            {
                // Creamos nuestra trama
                string strToken = Id_Usuario.ToString() + "/";
                                
                Cifrado.AES cifrado = new Cifrado.AES();
                strTokenCifrado = cifrado.Encrypt(strToken, "", "inicializa cadena", "SHA1", 1, "BSHP-3512-BACRFA", 128);
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
        public Token GenerateToken(int Id_Usuario, int Id_Device,long? TrickHoraCaducidad)
        {
            Token tok = null;
            try
            {
                string strToken = GenerateStringToken(Id_Usuario, Id_Device,TrickHoraCaducidad);
                tok = ReadToken(strToken);
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
        public Token ReadToken(string Token)
        {
            Token tokUsuario = null;

            try
            {
                // Desciframos y cargamos la trama
                tokUsuario = new Token(Token);
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
