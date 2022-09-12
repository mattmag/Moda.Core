// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Collections.Immutable;
using Moda.Core.Utility.Data;
using Optional;
using Optional.Collections;

namespace Moda.Core.Entity;

/// <summary>
///     A collection of entities, components, and systems, and the main entry point for creating
///     and modifying entities.
/// </summary>
public class EntityManager : IEntityManager
{
    //##############################################################################################
    //
    //  Fields
    //
    //##############################################################################################
    
    private readonly HashSet<IComponentSystem> registeredSystems = new();
    
    private UInt64 lastEntityID;
    
    private readonly Dictionary<Type, Dictionary<UInt64, Object>> entityComponents = new();

    private readonly Dictionary<UInt64, HashSet<Type>> entityComposition = new();


    //##############################################################################################
    //
    //  Public Methods
    //
    //##############################################################################################


    /// <inheritdoc/>
    public UInt64 AddEntity(params Object[] components)
    {
        return AddEntity((IEnumerable<Object>)components);
    }


    /// <inheritdoc/>
    public UInt64 AddEntity(IEnumerable<Object> components)
    {
        if (!components.Any())
        {
            throw new EmptyEntityException();
        }
        
        ValidateNewComponents(components, Enumerable.Empty<Type>());
        
        UInt64 entityID = ++this.lastEntityID;
        CommitNewComponents(entityID, components);
        
        return entityID;
    }


    /// <inheritdoc/>
    public void RemoveEntity(UInt64 entityID)
    {
        if (!EntityExists(entityID))
        {
            throw new EntityNotFoundException(entityID);
        }
        
        // ToArray to provide a copy
        RemoveComponents(entityID, this.entityComposition[entityID].ToArray());
    }


    /// <inheritdoc/>
    public void AddComponents(UInt64 entityID, params Object[] components)
    {
        AddComponents(entityID, (IEnumerable<Object>)components);
    }


    /// <inheritdoc/>
    public void AddComponents(UInt64 entityID, IEnumerable<Object> components)
    {
        if (!EntityExists(entityID))
        {
            throw new EntityNotFoundException(entityID);
        }

        ValidateNewComponents(components, this.entityComposition[entityID]);
        CommitNewComponents(entityID, components);
    }


    /// <inheritdoc/>
    public void RemoveComponents(UInt64 entityID, params Type[] types)
    {
        RemoveComponents(entityID, (IEnumerable<Type>)types);
    }


    /// <inheritdoc/>
    public void RemoveComponents(UInt64 entityID, IEnumerable<Type> types)
    {
        if (!EntityExists(entityID))
        {
            throw new EntityNotFoundException(entityID);
        }
        
        ImmutableHashSet<Type> oldComp = this.entityComposition[entityID].ToImmutableHashSet();
        
        foreach (Type type in types)
        {
            if (!this.entityComposition[entityID].Contains(type))
            {
                throw new ComponentNotFoundException(entityID, type);
            }
        }
        
        ImmutableHashSet<Type> newComp = oldComp.Except(types);
        foreach (IComponentSystem sys in this.registeredSystems)
        {
            if (!sys.ActsOn.IsSubsetOf(newComp) && sys.ActsOn.IsSubsetOf(oldComp))
            {
                sys.UnregisterEntity(entityID);
            }
        }

        foreach (Type type in types)
        {
            this.entityComposition[entityID].Remove(type);
            this.entityComponents[type].Remove(entityID);
        }

        if (!this.entityComposition[entityID].Any())
        {
            this.entityComposition.Remove(entityID);
        }
    }


    /// <inheritdoc/>
    public T GetComponent<T>(UInt64 entityID)
    {
        if (!EntityExists(entityID))
        {
            throw new EntityNotFoundException(entityID);
        }

        try
        {
            return (T)this.entityComponents[typeof(T)][entityID];
        }
        catch (KeyNotFoundException e)
        {
            throw new ComponentNotFoundException(entityID, typeof(T), e);
        }
    }


    /// <inheritdoc/>
    public Option<T> GetComponentOrNone<T>(UInt64 entityID)
    {
        if (!EntityExists(entityID))
        {
            throw new EntityNotFoundException(entityID);
        }

        return this.entityComponents
            .GetValueOrNone(typeof(T))
            .Match(a => a.GetValueOrNone(entityID).Map(b => (T)b), Option.None<T>);
    }


    /// <inheritdoc/>
    public ComponentCollection GetComponents(UInt64 entityID, params Type[] types)
    {
        return GetComponents(entityID, (IEnumerable<Type>)types);
    }


    /// <inheritdoc/>
    public ComponentCollection GetComponents(UInt64 entityID, IEnumerable<Type> types)
    {
        if (!EntityExists(entityID))
        {
            throw new EntityNotFoundException(entityID);
        }

        Dictionary<Type, Object> temp = new Dictionary<Type, Object>();
        foreach (Type type in types)
        {
            try
            {
                temp.Add(type, this.entityComponents[type][entityID]);
            }
            catch (KeyNotFoundException e)
            {
                throw new ComponentNotFoundException(entityID, type, e);
            }
        }
        return new ComponentCollection(temp);
    }


    /// <inheritdoc/>
    public ComponentCollection GetAllComponents(UInt64 entityID)
    {
        if (!EntityExists(entityID))
        {
            throw new EntityNotFoundException(entityID);
        }

        Dictionary<Type, Object> rval = new Dictionary<Type, Object>();
        foreach (Type type in this.entityComposition[entityID])
        {
            rval.Add(type, this.entityComponents[type][entityID]);
        }
        return new ComponentCollection(rval);
    }


    /// <inheritdoc/>
    public ImmutableHashSet<Type> GetComposition(UInt64 entityID)
    {
        if (!EntityExists(entityID))
        {
            throw new EntityNotFoundException(entityID);
        }
        
        return this.entityComposition[entityID].ToImmutableHashSet();
    }


    /// <inheritdoc/>
    public void RegisterSystem(IComponentSystem system)
    {
        if (system.ActsOn is null || !system.ActsOn.Any())
        {
            throw new InvalidSystemException($"ActsOn is empty for {system.GetType()}");
        }
        
        if (!this.registeredSystems.Add(system))
        {
            throw new SystemAlreadyRegisteredException(system.GetType());
        }
        
        // Start with entities that have at least one relevant component in hopes that it's
        // more efficient than looping through every entity.
        foreach (UInt64 entityID in this.entityComponents.GetValueOrNone(system.ActsOn.First())
            .Map(a => (IEnumerable<UInt64>)a.Keys)
            .ValueOr(Enumerable.Empty<UInt64>()))
        {
            if (system.ActsOn.IsSubsetOf(this.entityComposition[entityID]))
            {
                system.RegisterEntity(entityID);
            }
        }
    }


    /// <inheritdoc/>
    public void UnregisterSystem(IComponentSystem system)
    {
        if (!this.registeredSystems.Remove(system))
        {
            throw new SystemNotFoundException(system.GetType());
        }
    }

    

    //##############################################################################################
    //
    //  Private Methods
    //
    //##############################################################################################
    
    private Boolean EntityExists(UInt64 entityID)
    {
        return this.entityComposition.ContainsKey(entityID);
    }
    
    private void ValidateNewComponents(IEnumerable<Object> components,
        IEnumerable<Type> existingComposition)
    {
        HashSet<Type> stagedComposition = new(existingComposition);
        
        foreach (Object component in components)
        {
            Type type = component.GetType();
            if (!stagedComposition.Add(type))
            {
                throw new DuplicateComponentException(type);
            }
        }
    }


    private IEnumerable<IComponentSystem> GetNewlyInterestedSystems(
        IEnumerable<Type> newComponentTypes, IEnumerable<Type> existingComposition)
    {
        List<IComponentSystem> interestedSystems = new();
        HashSet<Type> stagedComposition = new(existingComposition.Concat(newComponentTypes));
        foreach (IComponentSystem system in this.registeredSystems)
        {
            if (system.ActsOn.IsSubsetOf(stagedComposition) &&
                !system.ActsOn.IsSubsetOf(existingComposition))
            {
                interestedSystems.Add(system);
            }
        }

        return interestedSystems;
    }
    
    private void CommitNewComponents(UInt64 entityID, IEnumerable<Object> newComponents)
    {
        // make a copy of the existing composition as we are about to alter it
        IEnumerable<Type> oldComposition = this.entityComposition.GetValueOrNone(entityID)
            .Match(a => a.ToArray(), () => new Type[] { });

        foreach (Object component in newComponents)
        {
            Type type = component.GetType();
            this.entityComponents.GetOrAdd(type)[entityID] = component;
            this.entityComposition.GetOrAdd(entityID).Add(type);
        }

        IEnumerable<IComponentSystem> systems = GetNewlyInterestedSystems(
            newComponents.Select(a => a.GetType()), oldComposition);
        foreach (IComponentSystem sys in systems)
        {
            sys.RegisterEntity(entityID);
        }
    }

}
