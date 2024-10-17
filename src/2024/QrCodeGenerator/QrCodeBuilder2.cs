using System.Text;

namespace QrCodeGenerator;

internal static class QrCodeBuilder2
{
    public static bool IsDemo {get; set; } = false;

    #region GET

    public static string GetQrCode(string text, ref QR qrCodeVersion, EncodedType codeType, ref CorrectionLevel? needCorrectionLevel, int? maskNum = null, bool invert = false)
    {    
        // Блоки с данными
        var sb = new StringBuilder();        
        sb.Append(GetServiceInformation(codeType, text));        
        var tmp = codeType switch {
            EncodedType.AlphaNumeric => EncodeAlphaNumeric(text.ToUpper()),
            EncodedType.Numeric => EncodeNumeric(text),
            _ => EncodeBinary(text)
        };
        foreach (var str in tmp)
            sb.Append(str);
        AlignByByteSize(sb); 
        var encodedData = sb.ToString(); 
        (needCorrectionLevel, qrCodeVersion) = GetCorrectionLevelAndVersion(encodedData.Length, qrCodeVersion, needCorrectionLevel); 

        // Блоки с байтами коррекции
        var length = _maxData[(needCorrectionLevel.Value, qrCodeVersion)];
        var codeText = FillCodeByCurrentSize(encodedData, length);

        var blockCount = _correctionLevelBlocksCount[needCorrectionLevel.Value][(byte)qrCodeVersion];      
        var blocks = SplitByBlock(codeText, blockCount);

        var correctionBlocks = new List<byte[]>();        
        foreach (var block in blocks)
        {
            var size = _correctionLevelBytesSize[needCorrectionLevel.Value][(byte)qrCodeVersion];
            correctionBlocks.Add(GetCorrectionBlock(block, size));
        }      
        
        var data = CombineDataAndCorrectionBlocks(blocks, correctionBlocks); 
               
        // Создание матрицы QR кода c лучшей маской      
        var qrCodeData = new QrCodeData
        { 
            Version = qrCodeVersion,
            CorrectionLevel = needCorrectionLevel.Value,
            Data = data,
        };

        var qrCodeMatrix = maskNum.HasValue
            ? qrCodeData.CreateMatrix(maskNum.Value)
            : qrCodeData.GetBestMatrix();

        return BuildString(qrCodeMatrix, invert);
    }

    #endregion

    #region Base Matrix

    /// <summary>
    /// Граница в два модуля вокруг QR-кода
    /// </summary>
    private const int BORDER = 2;
    private const int POSITION_DETECTION = 8;

    /// <summary>
    /// Активный модуль
    /// </summary>
    private const byte ACTIVE = 1;

    /// <summary>
    /// Неактивный модуль
    /// </summary>
    private const byte ZERO = 0;

    /// <summary>
    /// Недопустимый параметр
    /// </summary>
    private const byte NA = 0;

    /// <summary>
    /// Сборка матрицы в готовую строку QR кода
    /// </summary>
    private static string BuildString(this List<byte[]> matrix, bool invert)
    {
        var sb = new StringBuilder();
        for (int y = 0; y<matrix.Count; y+=2)
        {
            for(int x = 0; x<matrix[0].Length; x++)
            {
                var c = invert 
                    ? ScanInvert(matrix[y][x], matrix[y+1][x])
                    : Scan(matrix[y][x], matrix[y+1][x]);

                sb.Append(c);  
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
    
    private static char ScanInvert(byte a, byte b)
        => (a, b) switch
        {
            (1, 1) => ' ',
            (1, 0) => '▄',
            (0, 1) => '▀',
            (0, 0) => '█',
            _ => throw new NotImplementedException(),
        };

    /// <summary>
    /// Создание болванки матрицы, заполненный <see cref="ACTIVE"/>
    /// </summary>
    private static List<byte[]> CreateQrCodeMatrix(int size, byte value = ACTIVE)
    {
        List<byte[]> qrCodeMatrix = [];       
        for (int i = 0; i < size + 1; i++)
        {
            qrCodeMatrix.Add(new byte[size]);
        }
        return qrCodeMatrix.FillMatrix(value);
    }

    /// <summary>
    /// Размер матрицы в зависимости от версии QR-кода
    /// </summary>
    private static int GetMatrixSizeForVersion(QR version)
    {
    return 17 + 4 * (int)version + BORDER * 2;
    }

    #endregion

    #region Matrix Templates
    
    private static List<byte[]> AddPosition(this List<byte[]> matrix, int x, int y)
    {
        FillCube(matrix, x - 4, y - 4, 9, 1);
        FillCube(matrix, x - 3, y - 3, 7, 0);
        FillCube(matrix, x - 2, y - 2, 5, 1);
        FillCube(matrix, x - 1, y - 1, 3, 0);
        return matrix;
    }

    public static List<byte[]> InvertMatrix(this List<byte[]> matrix)
    {
        for (int i = 0; i < matrix.Count; i++)
        {
            for (int j = 0; j < matrix[0].Length; j++)
            {
                matrix[i][j] = (byte)(ACTIVE - matrix[i][j]);
            }
        }  
        return matrix;      
    }

    private static List<byte[]> FillMatrix(this List<byte[]> matrix, byte value)
    {
        for (int i = 0; i < matrix.Count; i++)
        {
            for (int j = 0; j < matrix[0].Length; j++)
            {
                matrix[i][j] = value;
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

    private static List<byte[]> AddTiming(this List<byte[]> matrix, bool isMask = false)
    {
        for (int i = BORDER; i < matrix.Count - BORDER - 1 - 6; i++)
        {
            matrix[i][BORDER + 6] = !isMask ? (byte)(i % 2) : ZERO;
        }
        for (int i = BORDER; i < matrix[0].Length - BORDER - 6; i++)
        {
            matrix[BORDER + 6][i] = !isMask ? (byte)(i % 2) : ZERO;
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

    private static readonly Dictionary<QR, int[]> _alignmentsPosition = new()
    {
        {QR.V1, []},
        {QR.V2, [18]},
        {QR.V3, [22]},
        {QR.V4, [26]},
        {QR.V5, [30]},        
        {QR.V6, [34]},
        {QR.V7, [6, 22, 38]},
        {QR.V8, [6, 24, 42]},
        {QR.V9, [6, 26, 46]}
    };

    private static readonly Dictionary<QR, string> _versionCodes = new()
    {
        //                    000010    ▄
        //                    111101 █▀▀▄▀
        {QR.V7,  "000010011110100110"},
        //                    010001  ▄   ▄
        //                    011100 ▄██▀ 
        {QR.V8,  "010001011100111000"},
        //                    110111 ▄▄ ▄▄▄
        //                    011000  ▀▀▄
        {QR.V9,  "110111011000000100"},
        //                    101001 ▄ ▄  ▄
        //                    111110 ▀▀▀▀▀
        {QR.V10, "101001111110000000"},
        //                    001111   ▄▄▄▄
        //                    111010 ███▄▀
        {QR.V11, "001111111010111100"},
        //                    001101   ▄▄ ▄
        //                    100100 ▀▄▄▀▄
        {QR.V12, "001101100100011010"},
        //                    101011 ▄ ▄ ▄▄ 
        //                    100000 █  ▄▄
        {QR.V13, "101011100000100110"},
        //                    110101 ▄▄ ▄ ▄
        //                    000110 ▄▀ █▄▀
        {QR.V14, "110101000110100010"},
        //                    010011  ▄  ▄▄
        //                    000010  ▄▄▄█
        {QR.V15, "010011000010011110"},
        //                    011100  ▄▄▄
        //                    010001 █▄▄ ▀
        {QR.V16, "011100010001011100"},
        //                    111010 ▄▄▄ ▄
        //                    010101 ▄▀ ▀ ▀
        {QR.V17, "111010010101100000"},
        //                    100100 ▄  ▄
        //                    110011 █▀ ▄▀▀
        {QR.V18, "100100110011100100"},
        //                    000010     ▄
        //                    110111 ▀█▄▀▀▀
        {QR.V19, "000010110111011000"},
    };

    private static List<byte[]> FillVersion(this List<byte[]> matrix, QR qrCodeVersion, bool isMask = false)
    {
        if ((byte)qrCodeVersion < 7) return matrix;

        int pos = 0;
        var version =  _versionCodes[qrCodeVersion];        
        int offsetColumn = BORDER;
        int offsetRow = matrix.Count - BORDER - 1 - POSITION_DETECTION - 3;

        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 6; column++)
            {
                byte value = isMask || version[pos++] == '1' ? ZERO : ACTIVE;                
                matrix[offsetColumn + column][offsetRow + row] = value;
                matrix[offsetRow + row][offsetColumn + column] = value;
            }
        }
        return matrix;
    }

    /// <summary>
    /// Готовая матрица QR-кода
    /// </summary>
    private static List<byte[]> CreateMatrix(this QrCodeData data, int maskNum)
    {
        var size = GetMatrixSizeForVersion(data.Version);
        var tmp = CreateQrCodeMatrix(size)                    
            .PutDataInMatrix(data.Data, data.Version)            
            .MaskInvertMatrix(GetMatrixMask(maskNum))  
            .AddMaskNumAndCorrectionLevel(data.CorrectionLevel, data.Version, maskNum);

        int posX1 = BORDER + 3;
        int posX2 = tmp.Count - BORDER - 5;
        int posY = BORDER + 3;

        tmp.AddTiming()
           .AddPosition(posX1, posY)
           .AddPosition(posX1, tmp[0].Length - BORDER - 4)            
           .AddPosition(posX2, posY);

        foreach (var x in _alignmentsPosition[data.Version])
            foreach (var y in _alignmentsPosition[data.Version].Where(y => CanFill(x + BORDER, y + BORDER, tmp)))               
                tmp.AddAlignment(x + BORDER, y + BORDER);  

        tmp.Fill(posX2 - 4, posY + 5, 0)
           .FillVersion(data.Version);    

        return tmp;
    }
    
    /// <summary>
    /// Ограничивающая матрица для размещения данных
    /// </summary>
    private static List<byte[]> CreateOrderMatrix(QR version)
    {  
        int matrixSize = GetMatrixSizeForVersion(version);
        var size = matrixSize - BORDER * 2;

        var tmp = CreateQrCodeMatrix(matrixSize, ZERO)
            .FillCube(BORDER, BORDER, size, ACTIVE);

        int cubeSize = 9;
        int posX1 = BORDER;
        int posY1 = BORDER;
        int posX2 = BORDER + cubeSize + (int)version * 4;           
        int posY2 = BORDER + cubeSize + (int)version * 4;     

        tmp.FillCube(posX1, posY1, cubeSize, ZERO)
           .FillCube(posX1, posY2, cubeSize, ZERO)            
           .FillCube(posX2, posY1, cubeSize, ZERO);

        foreach (var x in _alignmentsPosition[version])
            foreach (var y in _alignmentsPosition[version].Where(y => CanFill(x + BORDER, y + BORDER, tmp)))
                tmp.FillCube(x + BORDER - 2, y + BORDER - 2, 5, 0);

        tmp.AddTiming(true)
           .FillVersion(version, true);                 

        return tmp;
    }

    #endregion

    #region Place Data in Matrix

    /// <summary>
    /// Нельзя размещать в POSITION_DETECTION
    /// </summary>
    private static bool CanFill(int x, int y, List<byte[]> matrix)
        => !(x < POSITION_DETECTION + BORDER + 1 && y < POSITION_DETECTION + BORDER + 1 ||
                x < POSITION_DETECTION + BORDER + 1 && y > matrix[0].Length - POSITION_DETECTION - BORDER ||
                x > matrix.Count - POSITION_DETECTION - BORDER - 1 && y < POSITION_DETECTION + BORDER + 1);  

    /// <summary>
    /// Размещение данных на матрице
    /// </summary>
    private static List<byte[]> PutDataInMatrix(this List<byte[]> matrix, string text, QR version)
    {        
        var blockedModules = CreateOrderMatrix(version); 

        var size = matrix.Count - 1;
        var up = true; 
        var index = 0; 
        var count = text.Length; 
        
        for (var column = size - 3; column >= 2; column -= 2)
        {
            if (column == 8) column--;               

            for (var i = 0; i < size; i++)
            {    
                var row = up ? size - i : i;

                if (index < count && !blockedModules.IsBlocked(row, column))
                    PlaceData(matrix, blockedModules, row, column, text[index++]);                       
                    
                if (index < count && column > 0 && !blockedModules.IsBlocked(row, column - 1))
                    PlaceData(matrix, blockedModules, row, column - 1, text[index++]);

                if (IsDemo)
                {
                    Console.SetCursorPosition(0,0);
                    Console.Write(blockedModules.BuildString(false));
                    Thread.Sleep(1);
                }  
            }
            up = !up;
        }

        if (IsDemo) 
            Console.ReadKey(true);

        return matrix;
    }

    /// <summary>
    /// Запрещенное место для записи данных
    /// </summary>
    private static bool IsBlocked(this List<byte[]> blockedModules, int row, int column)
    {
        return blockedModules[row][column] == ZERO;
    }

    /// <summary>
    /// Размещение бита информации
    /// </summary>
    private static void PlaceData(List<byte[]> matrix, List<byte[]> blockedModules, int row, int column, char letter)
    {
        blockedModules[row][column] = ZERO;
        matrix[row][column] = letter != '1' ? ACTIVE : ZERO;
    }

    #endregion

    #region Encode Data

    private static readonly Dictionary<EncodedType, string> _codeTypeMode = new()
    {
        {EncodedType.Numeric,      "0001"},
        {EncodedType.AlphaNumeric, "0010"},
        {EncodedType.Binary,       "0100"}
    };

    private static readonly Dictionary<EncodedType, byte> _codeTypeSize = new()
    {
        {EncodedType.Numeric, 10},
        {EncodedType.AlphaNumeric, 9},
        {EncodedType.Binary, 8}
    };

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

    private static string GetDataLength(EncodedType codeType, string text)
    {
        var length = codeType switch
        {
            EncodedType.Binary => Encoding.UTF8.GetBytes(text).Length,
            _ => text.Length,
        };
        var str = Convert.ToString(length, 2).PadLeft(_codeTypeSize[codeType], '0');
        return str;
    }
    
    private static string GetServiceInformation(EncodedType codeType, string text)
    { 
        return _codeTypeMode[codeType] + GetDataLength(codeType, text);       
    }

    private static readonly string[] _magicTextArray = ["11101100", "00010001"];

    private static void AlignByByteSize(StringBuilder sb)
    {
        var res = string.Empty.PadLeft(sb.Length % 8, '0');
        if (res.Length > 0) 
            sb.Append(res);
    }

    private static string FillCodeByCurrentSize(string text, int size)
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

    private static List<byte[]> SplitByBlock(string text, int blocksCount)
    {
        List<byte> tmp = [];
        foreach (var line in text.SplitText(8))
        {
            tmp.Add(Convert.ToByte(line,2));
        }

        var size = text.Length / 8 / blocksCount;
        var extraSize = text.Length / 8 % blocksCount;

        List<byte[]> list = [];
        for (int i = blocksCount -1 ; i >= 0; i--)
        {
            var currentSize = size + (extraSize-- > 0 ? 1 : 0);
            list.Insert(0, new byte[currentSize]);        
        }

        var index = 0;
        foreach (var block in list)
        {
            for (int i = 0; i<block.Length; i++)
            {
                block[i] = tmp[index++];
            }
        }
        
        return list;
    }

    #endregion

    #region Correction Data

    private static readonly Dictionary<CorrectionLevel, byte[]> _correctionLevelBytesSize = new()
    {
        {CorrectionLevel.L, [NA,07,10,15,20,26,18,20,24,30]},
        {CorrectionLevel.M, [NA,10,16,26,18,24,16,18,22,22]},
        {CorrectionLevel.Q, [NA,13,22,18,26,18,24,18,22,20]},
        {CorrectionLevel.H, [NA,17,28,22,16,22,28,26,26,24]},
    };

    private static readonly Dictionary<CorrectionLevel, byte[]> _correctionLevelBlocksCount = new()
    {
        {CorrectionLevel.L, [NA,1,1,1,1,1,2,2,2,6]},
        {CorrectionLevel.M, [NA,1,1,1,2,2,4,4,4,5]},
        {CorrectionLevel.Q, [NA,1,1,2,2,4,4,6,6,8]},
        {CorrectionLevel.H, [NA,1,1,2,4,4,4,5,6,8]},
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
    
    private static byte[] GetCorrectionBlock(byte[] block, byte bytesSize)
    {       
        var size = Math.Max(block.Length, bytesSize);
        var m = new List<byte>(block);
        var g = _correctionLevelGeneratingPolynomial[bytesSize];
        var n = g.Length;
        while (m.Count != size)
            m.Add(0);

        for (int i = 0; i < block.Length; i++)
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
         
        return m.Take(n).ToArray();    
    }
       
    private static void ScanData(List<byte[]> data, StringBuilder sb)
    {
        if (data.Count == 1)
        {
            foreach (var b in data[0])
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            } 
            return;
        }

        var size = data.Max(x=> x.Length);
        for (int i = 0; i<size; i++)
        {
            foreach (var bytes in data)
            {
                if (i < bytes.Length)
                    sb.Append(Convert.ToString(bytes[i], 2).PadLeft(8, '0'));
            }            
        }
    }
    
    private static string CombineDataAndCorrectionBlocks(List<byte[]> data, List<byte[]> correctionBlocks)
    {       
        var sb = new StringBuilder();
        ScanData(data, sb);
        ScanData(correctionBlocks, sb);
        return sb.ToString();
    }

    private static readonly Dictionary<(CorrectionLevel correctionLevel, QR version), int> _maxData = new()
    {
        {(CorrectionLevel.H, QR.V1), 072},  
        {(CorrectionLevel.Q, QR.V1), 104}, 
        {(CorrectionLevel.M, QR.V1), 128},
        {(CorrectionLevel.L, QR.V1), 152},

        {(CorrectionLevel.H, QR.V2), 128},  
        {(CorrectionLevel.Q, QR.V2), 176}, 
        {(CorrectionLevel.M, QR.V2), 224},
        {(CorrectionLevel.L, QR.V2), 272},  

        {(CorrectionLevel.H, QR.V3), 208},
        {(CorrectionLevel.Q, QR.V3), 272},
        {(CorrectionLevel.M, QR.V3), 352},
        {(CorrectionLevel.L, QR.V3), 440},

        {(CorrectionLevel.H, QR.V4), 288},
        {(CorrectionLevel.Q, QR.V4), 384},
        {(CorrectionLevel.M, QR.V4), 512},
        {(CorrectionLevel.L, QR.V4), 640},

        {(CorrectionLevel.H, QR.V5), 368},
        {(CorrectionLevel.Q, QR.V5), 496},
        {(CorrectionLevel.M, QR.V5), 688},
        {(CorrectionLevel.L, QR.V5), 864},

        {(CorrectionLevel.H, QR.V6), 480},
        {(CorrectionLevel.Q, QR.V6), 608},        
        {(CorrectionLevel.M, QR.V6), 864},
        {(CorrectionLevel.L, QR.V6), 1088}, 

        {(CorrectionLevel.H, QR.V7), 528},
        {(CorrectionLevel.Q, QR.V7), 704},
        {(CorrectionLevel.M, QR.V7), 992},
        {(CorrectionLevel.L, QR.V7), 1248},

        {(CorrectionLevel.H, QR.V8), 688},
        {(CorrectionLevel.Q, QR.V8), 880},
        {(CorrectionLevel.M, QR.V8), 1232},
        {(CorrectionLevel.L, QR.V8), 1552},

        {(CorrectionLevel.H, QR.V9), 800},
        {(CorrectionLevel.Q, QR.V9), 1056},
        {(CorrectionLevel.M, QR.V9), 1456},
        {(CorrectionLevel.L, QR.V9), 1856},
    };
    
    private static (CorrectionLevel correctionLevel, QR version) GetCorrectionLevelAndVersion(int length, QR version, CorrectionLevel? needCorrectionLevel = null)
    {
        if (version == NA)
            throw new NotSupportedException("QR-code version start with 1!");

        if ((int)version > 9)
            throw new NotSupportedException($"Current QR-code does not support version {version} yet!");

        if (needCorrectionLevel.HasValue)
        {
            foreach (var found in _maxData
                .Where(v => v.Key.version == version && v.Key.correctionLevel == needCorrectionLevel.Value)
                .Where(l => length < l.Value))
            {
                return (found.Key.correctionLevel, version);
            }
        }

        foreach (var pair in _maxData.Where(v => v.Key.version == version))
        {
            if (length < pair.Value)
                return (pair.Key.correctionLevel, version);               
        }

        if ((int)version > 9)
            throw new NotSupportedException($"Current QR-code does not support data length {length} yet!");
        
        return GetCorrectionLevelAndVersion(length, version + 1);
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
    private static List<byte[]> AddMaskNumAndCorrectionLevel(this List<byte[]> matrix, CorrectionLevel level, QR version, int maskNum)
    {
        var maskNumAndCorrectionLevel = _masksAndCorrectionLevel[(level, maskNum)];        
        PutInMatrix(matrix, maskNumAndCorrectionLevel, version);
        return matrix;      
    }

    private static readonly Pair[] _masksAndCorrectionLevelTopLeftTemplate = [
                                                                              (02, 10), (03, 10), (04, 10), 
                                                                              (05, 10), (06, 10), (07, 10), 
                                                                              (09, 10), (10, 10), (10, 09),
                                                                              (10, 07), (10, 06), (10, 05),
                                                                              (10, 04), (10, 03), (10, 02),
                                                                             ];

    private static Pair[] CalcMasksAndCorrectionLevelSecondTemplate(QR version)
    {
        var offset = 11 + (int)version * 4;
        return [(10, offset + 7), (10, offset +6 ), (10, offset + 5), 
                (10, offset + 4), (10, offset + 3), (10, offset + 2), 
                (10, offset + 1), (10, offset), (offset + 1, 10),
                (offset + 2, 10), (offset + 3, 10), (offset + 4, 10),
                (offset + 5, 10), (offset + 6, 10), (offset + 7, 10)];               
    }

    /// <summary>
    /// Занесение в матрицу данных служебной информации
    /// </summary>
    private static void SetInMatrix(Pair[] position, List<byte[]> matrix, int i, char letter)
    {
        (var x, var y) = (position[i].X, position[i].Y); 
        matrix[y][x] = letter != '1' ? ACTIVE : ZERO;           
    }

    /// <summary>
    /// Занесение в матрицу данных служебной информации
    /// </summary>
    private static List<byte[]> PutInMatrix(this List<byte[]> matrix, string text, QR version)
    {
        for (int i = 0; i < text.Length; i++)
        {
            char letter = text[i];
            SetInMatrix(_masksAndCorrectionLevelTopLeftTemplate, matrix, i, letter);
            SetInMatrix(CalcMasksAndCorrectionLevelSecondTemplate(version), matrix, i, letter);            
        }
        return matrix;
    }

    #endregion

    #region Place Mask

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

    /// <summary>
    /// Наилучшая матрица из 8 масок
    /// </summary>
    private static List<byte[]> GetBestMatrix(this QrCodeData data)
        => Enumerable
            .Range(0, 8)
            .Select(maskNumber => (0, data.CreateMatrix(maskNumber))
                .ScoreByRule1()
                .ScoreByRule4())
            .MinBy(x=>x.score).matrix;

    #endregion
}
