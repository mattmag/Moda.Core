// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using Moda.Core.Entity;

namespace Moda.Core.Tests.Entity.Tests.Support;

/// <summary>
///     Used to declare the side-effect a given action is expected to impart on an
///     <see cref="IComponentSystem"/>.
/// </summary>
public enum RegistrationBehavior
{
    /// <summary>
    ///     Indicates that neither <see cref="IComponentSystem.RegisterEntity(UInt64)"/> nor
    ///     <see cref="IComponentSystem.UnregisterEntity(UInt64)"/> is expected to be called
    ///     on the system.
    /// </summary>
    NoChange,

    /// <summary>
    ///     <see cref="IComponentSystem.RegisterEntity(UInt64)"/> is expected to be called
    ///     on the system.
    /// </summary>
    RegisterEntity,

    /// <summary>
    ///     <see cref="IComponentSystem.UnregisterEntity(UInt64)"/> is expected to be called
    ///     on the system.
    /// </summary>
    UnregisterEntity
}