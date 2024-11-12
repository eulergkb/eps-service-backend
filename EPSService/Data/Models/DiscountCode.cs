using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESPService.Data.Models;

[PrimaryKey(nameof(Code))]
[Index(nameof(UsedAt))]
public class DiscountCode
{
    [Column(TypeName = "CHARACTER")]
    [StringLength(8)]
    public string Code { get; set; }

    public DateTime ? UsedAt { get; set; }

    public DateTime CreatedAt { get; set; }
}
