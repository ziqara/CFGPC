using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib.Component
{
    internal interface IComponentRepository
    {
        List<ComponentDto> GetComponentsByCategory(string category);
    }
}
