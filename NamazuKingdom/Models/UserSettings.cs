using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamazuKingdom.Models
{
    public class UserSettings
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Discord User Table ID")]
        public Guid UserRefId { get; set; }

        [Required]
        [Display(Name = "Discord User")]
        public DiscordUsers DiscordUser { get; set; }

        [Required]
        [Display(Name = "Use TTS")]
        public bool UseTTS { get; set; } = false;

        [Required]
        [Display(Name = "TTS Voice Name")]
        [StringLength(25)]
        public string TTSVoiceName { get; set; } = "Amy";

        [Required]
        [Display(Name = "Show Birthday")]
        public bool ShouldShowBirthday { get; set; } = false;
    }
}
