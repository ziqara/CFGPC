using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib
{
    public interface IUserRepository
    {
        User FindByEmail(string email);
        User Save(User user);
    }
}
