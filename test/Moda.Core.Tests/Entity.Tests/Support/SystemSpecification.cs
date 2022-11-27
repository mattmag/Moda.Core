// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections.Immutable;

namespace Moda.Core.Tests.Entity.Tests.Support;

/// <summary>
///     Declares how a system (defined by the components that it <see cref="ActsOn"/> should be
///     affected by a <see cref="EntityManagerStep"/>.
/// </summary>
public class SystemSpecification
{
    /// <summary>
    ///     Creates a new <see cref="SystemSpecification"/> instance.
    /// </summary>
    /// <param name="expectedRegistration">
    ///     The expected entity registration behavior
    /// </param>
    /// <param name="actsOn">
    ///     The components that the system acts on.
    /// </param>
    public SystemSpecification(RegistrationBehavior expectedRegistration, params Type[] actsOn)
    {
        this.ExpectedRegistration = expectedRegistration;
        this.ActsOn = ImmutableHashSet.Create(actsOn);
    }

    /// <summary>
    ///     The expected entity registration behavior
    /// </summary>
    public RegistrationBehavior ExpectedRegistration { get; set; }

    /// <summary>
    ///     The components that the system acts on.
    /// </summary>
    public ImmutableHashSet<Type> ActsOn { get; set; }
}