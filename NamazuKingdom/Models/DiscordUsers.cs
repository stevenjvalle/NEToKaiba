using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamazuKingdom.Models
{
    public class DiscordUsers
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Discord User ID")]
        public ulong DiscordUserId { get; set; }

        [Display(Name = "Preferred Nickname")]
        [StringLength(100)]
        public string Nickname { get; set; } = "";

        [Display(Name = "Preferred Pronouns")]
        [StringLength(15)]
        public string PreferredPronouns { get; set; } = "";

        [Display(Name = "User Birthday")]
        public DateTime Birthday { get; set; }

        [Display(Name = "User Settings")]
        public UserSettings UserSettings { get; set; }

    }
}
