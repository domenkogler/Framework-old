using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Kogler.Framework.Mef
{
    [Export]
    public class MefExportFactory<T> : IPartImportsSatisfiedNotification
    {
        [Import]
        internal ExportFactory<T> Factory { get; set; }

        public T Instance => lifeTime.Value;
        private ExportLifetimeContext<T> lifeTime; 
        
        private readonly Dictionary<T, ExportLifetimeContext<T>> Dic = new Dictionary<T, ExportLifetimeContext<T>>();

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            lifeTime = Factory.CreateExport();
        }

        public T Create()
        {
            var export = Factory.CreateExport();
            Dic[export.Value] = export;
            return export.Value;
        }

        public ExportLifetimeContext<T> this[T instance] => Dic[instance];
    }
}