using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace VSXtra
{
    public static class SiteManager
    {
        public static bool HasGlobalServiceProvider
        {
            get
            {
                return (GlobalServiceProvider != null);
            }
        }

        private static System.IServiceProvider _GlobalServiceProvider;

        public static System.IServiceProvider GlobalServiceProvider
        {
            get
            {
                return _GlobalServiceProvider;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dte"></param>
        public static void SuggestGlobalServiceProvider(EnvDTE.DTE dte)
        {
            if (dte == null)
                throw new ArgumentNullException("dte");
            SuggestGlobalServiceProvider(dte as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Please make sure that the serviceProvider was sited in a Visual Studio instance.</remarks>
        /// <param name="serviceProvider"></param>
        public static void SuggestGlobalServiceProvider(System.IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            _GlobalServiceProvider = serviceProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oleServiceProvider"></param>
        public static void SuggestGlobalServiceProvider(IOleServiceProvider oleServiceProvider)
        {
            if (oleServiceProvider == null)
                throw new ArgumentNullException("oleServiceProvider");

            _GlobalServiceProvider = new ServiceProvider(oleServiceProvider);
        }
    }
}