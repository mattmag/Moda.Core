// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.Entity;

/// <summary>
///     Acts as the central hub of the ECS relationship by housing entities, components, and
///     systems.
/// </summary>
public interface IEntityManager : IComponentDepository, IComponentProvider
{
    /// <summary>
    ///     Register a system with this <see cref="IEntityManager"/> in order to start acting on
    ///     relevant entities.
    /// </summary>
    /// <param name="system">
    ///     The system to register.
    /// </param>
    /// <exception cref="InvalidSystemException">
    ///     Thrown if the system's <see cref="IComponentSystem.ActsOn"/> collection is null or
    ///     empty.
    /// </exception>
    /// <exception cref="SystemAlreadyRegisteredException">
    ///     Thrown if the instance of the system is already registered. 
    /// </exception>
    /// <remarks>
    ///     Upon registration, any entities composed of a superset of the system's
    ///     <see cref="IComponentSystem.ActsOn"/> collection will be fed to the system
    ///     via <see cref="IComponentSystem.RegisterEntity(UInt64)"/>.
    ///
    ///     Once registered, the system will receive calls to
    ///     <see cref="IComponentSystem.RegisterEntity(UInt64)"/> as entities with a relevant
    ///     composition are added, or when components are added to existing entities which now make
    ///     them relevant.
    ///
    ///     Likewise, as entities are removed, or their composition is altered such that they are no
    ///     longer relevant to the system, a call will be made to
    ///     <see cref="IComponentSystem.UnregisterEntity(UInt64)"/>.
    /// </remarks>
    void RegisterSystem(IComponentSystem system);


    /// <summary>
    ///     Remove this system from the <see cref="IEntityManager"/>, preventing it from receiving
    ///     any further entity registration updates.
    /// </summary>
    /// <param name="system">
    ///     The system to unregister.
    /// </param>
    /// <exception cref="SystemNotFoundException">
    ///     Thrown when the system was not registered before calling this method.
    /// </exception>
    void UnregisterSystem(IComponentSystem system);
}
