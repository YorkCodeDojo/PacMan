﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NPacMan.Game.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NPacMan.Game.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to  XXXXXXXXXXXXXXXXXXXXXXXXXXXX
        /// X............XX............X
        /// X.XXXX.XXXXX.XX.XXXXX.XXXX.X
        /// X*X  X.X   X.XX.X   X.X  X*X
        /// X.XXXX.XXXXX.XX.XXXXX.XXXX.X
        /// X..........................X
        /// X.XXXX.XX.XXXXXXXX.XX.XXXX.X
        /// X.XXXX.XX.XXXXXXXX.XX.XXXX.X
        /// X......XX....XX....XX......X
        /// XXXXXX.XXXXX XX XXXXX.XXXXXX
        ///      X.XXXXX XX XXXXX.X     
        ///      X.XX   B      XX.X     
        ///      X.XX XXX--XXX XX.X     
        /// XXXXXX.XX X      X XX.XXXXXX
        ///T      .   X IC P X   .      T
        /// XXXXXX.XX X      X XX.XXXXXX
        ///      X.XX XXXX [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Board {
            get {
                return ResourceManager.GetString("Board", resourceCulture);
            }
        }
    }
}
