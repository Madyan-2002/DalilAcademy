using DalilAcademy.Core.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace DalilAcademy.Core.Security
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiersAt) CreateToken(User user);
    }
}
