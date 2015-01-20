using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtocolUtils.CacheHepler
{
    public class GlobalCachingProvider : CachingProviderBase, IGlobalCachingProvider
    {
        #region Singelton (inheriting enabled)

        protected GlobalCachingProvider()
        {

        }

        public static GlobalCachingProvider Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly GlobalCachingProvider instance = new GlobalCachingProvider();
        }

        #endregion

        #region ICachingProvider

        public virtual new void AddItem(string key, object value)
        {
            base.AddItem(key, value);
        }

        public virtual object GetItem(string key)
        {
            return base.GetItem(key, true);//Remove defulat is true because it's Global Cache!
        }

        public virtual new object GetItem(string key, bool remove)
        {
            return base.GetItem(key, remove);
        }

        #endregion


    }
}
