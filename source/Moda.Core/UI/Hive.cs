using Moda.Core.Entity;
using Moda.Core.Lengths;
using Moda.Core.UI.Builders;
using Moda.Core.Utility.Data;
using Moda.Core.Utility.Geometry;
using Optional;
using Optional.Unsafe;

namespace Moda.Core.UI;

public interface IHoneyComb
{
    Cell NewCell(Func<IParentAssigner, IReadyToBuild> builder);
}


public class Hive : IHoneyComb
{
    //##############################################################################################
    //
    //  Fields
    //
    //##############################################################################################
    
    private readonly IEntityManager entityManager;
    private readonly PrerequisiteGraph<Coordinate> coordinateGraph = new();

    private readonly Pixels rootWidth = new(0);
    private readonly Pixels rootHeight = new(0);

    private readonly HashSet<Coordinate> requestingMeasure = new();

    //##############################################################################################
    //
    //  Constructors
    //
    //##############################################################################################
    
    public Hive(IEntityManager entityManager)
    {
        this.entityManager = entityManager;
        this.Root = CreateRoot();
    }


    //##############################################################################################
    //
    //  Public Properties
    //
    //##############################################################################################
    
    public Cell Root { get; }
    

    private Size2 _viewPortSize;
    public Size2 ViewPortSize
    {
        get => this._viewPortSize;
        set
        {
            if (this._viewPortSize != value)
            {
                this._viewPortSize = value;
                this.rootWidth.Value = (Int32)Math.Round(this._viewPortSize.Width);
                this.rootHeight.Value = (Int32)this._viewPortSize.Height;
            }
        }
    }


    //##############################################################################################
    //
    //  Public Methods
    //
    //##############################################################################################
    
    public Cell NewCell(Func<IParentAssigner, IReadyToBuild> builder)
    {
        return BuildAndComposeCell(builder(new CellBuilder()).GetRecipe());
    }


    public void Layout()
    {
        this.coordinateGraph.ProcessFrom(this.requestingMeasure,
            coordinate =>
                {
                    Option<Single> oldValue = coordinate.RelativeValue;
                    
                    coordinate.Calculate();

                    // return GraphDirective.Continue;
                    return (oldValue == coordinate.RelativeValue) 
                        ? GraphDirective.DepthStop
                        : GraphDirective.Continue;
                });
        
        requestingMeasure.Clear();;
    }

    //##############################################################################################
    //
    //  Private Methods
    //
    //##############################################################################################


    private Cell CreateRoot()
    {
        Cell root = BuildCell(new()
            {
                XBoundary =
                {
                    Alpha = new Pixels(0).Some<ILength>(),
                    Beta = this.rootWidth.Some<ILength>(),
                },
                YBoundary =
                {
                    Alpha = new Pixels(0).Some<ILength>(),
                    Beta = this.rootHeight.Some<ILength>(),
                },
            });
        root.DebugName = "root";
        foreach (Coordinate coordinate in root.GetCoordinates())
        {
            coordinate.Tare = 0.0f.Some();
        }
        
        RegisterCell(root, Enumerable.Empty<Object>());
        SetupCell(root);
        return root;
    }


    private void SetupCell(Cell cell)
    {
        cell.ChildrenChanged += CellChildrenChanged;
        foreach (Coordinate coordinate in cell.GetCoordinates())
        {
            // subscribe
            coordinate.ValueInvalidated += CoordinateValueInvalidated;
            coordinate.PrerequisitesChanged += CoordinatePrerequisitesChanged;
            
            // dependencies
            this.coordinateGraph.AddNode(coordinate);
            foreach (Coordinate prereq in coordinate.Prerequisites)
            {
                this.coordinateGraph.DeclarePrerequisite(prereq:prereq, of:coordinate);
            }
            
            //
            this.requestingMeasure.Add(coordinate);
        }
    }


    private void TeardownCell(Cell cell)
    {
        cell.ChildrenChanged -= CellChildrenChanged;
        foreach (Coordinate coordinate in cell.GetCoordinates())
        {
            // unsubscribe
            coordinate.ValueInvalidated -= CoordinateValueInvalidated;
            coordinate.PrerequisitesChanged -= CoordinatePrerequisitesChanged;
            
            // dependencies
            this.coordinateGraph.RemoveNode(coordinate);
        }
    }


    private void CoordinateValueInvalidated(Coordinate sender)
    {
        requestingMeasure.Add(sender);
    }


    private void CoordinatePrerequisitesChanged(Coordinate sender,
        CollectionChangedArgs<Coordinate> changes)
    {
        foreach (Coordinate removed in changes.ItemsRemoved)
        {
            this.coordinateGraph.RevokePrerequisite(prereq:removed, from:sender);
        }
        
        foreach (Coordinate added in changes.ItemsAdded)
        {
            this.coordinateGraph.DeclarePrerequisite(prereq:added, of:sender);
        }

        this.requestingMeasure.Add(sender);
    }
    
    private void CellChildrenChanged(Object? sender, CollectionChangedArgs<Cell> changes)
    {
        foreach (Cell removed in changes.ItemsRemoved)
        {
            TeardownCell(removed);
        }
        
        foreach (Cell added in changes.ItemsAdded)
        {
            SetupCell(added);
        }
    }
    
    private Cell BuildAndComposeCell(CellRecipe recipe)
    {
        Cell cell = BuildCell(recipe.Boundaries);
        ComposeCell(cell, recipe.Composition);
        return cell;
    }
    
    private Cell BuildCell(BoundariesRecipe recipe)
    {
        Cell cell = new(this,
            new(recipe.XBoundary.Alpha.ValueOrFailure(), recipe.XBoundary.Beta.ValueOrFailure()),
            new(recipe.YBoundary.Alpha.ValueOrFailure(), recipe.YBoundary.Beta.ValueOrFailure())
        );
        return cell;
    }


    private void ComposeCell(Cell cell, CompositionRecipe recipe)
    {
        SetParent(cell, recipe.Parent.Get(), recipe.InsertionIndex.Get());
        RegisterCell(cell, recipe.Components.Get());
    }

    private void SetParent(Cell cell, Cell parent, Option<Int32> insertionIndex)
    {
        insertionIndex.Match(index =>
            {
                parent.InsertChild(cell, index);
            },
            () =>
            {
                parent.AppendChild(cell);
            });
    }

    private void RegisterCell(Cell cell, IEnumerable<Object> components)
    {
        UInt64 id = this.entityManager.AddEntity(components.Prepend(cell));
        cell.EntityID = id;
    }
    
}