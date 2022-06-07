using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hippo.Application.Jobs;

public enum JobStatus
{
    Running = 0,
    Dead = 1,
    Unknown = 2,
}
