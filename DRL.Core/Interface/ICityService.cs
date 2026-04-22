using DRL.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Core.Interface
{
    public interface ICityService
    {
        List<ENTLookUpItem> GetCitiesLookup(string state);
    }
}
