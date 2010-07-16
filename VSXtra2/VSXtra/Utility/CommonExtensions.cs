// ================================================================================================
// CommonExtensions.cs
//
// This source code is created by using the source code provided with the VS 2010 SDK. Many 
// patterns and implementation details are defined there. The code here is intended to be the base
// of a new framework for developing VSPackages.
// The code here is experimental and fully opened for community.
//
// Created: 2010.07.06, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class defines useful extension methods used by MPF.
  /// </summary>
  // ================================================================================================
  public static class CommonExtensions
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the service described by the <typeparamref name="TService"/>
    /// type parameter.
    /// </summary>
    /// <returns>
    /// The service instance requested by the <typeparamref name="TService"/> parameter if found; otherwise null.
    /// </returns>
    /// <typeparam name="TService">The type of the service requested.</typeparam>
    /// <param name="serviceProvider">
    /// Service provider instance to request the service from.
    /// </param>
    // --------------------------------------------------------------------------------------------
    public static TService GetService<TService>(this IServiceProvider serviceProvider)
    {
      return (TService)serviceProvider.GetService(typeof(TService));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the service described by the <typeparamref name="TSInterface" />
    /// type parameter and retrieves it as an interface type specified by the
    /// <typeparamref name="TInterface"/> type parameter.
    /// </summary>
    /// <returns>
    /// The service instance requested by the <see cref="TSInterface"/> parameter if
    /// found; otherwise null.
    /// </returns>
    /// <typeparam name="TSInterface">The type of the service requested.</typeparam>
    /// <typeparam name="TInterface">
    /// The type of interface retrieved. The object providing <see cref="TSInterface"/>
    /// must implement <see cref="TInterface"/>.
    /// </typeparam>
    /// <param name="serviceProvider">
    /// Service provider instance to request the service from.
    /// </param>
    // --------------------------------------------------------------------------------------------
    public static TInterface GetService<TSInterface, TInterface>(
      this IServiceProvider serviceProvider)
    {
      return (TInterface)serviceProvider.GetService(typeof(TSInterface));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds the specified service to the service container.
    /// </summary>
    /// <typeparam name="TService">Type of service to add to the container.</typeparam>
    /// <param name="container">Container to add the service instance.</param>
    /// <param name="callback">Callback method adding the service to the container.</param>
    // --------------------------------------------------------------------------------------------
    public static void AddService<TService>(this IServiceContainer container,
      ServiceCreatorCallback callback)
    {
      container.AddService(typeof(TService), callback);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds the specified service to the service container.
    /// </summary>
    /// <typeparam name="TService">Type of service to add to the container.</typeparam>
    /// <param name="container">Container to add the service instance.</param>
    /// <param name="serviceInstance">Service instance</param>
    // --------------------------------------------------------------------------------------------
    public static void AddService<TService>(this IServiceContainer container,
      TService serviceInstance)
    {
      container.AddService(typeof(TService), serviceInstance);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds the specified service to the service container.
    /// </summary>
    /// <typeparam name="TService">Type of service to add to the container.</typeparam>
    /// <param name="container">Container to add the service instance.</param>
    /// <param name="serviceInstance">Service instance</param>
    /// <param name="promote">
    /// True, if the service should be promoted to the parent service container.
    /// </param>
    // --------------------------------------------------------------------------------------------
    public static void AddService<TService>(this IServiceContainer container,
      TService serviceInstance, bool promote)
    {
      container.AddService(typeof(TService), serviceInstance, promote);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds the specified service to the service container.
    /// </summary>
    /// <typeparam name="TService">Type of service to add to the container.</typeparam>
    /// <param name="container">Container to add the service instance.</param>
    /// <param name="callback">Callback method adding the service to the container.</param>
    /// <param name="promote">
    /// True, if the service should be promoted to the parent service container.
    /// </param>
    // --------------------------------------------------------------------------------------------
    public static void AddService<TService>(this IServiceContainer container,
      ServiceCreatorCallback callback, bool promote)
    {
      container.AddService(typeof(TService), callback, promote);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes a simple action for all items in the specified collection.
    /// </summary>
    /// <typeparam name="T">Type of items in the collection.</typeparam>
    /// <param name="collection">Collection containing items.</param>
    /// <param name="action">Action to be executed on items.</param>
    // --------------------------------------------------------------------------------------------
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
      foreach (var item in collection)
        action(item);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the specified custom attributes of a given type.
    /// </summary>
    /// <typeparam name="TAttr">Type of attributes to enumerate.</typeparam>
    /// <param name="type">Type tosearch for attributes.</param>
    /// <param name="inherit">True if the base class chain should be searched for attributes.</param>
    /// <returns>Enumerated attributes.</returns>
    // --------------------------------------------------------------------------------------------
    public static IEnumerable<TAttr> AttributesOfType<TAttr>(this Type type, bool inherit)
      where TAttr : Attribute
    {
      return type.GetCustomAttributes(typeof(TAttr), inherit).OfType<TAttr>();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the specified custom attributes of a given type.
    /// </summary>
    /// <typeparam name="TAttr">Type of attributes to enumerate.</typeparam>
    /// <param name="type">Type tosearch for attributes.</param>
    /// <returns>Enumerated attributes.</returns>
    // --------------------------------------------------------------------------------------------
    public static IEnumerable<TAttr> AttributesOfType<TAttr>(this Type type)
      where TAttr : Attribute
    {
      return type.GetCustomAttributes(typeof(TAttr), false).OfType<TAttr>();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Obtains an attribute with the specified type.
    /// </summary>
    /// <typeparam name="TAttr">Attribute type</typeparam>
    /// <param name="type">Type to search for attributes</param>
    /// <param name="inherit">Should be base classes searched?</param>
    /// <returns>Attribute if found; otherwise, null</returns>
    // --------------------------------------------------------------------------------------------
    public static TAttr GetAttribute<TAttr>(this Type type, bool inherit)
      where TAttr : Attribute
    {
      var attrs = type.GetCustomAttributes(typeof(TAttr), inherit);
      return attrs.Length > 0
               ? attrs[0] as TAttr
               : null;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Obtains an attribute with the specified type.
    /// </summary>
    /// <typeparam name="TAttr">Attribute type</typeparam>
    /// <param name="type">Type to search for attributes</param>
    /// <returns>Attribute if found; otherwise, null</returns>
    // --------------------------------------------------------------------------------------------
    public static TAttr GetAttribute<TAttr>(this Type type)
      where TAttr : Attribute
    {
      var attrs = type.GetCustomAttributes(typeof(TAttr), true);
      return attrs.Length > 0
               ? attrs[0] as TAttr
               : null;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Checks if the specified type derives from the given generic type.
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <param name="generic">Generic type to check as base class</param>
    /// <returns>
    /// True, if the specified type derives from the given generic interface; otherwise, false.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static bool DerivesFromGenericType(this Type type, Type generic)
    {
      while (type != null)
      {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == generic)
          return true;
        type = type.BaseType;
      }
      return false;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the generic parameter of the specified generic types at the given posititon.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <param name="generic">Generic type definition to obtain parameter for.</param>
    /// <param name="position">Position of the parameter.</param>
    /// <returns>
    /// Type parameter at the specified position if the parameter exists; otherwise, null.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static Type GenericParameterOfType(this Type type, Type generic, int position)
    {
      while (type != null)
      {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == generic)
        {
          var args = type.GetGenericArguments();
          return position >= 0 && position < args.Length
                   ? args[position]
                   : null;
        }
        type = type.BaseType;
      }
      return null;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Checks if a type implements the specified generic interface.
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <param name="generic">Generic interface to check.</param>
    /// <returns>
    /// True, if the type implements the specified generic interface; otherwise, false.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static bool ImplementsGenericType(this Type type, Type generic)
    {
      // --- We check only for generic interfaces.
      if (!generic.IsGenericTypeDefinition || !generic.IsInterface) return false;

      return type.GetInterfaces().Any(
        t => t.IsGenericType && !t.IsGenericTypeDefinition &&
          t.GetGenericTypeDefinition() == generic);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the generic interface type
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <param name="generic">Generic interface to check.</param>
    /// <returns>Closed type definition implementing the generic interface.</returns>
    // --------------------------------------------------------------------------------------------
    public static Type GetImplementorOfGenericInterface(this Type type, Type generic)
    {
      return type.GetInterfaces().First(
        t => t.IsGenericType && !t.IsGenericTypeDefinition &&
          t.GetGenericTypeDefinition() == generic);
    }
  }
}
