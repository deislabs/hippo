using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hippo.Application.Jobs;

public enum JobStatus
{
    Unknown = 0,
    Pending = 1,
    Running = 2,
    Dead = 3,
}
