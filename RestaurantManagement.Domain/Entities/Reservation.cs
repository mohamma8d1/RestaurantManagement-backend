using RestaurantManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Domain.Entities;

public class Reservation : BaseEntity
{
    public Guid UserId { get; set; }
    public DateTime ReservationDate { get; set; }
    public int NumberOfGuests { get; set; }
    public string? SpecialRequests { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    public virtual User User { get; set; } = null!;
}
