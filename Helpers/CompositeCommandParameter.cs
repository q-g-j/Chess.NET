// Helper class for AdvancedInvokeCommandAction.
//
// Snippet taken from:
// https://stackoverflow.com/questions/66465149/pass-extra-argument-to-command-with-invokecommandaction



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDotNET.Helpers
{
    public class CompositeCommandParameter
    {
        public CompositeCommandParameter(object param, EventArgs eventArgs)
        {
            Parameter = param;
            EventArgs = eventArgs;
        }

        public object Parameter { get; }
        public EventArgs EventArgs { get; }
    }
}
