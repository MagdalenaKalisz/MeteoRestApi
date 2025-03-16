namespace Meteo.Application.OpenMeteo
{
    using System.Threading.Tasks;
    using Domain.ValueObjects;
    using Meteo.Application.OpenMeteo.Dto;

    /// <summary>
    /// Open Meteo client interface.
    /// </summary>
    public interface IOpenMeteoClient
    {
        /// <summary>
        /// Gets the weather forecast for the specified coordinates.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="forecastDays"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OpenMeteoWeatherResponse?> GetWeatherAsync(Coordinates coordinates,
                                                        ForecastDaysAmount forecastDays,
                                                        CancellationToken cancellationToken = default);
    }
}