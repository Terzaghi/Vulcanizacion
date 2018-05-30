using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Common.Security
{
    [Serializable]
    public class Token : ISerializable
    {            
        #region Propiedades
        public int Id_Usuario { get; private set; }
        public int Id_Device { get; private set; }
        public long TickHora { get; private set; }
        private bool _esValido = false;
        public bool EsValido
        {
            get
            {
                // Si es válido, comprobamos a ver si ha caducado
                if (_esValido)
                {
                    DateTime fchExpiracion = new DateTime(this.TickHora);
                    if (fchExpiracion < DateTime.Now)
                        _esValido = false;
                }
                return _esValido;
            }
        }

        public string TokenString { get; private set; }
            
        #endregion

        public Token()
        {
            Inicializa(); 
        }

        public Token(string tokenString) : base()
        {
            Inicializa();

            try
            {
                string pass = string.Empty;
              
                // Desciframos la trama
                Cifrado.AES cifrado = new Cifrado.AES();                
                string strToken = cifrado.Decrypt(tokenString, pass, "inicializa cadena", "SHA1", 1, "BSHP-3512-BACRFA", 128);

                // La parseamos
                string[] tramas = strToken.Split('/');

                // Cargamos el id_usuario
                this.Id_Usuario = Convert.ToInt32(tramas[0]);
               
                // Cargamos la fecha de caducidad
                long duracionToken = long.Parse(tramas[4]);
                if (duracionToken == 0)
                    duracionToken = DateTime.MaxValue.Ticks;
                this.TickHora = duracionToken;


                // Almacenamos el token original
                this.TokenString = tokenString;

                // Validamos la fecha de expiración
                DateTime fchCaducidad = new DateTime(this.TickHora);

                // Establece el estado tras la carga
                if (fchCaducidad < DateTime.Now)
                    this._esValido = false;
                else
                    this._esValido = true;
                
            }
            catch (Exception)
            {
                this._esValido = false;                
                /*
                log.Error(
                    string.Format("TokenUsuario({0}, {1})", token, pass), 
                    er);
                */
            }
        }

        private void Inicializa()
        {
            this._esValido = false;
           
            this.TokenString = "";
        }

        #region ISerializable
        protected Token(SerializationInfo info, StreamingContext context)
        {
            this.Id_Usuario = (int)info.GetValue("Id_Usuario", typeof(int));
            this.Id_Device = (int)info.GetValue("Id_Device", typeof(int));
            this.TickHora = (long)info.GetValue("TickHora", typeof(long));
            this.TokenString = (string)info.GetValue("Token", typeof(string));
            this._esValido = (bool)info.GetValue("_esValido", typeof(bool));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            info.AddValue("Id_Usuario", this.Id_Usuario);
            this.Id_Device = (int)info.GetValue("Id_Device", typeof(int));
            info.AddValue("TickHora", this.TickHora);
            info.AddValue("Token", this.TokenString);
            info.AddValue("_esValido", this._esValido);
        }
        #endregion
    }

    [Serializable]
    public class Funcionalidad
    {
        public int Id_Feature;
        public List<int> Permissions = new List<int>();

        public Funcionalidad(int Id_Feature, List<int> Permissions)
        {
            this.Id_Feature = Id_Feature;
            this.Permissions = Permissions;
        }
    }



}
