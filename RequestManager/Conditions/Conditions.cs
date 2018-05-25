using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Memory.Common;

namespace RequestManager.Conditions
{
    internal class IsActiveChange:ICondition
    {
        public ActionForRequest action { get; set; }
        public bool validateCondition(TagValue tagUpdated, TagValue memory)
        {
            bool validation=((tagUpdated.Type == TagType.Activa) && (tagUpdated.Value == "1")) && (memory.Value == "0");
            this.action = ActionForRequest.Generated;
            return validation;
        }
    }
    internal class IsNotActiveChange : ICondition
    {
        public ActionForRequest action { get; set; }
        public bool validateCondition(TagValue tagUpdated, TagValue memory)
        {
            bool validation= ((tagUpdated.Type == TagType.Activa) && (tagUpdated.Value == "0")) && (memory.Value == "1");
            this.action = ActionForRequest.Delete;
            return validation;
        }
    }
    internal class IsTempOptChange : ICondition
    {
        public ActionForRequest action { get; set; }
        public bool validateCondition(TagValue tagUpdated, TagValue memory)
        {
            bool validation= ((tagUpdated.Type == TagType.Temp) && (tagUpdated.Value == "0")) && (memory.Value == "1");
            this.action = ActionForRequest.Generated;
            return validation;
        }
    }
    internal class IsNotTempOptChange : ICondition
    {
        public ActionForRequest action { get; set; }
        public bool validateCondition(TagValue tagUpdated, TagValue memory)
        {
            bool validation = ((tagUpdated.Type == TagType.Temp) && (tagUpdated.Value == "1")) && (memory.Value == "0");
            this.action = ActionForRequest.Generated;
            return validation;
        }
    }

    public class Conditions
    {
        private List<ICondition> conditionsToEval;
        public Conditions()
        {
            this.conditionsToEval = new List<ICondition>();
            conditionsToEval.Add(new IsActiveChange());
            conditionsToEval.Add(new IsNotActiveChange());
            conditionsToEval.Add(new IsTempOptChange());
            conditionsToEval.Add(new IsNotTempOptChange());
           
        }
    }
}
