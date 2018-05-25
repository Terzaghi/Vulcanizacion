using System;

namespace Communication.SignalR.DTO
{
    public class StateToSend
    {
        public int Id_State { get; set; }
        public DateTime StateDate { get; set; }

        public StateToSend(int State, DateTime Date)
        {
            this.Id_State = State;
            this.StateDate = Date;
        }
    }
}
