using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal.Data
{
    public interface IEntityBase
    {
        int UniqueIdentifier { get; set; }
    }
    public abstract class EntityBase : IEntityBase
    {
        [NotMapped]
        public int UniqueIdentifier { get; set; }
    }
}
