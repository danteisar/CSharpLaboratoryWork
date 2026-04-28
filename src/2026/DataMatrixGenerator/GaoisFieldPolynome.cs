
namespace DataMatrixGenerator;

internal record GaoisFieldPolynome(int[] Coefficients)
{
    public int[] Coefficients { get; init; } = Coefficients.Length > 1
           ? [..Coefficients.SkipWhile(x => x == 0)]
           : Coefficients;

    public static implicit operator GaoisFieldPolynome(int zero) => new([zero]);
}
