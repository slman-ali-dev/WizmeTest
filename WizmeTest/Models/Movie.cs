using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WizmeTest.Models
{
    public class Movie
    {
        public Movie()
        {
            Actors = new HashSet<MovieActors>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [MaxLength(120)]
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }


        public virtual ICollection<MovieActors> Actors { get; set; }

    }
}
