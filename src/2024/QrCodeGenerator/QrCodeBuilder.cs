using System.Text;
using System.Linq;

namespace QrCodeGenerator;
 
public static class QrCodeBuilder
{    
    private const int W = 0, O = 0, M = 3, X = 0, Y = 0, Q = 0, H = 0;
    private static readonly List<int[]> _matrixTemplate =
    [
        [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],        
        [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
        [1, 1, W, W, W, W, W, W, W, 1, 1, 1, 1, 1, 1, 1, W, W, W, W, W, W, W, 1, 1],
        [1, 1, W, 1, 1, 1, 1, 1, W, 1, 1, 1, 1, 1, 1, 1, W, 1, 1, 1, 1, 1, W, 1, 1],
        [1, 1, W, 1, W, W, W, 1, W, 1, 1, 1, 1, 1, 1, 1, W, 1, W, W, W, 1, W, 1, 1],
        [1, 1, W, 1, W, W, W, 1, W, 1, 1, 1, 1, 1, 1, 1, W, 1, W, W, W, 1, W, 1, 1],
        [1, 1, W, 1, W, W, W, 1, W, 1, 1, 1, 1, 1, 1, 1, W, 1, W, W, W, 1, W, 1, 1],
        [1, 1, W, 1, 1, 1, 1, 1, W, 1, 1, 1, 1, 1, 1, 1, W, 1, 1, 1, 1, 1, W, 1, 1],
        [1, 1, W, W, W, W, W, W, W, 1, W, 1, W, 1, W, 1, W, W, W, W, W, W, W, 1, 1],
        [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
        [1, 1, 1, 1, 1, 1, 1, 1, W, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
        [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
        [1, 1, 1, 1, 1, 1, 1, 1, W, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
        [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
        [1, 1, 1, 1, 1, 1, 1, 1, W, 1, W, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
        [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
        [1, 1, W, W, W, W, W, W, W, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
        [1, 1, W, 1, 1, 1, 1, 1, W, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
        [1, 1, W, 1, W, W, W, 1, W, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
        [1, 1, W, 1, W, W, W, 1, W, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
        [1, 1, W, 1, W, W, W, 1, W, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
        [1, 1, W, 1, 1, 1, 1, 1, W, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
        [1, 1, W, W, W, W, W, W, W, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
        [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
        [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
        [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1]
    ];

    private static readonly Dictionary<(CorrectionLevel correctionLevel, int maskNum), string> _masksAndCorrectionLevel = new()
    {
        {(CorrectionLevel.L, 0), "111011111000100"},
        {(CorrectionLevel.M, 0), "101010000010010"},
        {(CorrectionLevel.Q, 0), "011010101011111"},
        {(CorrectionLevel.H, 0), "001011010001001"},

        {(CorrectionLevel.L, 1), "111001011110011"},
        {(CorrectionLevel.M, 1), "101000100100101"},
        {(CorrectionLevel.Q, 1), "011000001101000"},
        {(CorrectionLevel.H, 1), "001001110111110"},

        {(CorrectionLevel.L, 2), "111110110101010"},
        {(CorrectionLevel.M, 2), "101111001111100"},
        {(CorrectionLevel.Q, 2), "011111100110001"},
        {(CorrectionLevel.H, 2), "001110011100111"},
    };

    private const int A = 10, B = 11, C = 12, D = 13, E = 14, F = 15;

    private static readonly List<int[]> _masksAndCorrectionLevelMatrixTemplate =
    [
        [X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X],        
        [X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X],
        [X, X, W, W, W, W, W, W, W, X, F, X, X, X, X, X, W, W, W, W, W, W, W, X, X],
        [X, X, W, X, X, X, X, X, W, X, E, X, X, X, X, X, W, X, X, X, X, X, W, X, X],
        [X, X, W, X, W, W, W, X, W, X, D, X, X, X, X, X, W, X, W, W, W, X, W, X, X],
        [X, X, W, X, W, W, W, X, W, X, C, X, X, X, X, X, W, X, W, W, W, X, W, X, X],
        [X, X, W, X, W, W, W, X, W, X, B, X, X, X, X, X, W, X, W, W, W, X, W, X, X],
        [X, X, W, X, X, X, X, X, W, X, A, X, X, X, X, X, W, X, X, X, X, X, W, X, X],
        [X, X, W, W, W, W, W, W, W, X, W, X, W, X, W, X, W, W, W, W, W, W, W, X, X],
        [X, X, X, X, X, X, X, X, X, X, 9, X, X, X, X, X, X, X, X, X, X, X, X, X, X],
        [X, X, 1, 2, 3, 4, 5, 6, W, 7, 8, X, X, X, X, X, 9, A, B, C, D, E, F, X, X],
        [X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X],
        [X, X, X, X, X, X, X, X, W, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X],
        [X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X],
        [X, X, X, X, X, X, X, X, W, X, W, X, X, X, X, X, X, X, X, X, X, X, X, X, X],
        [X, X, X, X, X, X, X, X, X, X, 8, X, X, X, X, X, X, X, X, X, X, X, X, X, X],
        [X, X, W, W, W, W, W, W, W, X, 7, X, X, X, X, X, X, X, X, X, X, X, X, X, X],
        [X, X, W, X, X, X, X, X, W, X, 6, X, X, X, X, X, X, X, X, X, X, X, X, X, X],
        [X, X, W, X, W, W, W, X, W, X, 5, X, X, X, X, X, X, X, X, X, X, X, X, X, X],
        [X, X, W, X, W, W, W, X, W, X, 4, X, X, X, X, X, X, X, X, X, X, X, X, X, X],
        [X, X, W, X, W, W, W, X, W, X, 3, X, X, X, X, X, X, X, X, X, X, X, X, X, X],
        [X, X, W, X, X, X, X, X, W, X, 2, X, X, X, X, X, X, X, X, X, X, X, X, X, X],
        [X, X, W, W, W, W, W, W, W, X, 1, X, X, X, X, X, X, X, X, X, X, X, X, X, X],
        [X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X],
        [X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X],
        [X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X, X]
    ];

    private static readonly List<int[]> _maskNo0Matrix =
        [
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O],  
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O],       
            [O, O, W, W, W, W, W, W, W, X, X, M, X, M, X, X, W, W, W, W, W, W, W, O, O],
            [O, O, W, X, X, X, X, X, W, X, X, X, M, X, M, X, W, X, X, X, X, X, W, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, M, X, M, X, X, W, X, W, W, W, X, W, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, X, M, X, M, X, W, X, W, W, W, X, W, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, M, X, M, X, X, W, X, W, W, W, X, W, O, O],
            [O, O, W, X, X, X, X, X, W, X, X, X, M, X, M, X, W, X, X, X, X, X, W, O, O],
            [O, O, W, W, W, W, W, W, W, X, W, X, W, X, W, X, W, W, W, W, W, W, W, O, O],
            [O, O, X, X, X, X, X, X, X, X, X, X, M, X, M, X, X, X, X, X, X, X, X, O, O],
            [O, O, X, X, X, X, X, X, W, X, X, M, X, M, X, X, X, X, X, X, X, X, X, O, O],    
            [O, O, M, X, M, X, M, X, X, X, M, X, M, X, M, X, M, X, M, X, M, X, M, O, O],
            [O, O, X, M, X, M, X, M, W, M, X, M, X, M, X, M, X, M, X, M, X, M, X, O, O],
            [O, O, M, X, M, X, M, X, X, X, M, X, M, X, M, X, M, X, M, X, M, X, M, O, O],
            [O, O, X, M, X, M, X, M, W, M, X, M, X, M, X, M, X, M, X, M, X, M, X, O, O],    
            [O, O, X, X, X, X, X, X, X, X, X, X, M, X, M, X, M, X, M, X, M, X, M, O, O],
            [O, O, W, W, W, W, W, W, W, X, X, M, X, M, X, M, X, M, X, M, X, M, X, O, O],
            [O, O, W, X, X, X, X, X, W, X, X, X, M, X, M, X, M, X, M, X, M, X, M, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, M, X, M, X, M, X, M, X, M, X, M, X, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, X, M, X, M, X, M, X, M, X, M, X, M, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, M, X, M, X, M, X, M, X, M, X, M, X, O, O],
            [O, O, W, X, X, X, X, X, W, X, X, X, M, X, M, X, M, X, M, X, M, X, M, O, O],
            [O, O, W, W, W, W, W, W, W, X, X, M, X, M, X, M, X, M, X, M, X, M, X, O, O],
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O],
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O],
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O]
        ];

    private static readonly List<int[]> _maskNo1Matrix =
        [
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O],  
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O],       
            [O, O, W, W, W, W, W, W, W, X, X, X, X, X, X, X, W, W, W, W, W, W, W, O, O],
            [O, O, W, X, X, X, X, X, W, X, X, M, M, M, M, X, W, X, X, X, X, X, W, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, X, X, X, X, X, W, X, W, W, W, X, W, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, M, M, M, M, X, W, X, W, W, W, X, W, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, X, X, X, X, X, W, X, W, W, W, X, W, O, O],
            [O, O, W, X, X, X, X, X, W, X, X, M, M, M, M, X, W, X, X, X, X, X, W, O, O],
            [O, O, W, W, W, W, W, W, W, X, W, X, W, X, W, X, W, W, W, W, W, W, W, O, O],
            [O, O, X, X, X, X, X, X, X, X, X, M, M, M, M, X, X, X, X, X, X, X, X, O, O],
            [O, O, X, X, X, X, X, X, W, X, X, X, X, X, X, X, X, X, X, X, X, X, X, O, O],            
            [O, O, M, M, M, M, M, M, M, M, M, M, M, M, M, M, M, M, M, M, M, M, M, O, O],
            [O, O, X, X, X, X, X, X, W, X, X, X, X, X, X, X, X, X, X, X, X, X, X, O, O],
            [O, O, M, M, M, M, M, M, M, M, M, M, M, M, M, M, M, M, M, M, M, M, M, O, O],
            [O, O, X, X, X, X, X, X, W, X, X, X, X, X, X, X, X, X, X, X, X, X, X, O, O],            
            [O, O, X, X, X, X, X, X, X, X, X, M, M, M, M, M, M, M, M, M, M, M, M, O, O],
            [O, O, W, W, W, W, W, W, W, X, X, X, X, X, X, X, X, X, X, X, X, X, X, O, O],
            [O, O, W, X, X, X, X, X, W, X, X, M, M, M, M, M, M, M, M, M, M, M, M, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, X, X, X, X, X, X, X, X, X, X, X, X, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, M, M, M, M, M, M, M, M, M, M, M, M, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, X, X, X, X, X, X, X, X, X, X, X, X, O, O],
            [O, O, W, X, X, X, X, X, W, X, X, M, M, M, M, M, M, M, M, M, M, M, M, O, O],
            [O, O, W, W, W, W, W, W, W, X, X, X, X, X, X, X, X, X, X, X, X, X, X, O, O],
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O],
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O],
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O]
        ];

    private static readonly List<int[]> _maskNo2Matrix =
        [
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O],  
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O],       
            [O, O, W, W, W, W, W, W, W, X, X, M, X, X, M, X, W, W, W, W, W, W, W, O, O],
            [O, O, W, X, X, X, X, X, W, X, X, M, X, X, M, X, W, X, X, X, X, X, W, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, M, X, X, M, X, W, X, W, W, W, X, W, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, M, X, X, M, X, W, X, W, W, W, X, W, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, M, X, X, M, X, W, X, W, W, W, X, W, O, O],
            [O, O, W, X, X, X, X, X, W, X, X, M, X, X, M, X, W, X, X, X, X, X, W, O, O],
            [O, O, W, W, W, W, W, W, W, X, W, X, W, X, W, X, W, W, W, W, W, W, W, O, O],
            [O, O, X, X, X, X, X, X, X, X, X, M, X, X, M, X, X, X, X, X, X, X, X, O, O],
            [O, O, X, X, X, X, X, X, W, X, X, M, X, X, M, X, X, X, X, X, X, X, X, O, O],
            [O, O, M, X, X, M, X, X, X, X, X, M, X, X, M, X, X, M, X, X, M, X, X, O, O],
            [O, O, M, X, X, M, X, X, W, X, X, M, X, X, M, X, X, M, X, X, M, X, X, O, O],
            [O, O, M, X, X, M, X, X, X, X, X, M, X, X, M, X, X, M, X, X, M, X, X, O, O],
            [O, O, M, X, X, M, X, X, W, X, X, M, X, X, M, X, X, M, X, X, M, X, X, O, O],
            [O, O, X, X, X, X, X, X, X, X, X, M, X, X, M, X, X, M, X, X, M, X, X, O, O],
            [O, O, W, W, W, W, W, W, W, X, X, M, X, X, M, X, X, M, X, X, M, X, X, O, O],
            [O, O, W, X, X, X, X, X, W, X, X, M, X, X, M, X, X, M, X, X, M, X, X, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, M, X, X, M, X, X, M, X, X, M, X, X, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, M, X, X, M, X, X, M, X, X, M, X, X, O, O],
            [O, O, W, X, W, W, W, X, W, X, X, M, X, X, M, X, X, M, X, X, M, X, X, O, O],
            [O, O, W, X, X, X, X, X, W, X, X, M, X, X, M, X, X, M, X, X, M, X, X, O, O],
            [O, O, W, W, W, W, W, W, W, X, X, M, X, X, M, X, X, M, X, X, M, X, X, O, O],
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O],
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O],
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O]
        ];
    
    private static List<int[]> GetMaskMatrix(int makNum = 2)
     => makNum switch 
        {
            0 => _maskNo0Matrix,
            1 => _maskNo1Matrix,    
            2 => _maskNo2Matrix,
            _ => _maskNo2Matrix
        };
    
    private static List<int[]> CreateQrCodeMatrix(int size)
    {
        List<int[]> qrCodeMatrix = [];       
        for (int i = 0; i < size + 1; i++)
        {
            qrCodeMatrix.Add(new int[size]);
        }
        return qrCodeMatrix;
    }

    private static char Scan(int a, int b)
        => (a, b) switch
        {
            (0, 0) => ' ',
            (0, 1) => '▄',
            (1, 0) => '▀',
            (1, 1) => '█',
            _ => throw new NotImplementedException(),
        };

    private static char ScanAlt(int a, int b)
        => (a, b) switch
        {
            (0, 0) => '█',
            (0, 1) => '▀',
            (1, 0) => '▄',
            (1, 1) => ' ',
            _ => throw new NotImplementedException(),
        };

    private static void PutInMatrix(List<int[]> matrix, List<int[]> maskMatrix, int index, int value)
    {
        for (int x = 0; x<matrix.Count; x++)
        {
            for (int y = 0; y<matrix[x].Length; y++)
            {
                if (maskMatrix[x][y]==index)
                    matrix[x][y] = value;
            }
        }
    }

    private static void AddMaskNumAndCorrectionLevel(List<int[]> matrix, CorrectionLevel level, int maskNum)
    {
        var maskNumAndCorrectionLevel = _masksAndCorrectionLevel[(level, maskNum)];
        for (int i = 0; i< maskNumAndCorrectionLevel.Length; i++)
        {
            PutInMatrix(matrix, _masksAndCorrectionLevelMatrixTemplate, i + 1, maskNumAndCorrectionLevel[i] == '1' ? 1 : 0);
        }
        
    }
    
    private static string Combine(List<int[]> matrix, List<int[]> matrix2)
    {
        var sb = new StringBuilder();
        for (int y = 0; y<matrix.Count; y+=2)
        {
            for(int x = 0; x<matrix[0].Length; x++)
            {
                sb.Append(Scan(matrix2[y][x] ^ matrix[y][x], matrix2[y+1][x] ^ matrix[y+1][x]));  
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private static readonly List<int[]> _orderMatrix =
        [
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O],        
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O],
            [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   138, 137,   136, 135,    Y,  0,    0,  0,    0,  0,    0,  0,   O, O],
            [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   140, 139,   134, 133,    Y,  0,    0,  0,    0,  0,    0,  0,   O, O],
            [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   142, 141,   132, 131,    Y,  0,    0,  0,    0,  0,    0,  0,   O, O],
            [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   144, 143,   130, 129,    Y,  0,    0,  0,    0,  0,    0,  0,   O, O],
            [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   146, 145,   128, 127,    Y,  0,    0,  0,    0,  0,    0,  0,   O, O],
            [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   148, 147,   126, 125,    Y,  0,    0,  0,    0,  0,    0,  0,   O, O],
            [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   Q,     Q,   Q,     Q,   Q,    Y,  0,    0,  0,    0,  0,    0,  0,   O, O],
            [O, O,   Y,   Y,     Y,   Y,     Y,   Y, Y,     Y,   H,   150, 149,   124, 123,    Y,  Y,    Y,  Y,    Y,  Y,    Y,  Y,   O, O],
            [O, O,   H,   H,     H,   H,     H,   H, Q,     H,   H,   152, 151,   122, 121,    H,  H,    H,  H,    H,  H,    H,  H,   O, O], 
            [O, O, 202, 201,   200, 199,   186, 185, Q,   184, 183,   154, 153,   120, 119,   74, 73,   72, 71,   26, 25,   24, 23,   O, O], 
            [O, O, 204, 203,   198, 197,   188, 187, Q,   182, 181,   156, 155,   118, 117,   76, 75,   70, 69,   28, 27,   22, 21,   O, O], 
            [O, O, 206, 205,   196, 195,   190, 189, Q,   180, 179,   158, 157,   116, 115,   78, 77,   68, 67,   30, 29,   20, 19,   O, O], 
            [O, O, 208, 207,   194, 193,   192, 191, Q,   178, 177,   160, 159,   114, 113,   80, 79,   66, 65,   32, 31,   18, 17,   O, O],    
            [O, O,   Y,   Y,     Y,   Y,     Y,   Y, Y,     Y,   Q,   162, 161,   112, 111,   82, 81,   64, 63,   34, 33,   16, 15,   O, O],  
            [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   164, 163,   110, 109,   84, 83,   62, 61,   36, 35,   14, 13,   O, O], 
            [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   166, 165,   108, 107,   86, 85,   60, 59,   38, 37,   12, 11,   O, O], 
            [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   168, 167,   106, 105,   88, 87,   58, 57,   40, 39,   10, 09,   O, O], 
            [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   170, 169,   104, 103,   90, 89,   56, 55,   42, 41,   08, 07,   O, O], 
            [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   172, 171,   102, 101,   92, 91,   54, 53,   44, 43,   06, 05,   O, O], 
            [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   174, 173,   100, 099,   94, 93,   52, 51,   46, 45,   04, 03,   O, O], 
            [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   176, 175,   098, 097,   96, 95,   50, 49,   48, 47,   02, 01,   O, O],
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O],
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O],
            [O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O, O]
        ];

    private static (int x, int y, bool found) SearchPlace(List<int[]> matrix, int pos)
    {
        for (int x = 0; x<matrix.Count; x++)
        {
            for (int y = 0; y<matrix[x].Length; y++)
            {
                if (matrix[x][y]==pos)
                    return (x,y, true);
            }
        }
        return (default, default, false);
    }

    private static void PutInMatrix(List<int[]> matrix, string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            char letter = text[i];
            (var x, var y, var found) = SearchPlace(_orderMatrix, i + 1);
            if (found)
                matrix[x][y] = letter != '1' ? 0 : 1;
        }
    }
    
    private static void MaskInvertMatrix(List<int[]> matrix, List<int[]> mask)
    {
        for (int x = 0; x<matrix.Count; x++)
        {
            for (int y = 0; y<matrix[x].Length; y++)
            {
                if (mask[x][y] == M)
                    matrix[x][y] = matrix[x][y] == 1 ? 0 : 1;
            }
        }
    }        

    private static readonly char[] letterNumberArray = {
                                                    '0','1','2','3','4','5','6','7','8','9',
                                                    'A','B','C','D','E','F','G','H','I','J',
                                                    'K','L','M','N','O','P','Q','R','S','T',
                                                    'U','V','W','X','Y','Z',' ','$','%','*',
                                                    '+','-','.','/',':'
                                                };
    
    private static void AddTo(List<string> list, int num, byte lengthType = 10)
    {
        var str = Convert.ToString(num, 2);
        list.Add(str.PadLeft(lengthType, '0'));
    }

    private static List<string> CodeLetterDigital(string text)
    {
        var res = new List<string>();
        var pos = 0;
        while (pos < text.Length - 2)
        {
            var index1 = Array.IndexOf(letterNumberArray, text[pos]);
            var index2 = Array.IndexOf(letterNumberArray, text[pos + 1]);
            AddTo(res, index1 * 45 + index2, 11);
            pos += 2;
        }
        if (text.Length % 2 == 1)
        {
            AddTo(res, Array.IndexOf(letterNumberArray, text[^1]), 6);
        }
        return res;
    }

    private static List<string> CodeDigital(string text)
    {
        var res = new List<string>();
        var pos = 0;
        var length = text.Length;
        while (pos < text.Length - 3)
        {
            var number = text.Substring(pos, 3);
            AddTo(res, Convert.ToInt32(number));
            pos += 3;
        }
        if (text.Length % 3 == 2)
        {
            AddTo(res, Convert.ToInt32(text.Substring(length - 3, 2)), 7);
        }
        else
        if (text.Length % 3 == 1)
        {
            AddTo(res, Convert.ToInt32(text.Substring(length - 2, 1)), 4);
        }
        return res;
    }

    private static List<string> CodeBytes(string text)
    {
        var res = new List<string>();        
        var numbers = Encoding.UTF8.GetBytes(text);
        foreach (var number in numbers)
        {
            AddTo(res, Convert.ToInt32(number));
        }        
        return res;
    }

    private static readonly Dictionary<(CorrectionLevel correctionLevel1, byte version), int> _maxData = new()
    {
        {(CorrectionLevel.H, 1), 072},  
        {(CorrectionLevel.Q, 1), 104}, 
        {(CorrectionLevel.M, 1), 128},
        {(CorrectionLevel.L, 1), 152},
        
        {(CorrectionLevel.H, 2), 128},  
        {(CorrectionLevel.Q, 2), 176}, 
        {(CorrectionLevel.M, 2), 224},
        {(CorrectionLevel.L, 2), 272},
       
        {(CorrectionLevel.H, 3), 208},
        {(CorrectionLevel.Q, 3), 272},
        {(CorrectionLevel.M, 3), 352},
        {(CorrectionLevel.L, 3), 440},        

        {(CorrectionLevel.H, 4), 288},
        {(CorrectionLevel.Q, 4), 384},
        {(CorrectionLevel.M, 4), 512},
        {(CorrectionLevel.L, 4), 640}        
    };

    private static CorrectionLevel GetCorrectionLevelAndVersion(string codeText, int version)
    {
        foreach (var pair in _maxData.Where(v => v.Key.version == version))
        {
            if (pair.Value > codeText.Length)
                return pair.Key.correctionLevel1;
        }
        throw new NotSupportedException("Current version of QR-code does not support that data length!");
    }

    private static readonly Dictionary<CodeType, byte> _codeTypeSize = new()
    {
        {CodeType.Numeric, 10},
        {CodeType.AlphaNumeric, 9},
        {CodeType.Binary, 8}
    };

    private static readonly Dictionary<CodeType, string> _codeTypeMode = new()
    {
        {CodeType.Numeric, "0001"},
        {CodeType.AlphaNumeric, "0010"},
        {CodeType.Binary, "0100"}
    };

    private static readonly Dictionary<byte, byte> _matrixSizeByVersion = new()
    {
        {1, 21 + 4},
        {2, 25 + 4},
        {3, 29 + 4},
        {4, 33 + 4}
    };

    private static string GetDataAmount(CodeType codeType, int size)
    {
        var str = Convert.ToString(size, 2);
        return str.PadLeft(_codeTypeSize[codeType], '0');
    }
    
    private static string GetServiceInformation(CodeType codeType, string text)
    {
        return _codeTypeMode[codeType] + GetDataAmount(codeType, text.Length);       
    }

    private static readonly string[] _magicTextArray = ["11101100", "00010001"];

    private static void AlignByByteSize(StringBuilder sb)
    {
        var res = string.Empty.PadLeft(sb.Length % 8, '0');
        if (res.Length > 0) 
            sb.Append(res);
    }

    private static string FillCode(string text, int size)
    {
        var sb = new StringBuilder(text);  

        var cnt = (size - text.Length) / 8;       
        for (int i = 0; i < cnt; i++)
        {
            sb.Append(_magicTextArray[i % 2]);
        }
        return sb.ToString(); 
    }

    private static IEnumerable<string> SplitText(this string text, int chunkSize)
    {
        return Enumerable.Range(0, text.Length / chunkSize)
            .Select(i => text.Substring(i * chunkSize, chunkSize));
    }

    private static List<byte> SplitByBlock(string text)
    {
        List<byte> tmp = [];
        foreach (var line in text.SplitText(8))
        {
            tmp.Add(Convert.ToByte(line,2));
        }
        return tmp;
    }

    private static readonly Dictionary<CorrectionLevel, byte[]> _correctionLevelBytesSize = new()
    {
        {CorrectionLevel.L, [00,07,10,15,20]},
        {CorrectionLevel.M, [00,10,16,26,18]},
        {CorrectionLevel.Q, [00,13,22,18,26]},
        {CorrectionLevel.H, [00,17,28,22,16]},
    };

    private static readonly Dictionary<CorrectionLevel, byte[]> _correctionLevelBlocksCount = new()
    {
        {CorrectionLevel.L, [1,1,1,1]},
        {CorrectionLevel.M, [1,1,1,2]},
        {CorrectionLevel.Q, [1,1,2,2]},
        {CorrectionLevel.H, [1,1,2,4]},
    };

    private static readonly Dictionary<byte, byte[]> _correctionLevelGeneratingPolynomial = new()
    {
        {7, [87, 229, 146, 149, 238, 102, 21]},
        {10, [251, 67, 46, 61, 118, 70, 64, 94, 32, 45]},
        {13, [74, 152, 176, 100, 86, 100, 106, 104, 130, 218, 206, 140, 78]},
        {15, [8, 183, 61, 91, 202, 37, 51, 58, 58, 237, 140, 124, 5, 99, 105]},
        {16, [120, 104, 107, 109, 102, 161, 76, 3, 91, 191, 147, 169, 182, 194, 225, 120]},
        {17, [43, 139, 206, 78, 43, 239, 123, 206, 214, 147, 24, 99, 150, 39, 243, 163, 136]},
        {18, [215, 234, 158, 94, 184, 97, 118, 170, 79, 187, 152, 148, 252, 179, 5, 98, 96, 153]},
        {20, [17, 60, 79, 50, 61, 163, 26, 187, 202, 180, 221, 225, 83, 239, 156, 164, 212, 212, 188, 190]},
        {22, [210, 171, 247, 242, 93, 230, 14, 109, 221, 53, 200, 74, 8, 172, 98, 80, 219, 134, 160, 105, 165, 231]},
        {24, [229, 121, 135, 48, 211, 117, 251, 126, 159, 180, 169, 152, 192, 226, 228, 218, 111, 0, 117, 232, 87, 96, 227, 21]},
        {26, [173, 125, 158, 2, 103, 182, 118, 17, 145, 201, 111, 28, 165, 53, 161, 21, 245, 142, 13, 102, 48, 227, 153, 145, 218, 70]},
        {28, [168, 223, 200, 104, 224, 234, 108, 180, 110, 190, 195, 147, 205, 27, 232, 201, 21, 43, 245, 87, 42, 195, 212, 119, 242, 37, 9, 123]},
        {30, [41, 173, 145, 152, 216, 31, 179, 182, 50, 48, 110, 86, 239, 96, 222, 125, 42, 173, 226, 193, 224, 130, 156, 37, 251, 216, 238, 40, 192, 180]},
    };

    private static readonly byte[] _gaussianField = [1,2,4,8,16,32,64,128,29,58,116,232,205,135,19,38,
                                                     76,152,45,90,180,117,234,201,143,3,6,12,24,48,96,192,
                                                     157,39,78,156,37,74,148,53,106,212,181,119,238,193,159,35,
                                                     70,140,5,10,20,40,80,160,93,186,105,210,185,111,222,161,
                                                     95,190,97,194,153,47,94,188,101,202,137,15,30,60,120,240,
                                                     253,231,211,187,107,214,177,127,254,225,223,163,91,182,113,226,
                                                     217,175,67,134,17,34,68,136,13,26,52,104,208,189,103,206,
                                                     129,31,62,124,248,237,199,147,59,118,236,197,151,51,102,204,
                                                     133,23,46,92,184,109,218,169,79,158,33,66,132,21,42,84,
                                                     168,77,154,41,82,164,85,170,73,146,57,114,228,213,183,115,
                                                     230,209,191,99,198,145,63,126,252,229,215,179,123,246,241,255,
                                                     227,219,171,75,150,49,98,196,149,55,110,220,165,87,174,65,
                                                     130,25,50,100,200,141,7,14,28,56,112,224,221,167,83,166,
                                                     81,162,89,178,121,242,249,239,195,155,43,86,172,69,138,9,
                                                     18,36,72,144,61,122,244,245,247,243,251,235,203,139,11,22,
                                                     44,88,176,125,250,233,207,131,27,54,108,216,173,71,142,1
                                                    ];

    private static readonly byte[] _backGaussianField = [0,1,25,2,50,26,198,3,223,51,238,27,104,199,75,
                                                    4,100,224,14,52,141,239,129,28,193,105,248,200,8,76,113,
                                                    5,138,101,47,225,36,15,33,53,147,142,218,240,18,130,69,
                                                    29,181,194,125,106,39,249,185,201,154,9,120,77,228,114,166,
                                                    6,191,139,98,102,221,48,253,226,152,37,179,16,145,34,136,
                                                    54,208,148,206,143,150,219,189,241,210,19,92,131,56,70,64,
                                                    30,66,182,163,195,72,126,110,107,58,40,84,250,133,186,61,
                                                    202,94,155,159,10,21,121,43,78,212,229,172,115,243,167,87,
                                                    7,112,192,247,140,128,99,13,103,74,222,237,49,197,254,24,
                                                    227,165,153,119,38,184,180,124,17,68,146,217,35,32,137,46,
                                                    55,63,209,91,149,188,207,205,144,135,151,178,220,252,190,97,
                                                    242,86,211,171,20,42,93,158,132,60,57,83,71,109,65,162,
                                                    31,45,67,216,183,123,164,118,196,23,73,236,127,12,111,246,
                                                    108,161,59,82,41,157,85,170,251,96,134,177,187,204,62,90,
                                                    203,89,95,176,156,169,160,81,11,245,22,235,122,117,44,215,
                                                    79,174,213,233,230,231,173,232,116,214,244,234,168,80,88,175
                                                    ];
    
    private static List<byte> GetCorrectionBlock(List<byte> block, byte bytesSize)
    {       
        var size = Math.Max(block.Count, bytesSize);
        var m = new List<byte>(block);
        var g = _correctionLevelGeneratingPolynomial[bytesSize];
        var n = g.Length;
        while (m.Count != size)
            m.Add(0);

        for (int i = 0; i < block.Count; i++)
        {
            byte a = m[0];
            if (a == 0) continue;

            m.RemoveAt(0);
            m.Add(0);

            byte b = _backGaussianField[a-1];
            for (int x = 0; x < g.Length; x++)
            {
                var c = g[x] + b;
                if (c > 254)
                    c %= 255;
                var d = _gaussianField[c];
                m[x] = (byte)(d ^ m[x]);
            }
        }
         
        return m.Take(n).ToList();    
    }
   
    private static string SetCorrectionBlock(List<byte> block, string codeText)
    {       
        var sb = new StringBuilder(codeText);
        foreach (var b in block)
        {
            var tmp = Convert.ToString(b, 2).PadLeft(8, '0');
            sb.Append(tmp);
        }
        return sb.ToString();
    }
        
    public static string GetQrCode(string text, byte qrCodeVersion = 1, CodeType codeType = CodeType.Binary)
    {
        // Создание матрицы QR кода           
        var qrCodeMatrix = CreateQrCodeMatrix(_matrixSizeByVersion[qrCodeVersion]);
        // Блоки с данными
        var sb = new StringBuilder();        
        sb.Append(GetServiceInformation(codeType, text));        
        var tmp = codeType switch {
            CodeType.AlphaNumeric => CodeLetterDigital(text.ToUpper()),
            CodeType.Numeric => CodeDigital(text),
            _ => CodeBytes(text)
        };
        foreach (var str in tmp)
            sb.Append(str);
        AlignByByteSize(sb);        
        var correctionLevel = GetCorrectionLevelAndVersion(sb.ToString(), qrCodeVersion);   
        // Блоки с байтами коррекции
        var codeText = FillCode(sb.ToString(), _maxData[(correctionLevel, qrCodeVersion)]);        
        var correctionBlock = GetCorrectionBlock(SplitByBlock(codeText), _correctionLevelBytesSize[correctionLevel][qrCodeVersion]);
        var code = SetCorrectionBlock(correctionBlock, codeText); 
        // Информация о маске и уровне коррекции
        var maskNum = 2;
        AddMaskNumAndCorrectionLevel(qrCodeMatrix, correctionLevel, maskNum);         
        // Занесение в матрицу
        PutInMatrix(qrCodeMatrix, code);
        // Нанесение макси
        MaskInvertMatrix(qrCodeMatrix, GetMaskMatrix(maskNum));
        return Combine(_matrixTemplate, qrCodeMatrix);
    }
}
