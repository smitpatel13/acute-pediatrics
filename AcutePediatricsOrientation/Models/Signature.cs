using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AcutePediatricsOrientation.Models
{
    public class Signature
    {
        [Key]
        public int UserId { get; set; }
        [Key]
        public int TopicId { get; set; }
        public DateTime Date { get; set; }
        public virtual Account User { get; set; }
        public virtual Topic Topic { get; set; }
    }
}
