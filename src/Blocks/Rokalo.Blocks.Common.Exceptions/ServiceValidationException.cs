namespace Rokalo.Blocks.Common.Exceptions
{
    using System;
    using System.Collections.Generic;

    public class ServiceValidationException : ApplicationException
    {
        public ServiceValidationException(Dictionary<string, string[]> errors)
        {
            Title = "One or more validation errors occurred.";
            Detail = "Check the errors for more details";
            Errors = errors;
        }

        public ServiceValidationException(string error)
        {
            Title = "One or more validation errors occurred.";
            Detail = "Check the errors for more details";
            Errors = new Dictionary<string, string[]>()
            {
                { "General", new string[] { error } }
            };
        }

        public string Title { get; }

        public string Detail { get; }

        public Dictionary<string, string[]> Errors { get; }
    }
}
