using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtocolUtils.CacheHepler
{
    public interface IGlobalCachingProvider
    {
        void AddItem(string key, object value);

        object GetItem(string key);
    }
}
