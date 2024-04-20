using System.ComponentModel.DataAnnotations;

namespace LOT_FlightManagementAPI.Models;

public class Flight
{
    [Key]
    [Required]
    public UInt16 FlightId { get; private set; } // Had to add something in the FlightContext.cs to keep the setter private and get the Unit Tests to work
    [Required]
    public UInt16 FlightNumber { get; private set; }
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

    /// <summary>
    /// Check if two flights are the same field by field.
    /// </summary>
    /// <param name="obj">The other flight</param>
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (Flight)obj;
        return FlightId == other.FlightId
               && FlightNumber == other.FlightNumber
               && DepartureDate == other.DepartureDate
               && Destination == other.Destination
               && AircraftType == other.AircraftType;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(FlightId, FlightNumber, DepartureDate, Destination, AircraftType);
    }
}