using System;
using System.Collections.Generic;
using agsXMPP.protocol.extensions.caps;
using agsXMPP.protocol.iq.disco;
using xeus2.xeus.Data;

namespace xeus2.xeus.Core
{
    internal class CapsCache
    {
        private static readonly CapsCache _instance = new CapsCache();

        private Dictionary<string, DiscoInfo> _cache = null;
        private readonly object _cacheLock = new object();

        public static CapsCache Instance
        {
            get
            {
                return _instance;
            }
        }

        public void AddToCache(Capabilities capabilities, DiscoInfo discoInfo)
        {
            if (discoInfo.GetFeatures().Length > 0)
            {
                lock (_cacheLock)
                {
                    string caps = GetCapsString(capabilities);
                    _cache[GetCapsString(capabilities)] = discoInfo;
                    Database.SaveCapsCache(caps, discoInfo.ToString());
                }
            }
        }

        public DiscoInfo Get(Capabilities capabilities)
        {
            DiscoInfo discoInfo;

            _cache.TryGetValue(GetCapsString(capabilities), out discoInfo);

            return discoInfo;
        }

        static string GetCapsString(Capabilities capabilities)
        {
            if (string.IsNullOrEmpty(capabilities.Version))
            {
                return capabilities.Version;
            }

            string[] extensions = capabilities.Extensions;

            Array.Sort(extensions);

            return string.Format("{0}#{1}<{2}", capabilities.Node,
                                        capabilities.Version,
                                        string.Join("<", extensions));
        }

        public void Load()
        {
            _cache = Database.GetCapsCache();
        }
    }
}