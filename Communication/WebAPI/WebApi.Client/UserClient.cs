using Model.BL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Client.Classes;

namespace WebApi.Client
{
    public class UserClient
    {
        private readonly string _host;

        public UserClient(string host)
        {
            this._host = host;
        }

        public async Task<Usuario> GetUserByIdentityCode(string identityCode)
        {
            Usuario user = null;

            try
            {
                string url = "api/User/GetUserByIdentityCode";

                using (var request = new Request(this._host))
                {
                    var response = await request.PostAsync(url, typeof(Usuario), identityCode);

                    if (response != null)
                    {
                        user = response as Usuario;
                    }
                }
            }
            catch (Exception)
            {
            }
            return user;
        }
    }
}
