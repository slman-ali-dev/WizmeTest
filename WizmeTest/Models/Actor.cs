using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WizmeTest.Models
{
    public class Actor
    {
        public Actor()
        {
            Movies = new HashSet<MovieActors>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [MaxLength(120)]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(120)]
        [Required]
        public string LastName { get; set; }
                
        public virtual ICollection<MovieActors> Movies { get; set; } 


    }
}
