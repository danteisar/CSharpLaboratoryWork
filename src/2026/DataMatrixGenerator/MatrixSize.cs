namespace DataMatrixGenerator;

internal record MatrixSize(int Rows, int Columns, RegionCounts HorizontalRegions, RegionCounts VerticalRegions, int EccCount, int BlockCount)
{
    public static implicit operator MatrixSize((int rows, int columns, RegionCounts horizontalRegions, RegionCounts verticalRegion, int eccCount, int blockCount) size) 
        => new(size.rows, size.columns, size.horizontalRegions, size.verticalRegion, size.eccCount, size.blockCount);
}
