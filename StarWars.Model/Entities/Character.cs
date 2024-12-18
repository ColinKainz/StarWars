using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarWars.Model.Entities;

[Table("CHARACTERS")]
public class Character
{
    [Column("ID")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Column("NAME")]
    [Required]
    public string Name { get; set; }
    [Column("FACTION")]
    [Required]
    public string Faction { get; set; }
    [Column("SPECIES")]
    [Required]
    public string Species { get; set; }
    [Column("HOMEWORLD")]
    [Required]
    public string Homeworld { get; set; }
}