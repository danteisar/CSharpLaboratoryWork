using System.Diagnostics.Metrics;
using System.Text;

namespace DataMatrixGenerator;

/// <summary>
/// https://habr.com/ru/articles/241887/
/// </summary>
public static class DataMatrixBuilder
{
    /// <summary>
    ///  ToDo Необходимо восстановить функцию, она почему то сейчас возвращает не Data Matrix, а исходный текст
    /// </summary>
    public static string ToDataMatrix(this string text, int? rows = null, int? cols = null, bool invert = false)
    {
        IsInvert = invert;

        var data = Encode(text);

        (var version, var size) = GetMatrixSize(rows, cols, data.Length);

        data = AddPaddingCodes(data, size);

        data = CalculateEcc(data, version);

        var dataMatrix = Render(data, version);

        var res = CreateDataMatrix(dataMatrix, version);

        return res.BuildString();
    }

    #region EncodeText

    private const char ZERO = '0';

    private const byte EXTENDED_ASCII = 235;

    /// <summary>
    /// 
    /// </summary>
    private static byte[] Encode(string text)
    {
        var result = new List<byte>();

        for (var i = 0; i < text.Length; i++)
        {
            var first = text[i];
            
            if (!char.IsDigit(first))
            {
                result.Add(first);
                continue;
            }

            if (i < text.Length - 1)
            {
                var second = text[i + 1];

                if (char.IsDigit(second))
                {
                    result.Add(first, second);
                    i++;
                }
            }
            
        }
        return [.. result];
    }

    private static void Add(this List<byte> bytes, char first, char second)
    {
        bytes.Add((byte)((first - ZERO) * 10 + (second - ZERO) + 130));
    }

    private static void Add(this List<byte> bytes, char letter)
    {
        if (letter <= 127)
        {
            bytes.Add((byte)(letter + 1));
        }
        else
        {
            bytes.AddRange([EXTENDED_ASCII, (byte)(letter - 127)]);
        }
    }

    #endregion

    #region Matrix

    private static readonly List<MatrixSize> _supprotedVersions =
    [
        (10, 10, RegionCounts.One, RegionCounts.One, 5, 1),
        (12, 12, RegionCounts.One, RegionCounts.One, 7, 1),
        (14, 14, RegionCounts.One, RegionCounts.One, 10, 1),
        (16, 16, RegionCounts.One, RegionCounts.One, 12, 1),
        (18, 18, RegionCounts.One, RegionCounts.One, 14, 1),
        (20, 20, RegionCounts.One, RegionCounts.One, 18, 1),
        (22, 22, RegionCounts.One, RegionCounts.One, 20, 1),
        (24, 24, RegionCounts.One, RegionCounts.One, 24, 1),
        (26, 26, RegionCounts.One, RegionCounts.One, 28, 1),
        (32, 32, RegionCounts.Two, RegionCounts.Two, 36, 1),
        (36, 36, RegionCounts.Two, RegionCounts.Two, 42, 1),
        (40, 40, RegionCounts.Two, RegionCounts.Two, 48, 1),
        (44, 44, RegionCounts.Two, RegionCounts.Two, 56, 1),
        (48, 48, RegionCounts.Two, RegionCounts.Two, 68, 1),
        (52, 52, RegionCounts.Two, RegionCounts.Two, 84, 2),
        (64, 64, RegionCounts.For, RegionCounts.For, 112, 2),
        (72, 72, RegionCounts.For, RegionCounts.For, 144, 4),
        (80, 80, RegionCounts.For, RegionCounts.For, 192, 4),
        (88, 88, RegionCounts.For, RegionCounts.For, 224, 4),
        (96, 96, RegionCounts.For, RegionCounts.For, 272, 4),
        (104, 104, RegionCounts.For, RegionCounts.For, 336, 6),
        (120, 120, RegionCounts.Six, RegionCounts.Six, 408, 6),
        (132, 132, RegionCounts.Six, RegionCounts.Six, 496, 8),
        (144, 144, RegionCounts.Six, RegionCounts.Six, 620, 10),
        (8, 18, RegionCounts.One, RegionCounts.One, 7, 1),
        (8, 32, RegionCounts.Two, RegionCounts.One, 11, 1),
        (12, 26, RegionCounts.One, RegionCounts.One, 14, 1),
        (12, 36, RegionCounts.Two, RegionCounts.One, 18, 1),
        (16, 36, RegionCounts.Two, RegionCounts.One, 24, 1),
        (16, 48, RegionCounts.Two, RegionCounts.One, 28, 1),
        ];

    /// <summary>
    /// 
    /// </summary>
    private static (MatrixSize, int) GetMatrixSize(int? fixedNumberOfRows, int? fixedNumberOfColumns, int dataLength)
    {
        IEnumerable<MatrixSize> codeSizes = _supprotedVersions;

        if (fixedNumberOfRows.HasValue)
            codeSizes = codeSizes.Where(x => x.Rows == fixedNumberOfRows.Value);

        if (fixedNumberOfColumns.HasValue)
            codeSizes = codeSizes.Where(x => x.Columns == fixedNumberOfColumns.Value);


        var res = codeSizes.Select(x => (x, x.MatrixColumns() * x.MatrixRows() / 8 - x.EccCount))
            .FirstOrDefault(x => x.Item2 >= dataLength);

        if (res.x is null) throw new InvalidOperationException($"Current Data Matrix does not support");

        return res;
    }

    /// <summary>
    /// 
    /// </summary>
    private static byte[] AddPaddingCodes(byte[] data, int to)
    {
        if (data.Length == to) return data;

        var result = new List<byte>(data);

        if (result.Count < to)
            result.Add(129);

        while (result.Count < to)
        {
            result.Add((byte)((129 + ((149 * (result.Count + 1)) % 253 + 1)) % 255));
        }

        return [.. result];
    }

    private static int RegionRows(this MatrixSize version) => (version.Rows - (int)version.VerticalRegions * 2) / (int)version.VerticalRegions;

    private static int RegionColumns(this MatrixSize version) => (version.Columns - (int)version.HorizontalRegions * 2) / (int)version.HorizontalRegions;

    private static int MatrixRows(this MatrixSize version) => version.RegionRows() * (int)version.VerticalRegions;

    private static int MatrixColumns(this MatrixSize version) => version.RegionColumns() * (int)version.HorizontalRegions;

    private static int GetCountDataCodewordsForBlock(this MatrixSize version, int index) => version.Rows == 144 && version.Columns == 144
            ? index < 8 ? 156 : 155
            : (version.MatrixColumns() * version.MatrixRows() / 8 - version.EccCount) / version.BlockCount;

    private static int ErrorCorrectionCodewordsPerBlock(this MatrixSize version) => version.EccCount / version.BlockCount;

    #endregion

    #region ECC

    private const int Size = 256;
    private const int Base = 1;

    private static readonly byte[] _galoisField = [
        1,2,4,8,16,32,64,128,45,90,180,69,138,57,114,228,229,231,227,235,
        251,219,155,27,54,108,216,157,23,46,92,184,93,186,89,178,73,146,9,18,
        36,72,144,13,26,52,104,208,141,55,110,220,149,7,14,28,56,112,224,237,247,195,171,123,246,193,175,115,230,225,239,243,203,187,91,182,65,130,41,82,
        164,101,202,185,95,190,81,162,105,210,137,63,126,252,213,135,35,70,140,53,
        106,212,133,39,78,156,21,42,84,168,125,250,217,159,19,38,76,152,29,58,
        116,232,253,215,131,43,86,172,117,234,249,223,147,11,22,44,88,176,77,154,
        25,50,100,200,189,87,174,113,226,233,255,211,139,59,118,236,245,199,163,107,
        214,129,47,94,188,85,170,121,242,201,191,83,166,97,194,169,127,254,209,143,
        51,102,204,181,71,142,49,98,196,165,103,206,177,79,158,17,34,68,136,61,
        122,244,197,167,99,198,161,111,222,145,15,30,60,120,240,205,183,67,134,33,
        66,132,37,74,148,5,10,20,40,80,160,109,218,153,31,62,124,248,221,151,
        3,6,12,24,48,96,192,173,119,238,241,207,179,75,150,1,
    ];

    private static readonly byte[] _backGaloisField = [
        0,255,1,240,2,225,241,53,3,38,226,133,242,43,54,210,4,195,39,114,227,106,134,
        28,243,140,44,23,55,118,211,234,5,219,196,96,40,222,115,103,228,78,107,125,135,
        8,29,162,244,186,141,180,45,99,24,49,56,13,119,153,212,199,235,91,6,76,220,217,
        197,11,97,184,41,36,223,253,116,138,104,193,229,86,79,171,108,165,126,145,136,
        34,9,74,30,32,163,84,245,173,187,204,142,81,181,190,46,88,100,159,25,231,50,207,
        57,147,14,67,120,128,154,248,213,167,200,63,236,110,92,176,7,161,77,124,221,102,
        218,95,198,90,12,152,98,48,185,179,42,209,37,132,224,52,254,239,117,233,139,22,
        105,27,194,113,230,206,87,158,80,189,172,203,109,175,166,62,127,247,146,66,137,
        192,35,252,10,183,75,216,31,83,33,73,164,144,85,170,246,65,174,61,188,202,205,
        157,143,169,82,72,182,215,191,251,47,178,89,151,101,94,160,123,26,112,232,21,
        51,238,208,131,58,69,148,18,15,16,68,17,121,149,129,19,155,59,249,70,214,250,
        168,71,201,156,64,60,237,130,111,20,93,122,177,150
    ];

    /// <summary>
    /// 
    /// </summary>
    private static byte[] CalculateEcc(byte[] data, MatrixSize version)
    {
        List<GaoisFieldPolynome> polynomes = [new([1])];

        var dataSize = data.Length;
        var result = new byte[data.Length + version.EccCount];

        Array.Copy(data, result, data.Length);

        for (int block = 0; block < version.BlockCount; block++)
        {
            var dataCnt = version.GetCountDataCodewordsForBlock(block);
            var buff = new int[dataCnt];
            var j = 0;
            for (int i = block; i < dataSize; i += version.BlockCount)
            {
                buff[j] = result[i];
                j++;
            }
            var ecc = Encode(buff, version.ErrorCorrectionCodewordsPerBlock(), polynomes);
            j = 0;
            for (int i = block; i < version.ErrorCorrectionCodewordsPerBlock() * version.BlockCount; i += version.BlockCount)
            {
                result[dataSize + i] = (byte)ecc[j];
                j++;
            }
        }

        return result;
    }

    private static int[] Encode(int[] data, int eccCount, List<GaoisFieldPolynome> polynomes)
    {
        GaoisFieldPolynome generator = GetPolynomial(eccCount, polynomes);
        GaoisFieldPolynome info = new(data);
        info = info.MultiplyByMonominal(eccCount, 1);
        GaoisFieldPolynome remainder = info.Divide(generator);
        int[] result = new int[eccCount];
        int numZero = eccCount - remainder.Coefficients.Length;
        Array.Copy(remainder.Coefficients, 0, result, numZero, remainder.Coefficients.Length);
        return result;
    }

    private static GaoisFieldPolynome GetPolynomial(int degree, List<GaoisFieldPolynome> polynomes)
    {
        if (degree >= polynomes.Count)
        {
            for (int d = polynomes.Count; d <= degree; d++)
            {
                var next = polynomes[^1].Multiply(new GaoisFieldPolynome([1, _galoisField[d - 1 + Base]]));
                polynomes.Add(next);
                polynomes[^1] = next;
            }
        }
        return polynomes[degree];
    }

    private static int Multiply(int a, int b)
    {
        if (a == 0 || b == 0) return 0;

        return _galoisField[(_backGaloisField[a] + _backGaloisField[b]) % (Size - 1)];
    }

    private static int Degree(this GaoisFieldPolynome x) => x.Coefficients.Length - 1;

    private static bool IsZero(this GaoisFieldPolynome x) => x.Coefficients[0] == 0;

    private static GaoisFieldPolynome AddOrSubtract(this GaoisFieldPolynome x, GaoisFieldPolynome other)
    {
        if (x.IsZero()) return other;
        if (other.IsZero()) return x;

        var smallCoefficients = x.Coefficients;
        var largeCoefficients = other.Coefficients;
        if (smallCoefficients.Length > largeCoefficients.Length)
        {
            (largeCoefficients, smallCoefficients) = (smallCoefficients, largeCoefficients);
        }
        var sumDiff = new int[largeCoefficients.Length];
        var lenDiff = largeCoefficients.Length - smallCoefficients.Length;
        Array.Copy(largeCoefficients, sumDiff, lenDiff);
        for (var i = lenDiff; i < largeCoefficients.Length; i++)
        {
            sumDiff[i] = smallCoefficients[i - lenDiff] ^ largeCoefficients[i];
        }
        return new(sumDiff);
    }

    private static GaoisFieldPolynome MultiplyByMonominal(this GaoisFieldPolynome x, int degree, int coefficient)
    {
        if (coefficient == 0) return 0;

        var size = x.Coefficients.Length;
        var coefficients = new int[size + degree];
        for (int i = 0; i < size; i++)
        {
            coefficients[i] = Multiply(x.Coefficients[i], coefficient);
        }
        return new(coefficients);
    }

    private static GaoisFieldPolynome Multiply(this GaoisFieldPolynome x, GaoisFieldPolynome other)
    {
        if (x.IsZero() || other.IsZero()) return 0;

        var product = new int[x.Coefficients.Length + other.Coefficients.Length - 1];
        for (int i = 0; i < x.Coefficients.Length; i++)
        {
            int coefficient = x.Coefficients[i];
            for (int j = 0; j < other.Coefficients.Length; j++)
            {
                product[i + j] = product[i + j] ^ Multiply(coefficient, other.Coefficients[j]);
            }
        }
        return new(product);
    }

    private static GaoisFieldPolynome Divide(this GaoisFieldPolynome x, GaoisFieldPolynome other)
    {
        var remainder = x;
        var denomLeadTerm = other.Coefficients[other.Degree() - other.Degree()];
        var inversDenomLeadTerm = _galoisField[Size - 1 - _backGaloisField[denomLeadTerm]];
        while (remainder.Degree() >= other.Degree() && !remainder.IsZero())
        {
            int degreeDiff = remainder.Degree() - other.Degree();
            int scale = Multiply(remainder.Coefficients[0], inversDenomLeadTerm);
            var term = other.MultiplyByMonominal(degreeDiff, scale);
            remainder = remainder.AddOrSubtract(term);
        }
        return remainder;
    }

    #endregion

    #region Bits List    

    private static uint[] Render(byte[] data, MatrixSize version)
    {
        uint[] matrix = CreateBitsArray(version.MatrixColumns() * version.MatrixRows());
        uint[] blocked = CreateBitsArray(version.MatrixColumns() * version.MatrixRows());

        SetValues(version, blocked, matrix, data);

        return FillData(version, matrix);
    }

    private static uint[] CreateBitsArray(int capacity)
    {
        var x = 0;
        if ((capacity % 32) != 0)
            x = 1;
        return new uint[capacity / 32 + x];
    }

    private static void SetBit(this uint[] data, int index, bool value)
    {
        var itemIndex = index / 32;
        var itemBitShift = 31 - (index % 32);
        if (value)
            data[itemIndex] |= (uint)1 << itemBitShift;
        else
            data[itemIndex] &= ~((uint)1 << itemBitShift);
    }

    private static bool GetBit(this uint[] data, int index)
    {
        var itemIndex = index / 32;
        var itemBitShift = 31 - (index % 32);
        return ((data[itemIndex] >> itemBitShift) & 1) == 1;
    }

    private static void SetValues(MatrixSize version, uint[] blocked, uint[] matrix, byte[] data)
    {
        var idx = 0;
        var row = 4;
        var col = 0;

        while (row < version.MatrixRows() || col < version.MatrixColumns())
        {
            if (row == version.MatrixRows() && col == 0)
            {
                SetCornerType1(version, blocked, matrix, data[idx]);
                idx++;
            }

            if (row == version.MatrixRows() - 2 && col == 0 && (version.MatrixColumns() % 4) != 0)
            {
                SetCornerType2(version, blocked, matrix, data[idx]);
                idx++;
            }

            if (row == version.MatrixRows() - 2 && col == 0 && (version.MatrixColumns() % 8) == 4)
            {
                SetCornerType3(version, blocked, matrix, data[idx]);
                idx++;
            }

            if (row == version.MatrixRows() + 4 && col == 2 && (version.MatrixColumns() % 8) == 0)
            {
                SetCornerType4(version, blocked, matrix, data[idx]);
                idx++;
            }

            while (true)
            {
                if (row < version.MatrixRows() && col >= 0 && !Blocked(version, blocked, row, col))
                {
                    SetOrdinary(version, blocked, matrix, row, col, data[idx]);
                    idx++;
                }

                row -= 2;
                col += 2;

                if (row < 0 || col >= version.MatrixColumns())
                    break;
            }

            row += 1;
            col += 3;

            while (true)
            {
                if (row >= 0 && col < version.MatrixColumns() && !Blocked(version, blocked, row, col))
                {
                    SetOrdinary(version, blocked, matrix, row, col, data[idx]);
                    idx++;
                }

                row += 2;
                col -= 2;

                if (row >= version.MatrixRows() || col < 0)
                    break;
            }

            row += 3;
            col += 1;
        }

        if (!Blocked(version, blocked, version.MatrixRows() - 1, version.MatrixColumns() - 1))
        {
            Set(version, blocked, matrix, version.MatrixRows() - 1, version.MatrixColumns() - 1, 255, 0);
            Set(version, blocked, matrix, version.MatrixRows() - 2, version.MatrixColumns() - 2, 255, 0);
        }
    }

    private static bool Blocked(MatrixSize version, uint[] blocked, int row, int col) => blocked.GetBit(col + row * version.MatrixColumns());

    private static void Set(MatrixSize version, uint[] blocked, uint[] matrix, int row, int col, byte value, byte bitNum)
    {
        if (row < 0)
        {
            row += version.MatrixRows();
            col += 4 - ((version.MatrixRows() + 4) % 8);
        }

        if (col < 0)
        {
            col += version.MatrixColumns();
            row += 4 - ((version.MatrixColumns() + 4) % 8);
        }

        if (Blocked(version, blocked, row, col))
            throw new InvalidOperationException($"Failed to set data by row: {row} col: {col}");

        blocked.SetBit(col + row * version.MatrixColumns(), true);

        bool val = ((value >> (7 - bitNum)) & 1) == 1;
        matrix.SetBit(col + row * version.MatrixColumns(), val);
    }

    private static void SetOrdinary(MatrixSize version, uint[] blocked, uint[] matrix, int row, int col, byte value)
    {
        Set(version, blocked, matrix, row - 2, col - 2, value, 0);
        Set(version, blocked, matrix, row - 2, col - 1, value, 1);
        Set(version, blocked, matrix, row - 1, col - 2, value, 2);
        Set(version, blocked, matrix, row - 1, col - 1, value, 3);
        Set(version, blocked, matrix, row - 1, col - 0, value, 4);
        Set(version, blocked, matrix, row - 0, col - 2, value, 5);
        Set(version, blocked, matrix, row - 0, col - 1, value, 6);
        Set(version, blocked, matrix, row - 0, col - 0, value, 7);
    }

    private static void SetCornerType1(MatrixSize version, uint[] blocked, uint[] matrix, byte value)
    {
        Set(version, blocked, matrix, version.MatrixRows() - 1, 0, value, 0);
        Set(version, blocked, matrix, version.MatrixRows() - 1, 1, value, 1);
        Set(version, blocked, matrix, version.MatrixRows() - 1, 2, value, 2);
        Set(version, blocked, matrix, 0, version.MatrixColumns() - 2, value, 3);
        Set(version, blocked, matrix, 0, version.MatrixColumns() - 1, value, 4);
        Set(version, blocked, matrix, 1, version.MatrixColumns() - 1, value, 5);
        Set(version, blocked, matrix, 2, version.MatrixColumns() - 1, value, 6);
        Set(version, blocked, matrix, 3, version.MatrixColumns() - 1, value, 7);
    }

    private static void SetCornerType2(MatrixSize version, uint[] blocked, uint[] matrix, byte value)
    {
        Set(version, blocked, matrix, version.MatrixRows() - 3, 0, value, 0);
        Set(version, blocked, matrix, version.MatrixRows() - 2, 0, value, 1);
        Set(version, blocked, matrix, version.MatrixRows() - 1, 0, value, 2);
        Set(version, blocked, matrix, 0, version.MatrixColumns() - 4, value, 3);
        Set(version, blocked, matrix, 0, version.MatrixColumns() - 3, value, 4);
        Set(version, blocked, matrix, 0, version.MatrixColumns() - 2, value, 5);
        Set(version, blocked, matrix, 0, version.MatrixColumns() - 1, value, 6);
        Set(version, blocked, matrix, 1, version.MatrixColumns() - 1, value, 7);
    }

    private static void SetCornerType3(MatrixSize version, uint[] blocked, uint[] matrix, byte value)
    {
        Set(version, blocked, matrix, version.MatrixRows() - 3, 0, value, 0);
        Set(version, blocked, matrix, version.MatrixRows() - 2, 0, value, 1);
        Set(version, blocked, matrix, version.MatrixRows() - 1, 0, value, 2);
        Set(version, blocked, matrix, 0, version.MatrixColumns() - 2, value, 3);
        Set(version, blocked, matrix, 0, version.MatrixColumns() - 1, value, 4);
        Set(version, blocked, matrix, 1, version.MatrixColumns() - 1, value, 5);
        Set(version, blocked, matrix, 2, version.MatrixColumns() - 1, value, 6);
        Set(version, blocked, matrix, 3, version.MatrixColumns() - 1, value, 7);
    }

    private static void SetCornerType4(MatrixSize version, uint[] blocked, uint[] matrix, byte value)
    {
        Set(version, blocked, matrix, version.MatrixRows() - 1, 0, value, 0);
        Set(version, blocked, matrix, version.MatrixRows() - 1, version.MatrixColumns() - 1, value, 1);
        Set(version, blocked, matrix, 0, version.MatrixColumns() - 3, value, 2);
        Set(version, blocked, matrix, 0, version.MatrixColumns() - 2, value, 3);
        Set(version, blocked, matrix, 0, version.MatrixColumns() - 1, value, 4);
        Set(version, blocked, matrix, 1, version.MatrixColumns() - 3, value, 5);
        Set(version, blocked, matrix, 1, version.MatrixColumns() - 2, value, 6);
        Set(version, blocked, matrix, 1, version.MatrixColumns() - 1, value, 7);
    }

    private static uint[] FillData(MatrixSize version, uint[] matrix)
    {
        uint[] result = new uint[version.Rows * version.Columns];

        result
            .SetDottedHorizontal(version)
            .SetDottedVertical(version)
            .SetSolidHorizontal(version)
            .SetSolidVertical(version)
            .SetValues(version, matrix);

        return result;
    }

    private static uint[] SetValues(this uint[] result, MatrixSize version, uint[] matrix)
    {
        var count = 0;

        for (int hRegion = 0; hRegion < (int)version.HorizontalRegions; hRegion++)
        {
            for (int vRegion = 0; vRegion < (int)version.VerticalRegions; vRegion++)
            {
                for (int x = 0; x < version.RegionColumns(); x++)
                {
                    int colMatrix = (version.RegionColumns() * hRegion) + x;
                    int colResult = ((2 + version.RegionColumns()) * hRegion) + x + 1;

                    for (int y = 0; y < version.RegionRows(); y++)
                    {
                        int rowMatrix = (version.RegionRows() * vRegion) + y;
                        int rowResult = ((2 + version.RegionRows()) * vRegion) + y + 1;

                        bool val = matrix.GetBit(colMatrix + rowMatrix * version.MatrixColumns());

                        if (val)
                        {
                            count++;
                        }

                        result.SetBit(colResult * version.Rows + rowResult, val);
                    }
                }
            }
        }

        return result;
    }

    private static uint[] SetDottedHorizontal(this uint[] result, MatrixSize version)
    {
        for (int row = 0; row < version.Rows; row += (version.RegionRows() + 2))
        {
            for (int col = 0; col < version.Columns; col += 2)
            {
                result.SetBit(col * version.Rows + row, true);
            }
        }

        return result;
    }

    private static uint[] SetSolidHorizontal(this uint[] result, MatrixSize version)
    {
        for (int row = version.RegionRows() + 1; row < version.Rows; row += (version.RegionRows() + 2))
        {
            for (int rol = 0; rol < version.Columns; rol++)
            {
                result.SetBit(rol * version.Rows + row, true);
            }
        }

        return result;
    }

    private static uint[] SetDottedVertical(this uint[] result, MatrixSize version)
    {
        for (int col = version.RegionColumns() + 1; col < version.Columns; col += (version.RegionColumns() + 2))
        {
            for (int row = 1; row < version.Rows; row += 2)
            {
                result.SetBit(col * version.Rows + row, true);
            }
        }

        return result;
    }

    private static uint[] SetSolidVertical(this uint[] result, MatrixSize version)
    {
        for (int col = 0; col < version.Columns; col += (version.RegionColumns() + 2))
        {
            for (int row = 0; row < version.Rows; row++)
            {
                result.SetBit(col * version.Rows + row, true);
            }
        }

        return result;
    }

    #endregion

    #region Create Result

    /// <summary>
    /// 
    /// </summary>
    private static List<byte[]> CreateDataMatrix(uint[] code, MatrixSize version)
    {
        var sizeWithBorder = GetMatrixSize(int.Max(version.Columns, version.Rows));

        var dataMatrix = CreateDataMatrix(sizeWithBorder);

        for (int i = 0; i < version.Columns; i++)
        {
            for (int j = 0; j < version.Rows; j++)
            {
                dataMatrix[i + BORDER][j + BORDER] = code.Get(version, j, i) ? EMPTY : ACTIVE;
            }
        }

        return dataMatrix;
    }

    private static int GetMatrixSize(int size) => size + BORDER * 2;

    private static List<byte[]> CreateDataMatrix(int size, byte value = ACTIVE)
    {
        List<byte[]> qrCodeMatrix = [];
        for (int i = 0; i < size; i++)
        {
            qrCodeMatrix.Add(new byte[size]);
        }
        return qrCodeMatrix.FillMatrix(value);
    }

    private static List<byte[]> FillMatrix(this List<byte[]> matrix, byte value)
    {
        for (int i = 0; i < matrix.Count; i++)
        {
            for (int j = 0; j < matrix.Count; j++)
            {
                matrix[i][j] = value;
            }
        }
        return matrix;
    }

    private static bool Get(this uint[] data, MatrixSize version, int x, int y) => data.GetBit(x * version.Rows + y);

    #endregion

    #region Build string

    /// <summary>
    /// Активный модуль
    /// </summary>
    private const byte ACTIVE = 1;

    /// <summary>
    /// Неактивный модуль
    /// </summary>
    private const byte EMPTY = 0;

    /// <summary>
    /// Invert string <see cref="CharTemplate"/>
    /// </summary>
    private static bool IsInvert { get; set; }

    /// <summary>
    ///  ▄▀█ - 4 chars string
    /// </summary>
    private static string CharTemplate { get; set; } = " ▄▀█";

    /// <summary>
    /// Пустая рамка вокруг дата матрицы для распознавания на темном фоне
    /// </summary>
    private const int BORDER = 2;

    /// <summary>
    ///
    /// </summary>
    private static string BuildString(this List<byte[]> matrix)
    {
        var sb = new StringBuilder();
        sb.DuoFont(matrix);
        return sb.ToString();
    }

    private static StringBuilder DuoFont(this StringBuilder sb, List<byte[]> matrix)
    {
        var length = matrix.Count % 2 == 1 ? matrix.Count + 1 : matrix.Count;
        for (int row = 0; row < length; row += 2)
        {
            for (int column = 0; column < matrix[0].Length; column++)
            {
                byte scanModule1 = matrix[row][column];
                byte scanModule2 = row < matrix.Count - 1 ? matrix[row + 1][column] : ACTIVE;
                sb.Append(IsInvert ? ScanInvert(scanModule1, scanModule2) : Scan(scanModule1, scanModule2));
            }
            sb.AppendLine();
        }
        return sb;
    }

    private static char Scan(byte a, byte b)
        => (a, b) switch
        {
            (0, 0) => CharTemplate[0],
            (0, 1) => CharTemplate[1],
            (1, 0) => CharTemplate[2],
            (1, 1) => CharTemplate[3],
            _ => throw new NotImplementedException(),
        };

    private static char ScanInvert(byte a, byte b)
        => (a, b) switch
        {
            (0, 0) => CharTemplate[^1],
            (0, 1) => CharTemplate[^2],
            (1, 0) => CharTemplate[^3],
            (1, 1) => CharTemplate[^4],
            _ => throw new NotImplementedException(),
        };

    #endregion
}
