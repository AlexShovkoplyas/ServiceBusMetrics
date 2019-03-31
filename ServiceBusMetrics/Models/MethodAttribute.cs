using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceBusMetrics
{
    public class MethodAttribute : Attribute
    {
        public MethodAttribute(string name, ActionType actionType)
        {
            Description = name;
            ActionType = actionType;
        }

        public string Description { get; set; }
        public ActionType ActionType { get; set; }
    }
}
