using Memory.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManager.Conditions
{
    public interface ICondition
    {
        ActionForRequest action { get; set; }
        bool validateCondition(TagValue tagUpdated, TagValue memory);
    }
}
