using System.Text;

namespace QrCodeGenerator;

internal static class QrCodeBuilder2
{
    #region GET

    public static string GetQrCode(string text, byte qrCodeVersion, CodeType codeType)
    {    
        // Блоки с данными
        var sb = new StringBuilder();        
        sb.Append(GetServiceInformation(codeType, text));        
        var tmp = codeType switch {
            CodeType.AlphaNumeric => EncodeAlphaNumeric(text.ToUpper()),
            CodeType.Numeric => EncodeNumeric(text),
            _ => EncodeBinary(text)
        };
        foreach (var str in tmp)
            sb.Append(str);
        AlignByByteSize(sb); 
        var encodedData = sb.ToString(); 
        (var correctionLevel, qrCodeVersion) = GetCorrectionLevelAndVersion(encodedData.Length, qrCodeVersion); 

        // Блоки с байтами коррекции
        var length = _maxData[(correctionLevel, qrCodeVersion)];
        var codeText = FillCode(encodedData, length);        
        var correctionBlock = GetCorrectionBlock(SplitByBlock(codeText), _correctionLevelBytesSize[correctionLevel][qrCodeVersion]);
        var data = SetCorrectionBlock(correctionBlock, codeText); 
               
        // Создание матрицы QR кода c лучшей маской      
        var qrCodeMatrix = new QrCodeData
        { 
            Version = qrCodeVersion,
            CorrectionLevel = correctionLevel,
            Data = data,
        }
        .GetBestMatrix();

        return BuildString(qrCodeMatrix);
    }

    #endregion

    #region Base Matrix

    /// <summary>
    /// Граница в два модуля вокруг QR-кода
    /// </summary>
    private const int BORDER = 2;

    /// <summary>
    /// Активный модуль
    /// </summary>
    private const byte ACTIVE = 1;

    /// <summary>
    /// Неактивный модуль
    /// </summary>
    private const byte ZERO = 0;

    /// <summary>
    /// Сборка матрицы в готовую строку QR кода
    /// </summary>
    private static string BuildString(List<byte[]> matrix)
    {
        var sb = new StringBuilder();
        for (int y = 0; y<matrix.Count; y+=2)
        {
            for(int x = 0; x<matrix[0].Length; x++)
            {
                sb.Append(Scan(matrix[y][x], matrix[y+1][x]));  
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
    
    /// <summary>
    /// преобразования данных с модуля в символ QR кода
    /// </summary>
    private static char Scan(byte a, byte b)
        => (a, b) switch
        {
            (0, 0) => ' ',
            (0, 1) => '▄',
            (1, 0) => '▀',
            (1, 1) => '█',
            _ => throw new NotImplementedException(),
        };

    /// <summary>
    /// Создание болванки матрицы, заполненный <see cref="ACTIVE"/>
    /// </summary>
    private static List<byte[]> CreateQrCodeMatrix(int size)
    {
        List<byte[]> qrCodeMatrix = [];       
        for (int i = 0; i < size + 1; i++)
        {
            qrCodeMatrix.Add(new byte[size]);
        }
        return qrCodeMatrix.FillMatrix();
    }

    /// <summary>
    /// Поддерживаемые версии QR кодов
    /// </summary>
    private static readonly Dictionary<byte, byte> _matrixSizeByVersion = new()
    {
        {1, 21 + BORDER * 2},
        {2, 25 + BORDER * 2},
        {3, 29 + BORDER * 2},
        {4, 33 + BORDER * 2},
        {5, 37 + BORDER * 2},
        {6, 41 + BORDER * 2}
    };

    #endregion

    #region Fill Matrix

    private static List<byte[]> AddPosition(this List<byte[]> matrix, int x, int y)
    {
        FillCube(matrix, x - 4, y - 4, 9, 1);
        FillCube(matrix, x - 3, y - 3, 7, 0);
        FillCube(matrix, x - 2, y - 2, 5, 1);
        FillCube(matrix, x - 1, y - 1, 3, 0);
        return matrix;
    }

    private static List<byte[]> FillMatrix(this List<byte[]> matrix)
    {
        for (int i = 0; i < matrix.Count; i++)
        {
            for (int j = 0; j < matrix[0].Length; j++)
            {
                matrix[i][j] = 1;
            }
        }  
        return matrix;      
    }

    private static List<byte[]> AddAlignment(this List<byte[]> matrix, int x, int y)
    {
        FillCube(matrix, x - 2, y - 2, 5, 0);
        FillCube(matrix, x - 1, y - 1, 3, 1);
        matrix[x][y] = 0;
        return matrix;
    }

    private static List<byte[]> AddTiming(this List<byte[]> matrix)
    {
        for (int i = BORDER; i < matrix.Count - BORDER - 1; i++)
        {
            matrix[i][BORDER + 6] = (byte)(i % 2);
        }
        for (int i = BORDER; i < matrix[0].Length - BORDER; i++)
        {
            matrix[BORDER + 6][i] = (byte)(i % 2);
        }
        return matrix;
    }

    private static List<byte[]> FillCube(this List<byte[]> matrix, int x, int y, int size, byte value)
    {
        for(int i = 0; i<size; i++)
        {
            for(int j = 0; j<size; j++)
            {
                matrix[i + x][j + y] = value;
            }
        }
        return matrix;
    }

    private static List<byte[]> Fill(this List<byte[]> matrix, int x, int y, byte value)
    {
        matrix[x][y] = value;
        return matrix;
    }

    private static readonly Dictionary<int, int> _alignmentsPosition = new()
    {
        {1, -1},
        {2, 18},
        {3, 22},
        {4, 26},
        {5, 30},
        {6, 34}
    };

    #endregion

    #region FillData

    private const int O = 0, Y = 0, Q = 0, H = 0;

    private static readonly List<int[]> _orderMatrix =
    [
        [O, O,   O,   O,     O,   O,     O,   O, O,     O,   O,     O,   O,    O,   O,    O,  O,    O,  O,    O,  O,    O,  O,   O, O],        
        [O, O,   O,   O,     O,   O,     O,   O, O,     O,   O,     O,   O,    O,   O,    O,  O,    O,  O,    O,  O,    O,  O,   O, O],
        [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   138, 137,  136, 135,    Y,  0,    0,  0,    0,  0,    0,  0,   O, O],
        [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   140, 139,  134, 133,    Y,  0,    0,  0,    0,  0,    0,  0,   O, O],
        [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   142, 141,  132, 131,    Y,  0,    0,  0,    0,  0,    0,  0,   O, O],
        [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   144, 143,  130, 129,    Y,  0,    0,  0,    0,  0,    0,  0,   O, O],
        [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   146, 145,  128, 127,    Y,  0,    0,  0,    0,  0,    0,  0,   O, O],
        [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   148, 147,  126, 125,    Y,  0,    0,  0,    0,  0,    0,  0,   O, O],
        [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   Q,     Q,   Q,    Q,   Q,    Y,  0,    0,  0,    0,  0,    0,  0,   O, O],
        [O, O,   Y,   Y,     Y,   Y,     Y,   Y, Y,     Y,   H,   150, 149,  124, 123,    Y,  Y,    Y,  Y,    Y,  Y,    Y,  Y,   O, O],
        [O, O,   H,   H,     H,   H,     H,   H, Q,     H,   H,   152, 151,  122, 121,    H,  H,    H,  H,    H,  H,    H,  H,   O, O], 
        [O, O, 202, 201,   200, 199,   186, 185, Q,   184, 183,   154, 153,  120, 119,   74, 73,   72, 71,   26, 25,   24, 23,   O, O], 
        [O, O, 204, 203,   198, 197,   188, 187, Q,   182, 181,   156, 155,  118, 117,   76, 75,   70, 69,   28, 27,   22, 21,   O, O], 
        [O, O, 206, 205,   196, 195,   190, 189, Q,   180, 179,   158, 157,  116, 115,   78, 77,   68, 67,   30, 29,   20, 19,   O, O], 
        [O, O, 208, 207,   194, 193,   192, 191, Q,   178, 177,   160, 159,  114, 113,   80, 79,   66, 65,   32, 31,   18, 17,   O, O],    
        [O, O,   Y,   Y,     Y,   Y,     Y,   Y, Y,     Y,   Q,   162, 161,  112, 111,   82, 81,   64, 63,   34, 33,   16, 15,   O, O],  
        [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   164, 163,  110, 109,   84, 83,   62, 61,   36, 35,   14, 13,   O, O], 
        [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   166, 165,  108, 107,   86, 85,   60, 59,   38, 37,   12, 11,   O, O], 
        [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   168, 167,  106, 105,   88, 87,   58, 57,   40, 39,   10, 09,   O, O], 
        [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   170, 169,  104, 103,   90, 89,   56, 55,   42, 41,   08, 07,   O, O], 
        [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   172, 171,  102, 101,   92, 91,   54, 53,   44, 43,   06, 05,   O, O], 
        [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   174, 173,  100, 099,   94, 93,   52, 51,   46, 45,   04, 03,   O, O], 
        [O, O,   0,   0,     0,   0,     0,   0, 0,     Y,   H,   176, 175,  098, 097,   96, 95,   50, 49,   48, 47,   02, 01,   O, O],
        [O, O,   O,   O,     O,   O,     O,   O, O,     O,   O,     O,   O,    O,   O,    O,  O,    O,  O,    O,  O,    O,  O,   O, O],
        [O, O,   O,   O,     O,   O,     O,   O, O,     O,   O,     O,   O,    O,   O,    O,  O,    O,  O,    O,  O,    O,  O,   O, O],
        [O, O,   O,   O,     O,   O,     O,   O, O,     O,   O,     O,   O,    O,   O,    O,  O,    O,  O,    O,  O,    O,  O,   O, O]
    ];

    private static (int x, int y, bool found) SearchPlace(this List<int[]> matrix, int pos)
    {
        for (int x = BORDER; x<matrix.Count-BORDER-1; x++)
        {
            for (int y = BORDER; y<matrix[x].Length-BORDER; y++)
            {
                if (matrix[x][y]==pos)
                    return (x,y, true);
            }
        }
        return (default, default, false);
    }

    /// <summary>
    /// Занесение в матрицу данных
    /// </summary>
    private static List<byte[]> PutInMatrix(this List<byte[]> matrix, string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            char letter = text[i];
            (var x, var y, var found) = SearchPlace(_orderMatrix, i + 1);
            if (found)
                matrix[x][y] = letter != '1' ? ACTIVE : ZERO;
        }
        return matrix;
    }

    #endregion

    #region Decode

    private static readonly char[] letterNumberArray = {
                                                    '0','1','2','3','4','5','6','7','8','9',
                                                    'A','B','C','D','E','F','G','H','I','J',
                                                    'K','L','M','N','O','P','Q','R','S','T',
                                                    'U','V','W','X','Y','Z',' ','$','%','*',
                                                    '+','-','.','/',':'
                                                };

    private static void AddTo(List<string> list, int num, byte lengthType = 8)
    {
        var str = Convert.ToString(num, 2);
        list.Add(str.PadLeft(lengthType, '0'));
    }

    private static List<string> EncodeAlphaNumeric(string text)
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

    private static List<string> EncodeNumeric(string text)
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

    private static List<string> EncodeBinary(string text)
    {
        var res = new List<string>();        
        var numbers = Encoding.UTF8.GetBytes(text);
        foreach (var number in numbers)
        {
            AddTo(res, Convert.ToInt32(number));
        }        
        return res;
    }

    private static readonly Dictionary<CodeType, byte> _codeTypeSize = new()
    {
        {CodeType.Numeric, 10},
        {CodeType.AlphaNumeric, 9},
        {CodeType.Binary, 8}
    };

    private static readonly Dictionary<CodeType, string> _codeTypeMode = new()
    {
        {CodeType.Numeric,      "0001"},
        {CodeType.AlphaNumeric, "0010"},
        {CodeType.Binary,       "0100"}
    };

    private static string GetDataAmount(CodeType codeType, string text)
    {
        var length = codeType switch
        {
            CodeType.Binary => Encoding.UTF8.GetBytes(text).Length,
            _ => text.Length,
        };
        var str = Convert.ToString(length, 2).PadLeft(_codeTypeSize[codeType], '0');
        return str;
    }
    
    private static string GetServiceInformation(CodeType codeType, string text)
    { 
        return _codeTypeMode[codeType] + GetDataAmount(codeType, text);       
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

    #endregion

    #region Correction

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

    private static readonly byte[] _galuaField = [1,2,4,8,16,32,64,128,29,58,116,232,205,135,19,38,
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

    private static readonly byte[] _backGaluaField = [0,1,25,2,50,26,198,3,223,51,238,27,104,199,75,
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

            byte b = _backGaluaField[a-1];
            for (int x = 0; x < g.Length; x++)
            {
                var c = g[x] + b;
                if (c > 254)
                    c %= 255;
                var d = _galuaField[c];
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
        {(CorrectionLevel.L, 4), 640},

        {(CorrectionLevel.H, 5), 864},
        {(CorrectionLevel.Q, 5), 688},
        {(CorrectionLevel.M, 5), 496},
        {(CorrectionLevel.L, 5), 368},

        {(CorrectionLevel.H, 6), 1088},
        {(CorrectionLevel.Q, 6), 864},
        {(CorrectionLevel.M, 6), 608},
        {(CorrectionLevel.L, 6), 480}      
    };
    
    private static (CorrectionLevel correctionLevel, byte version) GetCorrectionLevelAndVersion(int length, byte version)
    {
        foreach (var pair in _maxData.Where(v => v.Key.version == version))
        {
            if (pair.Value > length)
                return (pair.Key.correctionLevel1, version);
        }

        if (version > 6)
            throw new NotSupportedException("Current version of QR-code does not support that data length!");

        return GetCorrectionLevelAndVersion(length, (byte)(version + 1));
    }

    #endregion

    #region Service Information

    private static readonly Dictionary<(CorrectionLevel correctionLevel, int maskNum), string> _masksAndCorrectionLevel = new()
    {
        {(CorrectionLevel.L, 0), "111011111000100"},
        {(CorrectionLevel.L, 1), "111001011110011"},
        {(CorrectionLevel.L, 2), "111110110101010"},
        {(CorrectionLevel.L, 3), "111100010011101"},
        {(CorrectionLevel.L, 4), "110011000101111"},
        {(CorrectionLevel.L, 5), "110001100011000"},
        {(CorrectionLevel.L, 6), "110110001000001"},
        {(CorrectionLevel.L, 7), "110100101110110"},        
        {(CorrectionLevel.M, 0), "101010000010010"},
        {(CorrectionLevel.M, 1), "101000100100101"},
        {(CorrectionLevel.M, 2), "101111001111100"},
        {(CorrectionLevel.M, 3), "101101101001011"},
        {(CorrectionLevel.M, 4), "100010111111001"},
        {(CorrectionLevel.M, 5), "100000011001110"},
        {(CorrectionLevel.M, 6), "100111110010111"},
        {(CorrectionLevel.M, 7), "100101010100000"},        
        {(CorrectionLevel.Q, 0), "011010101011111"},
        {(CorrectionLevel.Q, 1), "011000001101000"},
        {(CorrectionLevel.Q, 2), "011111100110001"},
        {(CorrectionLevel.Q, 3), "011101000000110"},
        {(CorrectionLevel.Q, 4), "010010010110100"},
        {(CorrectionLevel.Q, 5), "010000110000011"},
        {(CorrectionLevel.Q, 6), "010111011011010"},
        {(CorrectionLevel.Q, 7), "010101111101101"},
        {(CorrectionLevel.H, 0), "001011010001001"},
        {(CorrectionLevel.H, 1), "001001110111110"},
        {(CorrectionLevel.H, 2), "001110011100111"},
        {(CorrectionLevel.H, 3), "001100111010000"},
        {(CorrectionLevel.H, 4), "000011101100010"},
        {(CorrectionLevel.H, 5), "000001001010101"},
        {(CorrectionLevel.H, 6), "000110100001100"},
        {(CorrectionLevel.H, 7), "000100000111011"},
    };

    /// <summary>
    /// Информация о маске и уровне коррекции
    /// </summary>
    private static List<byte[]> AddMaskNumAndCorrectionLevel(this List<byte[]> matrix, CorrectionLevel level, int maskNum)
    {
        var maskNumAndCorrectionLevel = _masksAndCorrectionLevel[(level, maskNum)];
        for (int i = 0; i< maskNumAndCorrectionLevel.Length; i++)
        {
            PutInMatrix(matrix, _masksAndCorrectionLevelMatrixTemplate, i + 1, maskNumAndCorrectionLevel[i] == '1' ? ZERO : ACTIVE);
        }
        return matrix;      
    }

    private const byte o = 0, W= 0, A = 10, B = 11, C = 12, D = 13, E = 14, F = 15;

    private static readonly List<byte[]> _masksAndCorrectionLevelMatrixTemplate =
    [
        [o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o],        
        [o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o],
        [o, o, W, W, W, W, W, W, W, o, F, o, o, o, o, o, W, W, W, W, W, W, W, o, o],
        [o, o, W, o, o, o, o, o, W, o, E, o, o, o, o, o, W, o, o, o, o, o, W, o, o],
        [o, o, W, o, W, W, W, o, W, o, D, o, o, o, o, o, W, o, W, W, W, o, W, o, o],
        [o, o, W, o, W, W, W, o, W, o, C, o, o, o, o, o, W, o, W, W, W, o, W, o, o],
        [o, o, W, o, W, W, W, o, W, o, B, o, o, o, o, o, W, o, W, W, W, o, W, o, o],
        [o, o, W, o, o, o, o, o, W, o, A, o, o, o, o, o, W, o, o, o, o, o, W, o, o],
        [o, o, W, W, W, W, W, W, W, o, W, o, W, o, W, o, W, W, W, W, W, W, W, o, o],
        [o, o, o, o, o, o, o, o, o, o, 9, o, o, o, o, o, o, o, o, o, o, o, o, o, o],
        [o, o, 1, 2, 3, 4, 5, 6, W, 7, 8, o, o, o, o, o, 9, A, B, C, D, E, F, o, o],
        [o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o],
        [o, o, o, o, o, o, o, o, W, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o],
        [o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o],
        [o, o, o, o, o, o, o, o, W, o, W, o, o, o, o, o, o, o, o, o, o, o, o, o, o],
        [o, o, o, o, o, o, o, o, o, o, 8, o, o, o, o, o, o, o, o, o, o, o, o, o, o],
        [o, o, W, W, W, W, W, W, W, o, 7, o, o, o, o, o, o, o, o, o, o, o, o, o, o],
        [o, o, W, o, o, o, o, o, W, o, 6, o, o, o, o, o, o, o, o, o, o, o, o, o, o],
        [o, o, W, o, W, W, W, o, W, o, 5, o, o, o, o, o, o, o, o, o, o, o, o, o, o],
        [o, o, W, o, W, W, W, o, W, o, 4, o, o, o, o, o, o, o, o, o, o, o, o, o, o],
        [o, o, W, o, W, W, W, o, W, o, 3, o, o, o, o, o, o, o, o, o, o, o, o, o, o],
        [o, o, W, o, o, o, o, o, W, o, 2, o, o, o, o, o, o, o, o, o, o, o, o, o, o],
        [o, o, W, W, W, W, W, W, W, o, 1, o, o, o, o, o, o, o, o, o, o, o, o, o, o],
        [o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o],
        [o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o],
        [o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o, o]
    ];

    private static void PutInMatrix(List<byte[]> matrix, List<byte[]> maskMatrix, int index, byte value)
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

    #endregion

    #region Mask

    private static bool Mask0((int x, int y) m) => (m.x + m.y) % 2 == 0;
    private static bool Mask1((int x, int y) m) => m.y % 2 == 0;
    private static bool Mask2((int x, int y) m) => m.x % 3 == 0;
    private static bool Mask3((int x, int y) m) => (m.x + m.y) % 3 == 0;
    private static bool Mask4((int x, int y) m) => (m.x/3 + m.y/2) % 2 == 0;
    private static bool Mask5((int x, int y) m) => (m.x * m.y) % 2 + (m.x * m.y) % 3 == 0;
    private static bool Mask6((int x, int y) m) => ((m.x * m.y) % 2 + (m.x * m.y) % 3) % 2 == 0;
    private static bool Mask7((int x, int y) m) => ((m.x * m.y) % 3 + (m.x + m.y) % 2) % 2 == 0;

    /// <summary>
    /// Получение маски для инвертирования модулей
    /// </summary>
    private static Predicate<(int,int)> GetMatrixMask(int makNum = 2)
     => makNum switch 
        {
            0 => Mask0,
            1 => Mask1,    
            2 => Mask2,
            3 => Mask3,
            4 => Mask4,
            5 => Mask5,
            6 => Mask6,
            _ => Mask7
        };

    /// <summary>
    /// Нанесение макси
    /// </summary>
    private static List<byte[]> MaskInvertMatrix(this List<byte[]> matrix, Predicate<(int,int)> maskFunc)
    {        
        for (int x = BORDER; x < matrix.Count - BORDER - 1; x++)
        {
            for (int y = BORDER; y < matrix[x].Length - BORDER; y++)
            {
                if (maskFunc((y - BORDER, x - BORDER)))
                    matrix[x][y] = (byte)(ACTIVE - matrix[x][y]);
            }
        }
        return matrix;
    } 

    /// <summary>
    /// полоски длиной 5 и более нежелательны
    /// </summary>
    private static (int score, List<byte[]>matrix) ScoreByRule1(this (int score, List<byte[]>matrix) mask)  
    {
        var length = 5;
        var score = 0;

        int cnt;
        int current;
        for (int x = BORDER; x < mask.matrix.Count - BORDER - 1; x++)
        {
            cnt = 0;
            current = 3;
            for (int y = BORDER; y < mask.matrix[x].Length - BORDER; y++)
            {
                if (mask.matrix[x][y] == current) cnt++;
                else
                {
                    if (cnt >= length)
                        score += cnt - 2;
                    current = mask.matrix[x][y];
                    cnt = 0;
                }
            }
        }

        for (int y = BORDER; y<mask.matrix[0].Length - BORDER; y++)
        {
            cnt = 0;
            current = 3;
            for (int x = BORDER; x<mask.matrix.Count - BORDER - 1; x++)
            {
                if (mask.matrix[x][y] == current) cnt++;
                else 
                {
                    if (cnt>=length) 
                        score += cnt - 2;
                    current = mask.matrix[x][y];
                    cnt = 0;
                }               
            }
        }
        
        return (score + mask.score, mask.matrix);
    }

    /// <summary>
    /// Примерно половина модулей должна быть черной
    /// </summary>
    private static (int score, List<byte[]>matrix) ScoreByRule4(this (int score, List<byte[]>matrix) mask)  
    {   
        var cntActive = 0.0;
        var cntTotal = 0.0;
        for (int x = BORDER; x<mask.matrix.Count - BORDER - 1; x++)
        {
            for (int y = BORDER; y<mask.matrix[x].Length - BORDER; y++)
            {
                if (mask.matrix[x][y] == ACTIVE) cntActive++;
                cntTotal++;
            }
        }

        double score = cntActive / cntTotal;
        score = score * 100 - 50;
        return (Math.Abs((int)score) * BORDER + mask.score, mask.matrix);
    }

    private static List<byte[]> FormMatrix(this QrCodeData data, int maskNum)
    {
        var tmp = CreateQrCodeMatrix(_matrixSizeByVersion[data.Version])                       
            .PutInMatrix(data.Data)            
            .MaskInvertMatrix(GetMatrixMask(maskNum))  
            .AddMaskNumAndCorrectionLevel(data.CorrectionLevel, maskNum)
            ;        

        int posX1 = BORDER + 3;
        int posX2 = tmp.Count - BORDER - 5;
        int posY = BORDER + 3;
        // Заполнение информации для определения
        tmp.AddTiming()
           .AddPosition(posX1, posY)
           .AddPosition(posX1, tmp[0].Length - BORDER - 4)            
           .AddPosition(posX2, posY)
           .Fill(posX2 - 4, posY + 5, 0);
        
        if (data.Version > 1)
        {
           var pos = _alignmentsPosition[data.Version];
           tmp.AddAlignment(pos + BORDER, pos + BORDER);
        }
            //
            ;
        return tmp;
    }
    
    /// <summary>
    /// Наилучшая матрица из 8 масок
    /// </summary>
    private static List<byte[]> GetBestMatrix(this QrCodeData data)
        => Enumerable
            .Range(0, 8)
            .Select(maskNumber => (0, data.FormMatrix(maskNumber))
                .ScoreByRule1()
                .ScoreByRule4())
            .MinBy(x=>x.score).matrix;

    #endregion
}
