// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Maths;
using Optional.Collections;

namespace Moda.Core.Utility.Data;

/// <summary>
///     A directed graph implementation intended to represent complex dependencies between nodes.
///     It allows for multiple roots, and offers methods to iterate nodes in a prerequisite-first
///     order.
/// </summary>
/// <typeparam name="T">
///     The type of the node stored in the graph.
/// </typeparam>
public class PrerequisiteGraph<T>
    where T : notnull
{
    //##############################################################################################
    //
    //  Fields
    //
    //##############################################################################################
    
    /// <summary>
    ///     node -> prerequisites (parents)
    /// </summary>
    private readonly Dictionary<T, HashSet<T>> prerequisites = new();
    
    /// <summary>
    ///     node -> dependents (children)
    /// </summary>
    private readonly Dictionary<T, HashSet<T>> dependents = new();
    
    /// <summary>
    ///     nodes with no prerequisites
    /// </summary>
    private readonly HashSet<T> sourceNodes = new();


    //##############################################################################################
    //
    //  Public Methods
    //
    //##############################################################################################
    
    /// <summary>
    ///     Add a node to the graph, if it does already already exist.
    /// </summary>
    /// <param name="node">
    ///     The node to add.
    /// </param>
    /// <remarks>
    ///     Use <see cref="DeclarePrerequisite"/> to add a node and assign it as a prerequisite to an
    ///     existing node in one operation.
    /// </remarks>
    public void AddNode(T node)
    {
        if (!this.prerequisites.ContainsKey(node) && !this.dependents.ContainsKey(node))
        {
            this.sourceNodes.Add(node);
        }
    }
    
    /// <summary>
    ///     Remove a node from the graph.
    /// </summary>
    /// <param name="node">
    ///     The node to remove.
    /// </param>
    /// <remarks>
    ///     Nodes that list this node as a prerequisite will become source nodes, breaking up the
    ///     graph.  Nodes that were listed as prerequisites to this node will have it removed from
    ///     their dependents.
    /// </remarks>
    public void RemoveNode(T node)
    {
        if (this.prerequisites.ContainsKey(node))
        {
            foreach (T prereqOfNode in this.prerequisites[node])
            {
                RevokePrerequisite(prereq:prereqOfNode, from:node);
            }
        }

        if (this.dependents.ContainsKey(node))
        {
            foreach (T dependentOfNode in this.dependents[node])
            {
                RevokePrerequisite(prereq:node, from:dependentOfNode);
            }
        }
        
        this.sourceNodes.Remove(node);
    }


    /// <summary>
    ///     Assign the node <paramref name="prereq"/> to be a prerequisite of the node
    ///     <paramref name="of"/>.
    /// </summary>
    /// <param name="prereq">
    ///     The node that is to be a prerequisite of another node.
    /// </param>
    /// <param name="of">
    ///     The node that requires the other node as a prerequisite.
    /// </param>
    /// <remarks>
    ///     Prerequisites can be thought of as parent nodes in the tree, where as dependents are
    ///     children.  Prerequisites will be returned first when the graph is iterated.
    /// </remarks>
    /// <exception cref="CycleDetectedException">
    ///     Thrown if the operation results in a cyclical graph, at which point it will be reverted
    ///     to it's state before the method call.
    /// </exception>
    public void DeclarePrerequisite(T prereq, T of)
    {
        AddNode(prereq);
        
        this.prerequisites.GetOrAdd(of).Add(prereq);
        this.dependents.GetOrAdd(prereq).Add(of);
        
        this.sourceNodes.Remove(of);

        if (IsGraphCyclical(prereq))
        {
            RevokePrerequisite(prereq, of);
            throw new CycleDetectedException();
        }
    }


    /// <summary>
    ///     Remove the link between the prerequisite node and it's dependent, but leave both nodes
    ///     in the graph.
    /// </summary>
    /// <param name="prereq">
    ///     The node that is a prerequisite to <paramref name="from"/>.
    /// </param>
    /// <param name="from">
    ///     The node that is a dependent of <paramref name="prereq"/>.
    /// </param>
    /// <remarks>
    ///     If node <paramref name="from"/> has no more remaining prerequisites, it becomes a
    ///     source node.
    /// </remarks>
    public void RevokePrerequisite(T prereq, T from)
    {
        this.prerequisites.GetValueOrNone(from).MatchSome(prereqs =>
            {
                prereqs.Remove(prereq);
                if (!prereqs.Any())
                {
                    this.prerequisites.Remove(from);
                    this.sourceNodes.Add(from);
                }
            });
        this.dependents.GetValueOrNone(prereq).MatchSome(deps =>
            {
                deps.Remove(from);
                if (!deps.Any())
                {
                    this.dependents.Remove(prereq);
                }
            });
    }

    
    /// <summary>
    ///     Iterate through the graph, yielding prerequisites before their dependents.
    /// </summary>
    /// <return>
    ///     Each node in the graph, order determined by declared prerequisites.
    /// </return>
    /// <remarks>
    ///     Prerequisites are guaranteed to be returned before their dependents, but order of
    ///     unrelated nodes is not.
    /// </remarks>
    public IEnumerable<T> Iterate()
    {
        return IterateWithKahnSort(this.sourceNodes);
    }
    
    
    /// <summary>
    ///     Iterate through the graph beginning at the specified nodes, yielding prerequisites
    ///     before their dependents.
    /// </summary>
    /// <param name="startingNodes">
    ///     The nodes to start iterating from.
    /// </param>
    /// <return>
    ///     Each node in the graph, order determined by declared prerequisites.
    /// </return>
    /// <remarks>
    ///     Prerequisites are guaranteed to be returned before their dependents, but order of
    ///     unrelated nodes is not.
    ///
    ///     A subgraph is created from the starting nodes, so overlapping trees resulting from the
    ///     starting nodes and/or starting nodes that are decedents of others will be handled and
    ///     will not result in duplicate or unoptimized results.
    /// </remarks>
    public IEnumerable<T> IterateFrom(params T[] startingNodes)
    {
        return IterateFrom((IEnumerable<T>)startingNodes);
    }
    
    
    /// <inheritdoc cref="IterateFrom(T[])"/>
    public IEnumerable<T> IterateFrom(IEnumerable<T> startingNodes)
    {
        return SubGraphFrom(startingNodes).Iterate();
    }
    
    
    /// <summary>
    ///     Iterate the graph and apply the <paramref name="process"/> function to each item,
    ///     respecting the <see cref="GraphDirective"/> return value for control.
    /// </summary>
    /// <param name="process">
    ///     A function to execute on each item, the return value of which will control iteration
    ///     through the graph.
    /// </param>
    public void Process(Func<T,GraphDirective> process)
    {
        ProcessWithKahnSort(this.sourceNodes, process);
    }
    
    
    /// <summary>
    ///     Iterate the graph and apply the <paramref name="process"/> function to each item,
    ///     respecting the <see cref="GraphDirective"/> return value for control.
    /// </summary>
    /// <param name="startingNodes">
    ///     The nodes to start iterating from.
    /// </param>
    /// <param name="process">
    ///     A function to execute on each item, the return value of which will control iteration
    ///     through the graph.
    /// </param>
    public void ProcessFrom(IEnumerable<T> startingNodes, Func<T, GraphDirective> process)
    {
        SubGraphFrom(startingNodes).Process(process);
    }


    //##############################################################################################
    //
    //  Private Methods
    //
    //##############################################################################################
    
    private IEnumerable<T> IterateWithKahnSort(IEnumerable<T> startingNodes)
    {
        Stack<T> noPrereqs = new(startingNodes);
        Dictionary<T, Int32> prereqTally = new();
    
        while (noPrereqs.TryPop(out T? currentNode))
        {
            yield return currentNode;
            KahnSort(currentNode, noPrereqs, prereqTally);
        }
    }
    
    
    private void ProcessWithKahnSort(IEnumerable<T> startingNodes, Func<T, GraphDirective> process)
    {
        Stack<T> noPrereqs = new(startingNodes);
        Dictionary<T, HashSet<T>> prereqChecklist = new();
        Dictionary<T, HashSet<T>> ignoredPrereqs = new();


        void removeRequiredPrereq(T prereq, T from)
        {
            prereqChecklist.GetValueOrNone(from).Match(
                val => val.Remove(prereq),
                () => this.prerequisites.GetValueOrNone(from)
                    .MatchSome(p => prereqChecklist[from] =
                        new(p.Except(new [] { prereq }))));
        }
        
        void addIgnoredPrereq(T toIgnore, T by)
        {
            ignoredPrereqs.GetOrAdd(by).Add(toIgnore);
        }
        

        while (noPrereqs.TryPop(out T? currentNode))
        {
            if (process(currentNode) == GraphDirective.DepthStop)
            {
                // #error does marking as a depth stop mean it has no dependents anymore? remove them before next step below? 
                // #error rather than requiredPrereqs, do satisfiedPrereqs, then below check if existing/remaining prereqs are all satisfied?
                // #error will this prevent orphaned nodes from being iterated?
                Stack<T> lookAhead = new();
                lookAhead.Push(currentNode);
                while (lookAhead.TryPop(out T? lookAheadNode))
                {
                    if (this.dependents.TryGetValue(lookAheadNode, out HashSet<T>? lookAheadDeps))
                    {
                        foreach (T lookAheadDependent in lookAheadDeps)
                        {
                            addIgnoredPrereq(toIgnore:lookAheadNode, by:lookAheadDependent);
                            if (this.prerequisites.TryGetValue(lookAheadDependent,
                                out HashSet<T>? lookBackPrereqs))
                            {
                                if (lookBackPrereqs.IsSubsetOf(ignoredPrereqs[lookAheadDependent]))
                                {
                                    // this node can be completely ignored because all if it's prerequisites are ignored
                                    lookAhead.Push(lookAheadDependent);
                                }
                                else if (prereqChecklist.TryGetValue(lookAheadDependent, out HashSet<T>? checklist))
                                {
                                    if (!checklist
                                        .Except(ignoredPrereqs.GetValueOrNone(lookAheadDependent)
                                            .Map(a => (IEnumerable<T>)a)
                                            .ValueOr(Enumerable.Empty<T>()))
                                        .Any())
                                    {
                                        // this node's remaining prerequisites were satisfied by the depth stop,
                                        // so they are ready to process
                                        noPrereqs.Push(lookAheadDependent);
                                        prereqChecklist.Remove(lookAheadDependent);
                                    }
                                }
                            }
                        }
                    }
                }
                continue;
            }
            
            
            if (this.dependents.TryGetValue(currentNode, out HashSet<T>? deps))
            {
                T currentNodeCopy = currentNode;
                foreach (T dependent in deps)
                {
                    // requiredPrereqs.GetValueOrNone(dependent).Match(
                    //         val => val.Remove(currentNodeCopy),
                    //         () => this.prerequisites.GetValueOrNone(dependent)
                    //             .MatchSome(p => requiredPrereqs[dependent] =
                    //                 new(p.Except(new [] { currentNodeCopy }))));
                    removeRequiredPrereq(prereq:currentNodeCopy, from:dependent);

                    if (!prereqChecklist[dependent]
                        .Except(ignoredPrereqs.GetValueOrNone(dependent)
                            .Map(a =>(IEnumerable<T>)a).ValueOr(Enumerable .Empty<T>()))
                        .Any())
                    {
                        // node is ready
                        noPrereqs.Push(dependent);
                        // clean up
                        prereqChecklist.Remove(dependent);
                    }
                }
            }
        }
    }
    
    
    private void KahnSort(T currentNode,
        Stack<T> noPrereqs,
        Dictionary<T, Int32> prereqTally)
    {
        if (this.dependents.TryGetValue(currentNode, out HashSet<T>? deps))
        {
            foreach (T dependent in deps)
            {
                Int32 numberOfPrereqs = prereqTally.GetValueOrNone(dependent).Match
                (
                    val => prereqTally[dependent] = val.Decrement(),
                    () => prereqTally[dependent] = GetPrereqTally(dependent).Decrement()
                );
    
                if (numberOfPrereqs == 0)
                {
                    prereqTally.Remove(dependent);
                    noPrereqs.Push(dependent);  //
                }
            }
        }
    }
    
    private int GetPrereqTally(T dependent)
    {
        return this.prerequisites.GetValueOrNone(dependent)
            .Map(a => a.Count)
            .ValueOr(0);
    }




    
    
    
    private Boolean IsGraphCyclical(T startingNode)
    {
        Stack<T> depthStack = new();
        HashSet<T> visitedGrey = new();
        HashSet<T> exploredBlack = new();
        
        depthStack.Push(startingNode);
        while (depthStack.TryPeek(out T? currentNode))
        {
            if (!visitedGrey.Contains(currentNode))
            {
                visitedGrey.Add(currentNode);
                if (this.dependents.TryGetValue(currentNode, out HashSet<T>? deps))
                {
                    foreach (T dependent in deps)
                    {
                        if (!visitedGrey.Contains(dependent))
                        {
                            if (!exploredBlack.Contains(dependent))
                            {
                                depthStack.Push(dependent);
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                depthStack.Pop();
                visitedGrey.Remove(currentNode);
                exploredBlack.Add(currentNode);
            }
        }

        return false;
    }
    
    
    private PrerequisiteGraph<T> SubGraphFrom(IEnumerable<T> potentialSourceNodes)
    {
        IEnumerable<T> sources = FindTrueSources(potentialSourceNodes);
        
        // removed to always provide a copy
        // if (this.sourceNodes.SetEquals(sources))
        // {
        //     return this;
        // }
        
        PrerequisiteGraph<T> subGraph = new();
        
        foreach (T sourceNode in sources)
        {
            Stack<T> depthStack = new();
            depthStack.Push(sourceNode);
            while (depthStack.TryPop(out T? currentNode))
            {
                subGraph.AddNode(currentNode);
                if (this.dependents.TryGetValue(currentNode, out HashSet<T>? deps))
                {
                    foreach (T dependent in deps)
                    {
                        subGraph.DeclarePrerequisite(prereq:currentNode, of:dependent);
                        depthStack.Push(dependent);
                    }
                }
            }
        }
        
        return subGraph;
    }
    
    
    private IEnumerable<T> FindTrueSources(IEnumerable<T> potentialSources)
    {
        // TODO: optimization: if sourcenodes.IsSubsetOf(potentialSourceNodes) then just choose source nodes without having to find true sources
        
        HashSet<T> sources = new(potentialSources);
        HashSet<T> discoveredSources = new();
        
        foreach (T potentialSource in potentialSources)
        {
            Stack<T> depthStack = new();
            depthStack.Push(potentialSource);
            while (depthStack.TryPop(out T? currentNode))
            {
                if (potentialSources.Contains(currentNode))
                {
                    if (discoveredSources.Contains(currentNode))
                    {
                        sources.Remove(currentNode);
                    }

                    discoveredSources.Add(currentNode);
                }
                
                this.dependents.GetValueOrNone(currentNode).MatchSome(a => depthStack.PushRange(a));
            }
        }

        return sources;
    }


    
}
