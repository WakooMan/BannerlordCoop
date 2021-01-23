﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Sync.Behaviour;

namespace Sync
{
    /// <summary>
    ///     Patch generator for method calls.
    /// </summary>
    public class MethodPatch
    {
        private const BindingFlags All = BindingFlags.Instance | 
                                         BindingFlags.Static | 
                                         BindingFlags.Public | 
                                         BindingFlags.NonPublic | 
                                         BindingFlags.GetField | 
                                         BindingFlags.SetField | 
                                         BindingFlags.GetProperty | 
                                         BindingFlags.SetProperty;
        private readonly List<MethodAccess> m_Access = new List<MethodAccess>();
        private readonly Type m_Declaring;

        public MethodPatch([NotNull] Type declaringClass)
        {
            m_Declaring = declaringClass;
        }

        public IEnumerable<MethodAccess> Methods => m_Access;

        ~MethodPatch()
        {
            foreach (MethodAccess syncMethod in m_Access)
            {
                MethodPatchFactory.RemovePrefix(syncMethod.MethodBase);
            }
        }

        /// <summary>
        ///     Patches all member methods of the declaring class with a prefix that relays all calls to
        ///     <see cref="MethodAccess.InvokePrefix" />.
        /// </summary>
        /// <param name="eBindingFlags"></param>
        /// <param name="eFlags"></param>
        /// <returns></returns>
        public MethodPatch InterceptAll(
            BindingFlags eBindingFlags =
                BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly,
            EMethodPatchFlag eFlags = EMethodPatchFlag.None)
        {
            foreach (MethodInfo method in m_Declaring.GetMethods(eBindingFlags))
            {
                Intercept(method, eFlags);
            }

            return this;
        }

        /// <summary>
        ///     Creates a <see cref="MethodAccess" /> and patches in a prefix that relays all calls to
        ///     <see cref="MethodAccess.InvokePrefix" />.
        /// </summary>
        /// <param name="method">Method to track.</param>
        /// <param name="eFlags">Flags for the generated interceptor.</param>
        /// <returns>this</returns>
        /// <exception cref="ArgumentException">
        ///     If the method is not declared in class
        ///     <see cref="m_Declaring" />
        /// </exception>
        public MethodPatch Intercept(
            MethodBase method,
            EMethodPatchFlag eFlags = EMethodPatchFlag.None)
        {
            if (method.DeclaringType != m_Declaring)
            {
                throw new ArgumentException(
                    $"Provided method {method} is not declared in {m_Declaring}",
                    nameof(method));
            }

            PatchPrefix(method, eFlags);
            return this;
        }

        /// <summary>
        ///     Creates a <see cref="MethodAccess" /> and patches in a prefix that relays all calls to
        ///     <see cref="MethodAccess.InvokePrefix" />.
        /// </summary>
        /// <param name="sMethodName">Name of the method</param>
        /// <param name="eFlags">Flags for the generated interceptor.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If no method with that name exists.</exception>
        public MethodPatch Intercept(
            string sMethodName,
            EMethodPatchFlag eFlags = EMethodPatchFlag.None,
            BindingFlags eBindingFlags = All)
        {
            foreach (MethodInfo info in m_Declaring.GetMethods(eBindingFlags))
            {
                if (info.Name == sMethodName)
                {
                    Intercept(info, eFlags);
                }
            }

            return this;
        }

        /// <summary>
        ///     Do not use, generics cannot be reliably patched as of right now. See
        ///     https://github.com/pardeike/Harmony/issues/320
        /// </summary>
        /// <param name="sMethodName"></param>
        /// <param name="genericInstantiations"></param>
        /// <param name="eFlags"></param>
        /// <param name="eBehaviour"></param>
        /// <returns></returns>
        [Obsolete]
        public MethodPatch InterceptGeneric(
            string sMethodName,
            Type[] genericInstantiations,
            EMethodPatchFlag eFlags = EMethodPatchFlag.None,
            BindingFlags eBindingFlags = All)
        {
            foreach (MethodInfo info in m_Declaring.GetMethods(eBindingFlags))
            {
                if (info.IsGenericMethod && info.Name == sMethodName)
                {
                    foreach (Type genericArg in genericInstantiations)
                    {
                        Intercept(info.MakeGenericMethod(genericArg), eFlags);
                    }
                }
            }

            return this;
        }
        
        public MethodPatch Postfix(
            string sMethodName, 
            EMethodPatchFlag eFlags = EMethodPatchFlag.None,
            BindingFlags eBindingFlags = All)
        {
            foreach (MethodInfo info in m_Declaring.GetMethods(eBindingFlags))
            {
                if (info.Name == sMethodName)
                {
                    Postfix(info, eFlags);
                }
            }

            return this;
        }
        
        public MethodPatch Postfix(
            MethodBase method, 
            EMethodPatchFlag eFlags = EMethodPatchFlag.None)
        {
            if (method.DeclaringType != m_Declaring)
            {
                throw new ArgumentException(
                    $"Provided method {method} is not declared in {m_Declaring}",
                    nameof(method));
            }

            PatchPostfix(method, eFlags);
            return this;
        }

        public bool TryGetMethod(string sMethodName, out MethodAccess methodAccess)
        {
            MethodInfo method = AccessTools.Method(m_Declaring, sMethodName);
            if (method.IsGenericMethod)
            {
                throw new ArgumentException(
                    $"Unable to generate patch: provided method {method} is generic. Use a [HarmonyPatch] with TargetMethod instead.",
                    nameof(method));
            }

            return TryGetMethod(method, out methodAccess);
        }

        public bool TryGetMethod(
            string sMethodName,
            Type[] genericArguments,
            out MethodAccess methodAccess)
        {
            MethodInfo method = AccessTools.Method(
                m_Declaring,
                sMethodName,
                null,
                genericArguments);
            if (method.IsGenericMethod)
            {
                throw new ArgumentException(
                    $"Unable to generate patch: provided method {method} is generic. Use a [HarmonyPatch] with TargetMethod instead.",
                    nameof(method));
            }

            return TryGetMethod(method, out methodAccess);
        }

        public bool TryGetMethod(MethodBase methodInfo, out MethodAccess methodAccess)
        {
            methodAccess = m_Access.FirstOrDefault(m => m.MethodBase.Equals(methodInfo));
            return methodAccess != null;
        }

        /// <summary>
        ///     Dynamically creates a new prefix for a call to <paramref name="original"/> that redirects the call
        ///     to our static dispatcher <see cref="DispatchPrefixExecution"/>.
        /// </summary>
        /// <param name="original">Method to be patched.</param>
        /// <param name="eFlags">Flags to the patch generator.</param>
        private void PatchPrefix(
            MethodBase original,
            EMethodPatchFlag eFlags)
        {
            MethodInfo dispatcher = AccessTools.Method(
                typeof(MethodPatch),
                nameof(DispatchPrefixExecution));
            MethodAccess access = MethodPatchFactory.AddPrefix(original, dispatcher);
            access.AddFlags(eFlags);
            m_Access.Add(access);
        }

        /// <summary>
        ///     Dispatcher that is being called for prefixes to forward the call to <see cref="MethodAccess.InvokePrefix" />.
        /// </summary>
        /// <param name="methodAccess">Access to the patched method that is being called.</param>
        /// <param name="instance">Instance that the method is being called on.</param>
        /// <param name="args">Parameters to the method call.</param>
        /// <returns></returns>
        private static bool DispatchPrefixExecution(
            MethodAccess methodAccess,
            [CanBeNull] object instance,
            params object[] args)
        {
            return methodAccess.InvokePrefix(ETriggerOrigin.Local, instance, args);
        }
        
        private void PatchPostfix(
            MethodBase method, 
            EMethodPatchFlag eFlags)
        {
            MethodInfo dispatcher = AccessTools.Method(
                typeof(MethodPatch),
                nameof(DispatchPostfixExecution));
            MethodAccess access = MethodPatchFactory.AddPostfix(method, dispatcher);
            access.AddFlags(eFlags);
            m_Access.Add(access);
        }
        
        private static void DispatchPostfixExecution(
            MethodAccess methodAccess,
            [CanBeNull] object instance,
            params object[] args)
        {
            methodAccess.InvokePostfix(ETriggerOrigin.Local, instance, args);
        }
    }
}
