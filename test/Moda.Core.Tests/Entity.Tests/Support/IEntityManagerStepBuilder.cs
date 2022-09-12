// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;

namespace Moda.Core.Entity.Tests.Support;

/// <summary>
///     Provides methods to construct an <see cref="EntityManagerStep"/>.
/// </summary>
public interface IEntityManagerStepBuilder
{
    /// <summary>
    ///     Mark that the step should have the affect of registering the active entity with
    ///     systems that act on the specified types.
    /// </summary>
    /// <param name="actsOn">
    ///     The types that these imaginary systems act on.
    /// </param>
    /// <returns>
    ///     The <see cref="IEntityManagerSequenceBuilder"/>, in order to continue building
    ///     the sequence.
    /// </returns>
    IEntityManagerStepBuilder ShouldRegisterWithSystemThatActOn(params Type[] actsOn);

    /// <summary>
    ///     Mark that the step should have no affect on systems that act on the specified types.
    /// </summary>
    /// <param name="actsOn">
    ///     The types that these imaginary systems act on.
    /// </param>
    /// <returns>
    ///     The <see cref="IEntityManagerSequenceBuilder"/>, in order to continue building
    ///     the sequence.
    /// </returns>
    IEntityManagerStepBuilder ShouldNotAffectSystemsThatActOn(params Type[] actsOn);

    /// <summary>
    ///     Mark that the step should have the affect of unregistering the active entity with
    ///     systems that act on the specified types.
    /// </summary>
    /// <param name="actsOn">
    ///     The types that these imaginary systems act on.
    /// </param>
    /// <returns>
    ///     The <see cref="IEntityManagerSequenceBuilder"/>, in order to continue building
    ///     the sequence.
    /// </returns>
    IEntityManagerStepBuilder ShouldUnregisterWithSystemsThatActOn(params Type[] actsOn);

    
    /// <summary>
    ///     End the builder pattern by testing the entire <see cref="EntityManagerSequence"/>.
    /// </summary>
    void Test();

    /// <summary>
    ///     Finish defining this step and being defining a new one.
    /// </summary>
    /// <returns>
    ///     The <see cref="IEntityManagerSequenceBuilder"/>, in order to continue building
    ///     the sequence.
    /// </returns>
    IEntityManagerSequenceBuilder Then();
}
