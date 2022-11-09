// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Collections;
using Optional;

namespace Moda.Core.Utility.Data;

/// <summary>
///     Provides a base implementation of a tree node (parent, children relationship).
/// </summary>
/// <typeparam name="T">
///     The derived type.
/// </typeparam>
public abstract class TreeNode<T> : IEnumerable<T>
    where T : TreeNode<T>
{
    //##############################################################################################
    //
    //  Public Properties
    //
    //##############################################################################################
    
    private Option<T> _parent;
    /// <summary>
    ///     The parent of this node, or Option.None{T} if this node is not a child of any other
    ///     node.
    /// </summary>
    public Option<T> Parent
    {
        get => this._parent;
        private set
        {
            if (this._parent != value)
            {
                Option<T> oldValue = this._parent;
                this._parent = value;
                this.ParentChanged?.Invoke(this, new(oldValue, value));
            }
        }
    }
    /// <summary>
    ///     Fired when the value of <see cref="Parent"/> has changed.
    /// </summary>
    public event ValueChangedHandler<Option<T>>? ParentChanged;
    
    
    private readonly List<T> _children = new();
    /// <summary>
    ///     The children belonging to this node.
    /// </summary>
    public IEnumerable<T> Children => this._children;
    /// <summary>
    ///     Fired when children are added to or removed from <see cref="Children"/>.
    /// </summary>
    public event CollectionChangedHandler<T>? ChildrenChanged;

    

    //##############################################################################################
    //
    //  Public Methods
    //
    //##############################################################################################

    /// <summary>
    ///     Assign the specified node as a child to this node, appending it to the end this node's
    ///     <see cref="Children"/>.
    /// </summary>
    /// <param name="child">
    ///     The node to assign as a child to this node.
    /// </param>
    /// <exception cref="ChildAlreadyExistsException">
    ///     Thrown when this node already contains the specified node as a child.
    /// </exception>
    /// <remarks>
    ///     The node's <see cref="Parent"/> property will be updated to be this node.
    /// </remarks>
    public void AppendChild(T child)
    {
        if (this._children.Contains(child))
        {
            throw new ChildAlreadyExistsException();
        }
        
        child.Parent.MatchSome(oldParent => oldParent.RemoveFromChildren(child));
        child.Parent = ((T)this).Some();
        this._children.Add(child);
        OnChildAdded(child);
        this.ChildrenChanged?.Invoke(this, new(new[] { child }, Enumerable.Empty<T>()));
    }


    /// <summary>
    ///     Remove the specified node from this nodes <see cref="Children"/> list.
    /// </summary>
    /// <param name="child">
    ///     The child node to remove.
    /// </param>
    /// <exception cref="ChildDoesNotExistException">
    ///     Thrown if the node is not a child of this node.
    /// </exception>
    /// <remarks>
    ///     The node's <see cref="Parent"/> property will be set to <see cref="Option.None{T}"/>
    /// </remarks>
    public void RemoveChild(T child)
    {
        RemoveFromChildren(child);
        child.Parent = Option.None<T>();
    }


    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        Queue<T> queue = new();
        queue.Enqueue((T)this);
        while(queue.Count > 0)
        {
            T currentNode = queue.Dequeue();
            foreach (T child in currentNode.Children)
            {
                queue.Enqueue(child);
            }

            yield return currentNode;
        }
    }


    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    //##############################################################################################
    //
    //  Private Methods
    //
    //##############################################################################################
    
    /// <summary>
    ///     Called after a child is added, but before <see cref="ChildrenChanged"/> is fired.
    /// </summary>
    /// <param name="child">
    ///     The child that was added.
    /// </param>
    protected virtual void OnChildAdded(T child)
    {
        
    }
    
    /// <summary>
    ///     Called after a child is removed, but before <see cref="ChildrenChanged"/> is fired.
    /// </summary>
    /// <param name="child">
    ///     The child that was added.
    /// </param>
    protected virtual void OnChildRemoved(T child)
    {
        
    }
    

    private void RemoveFromChildren(T child)
    {
        if (this._children.Remove(child))
        {
            OnChildRemoved(child);
            this.ChildrenChanged?.Invoke(this, new(Enumerable.Empty<T>(), new[] { child }));
        }
        else
        {
            throw new ChildDoesNotExistException();
        }
    }

    
}
