// Helper class for AdvancedInvokeCommandAction.
//
// Snippet taken from:
// https://stackoverflow.com/questions/66465149/pass-extra-argument-to-command-with-invokecommandaction


using System;


namespace ChessDotNET.GUI.ViewHelpers
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
