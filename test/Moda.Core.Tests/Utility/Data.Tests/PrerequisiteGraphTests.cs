// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Moda.Core.Utility.Data.Tests;

public class PrerequisiteGraphTests
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private class Node
    {
        public Node(String name)
        {
            this.Name = name;
        }
        
        public String Name { get; }
        
    }


    // AddNode() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void AddNodeShouldAddNodeToGraph()
    {
        PrerequisiteGraph<Node> graph = new();
        Node node = new("Node A");
        graph.AddNode(node);
        graph.Iterate().Should().BeEquivalentTo(new[] { node });
    }
    
    [Test]
    public void AddNodeShouldNotAddNodeToGraphIfItAlreadyExists()
    {
        PrerequisiteGraph<Node> graph = new();
        Node node = new("Node A");
        graph.AddNode(node);
        graph.AddNode(node);
        graph.Iterate().Should().BeEquivalentTo(new[] { node });
    }
    
    [Test]
    public void MultipleCallsToAddNodeShouldAddEachNodeToGraph()
    {
        PrerequisiteGraph<Node> graph = new();
        Node nodeA = new("Node A");
        graph.AddNode(nodeA);
        Node nodeB = new("Node B");
        graph.AddNode(nodeB);
        Node nodeC = new("Node C");
        graph.AddNode(nodeC);
        graph.Iterate().Should().BeEquivalentTo(new[] { nodeA, nodeB, nodeC });
    }


    // RemoveNode() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void RemoveNodeShouldRemoveFromGraph()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        graph.AddNode(nodeA);
        Node nodeB = new("Node B");
        graph.AddNode(nodeB);
        Node nodeC = new("Node C");
        graph.AddNode(nodeC);
        
        graph.RemoveNode(nodeB);
        
        graph.Iterate().Should().BeEquivalentTo(new[] { nodeA, nodeC });
    }
    
    [Test]
    public void RemoveNodeShouldRemoveIfPrerequisite()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        graph.AddNode(nodeA);
        Node nodeB = new("Node B");
        graph.AddNode(nodeB);
        Node nodeC = new("Node C");
        graph.AddNode(nodeC);
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB);
        graph.DeclarePrerequisite(prereq:nodeB, of:nodeC);
        
        graph.RemoveNode(nodeB);
        
        graph.Iterate().Should().BeEquivalentTo(new[] { nodeA, nodeC });
    }
    
    [Test]
    public void RemoveNodeShouldFailSilentlyIfNodeIsNotFound()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        graph.AddNode(nodeA);
        Node nodeB = new("Node B");
        graph.AddNode(nodeB);
        
        Node nodeC = new("Node C");
        graph.RemoveNode(nodeC);
        
        graph.Iterate().Should().BeEquivalentTo(new[] { nodeA, nodeB });
    }

    [Test]
    public void RemoveNodeShouldBreakTree()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        graph.AddNode(nodeA);
        Node nodeB = new("Node B");
        graph.AddNode(nodeB);
        Node nodeC = new("Node C");
        graph.AddNode(nodeC);
        Node nodeD1 = new("Node D1");
        graph.AddNode(nodeD1);
        Node nodeD2 = new("Node D2");
        graph.AddNode(nodeD2);
        Node nodeE = new("Node E");
        graph.AddNode(nodeE);
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB);
        graph.DeclarePrerequisite(prereq:nodeB, of:nodeC);
        graph.DeclarePrerequisite(prereq:nodeC, of:nodeD1);
        graph.DeclarePrerequisite(prereq:nodeB, of:nodeD2);
        graph.DeclarePrerequisite(prereq:nodeC, of:nodeD2);
        graph.DeclarePrerequisite(prereq:nodeD1, of:nodeE);
        
        graph.RemoveNode(nodeC);

        graph.IterateFrom(nodeA).Should()
            .BeEquivalentTo(new[] { nodeA, nodeB, nodeD2 }, a => a.WithStrictOrdering());
        graph.IterateFrom(nodeD1).Should()
            .BeEquivalentTo(new[] { nodeD1, nodeE }, a => a.WithStrictOrdering());
    }

    // DeclarePrerequisite() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void DeclarePrerequisiteShouldDeclarePrerequisiteToNodes()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        graph.AddNode(nodeA);
        
        Node nodeB = new("Node B");
        graph.DeclarePrerequisite(prereq:nodeB, of:nodeA);
        graph.Iterate().Should().BeEquivalentTo(new[] { nodeA, nodeB });
    }
    
    [Test]
    public void DeclarePrerequisiteShouldAddNodeToNodes()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        Node nodeB = new("Node B");
        
        graph.DeclarePrerequisite(prereq:nodeB, of:nodeA);
        
        graph.Iterate().Should().BeEquivalentTo(new[] { nodeA, nodeB });
    }
    
    [Test]
    public void MultipleCallsToDeclarePrerequisiteShouldAddNodeToNodeOnce()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        Node nodeB = new("Node B");
        Node nodeC = new("Node C");
        
        graph.DeclarePrerequisite(prereq:nodeB, of:nodeA);
        graph.DeclarePrerequisite(prereq:nodeC, of:nodeA);
        
        graph.Iterate().Should().BeEquivalentTo(new[] { nodeA, nodeB, nodeC });
    }
    
    [Test]
    public void DeclarePrerequisiteShouldThrowExceptionWhenCycleIsDetected()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        Node nodeB = new("Node B");
        
        graph.DeclarePrerequisite(prereq:nodeB, of:nodeA);

        graph.Invoking(g => g.DeclarePrerequisite(prereq: nodeA, of: nodeB))
            .Should().Throw<CycleDetectedException>();
    }
    
    [Test]
    public void DeclarePrerequisiteShouldThrowExceptionWhenCycleIsDetectedInComplexGraph()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node step1 = new("Step 1");
        Node step2 = new("Step 2");
        Node step3a = new("Step 3a");
        Node step3b = new("Step 3b");
        Node step4 = new("Step 4");
        Node step5a = new("Step 5a");
        Node step5b = new("Step 5b");
        
        graph.DeclarePrerequisite(prereq:step1, of:step2);
        graph.DeclarePrerequisite(prereq:step2, of:step3a);
        graph.DeclarePrerequisite(prereq:step2, of:step3b);
        graph.DeclarePrerequisite(prereq:step3b, of:step4);
        graph.DeclarePrerequisite(prereq:step4, of:step5a);
        graph.DeclarePrerequisite(prereq:step3a, of:step5a);
        graph.DeclarePrerequisite(prereq:step4, of:step5b);

        graph.Invoking(g => g.DeclarePrerequisite(prereq:step5b, of:step2))
            .Should().Throw<CycleDetectedException>();
    }


    [Test]
    [Timeout(500)] // failure could result in infinite loop 
    public void DeclarePrerequisiteShouldNotKeepEdgeIfResultingGraphIsCyclical()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        graph.AddNode(nodeA);
        Node nodeB = new("Node B");
        graph.AddNode(nodeB);
        Node nodeC = new("Node C");
        graph.AddNode(nodeC);
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB);
        graph.DeclarePrerequisite(prereq:nodeB, of:nodeC);
        
        graph.Invoking(g => g.DeclarePrerequisite(prereq:nodeC, of:nodeA))
            .Should().Throw<CycleDetectedException>();

        graph.IterateFrom(nodeA).Should().BeEquivalentTo(new [] { nodeA, nodeB, nodeC });
    }


    // Iterate() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void IterateShouldReturnPrerequisitesFirst()
    {
        PrerequisiteGraph<Node> graph = new();

        Node step1 = new("Step 1");
        Node step2 = new("Step 2");
        Node step3 = new("Step 3");

        graph.DeclarePrerequisite(prereq:step1, of:step2);
        graph.DeclarePrerequisite(prereq:step2, of:step3);
        
        graph.Iterate().Should().BeEquivalentTo(new[] { step1, step2, step3 },
            a => a.WithStrictOrdering());
    }
    
    
    [Test]
    public void IterateShouldReturnPrerequisitesFirstInMultiRootGraph()
    {
        PrerequisiteGraph<Node> graph = new();

        Node procAStep1 = new("Process A - Step 1");
        Node procAStep2 = new("Process A - Step 2");
        Node procAStep3 = new("Process A - Step 3");
        
        graph.DeclarePrerequisite(prereq:procAStep1, of:procAStep2);
        graph.DeclarePrerequisite(prereq:procAStep2, of:procAStep3);
        
        Node procBStep1 = new("Process B - Step 1");
        Node procBStep2 = new("Process B - Step 2");
        Node procBStep3 = new("Process B - Step 3");

        graph.DeclarePrerequisite(prereq:procBStep1, of:procBStep2);
        graph.DeclarePrerequisite(prereq:procBStep2, of:procBStep3);


        IEnumerable<Node> result = graph.Iterate().ToList();
        result.Should().ContainInOrder(procAStep1, procAStep2, procAStep3);
        result.Should().ContainInOrder(procBStep1, procBStep2, procBStep3);
    }


    [Test]
    public void IterateShouldReturnAllPrerequisitesFirst()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node step1a = new("Step 1a");
        Node step1b = new("Step 1b");
        Node step1c = new("Step 1c");
        Node step2 = new("Step 2");
        Node step3 = new("Step 3");
        
        graph.DeclarePrerequisite(prereq:step1a, of:step2);
        graph.DeclarePrerequisite(prereq:step1b, of:step2);
        graph.DeclarePrerequisite(prereq:step1c, of:step2);
        graph.DeclarePrerequisite(prereq:step2, of:step3);
        
        IEnumerable<Node> result = graph.Iterate().ToList();
        result.Take(3).Should().BeEquivalentTo(new[] { step1a, step1b, step1c });
        result.Skip(3).Should().BeEquivalentTo(new [] { step2, step3 }, a => a.WithStrictOrdering());
    }


    [Test]
    public void IterateShouldReturnPrerequisitesFirstWithComplexDependencies()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node step1a = new("Step 1a");
        Node step1b = new("Step 1b");
        Node step2 = new("Step 2");
        Node step3 = new("Step 3");
        
        graph.DeclarePrerequisite(prereq:step1a, of:step2);
        graph.DeclarePrerequisite(prereq:step1b, of:step3);
        graph.DeclarePrerequisite(prereq:step2, of:step3);
        
        IEnumerable<Node> result = graph.Iterate().ToList();
        result.Should().ContainInOrder(step1a, step2, step3);
        result.Should().ContainInOrder(step1b, step3);
    }
    
    
    [Test]
    public void IterateShouldReturnPrerequisitesFirstWithComplexDependencies2()
    {
        PrerequisiteGraph<Node> graph = new();

        Node stepRoot = new("Root");
        Node step1a = new("Step 1a");
        Node step1b = new("Step 1b");
        Node step2a = new("Step 2a");
        Node step2b = new("Step 2b");
        Node step3 = new("Step 3");
        
        graph.DeclarePrerequisite(prereq:stepRoot, of:step1a);
        graph.DeclarePrerequisite(prereq:step1a, of:step2a);
        graph.DeclarePrerequisite(prereq:step1b, of:step2b);
        graph.DeclarePrerequisite(prereq:step2a, of:step3);
        graph.DeclarePrerequisite(prereq:step2b, of:step3);
        graph.DeclarePrerequisite(prereq:stepRoot, of:step3);
        
        IEnumerable<Node> result = graph.Iterate().ToList();
        result.Should().ContainInOrder(stepRoot, step1a, step2a, step3);
        result.Should().ContainInOrder(step1b, step2b, step3);
    }


    // RevokePrerequisite() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void RevokePrerequisiteThenDeclareShouldReflectInIterate()
    {
        PrerequisiteGraph<Node> graph = new();

        Node step1 = new("Step 1");
        Node step2 = new("Step 2");
        Node step3 = new("Step 3");

        graph.DeclarePrerequisite(prereq:step1, of:step2);
        graph.DeclarePrerequisite(prereq:step2, of:step3);

        graph.RevokePrerequisite(prereq:step1, from:step2);
        graph.RevokePrerequisite(prereq:step2, from:step3);
        graph.DeclarePrerequisite(prereq:step2, of:step1);
        
        graph.Iterate().Should().BeEquivalentTo(new[] { step2, step1, step3 });
    }
    
    
    [Test]
    public void RevokeShouldBreakTree()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        Node nodeB = new("Node B");
        Node nodeC = new("Node C");
        Node nodeD1 = new("Node D1");
        Node nodeD2 = new("Node D2");
        Node nodeE = new("Node E");
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB);
        graph.DeclarePrerequisite(prereq:nodeB, of:nodeC);
        graph.DeclarePrerequisite(prereq:nodeC, of:nodeD1);
        graph.DeclarePrerequisite(prereq:nodeC, of:nodeD2);
        graph.DeclarePrerequisite(prereq:nodeB, of:nodeD2);
        graph.DeclarePrerequisite(prereq:nodeD1, of:nodeE);
        
        graph.RevokePrerequisite(prereq:nodeC, from:nodeD1);

        graph.IterateFrom(nodeA).Should()
            .BeEquivalentTo(new[] { nodeA, nodeB, nodeC, nodeD2 }, a => a.WithStrictOrdering());
        graph.IterateFrom(nodeD1).Should()
            .BeEquivalentTo(new[] { nodeD1, nodeE }, a => a.WithStrictOrdering());
        graph.IterateFrom(nodeC).Should()
            .BeEquivalentTo(new[] { nodeC, nodeD2 }, a => a.WithStrictOrdering());
    }


    // IterateFrom() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void IterateFromShouldIncludeAllDependents()
    {
        PrerequisiteGraph<Node> graph = new();

        Node nodeA = new("Node A");
        Node nodeB1 = new("Node B1");
        Node nodeB2 = new("Node B2");
        Node nodeC1 = new("Node C1");
        Node nodeC2 = new("Node C2");
        Node nodeD = new("Node D");
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB1);
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB2);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC2);
        graph.DeclarePrerequisite(prereq:nodeC1, of:nodeD);

        graph.IterateFrom(nodeB2).Should().BeEquivalentTo(new[] { nodeB2, nodeC1, nodeC2, nodeD });
    }
    
    
    [Test]
    public void IterateFromShouldIncludeAllDependentsOnceWithOverlappingSubTrees()
    {
        PrerequisiteGraph<Node> graph = new();

        Node nodeA = new("Node A");
        Node nodeB1 = new("Node B1");
        Node nodeB2 = new("Node B2");
        Node nodeB3 = new("Node B3");
        Node nodeC1 = new("Node C1");
        Node nodeC2 = new("Node C2");
        Node nodeC3 = new("Node C3");
        Node nodeC4 = new("Node C4");
        Node nodeC5 = new("Node C5");
        Node nodeD1 = new("Node D1");
        Node nodeD2 = new("Node D2");
        Node nodeD3 = new("Node D3");
        
        graph.DeclarePrerequisite(prereq:nodeA, nodeB1);
        graph.DeclarePrerequisite(prereq:nodeA, nodeB2);
        graph.DeclarePrerequisite(prereq:nodeA, nodeB3);
        
        graph.DeclarePrerequisite(prereq:nodeB1, nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB1, nodeC2);
        graph.DeclarePrerequisite(prereq:nodeB2, nodeC2);
        graph.DeclarePrerequisite(prereq:nodeB2, nodeC3);
        graph.DeclarePrerequisite(prereq:nodeB3, nodeC4);
        graph.DeclarePrerequisite(prereq:nodeB3, nodeC5);
        
        graph.DeclarePrerequisite(prereq:nodeC1, nodeD1);
        graph.DeclarePrerequisite(prereq:nodeC2, nodeD2);
        graph.DeclarePrerequisite(prereq:nodeC3, nodeD1);
        graph.DeclarePrerequisite(prereq:nodeC3, nodeD3);
        graph.DeclarePrerequisite(prereq:nodeC4, nodeD3);
        graph.DeclarePrerequisite(prereq:nodeC5, nodeD3);

        graph.IterateFrom(new[] { nodeB1, nodeB2 }).Should()
            .BeEquivalentTo(new[]
                {
                    nodeB1, nodeB2,
                    nodeC1, nodeC2, nodeC3,
                    nodeD1, nodeD2, nodeD3
                });
    }
    
    [Test]
    public void IterateFromShouldReturnPrerequisitesFirstWithComplexDependencies()
    {
        PrerequisiteGraph<Node> graph = new();

        Node nodeA = new("Node A");
        Node nodeB1 = new("Node B1");
        Node nodeB2 = new("Node B2");
        Node nodeB3 = new("Node B3");
        Node nodeC1 = new("Node C1");
        Node nodeC2 = new("Node C2");
        Node nodeC3 = new("Node C3");
        Node nodeC4 = new("Node C4");
        Node nodeC5 = new("Node C5");
        Node nodeD1 = new("Node D1");
        Node nodeD2 = new("Node D2");
        Node nodeD3 = new("Node D3");
        
        graph.DeclarePrerequisite(prereq:nodeA, nodeB1);
        graph.DeclarePrerequisite(prereq:nodeA, nodeB2);
        graph.DeclarePrerequisite(prereq:nodeA, nodeB3);
        
        graph.DeclarePrerequisite(prereq:nodeB1, nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB1, nodeC2);
        graph.DeclarePrerequisite(prereq:nodeB2, nodeC2);
        graph.DeclarePrerequisite(prereq:nodeB2, nodeC3);
        graph.DeclarePrerequisite(prereq:nodeB3, nodeC4);
        graph.DeclarePrerequisite(prereq:nodeB3, nodeC5);
        
        graph.DeclarePrerequisite(prereq:nodeC1, nodeD1);
        graph.DeclarePrerequisite(prereq:nodeC2, nodeD2);
        graph.DeclarePrerequisite(prereq:nodeC3, nodeD1);
        graph.DeclarePrerequisite(prereq:nodeC3, nodeD3);
        graph.DeclarePrerequisite(prereq:nodeC4, nodeD3);
        graph.DeclarePrerequisite(prereq:nodeC5, nodeD3);

        List<Node> result = graph.IterateFrom(nodeB1, nodeB2).ToList();
        result.Should().ContainInOrder(nodeB1, nodeC1, nodeD1);
        result.Should().ContainInOrder(nodeB1, nodeC2, nodeD2);
        
        result.Should().ContainInOrder(nodeB2, nodeC2, nodeD2);
        result.Should().ContainInOrder(nodeB2, nodeC3, nodeD2);
        result.Should().ContainInOrder(nodeB2, nodeC3, nodeD3);
    }


    [Test]
    public void IterateFromShouldIncludeAllDependentsOnceWhenStartingPointsAreDescendents()
    {
        PrerequisiteGraph<Node> graph = new();

        Node nodeA = new("Node A");
        Node nodeB1 = new("Node B1");
        Node nodeB2 = new("Node B2");
        Node nodeC1 = new("Node C1");
        Node nodeC2 = new("Node C2");
        Node nodeD1 = new("Node D1");
        Node nodeD2 = new("Node D2");
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB1);
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB2);
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC2);
        graph.DeclarePrerequisite(prereq:nodeC2, of:nodeD1);
        graph.DeclarePrerequisite(prereq:nodeC2, of:nodeD2);

        graph.IterateFrom(nodeB1, nodeC2).Should().BeEquivalentTo(new[]
            {
                nodeB1, nodeC1, nodeC2, nodeD1, nodeD2
            });
    }
    
    
    [Test]
    public void IterateFromShouldReturnPrerequisitesFirstWhenStaringPointsAreDescendents()
    {
        PrerequisiteGraph<Node> graph = new();

        Node nodeA = new("Node A");
        Node nodeB1 = new("Node B1");
        Node nodeB2 = new("Node B2");
        Node nodeC1 = new("Node C1");
        Node nodeC2 = new("Node C2");
        Node nodeD1 = new("Node D1");
        Node nodeD2 = new("Node D2");
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB1);
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB2);
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC2);
        graph.DeclarePrerequisite(prereq:nodeC2, of:nodeD1);
        graph.DeclarePrerequisite(prereq:nodeC2, of:nodeD2);

        List<Node> result = graph.IterateFrom(nodeB1, nodeC2).ToList();
        result.Should().ContainInOrder(nodeB1, nodeC1);
        result.Should().ContainInOrder(nodeB1, nodeC2, nodeD1);
        result.Should().ContainInOrder(nodeB1, nodeC2, nodeD2);
    }


    [Test]
    public void IterateFromShouldReturnEntireGraphWhenStartingPointsEqualsAllSources()
    {
        PrerequisiteGraph<Node> graph = new();

        Node nodeA1 = new("Node A1");
        Node nodeA2 = new("Node A2");
        Node nodeB1 = new("Node B1");
        Node nodeB2 = new("Node B2");
        Node nodeB3 = new("Node B3");
        
        graph.DeclarePrerequisite(prereq:nodeA1, of:nodeB1);
        graph.DeclarePrerequisite(prereq:nodeA1, of:nodeB2);
        graph.DeclarePrerequisite(prereq:nodeA2, of:nodeB3);

        graph.IterateFrom(nodeA1, nodeA2).Should().BeEquivalentTo(new[]
            {
                nodeA1, nodeA2, nodeB1, nodeB2, nodeB3
            });
    }
    
    
    // Process() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void ProcessShouldExcludeNodesAfterDepthStop()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        Node nodeB = new("Node B");
        Node nodeC = new("Node C");
        Node nodeD = new("Node D");
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB);
        graph.DeclarePrerequisite(prereq:nodeB, of:nodeC);
        graph.DeclarePrerequisite(prereq:nodeC, of:nodeD);

        List<Node> results = new();
        graph.Process(node =>
            {
                results.Add(node);
                return node == nodeB ? GraphDirective.DepthStop : GraphDirective.Continue;
            });
        results.Should().BeEquivalentTo(new[] { nodeA, nodeB });
    }

    
    [Test]
    public void ProcessShouldIncludeNodesFromOtherBranchesAfterDepthStop()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        Node nodeB1 = new("Node B1");
        Node nodeB2 = new("Node B2");
        Node nodeC1 = new("Node C1");
        Node nodeC2 = new("Node C2");
        Node nodeC3 = new("Node C3");
        Node nodeC4 = new("Node C4");
        Node nodeD1 = new("Node D1");
        Node nodeD2 = new("Node D2");
        Node nodeD3 = new("Node D3");
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB1);
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB2);
        
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC2);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC3);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC4);
        
        graph.DeclarePrerequisite(prereq:nodeC1, of:nodeD1);
        graph.DeclarePrerequisite(prereq:nodeC1, of:nodeD2);
        graph.DeclarePrerequisite(prereq:nodeC2, of:nodeD3);

        List<Node> results = new();
        graph.Process(node =>
            {
                results.Add(node);
                return node == nodeC1 ? GraphDirective.DepthStop : GraphDirective.Continue;
            });
        results.Should().BeEquivalentTo(new[]
            {
                nodeA, nodeB1, nodeB2,
                nodeC1, nodeC2, nodeC3, nodeC4,
                nodeD3
            });
    }
    
    [Test]
    public void ProcessShouldIncludeNodesFromOtherBranchesAfterComplexDepthStop()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        Node nodeB1 = new("Node B1");
        Node nodeB2 = new("Node B2");
        Node nodeC1 = new("Node C1");
        Node nodeC2 = new("Node C2");
        Node nodeC3 = new("Node C3");
        Node nodeC4 = new("Node C4");
        Node nodeD1 = new("Node D1");
        Node nodeD2 = new("Node D2");
        Node nodeD3 = new("Node D3");
        Node nodeD4 = new("Node D4");
        Node nodeD5 = new("Node D5");
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB1);
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB2);
        
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC2);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC3);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC4);
        
        graph.DeclarePrerequisite(prereq:nodeC1, of:nodeD1);
        graph.DeclarePrerequisite(prereq:nodeC1, of:nodeD2);
        graph.DeclarePrerequisite(prereq:nodeC2, of:nodeD3);
        graph.DeclarePrerequisite(prereq:nodeC3, of:nodeD4);
        graph.DeclarePrerequisite(prereq:nodeC3, of:nodeD5);

        List<Node> results = new();
        graph.Process(node =>
            {
                results.Add(node);
                return node == nodeC1 || node == nodeC3
                    ? GraphDirective.DepthStop
                    : GraphDirective.Continue;
            });
        results.Should().BeEquivalentTo(new[]
            {
                nodeA, nodeB1, nodeB2,
                nodeC1, nodeC2, nodeC3, nodeC4,
                nodeD3
            });
    }


    [Test]
    public void ProcessShouldIncludeNodesFromSharedBranchesAfterDepthStop1()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        Node nodeB1 = new("Node B1");
        Node nodeB2 = new("Node B2");
        Node nodeC1 = new("Node C1");
        Node nodeC2 = new("Node C2");
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB1);
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB2);
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC2);
        
        List<Node> results = new();
        graph.Process(node =>
            {
                results.Add(node);
                return node == nodeB2
                    ? GraphDirective.DepthStop
                    : GraphDirective.Continue;
            });
        results.Should().BeEquivalentTo(new[]
            {
                nodeA, nodeB1, nodeB2,
                nodeC1
            });
        results.Should().ContainInOrder(nodeA, nodeB1, nodeC1);
        results.Should().ContainInOrder(nodeA, nodeB2, nodeC1);
    }
    
    [Test]
    public void ProcessShouldIncludeNodesFromSharedBranchesAfterDepthStop2()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        Node nodeB1 = new("Node B1");
        Node nodeB2 = new("Node B2");
        Node nodeC1 = new("Node C1");
        Node nodeC2 = new("Node C2");
        Node nodeD1 = new("Node D1");
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB1);
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB2);
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC2);
        graph.DeclarePrerequisite(prereq:nodeC1, of:nodeD1);
        graph.DeclarePrerequisite(prereq:nodeC2, of:nodeD1);

        
        List<Node> results = new();
        graph.Process(node =>
            {
                results.Add(node);
                return node == nodeB1
                    ? GraphDirective.DepthStop
                    : GraphDirective.Continue;
            });
        results.Should().BeEquivalentTo(new[]
            {
                nodeA,
                nodeB1, nodeB2,
                nodeC1, nodeC2,
                nodeD1,
            });
        results.Should().ContainInOrder(nodeA, nodeB1, nodeC1, nodeD1);
        results.Should().ContainInOrder(nodeA, nodeB2, nodeC1, nodeD1);
        results.Should().ContainInOrder(nodeA, nodeB2, nodeC2, nodeD1);
    }
    
    
    [Test]
    public void ProcessShouldIncludeNodesFromSharedBranchesAfterDepthStop3()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        Node nodeB1 = new("Node B1");
        Node nodeB2 = new("Node B2");
        Node nodeC1 = new("Node C1");
        Node nodeC2 = new("Node C2");
        Node nodeD1 = new("Node D1");
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB1);
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB2);
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC2);
        graph.DeclarePrerequisite(prereq:nodeC1, of:nodeD1);
        graph.DeclarePrerequisite(prereq:nodeC2, of:nodeD1);
        
        List<Node> results = new();
        graph.Process(node =>
            {
                results.Add(node);
                return node == nodeB2
                    ? GraphDirective.DepthStop
                    : GraphDirective.Continue;
            });
        results.Should().BeEquivalentTo(new[]
            {
                nodeA, nodeB1, nodeB2,
                nodeC1,
                nodeD1,
            });
        results.Should().ContainInOrder(nodeA, nodeB1, nodeC1, nodeD1);
        results.Should().ContainInOrder(nodeA, nodeB2, nodeC1, nodeD1);
    }
    
    [Test]
    public void ProcessShouldIncludeNodesFromSharedBranchesAfterDepthStop4()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        Node nodeB1 = new("Node B1");
        Node nodeB2 = new("Node B2");
        Node nodeC1 = new("Node C1");
        Node nodeC2 = new("Node C2");
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB1);
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB2);
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC2);

        
        List<Node> results = new();
        graph.Process(node =>
            {
                results.Add(node);
                return node == nodeB1 || node == nodeB2
                    ? GraphDirective.DepthStop
                    : GraphDirective.Continue;
            });
        results.Should().BeEquivalentTo(new[]
            {
                nodeA,
                nodeB1, nodeB2,
            });
        results.Should().ContainInOrder(nodeA, nodeB1);
        results.Should().ContainInOrder(nodeA, nodeB2);
    }
    
    
    [Test]
    public void ProcessShouldIncludeNodesFromSharedBranchesAfterDepthStop5()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        Node nodeB1 = new("Node B1");
        Node nodeB2 = new("Node B2");
        Node nodeC1 = new("Node C1");
        Node nodeD1 = new("Node D1");
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB1);
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB2);
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeC1, of:nodeD1);

        
        List<Node> results = new();
        graph.Process(node =>
            {
                results.Add(node);
                return node == nodeB1 || node == nodeB2
                    ? GraphDirective.DepthStop
                    : GraphDirective.Continue;
            });
        results.Should().BeEquivalentTo(new[]
            {
                nodeA,
                nodeB1, nodeB2,
            });
        results.Should().ContainInOrder(nodeA, nodeB1);
        results.Should().ContainInOrder(nodeA, nodeB2);
    }
    
    
    [Test]
    public void ProcessShouldIncludeNodesFromSharedBranchesAfterDepthStop6()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        Node nodeB1 = new("Node B1");
        Node nodeB2 = new("Node B2");
        Node nodeC1 = new("Node C1");
        Node nodeC2 = new("Node C2");
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB1);
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB2);
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC2);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC2);
        
        List<Node> results = new();
        graph.Process(node =>
            {
                results.Add(node);
                return node == nodeB2
                    ? GraphDirective.DepthStop
                    : GraphDirective.Continue;
            });
        results.Should().BeEquivalentTo(new[]
            {
                nodeA, nodeB1, nodeB2,
                nodeC1, nodeC2
            });
    }
    
    
  


    [Test]
    public void ProcessShouldReturnPrerequisitesFirst()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        Node nodeB1 = new("Node B1");
        Node nodeB2 = new("Node B2");
        Node nodeC1 = new("Node C1");
        Node nodeC2 = new("Node C2");
        Node nodeC3 = new("Node C3");
        Node nodeC4 = new("Node C4");
        Node nodeD1 = new("Node D1");
        Node nodeD2 = new("Node D2");
        Node nodeD3 = new("Node D3");
        Node nodeD4 = new("Node D4");
        Node nodeD5 = new("Node D5");
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB1);
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB2);
        
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC2);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC3);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC4);
        
        graph.DeclarePrerequisite(prereq:nodeC1, of:nodeD1);
        graph.DeclarePrerequisite(prereq:nodeC1, of:nodeD2);
        graph.DeclarePrerequisite(prereq:nodeC2, of:nodeD3);
        graph.DeclarePrerequisite(prereq:nodeC3, of:nodeD4);
        graph.DeclarePrerequisite(prereq:nodeC3, of:nodeD5);

        List<Node> results = new();
        graph.Process(node =>
            {
                results.Add(node);
                return node == nodeC1 || node == nodeC3
                    ? GraphDirective.DepthStop
                    : GraphDirective.Continue;
            });
        results.Should().ContainInOrder(nodeA, nodeB1, nodeC1);
        results.Should().ContainInOrder(nodeA, nodeC2, nodeD3);
        results.Should().ContainInOrder(nodeA, nodeB2, nodeC3);
        results.Should().ContainInOrder(nodeA, nodeB2, nodeC4);
    }


    // ProcessFrom() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void ProcessFromShouldIncludeNodesFromOtherBranchesAfterComplexDepthStop()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        Node nodeB1 = new("Node B1");
        Node nodeB2 = new("Node B2");
        Node nodeC1 = new("Node C1");
        Node nodeC2 = new("Node C2");
        Node nodeC3 = new("Node C3");
        Node nodeC4 = new("Node C4");
        Node nodeD1 = new("Node D1");
        Node nodeD2 = new("Node D2");
        Node nodeD3 = new("Node D3");
        Node nodeD4 = new("Node D4");
        Node nodeD5 = new("Node D5");
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB1);
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB2);
        
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC2);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC3);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC4);
        
        graph.DeclarePrerequisite(prereq:nodeC1, of:nodeD1);
        graph.DeclarePrerequisite(prereq:nodeC1, of:nodeD2);
        graph.DeclarePrerequisite(prereq:nodeC2, of:nodeD3);
        graph.DeclarePrerequisite(prereq:nodeC3, of:nodeD4);
        graph.DeclarePrerequisite(prereq:nodeC3, of:nodeD5);

        List<Node> results = new();
        graph.ProcessFrom(new [] { nodeB1, nodeC3 },node =>
            {
                results.Add(node);
                return node == nodeC1 || node == nodeC3
                    ? GraphDirective.DepthStop
                    : GraphDirective.Continue;
            });
        results.Should().BeEquivalentTo(new[]
            {
                nodeB1,
                nodeC1, nodeC2, nodeC3,
                nodeD3
            });
    }
    
    
    [Test]
    public void ProcessFromShouldReturnPrerequisitesFirst()
    {
        PrerequisiteGraph<Node> graph = new();
        
        Node nodeA = new("Node A");
        Node nodeB1 = new("Node B1");
        Node nodeB2 = new("Node B2");
        Node nodeC1 = new("Node C1");
        Node nodeC2 = new("Node C2");
        Node nodeC3 = new("Node C3");
        Node nodeC4 = new("Node C4");
        Node nodeD1 = new("Node D1");
        Node nodeD2 = new("Node D2");
        Node nodeD3 = new("Node D3");
        Node nodeD4 = new("Node D4");
        Node nodeD5 = new("Node D5");
        
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB1);
        graph.DeclarePrerequisite(prereq:nodeA, of:nodeB2);
        
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC1);
        graph.DeclarePrerequisite(prereq:nodeB1, of:nodeC2);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC3);
        graph.DeclarePrerequisite(prereq:nodeB2, of:nodeC4);
        
        graph.DeclarePrerequisite(prereq:nodeC1, of:nodeD1);
        graph.DeclarePrerequisite(prereq:nodeC1, of:nodeD2);
        graph.DeclarePrerequisite(prereq:nodeC2, of:nodeD3);
        graph.DeclarePrerequisite(prereq:nodeC3, of:nodeD4);
        graph.DeclarePrerequisite(prereq:nodeC3, of:nodeD5);

        List<Node> results = new();
        graph.ProcessFrom(new [] { nodeB1, nodeC3 },node =>
            {
                results.Add(node);
                return node == nodeC1 || node == nodeC3
                    ? GraphDirective.DepthStop
                    : GraphDirective.Continue;
            });
        results.Should().ContainInOrder(nodeB1, nodeC1);
        results.Should().ContainInOrder(nodeB1, nodeC2, nodeD3);
    }
}
