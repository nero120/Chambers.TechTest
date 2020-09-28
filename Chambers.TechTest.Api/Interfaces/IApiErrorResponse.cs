namespace Chambers.TechTest.Api.Interfaces
{
    /// <summary>
    /// Describes an error response returned by the api
    /// </summary>
    public interface IApiErrorResponse
    {
        string Error { get; }

        string Message { get; set; }
    }
}
