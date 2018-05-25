namespace Model.DAL.DTO
{
    public class Tag_Provider
    {
        public int Id_Proveedor{ get; set; }
        public string Nombre { get; set; }
        public string OPCServer { get; set; }
        public int PollingInterval { get; set; }
        public int RequestConnection { get; set; }
    }
}
