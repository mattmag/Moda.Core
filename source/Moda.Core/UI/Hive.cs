using Moda.Core.Entity;
using Moda.Core.Lengths;
using Moda.Core.UI.Builders;
using Moda.Core.Utility.Data;
using Moda.Core.Utility.Geometry;
using Optional;
using Optional.Unsafe;

namespace Moda.Core.UI;

public class Hive
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
    
    public Cell NewCell(Func<IInitializeCell, IReadyForConstruction> builder)
    {
        return BuildAndComposeCell(builder(new CellBuilder(new())).GetRecipe());
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
                    Alpha = new Pixels(0).Some<Length>(),
                    Beta = this.rootWidth.Some<Length>(),
                },
                YBoundary =
                {
                    Alpha = new Pixels(0).Some<Length>(),
                    Beta = this.rootHeight.Some<Length>(),
                },
            });
        root.DebugName = "root";
        
        ComposeAndRegisterCell(root, new());
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


    private void CoordinateValueInvalidated(object? sender, EventArgs args)
    {
        if (sender == null)
        {
            return;
        }
        requestingMeasure.Add((Coordinate)sender);
    }


    private void CoordinatePrerequisitesChanged(object? sender,
        CollectionChangedArgs<Coordinate> changes)
    {
        
    }
    
    private void CellChildrenChanged(object? sender, CollectionChangedArgs<Cell> changes)
    {
        foreach (Cell added in changes.ItemsAdded)
        {
            SetupCell(added);
        }

        foreach (Cell removed in changes.ItemsRemoved)
        {
            TeardownCell(removed);
        }
    }
    
    private Cell BuildAndComposeCell(CellRecipe recipe)
    {
        Cell cell = BuildCell(recipe.Boundaries);
        ComposeAndRegisterCell(cell, recipe.Composition);
        return cell;
    }
    
    private Cell BuildCell(BoundariesRecipe recipe)
    {
        Cell cell = new(
            new(recipe.XBoundary.Alpha.ValueOrFailure(), recipe.XBoundary.Beta.ValueOrFailure()),
            new(recipe.YBoundary.Alpha.ValueOrFailure(), recipe.YBoundary.Beta.ValueOrFailure())
        );
        return cell;
    }


    private void ComposeAndRegisterCell(Cell cell, CompositionRecipe recipe)
    {
        recipe.Parent.MatchSome(parent =>
            recipe.InsertionIndex.Match(index =>
                    {
                        parent.InsertChild(cell, index);
                    },
                () =>
                    {
                        parent.AppendChild(cell);
                    }));
        
        UInt64 id = this.entityManager.AddEntity(recipe.CellComponents.Prepend(cell));
        cell.EntityID = id;
    }
    
}