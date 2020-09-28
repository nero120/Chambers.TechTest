using Chambers.TechTest.Api.Interfaces;

namespace Chambers.TechTest.Api.Models
{
    /// <summary>
    /// Generic class for an error response returned by the api
    /// </summary>
    public class ApiErrorResponse : IApiErrorResponse
    {
        public string Error
        {
            get
            {
                return GetType().Name.Replace("ApiErrorResponse", "");
            }
        }

        public string Message { get; set; }
    }
}
