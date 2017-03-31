using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace DeviceService2.Counters
{
    public class PerfCounterWrapper
    {
        public PerfCounterWrapper(string name, string category, string counter, string instance = "")
        {
            this._counter = this.CreatePerformanceCounter(name, category, counter, instance);
            Name = name;
        }

        

        public string Name { get; set; }

        public float Value
        {
            get { return this.GetNextValue(); }
        }

        private PerformanceCounter _counter;

        private PerformanceCounter CreatePerformanceCounter(string name, string category, string counter, string instance = "")
        {
            try
            {
                return new PerformanceCounter(category, counter, instance, readOnly: true);
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private float GetNextValue()
        {
            if (this._counter == null)
            {
                return 0;
            }
            else
            {
                return this._counter.NextValue();
            }
            
        }
    }
}