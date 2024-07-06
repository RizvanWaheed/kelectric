using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using Quartz;

namespace Insight360.Scheduling.Quartz
{
    public class TriggerListener : ITriggerListener
    {
        public void TriggerFired(ITrigger trigger, IJobExecutionContext context)
        {
            //Console.WriteLine("The scheduler called {0} for trigger {1}", MethodBase.GetCurrentMethod().Name, trigger.Key);
        }

        public bool VetoJobExecution(ITrigger trigger, IJobExecutionContext context)
        {
            //Console.WriteLine("The scheduler called {0} for trigger {1}", MethodBase.GetCurrentMethod().Name,trigger.Key);
            return false;
        }

        public void TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode)
        {
            //Debug.WriteLine("The scheduler called {0} for trigger {1}", MethodBase.GetCurrentMethod().Name, trigger.Key);
        }

        public void TriggerMisfired(ITrigger trigger)
        {
            //Debug.WriteLine("The scheduler called {0} for trigger {1}", MethodBase.GetCurrentMethod().Name, trigger.Key);
        }

        public string Name
        {
            get { return "CurrentTimeTriggerListener"; }
        }
    }
}