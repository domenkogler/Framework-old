using System;

namespace Kogler.Framework
{
    public interface INullableIntegrationID
    {
        long? NullableIntegrationID { get; }
    }

    public interface IIntegrationID
    {
        long IntegrationID { get; set; }
    }

    public interface IIntegrationKey
    {
        string IntegrationKey { get; set; }
    }
}
