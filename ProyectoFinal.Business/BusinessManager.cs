using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyectoFinal.Data;

namespace ProyectoFinal.Business
{
    public class BusinessManager<T> where T : IEntityBase, new()
    {
        public T GetEntityBaseObject()
        {
            return new T();
        }
    }
}
