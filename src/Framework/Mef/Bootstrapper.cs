using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;

namespace Kogler.Framework.Mef
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper(bool useApplication) : base(useApplication)
        {
            Mef = Mef.Instance;
            Initialize();
        }

        internal Bootstrapper(bool useApplication, Mef mef) : base(useApplication)
        {
            Mef = mef;
            Initialize();
        }

        internal Mef Mef { get; }

        protected override void InitContainer()
        {
            AssemblySource.Instance.Apply(Mef.Add);
            //Mef.Batch.AddExportedValue<IWindowManager>(new WindowManager());
            //Mef.Batch.AddExportedValue<IEventAggregator>(new EventAggregator());
        }
        
        protected override void FinishContainer()
        {
            Mef.Compose();
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = Mef.Container.GetExportedValues<object>(contract);

            if (exports.Any()) return exports.First();

            throw new Exception($"Could not locate any instances of contract {contract}.");
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Mef.Container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            Mef.Container.SatisfyImportsOnce(instance);
        }

        protected override void DestroyContainer()
        {
            Mef.Container.Dispose();
            Mef.Catalog.Dispose();
        }
    }
}