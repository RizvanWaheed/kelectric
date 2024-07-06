using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Common.Enum
{
    public enum AccountSubscriberType
    {
        Basic = 1,
        Silver=2,
        Gold=3,
        Platinum=4,
        Trial=5
    }

    public enum RestrictionType
    {
        Group = 1,
        Team = 2,
        Keyword = 3,
        Tag = 4,
        FBProfile = 5,
        TWProfile = 6,
        INProfile = 7
    }
}
