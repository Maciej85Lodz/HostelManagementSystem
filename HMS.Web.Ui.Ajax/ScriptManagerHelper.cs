using System;
using System.Reflection;
using System.Web.UI;

namespace HMS.Web.App.Ui.Ajax
{
    internal static class ScriptManagerHelper
    {
        private static readonly object ReflectionLock = new object();

        private static bool MethodsInitialized;

        private static MethodInfo RegisterStartupScriptMethod;

        private static MethodInfo RegisterClientScriptIncludeMethod;

        public static bool IsMono
        {
            get
            {
                return Type.GetType("Mono.Runtime") != null;
            }
        }

        private static void InitializeReflection()
        {
            if (!ScriptManagerHelper.MethodsInitialized)
            {
                lock (ScriptManagerHelper.ReflectionLock)
                {
                    if (!ScriptManagerHelper.MethodsInitialized)
                    {
                        Type type = null;
                        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                        for (int i = 0; i < assemblies.Length; i++)
                        {
                            Assembly assembly = assemblies[i];
                            string fullName;
                            if ((fullName = assembly.FullName) != null)
                            {
                                if (!(fullName == "System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"))
                                {
                                    if (!(fullName == "System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"))
                                    {
                                        if (fullName == "System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")
                                        {
                                            type = Type.GetType("System.Web.UI.ScriptManager, System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", false);
                                        }
                                    }
                                    else
                                    {
                                        type = Type.GetType("System.Web.UI.ScriptManager, System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", false);
                                    }
                                }
                                else
                                {
                                    type = Type.GetType("System.Web.UI.ScriptManager, System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", false);
                                }
                            }
                            if (type != null)
                            {
                                break;
                            }
                        }
                        if (type != null && !ScriptManagerHelper.IsMono)
                        {
                            ScriptManagerHelper.RegisterStartupScriptMethod = type.GetMethod("RegisterStartupScript", new Type[]
                            {
                                typeof(Page),
                                typeof(Type),
                                typeof(string),
                                typeof(string),
                                typeof(bool)
                            });
                            ScriptManagerHelper.RegisterClientScriptIncludeMethod = type.GetMethod("RegisterClientScriptInclude", new Type[]
                            {
                                typeof(Page),
                                typeof(Type),
                                typeof(string),
                                typeof(string)
                            });
                        }
                        ScriptManagerHelper.MethodsInitialized = true;
                    }
                }
            }
        }

        public static bool IsMicrosoftAjaxAvailable()
        {
            ScriptManagerHelper.InitializeReflection();
            return ScriptManagerHelper.RegisterStartupScriptMethod != null;
        }

        public static void RegisterClientScriptInclude(Control control, Type type, string key, string url)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            if (control.Page == null)
            {
                throw new ArgumentException("The control must be on a page.", "control");
            }
            ScriptManagerHelper.InitializeReflection();
            if (ScriptManagerHelper.RegisterClientScriptIncludeMethod != null)
            {
                ScriptManagerHelper.RegisterClientScriptIncludeMethod.Invoke(null, new object[]
                {
                    control.Page,
                    type,
                    key,
                    url
                });
                return;
            }
            control.Page.ClientScript.RegisterClientScriptInclude(type, key, url);
        }

        public static void RegisterStartupScript(Control control, Type type, string key, string script, bool addScriptTags)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            if (control.Page == null)
            {
                throw new ArgumentException("The control must be on a page.", "control");
            }
            ScriptManagerHelper.InitializeReflection();
            if (ScriptManagerHelper.RegisterStartupScriptMethod != null)
            {
                ScriptManagerHelper.RegisterStartupScriptMethod.Invoke(null, new object[]
                {
                    control.Page,
                    type,
                    key,
                    script,
                    addScriptTags
                });
                return;
            }
            control.Page.ClientScript.RegisterStartupScript(type, key, script, addScriptTags);
        }
    }
}
