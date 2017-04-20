using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneEchan.Server.Models
{
    public class TranscodeModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int VideoId { get; set; }
        public string RunId { get; set; }
        public string MediaId { get; set; }
    }
}