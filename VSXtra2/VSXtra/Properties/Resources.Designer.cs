﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VSXtra.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("VSXtra.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A site has already been set on package {0}..
        /// </summary>
        internal static string Package_SiteAlreadySet {
            get {
                return ResourceManager.GetString("Package_SiteAlreadySet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Object {0} was returned from a service creator callback but it does not implement the registered type of {1}.
        /// </summary>
        internal static string PackageBase_GetService_BadService {
            get {
                return ResourceManager.GetString("PackageBase_GetService_BadService", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to GetService is recursing on itself while trying to resolve the service {0}. This means that someone is asking for this service while the service is trying to create itself.  Breaking the recursion now and aborting this GetService call..
        /// </summary>
        internal static string PackageBase_GetService_Recursion {
            get {
                return ResourceManager.GetString("PackageBase_GetService_Recursion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An object was not returned from a service creator callback for the registered type of {0}. This may mean that it failed a type equivalence comparison.  To compare type objects you must use Type.IsEquivalentTo(Type).  Do not use .Equals or the == operator..
        /// </summary>
        internal static string PackageBase_GetService_TypeEquivalence {
            get {
                return ResourceManager.GetString("PackageBase_GetService_TypeEquivalence", resourceCulture);
            }
        }
    }
}
