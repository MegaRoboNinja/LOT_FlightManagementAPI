using System.ComponentModel.DataAnnotations;

namespace LOT_FlightManagementAPI.Models;

public class Flight
{
    [Key]
    [Required]
    public UInt16 FlightId { get; private set; } // Had to add something in the FlightContext.cs to keep the setter private and get the Unit Tests to work
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

    public Flight(){} // Needed for the unit tests (I know it shouldn't be public though)
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