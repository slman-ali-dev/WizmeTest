using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WizmeTest.Models
{
    public class MovieActors
    {
        public long MovieId { get; set; }
        public Movie Movie { get; set; }
        
        public long ActorId { get; set; }
        public Actor Actor { get; set; }
    }
}
