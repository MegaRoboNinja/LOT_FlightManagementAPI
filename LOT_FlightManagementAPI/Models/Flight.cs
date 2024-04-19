using System.ComponentModel.DataAnnotations;

namespace LOT_FlightManagementAPI.Models;

public class Flight
{
    [Required]
    public UInt16 FlightId { get; }
    [Required]
    public UInt16 FlightNumber { get; }
    [Required]
    public DateTime DepartureDate { get; private set; }
    [Required] [MaxLength(30)] [MinLength(3)]
    public string Origin { get; private set; }
    [Required] [MaxLength(30)] [MinLength(3)]
    public string Destination { get; private set; }
    [Required] [MaxLength(30)] [MinLength(3)]
    public string AircraftType { get; private set; }

    public Flight(ushort flightId, ushort flightNumber, DateTime departureDate, string origin, string destination, string aircraftType)
    {
        FlightId = flightId;
        FlightNumber = flightNumber;
        DepartureDate = departureDate;
        Origin = origin;
        Destination = destination;
        AircraftType = aircraftType;
    }
}