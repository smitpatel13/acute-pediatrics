using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AcutePediatricsOrientation.Models
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public virtual ICollection<Documents> Documents { get; set; }
        public virtual ICollection<Signature> Signatures { get; set; }

        public virtual Category Category { get; set; }
    }
}
