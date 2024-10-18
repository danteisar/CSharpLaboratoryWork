using System.Text;

namespace QrCodeGenerator;

internal static class QrCodeBuilder
{
    public static bool IsDemo {get; set; }

    #region GET

    public static string GetQrCode(string text, ref QR qrCodeVersion, EncodingMode codeType, ref EccLevel? needCorrectionLevel, ref Mask? maskNum, bool invert = false)
    {    
        // Полный блок с данными + подходящий уровень коррекции ошибок + нужная версия QR-кода 
        (var encodedData, needCorrectionLevel, qrCodeVersion) = GetEncodingData(text, EncodeData(text, codeType), codeType, qrCodeVersion, needCorrectionLevel); 

        // Блоки с данными + байты коррекции
        var length = _maxData[(needCorrectionLevel.Value, qrCodeVersion)];
        var codeText = FillCodeByCurrentSize(encodedData, length);
        var blockCount = _correctionLevelBlocksCount[needCorrectionLevel.Value][(byte)qrCodeVersion];      
        var blocks = SplitByBlock(codeText, blockCount);
        var correctionBlocks = new List<byte[]>();        
        foreach (var block in blocks)
        {
            var size = _countOfErrorCorrectionCodeWords[needCorrectionLevel.Value][(byte)qrCodeVersion];
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
            : qrCodeData.GetBestMatrix(ref maskNum);

        return BuildString(qrCodeMatrix, invert);
    }

    #endregion

    #region Base Matrix

    /// <summary>
    /// Граница в два модуля вокруг QR-кода
    /// </summary>
    private const int BORDER = 2;
   
    /// <summary>
    /// Размер рамки для позиционирования QR кода при сканировании
    /// </summary>
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
        var length = matrix.Count % 2 == 1 ? matrix.Count + 1 : matrix.Count;

        for (int row = 0; row < length; row+=2)
        {
            for(int column = 0; column < matrix[0].Length; column++)
            {
                byte scanModule1 = matrix[row][column];
                byte scanModule2 = row < matrix.Count - 1 ? matrix[row+1][column] : ACTIVE;

                var c = invert 
                    ? ScanInvert(scanModule1, scanModule2)
                    : Scan(scanModule1, scanModule2);

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
        for (int i = 0; i < size; i++)
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
            for (int j = 0; j < matrix.Count; j++)
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
            for (int j = 0; j < matrix.Count; j++)
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
        for (int i = BORDER; i < matrix.Count - BORDER - 6; i++)
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
        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
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
        {QR.V9, [6, 26, 46]},        
        {QR.V10, [6, 28, 50]},
        {QR.V11, [6, 30, 54]},
        {QR.V12, [6, 32, 58]},
        {QR.V13, [6, 34, 62]},
        {QR.V14, [6, 26, 46, 66]},
        {QR.V15, [6, 26, 48, 70]},
        {QR.V16, [6, 26, 50, 74]},
        {QR.V17, [6, 30, 54, 78]},
        {QR.V18, [6, 30, 56, 82]},
        {QR.V19, [6, 30, 58, 86]},
        {QR.V20, [6, 34, 62, 90]},
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
        //                    000000     
        //                    101001 █▄█▄▄▀
        {QR.V20, "000000101001111110"},
    };

    private static List<byte[]> FillVersion(this List<byte[]> matrix, QR qrCodeVersion, bool isMask = false)
    {
        if ((byte)qrCodeVersion < 7) return matrix;

        int pos = 0;
        var version =  _versionCodes[qrCodeVersion];        
        int offsetColumn = BORDER;
        int offsetRow = matrix.Count - BORDER - POSITION_DETECTION - 3;

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
    private static List<byte[]> CreateMatrix(this QrCodeData data, Mask maskNum)
    {
        var size = GetMatrixSizeForVersion(data.Version);
        var tmp = CreateQrCodeMatrix(size)                    
            .PutDataInMatrix(data.Data, data.Version)            
            .MaskInvertMatrix(GetMatrixMask(maskNum))  
            .AddFormatInformation(data.CorrectionLevel, data.Version, maskNum)
            ;

        int posX1 = BORDER + 3;
        int posX2 = tmp.Count - BORDER - 4;
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
                x < POSITION_DETECTION + BORDER + 1 && y > matrix.Count - POSITION_DETECTION - BORDER ||
                x > matrix.Count - POSITION_DETECTION - BORDER && y < POSITION_DETECTION + BORDER + 1);  

    /// <summary>
    /// Размещение данных на матрице
    /// </summary>
    private static List<byte[]> PutDataInMatrix(this List<byte[]> matrix, string text, QR version)
    {        
        var blockedModules = CreateOrderMatrix(version); 

        var size = matrix.Count - BORDER * 2;
        var up = true; 
        var index = 0; 
        var count = text.Length; 
        
        for (var column = size + BORDER - 1; column >= BORDER; column -= 2)
        {
            if (column == 8) column--;               

            for (var i = 0; i < size; i++)
            {    
                var row = up ? size + BORDER - i - 1 : i + BORDER;

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
        {
            Console.ReadKey(true);
            IsDemo = false;
        }

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

    private static string EncodeData(string text, EncodingMode codeType){
        var sb = new StringBuilder();
        var tmp = codeType switch {
            EncodingMode.AlphaNumeric => EncodeAlphaNumeric(text.ToUpper()),
            EncodingMode.Numeric => EncodeNumeric(text),
            _ => EncodeBinary(text)
        };
        foreach (var str in tmp)
            sb.Append(str);

        return sb.ToString();
    }
    
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
        else if (text.Length % 3 == 1)
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

    private static readonly Dictionary<EncodingMode, string> _codeTypeMode = new()
    {
        {EncodingMode.Numeric,      "0001"},
        {EncodingMode.AlphaNumeric, "0010"},
        {EncodingMode.Binary,       "0100"}
    };

    private static byte GetDataLengthBlockSize(EncodingMode mode, QR version)
    {
        return ((int)version, mode) switch
        {
            (< 10, EncodingMode.Numeric) => 10,
            (< 10, EncodingMode.AlphaNumeric) => 9,
            (< 10, EncodingMode.Binary) => 8,
            (< 27, EncodingMode.Numeric) => 12,
            (< 27, EncodingMode.AlphaNumeric) => 11,
            (< 27, EncodingMode.Binary) => 16,
            (< 27, _) => 10,
            (_, EncodingMode.Numeric) => 14,
            (_, EncodingMode.AlphaNumeric) => 13,
            (_, EncodingMode.Binary) => 16,
            (_, _) => 12,
        };
    }

    private static string GetDataLength(EncodingMode codeType, QR version, string text)
    {
        var length = codeType switch
        {
            EncodingMode.Binary => Encoding.UTF8.GetBytes(text).Length,
            _ => text.Length,
        };
        var size = GetDataLengthBlockSize(codeType, version);
        var str = Convert.ToString(length, 2).PadLeft(size, '0');
        return str;
    }
    
    private static StringBuilder AppendServiceInformation(this StringBuilder sb, EncodingMode codeType, QR version, string text)
    {
        return sb.Append(GetServiceInformation(codeType, version, text));
    }
    
    private static string GetServiceInformation(EncodingMode codeType, QR version, string text)
    { 
        return _codeTypeMode[codeType] + GetDataLength(codeType, version, text);       
    }

    private static readonly string[] _magicTextArray = ["11101100", "00010001"];

    private static StringBuilder AlignByByteSize(this StringBuilder sb)
    {
        var res = string.Empty.PadLeft(sb.Length % 8, '0');
        if (res.Length > 0) 
            sb.Append(res);
        return sb;
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

    private static IEnumerable<string> SplitText(this string text, int length)
    {
        return Enumerable.Range(0, text.Length / length)
            .Select(i => text.Substring(i * length, length));
    }

    public static List<byte[]> SplitByBlock(string text, int blocksCount)
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

    private static readonly Dictionary<EccLevel, byte[]> _countOfErrorCorrectionCodeWords = new()
    {
        {EccLevel.L, [NA,07,10,15,20,26,18,20,24,30,18,20,24,26,30,22,24,28,30,28,28]},
        {EccLevel.M, [NA,10,16,26,18,24,16,18,22,22,26,30,22,22,24,24,28,28,26,26,26]},
        {EccLevel.Q, [NA,13,22,18,26,18,24,18,22,20,24,28,26,24,20,30,24,28,28,26,30]},
        {EccLevel.H, [NA,17,28,22,16,22,28,26,26,24,28,24,28,22,24,24,30,28,28,26,28]},
    };

    private static readonly Dictionary<EccLevel, byte[]> _correctionLevelBlocksCount = new()
    {
        {EccLevel.L, [NA,1,1,1,1,1,2,2,2,2,4,4,4,4,4,4,6,6,6,6,7,8]},
        {EccLevel.M, [NA,1,1,1,2,2,4,4,4,5,5,5,8,9,9,10,10,11,13,14,16]},
        {EccLevel.Q, [NA,1,1,2,2,4,4,6,6,8,8,8,10,12,16,12,17,16,18,21,20]},
        {EccLevel.H, [NA,1,1,2,4,4,4,5,6,8,8,11,11,16,16,16,18,16,19,21,25,25]},
    };

    private static readonly Dictionary<byte, byte[]> _correctionLevelGeneratingPolynomial = new()
    {
        // x^7 + α^87x^6 + α^229x^5 + α^146x^4 + 
        // α^149x^3 + α^238x^2 + α^102x^ + α^21
        {7, [87, 229, 146, 149, 238, 102, 21]}, 
        // x^10 + α^251x^9 + α^67x^8 + α^46x^7  + α^61x^6 + 
        // α^118x^5 + α^70x^4 + α^64x^3 + α^94x^2 + α^32x^ + α^45
        {10, [251, 67, 46, 61, 118, 70, 64, 94, 32, 45]}, 
        // x^13 + α^74x^12 + α^152x^11 + α^176x^10 + α^100x^9 + α^86x^8 + 
        // α^100x^7 + α^106x^6 + α^104x^5 + α^130x^4 + α^218x^3 + α^206x^2 + α^140x^ + α^78
        {13, [74, 152, 176, 100, 86, 100, 106, 104, 130, 218, 206, 140, 78]},        
        {15, [8, 183, 61, 91, 202, 37, 51, 58, 58, 237, 140, 124, 5, 99, 105]}, 
        // x^16 + α^120x^15 + α^104x^14 + α^107x^13 + α^109x^12 + α^102x^11 + α^161x^10 + α^76x^9 +
        // α^3x^8 + α^91x^7 + α^191x^6 +α^147x^5 + α^169x^4 + α^182x^3 + α^194x^2 + α^225x^ + α^120
        {16, [120, 104, 107, 109, 102, 161, 76, 3, 91, 191, 147, 169, 182, 194, 225, 120]}, 
        // x^17 + α^43x^16 + α^139x^15 + α^206x^14 + α^78x^13 + α^43x^12 + α^239x^11  + α^123x^10 + α^206x^9 + α^214x^8 + α^147x^7 + α^24x^6 + 
        // α^99x^5 + α^150x^4 + α^39x^3 + α^243x^2 + α^163x^ + α^136
        {17, [43, 139, 206, 78, 43, 239, 123, 206, 214, 147, 24, 99, 150, 39, 243, 163, 136]},
        // x^18 + α^215x^17 + α^234x^16 + α^158x^15 + α^94x^14 + α^184x^13 + α^97x^12 + α^118x^11  + α^170x^10 + α^79x^9 + α^187x^8 + α^152x^7 + 
        // α^148x^6 + α^252x^5 + α^179x^4 + α^5x^3 + α^98x^2 + α^96x^ + α^153
        {18, [215, 234, 158, 94, 184, 97, 118, 170, 79, 187, 152, 148, 252, 179, 5, 98, 96, 153]},
        {20, [17, 60, 79, 50, 61, 163, 26, 187, 202, 180, 221, 225, 83, 239, 156, 164, 212, 212, 188, 190]},
        // x^22 + α^210x^21 + α^171x^20 + α^247x^19 + α^242x^18 + α^93x^17 + α^230x^16 + α^14x^15 + α^109x^14 + α^221x^13 + α^53x^12 +
        // α^200x^11 + α^74x^10 + α^8x^9 + α^172x^8 + α^98x^7 + α^80x^6 + α^219x^5 + α^134x^4 + α^160x^3 + α^105x^2 + α^165x^ + α^231
        {22, [210, 171, 247, 242, 93, 230, 14, 109, 221, 53, 200, 74, 8, 172, 98, 80, 219, 134, 160, 105, 165, 231]},
        {24, [229, 121, 135, 48, 211, 117, 251, 126, 159, 180, 169, 152, 192, 226, 228, 218, 111, 0, 117, 232, 87, 96, 227, 21]},
        {26, [173, 125, 158, 2, 103, 182, 118, 17, 145, 201, 111, 28, 165, 53, 161, 21, 245, 142, 13, 102, 48, 227, 153, 145, 218, 70]},
        // x^28 + α^168x^27 + α^223x^26 + α^200x^25 + α^104x^24 + α^224x^23 + α^234x^22 + α^108x^21 + α^180x^20 + α^110x^19 + α^190x^18 + α^195x^17 + 
        // α^147x^16 + α^205x^15 + α^27x^14 + α^232x^13 + α^201x^12 + α^21x^11 + α^43x^10 + α^245x^9 + α^87x^8 + α^42x^7 + α^195x^6 + α^212x^5 + α^119x^4 + 
        // α^242x^3 + α^37x^2 + α^9x^ + α^123
        {28, [168, 223, 200, 104, 224, 234, 108, 180, 110, 190, 195, 147, 205, 27, 232, 201, 21, 43, 245, 87, 42, 195, 212, 119, 242, 37, 9, 123]},
        {30, [41, 173, 145, 152, 216, 31, 179, 182, 50, 48, 110, 86, 239, 96, 222, 125, 42, 173, 226, 193, 224, 130, 156, 37, 251, 216, 238, 40, 192, 180]},
    };

    private static readonly byte[] _galoisField = [1, 2, 4, 8, 16, 32, 64, 128, 29, 58, 116, 232, 205, 135, 19, 38, 
                                                  76, 152, 45, 90, 180, 117, 234, 201, 143, 3, 6, 12, 24, 48, 96, 192, 
                                                  157, 39, 78, 156, 37, 74, 148, 53, 106, 212, 181, 119, 238, 193, 159, 35, 
                                                  70, 140, 5, 10, 20, 40, 80, 160, 93, 186, 105, 210, 185, 111, 222, 161, 
                                                  95, 190, 97, 194, 153, 47, 94, 188, 101, 202, 137, 15, 30, 60, 120, 240, 
                                                  253, 231, 211, 187, 107, 214, 177, 127, 254, 225, 223, 163, 91, 182, 113, 226, 
                                                  217, 175, 67, 134, 17, 34, 68, 136, 13, 26, 52, 104, 208, 189, 103, 206, 
                                                  129, 31, 62, 124, 248, 237, 199, 147, 59, 118, 236, 197, 151, 51, 102, 204, 
                                                  133, 23, 46, 92, 184, 109, 218, 169, 79, 158, 33, 66, 132, 21, 42, 84, 
                                                  168, 77, 154, 41, 82, 164, 85, 170, 73, 146, 57, 114, 228, 213, 183, 115, 
                                                  230, 209, 191, 99, 198, 145, 63, 126, 252, 229, 215, 179, 123, 246, 241, 255, 
                                                  227, 219, 171, 75, 150, 49, 98, 196, 149, 55, 110, 220, 165, 87, 174, 65, 
                                                  130, 25, 50, 100, 200, 141, 7, 14, 28, 56, 112, 224, 221, 167, 83, 166, 
                                                  81, 162, 89, 178, 121, 242, 249, 239, 195, 155, 43, 86, 172, 69, 138, 9, 
                                                  18, 36, 72, 144, 61, 122, 244, 245, 247, 243, 251, 235, 203, 139, 11, 22, 
                                                  44, 88, 176, 125, 250, 233, 207, 131, 27, 54, 108, 216, 173, 71, 142, 1
                                                 ];

    private static readonly byte[] _backGaloisField = [NA, 0, 1, 25, 2, 50, 26, 198, 3, 223, 51, 238, 27, 104, 199, 75, 
                                                      4, 100, 224, 14, 52, 141, 239, 129, 28, 193, 105, 248, 200, 8, 76, 113, 
                                                      5, 138, 101, 47, 225, 36, 15, 33, 53, 147, 142, 218, 240, 18, 130, 69, 
                                                      29, 181, 194, 125, 106, 39, 249, 185, 201, 154, 9, 120, 77, 228, 114, 166, 
                                                      6, 191, 139, 98, 102, 221, 48, 253, 226, 152, 37, 179, 16, 145, 34, 136, 
                                                      54, 208, 148, 206, 143, 150, 219, 189, 241, 210, 19, 92, 131, 56, 70, 64, 
                                                      30, 66, 182, 163, 195, 72, 126, 110, 107, 58, 40, 84, 250, 133, 186, 61, 
                                                      202, 94, 155, 159, 10, 21, 121, 43, 78, 212, 229, 172, 115, 243, 167, 87, 
                                                      7, 112, 192, 247, 140, 128, 99, 13, 103, 74, 222, 237, 49, 197, 254, 24, 
                                                      227, 165, 153, 119, 38, 184, 180, 124, 17, 68, 146, 217, 35, 32, 137, 46, 
                                                      55, 63, 209, 91, 149, 188, 207, 205, 144, 135, 151, 178, 220, 252, 190, 97, 
                                                      242, 86, 211, 171, 20, 42, 93, 158, 132, 60, 57, 83, 71, 109, 65, 162, 
                                                      31, 45, 67, 216, 183, 123, 164, 118, 196, 23, 73, 236, 127, 12, 111, 246, 
                                                      108, 161, 59, 82, 41, 157, 85, 170, 251, 96, 134, 177, 187, 204, 62, 90, 
                                                      203, 89, 95, 176, 156, 169, 160, 81, 11, 245, 22, 235, 122, 117, 44, 215, 
                                                      79, 174, 213, 233, 230, 231, 173, 232, 116, 214, 244, 234, 168, 80, 88, 175
                                                    ];
    
    /// <summary>
    /// Рассчитать блок коррекции
    /// </summary>
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

            byte b = _backGaloisField[a];
            for (int x = 0; x < g.Length; x++)
            {
                var c = (g[x] + b) % 255;
                var d = _galoisField[c];
                m[x] = (byte)(m[x] ^ d);
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

    private static readonly Dictionary<(EccLevel correctionLevel, QR version), int> _maxData = new()
    {
        {(EccLevel.H, QR.V1),    72}, {(EccLevel.Q, QR.V1),   104}, {(EccLevel.M, QR.V1),   128}, {(EccLevel.L, QR.V1),   152},
        {(EccLevel.H, QR.V2),   128}, {(EccLevel.Q, QR.V2),   176}, {(EccLevel.M, QR.V2),   224}, {(EccLevel.L, QR.V2),   272},  
        {(EccLevel.H, QR.V3),   208}, {(EccLevel.Q, QR.V3),   272}, {(EccLevel.M, QR.V3),   352}, {(EccLevel.L, QR.V3),   440},
        {(EccLevel.H, QR.V4),   288}, {(EccLevel.Q, QR.V4),   384}, {(EccLevel.M, QR.V4),   512}, {(EccLevel.L, QR.V4),   640},
        {(EccLevel.H, QR.V5),   368}, {(EccLevel.Q, QR.V5),   496}, {(EccLevel.M, QR.V5),   688}, {(EccLevel.L, QR.V5),   864},
        {(EccLevel.H, QR.V6),   480}, {(EccLevel.Q, QR.V6),   608}, {(EccLevel.M, QR.V6),   864}, {(EccLevel.L, QR.V6),  1088},
        {(EccLevel.H, QR.V7),   528}, {(EccLevel.Q, QR.V7),   704}, {(EccLevel.M, QR.V7),   992}, {(EccLevel.L, QR.V7),  1248},
        {(EccLevel.H, QR.V8),   688}, {(EccLevel.Q, QR.V8),   880}, {(EccLevel.M, QR.V8),  1232}, {(EccLevel.L, QR.V8),  1552},
        {(EccLevel.H, QR.V9),   800}, {(EccLevel.Q, QR.V9),  1056}, {(EccLevel.M, QR.V9),  1456}, {(EccLevel.L, QR.V9),  1856},
        {(EccLevel.H, QR.V10),  976}, {(EccLevel.Q, QR.V10), 1232}, {(EccLevel.M, QR.V10), 1728}, {(EccLevel.L, QR.V10), 2192},        
        {(EccLevel.H, QR.V11), 1120}, {(EccLevel.Q, QR.V11), 1440}, {(EccLevel.M, QR.V11), 2032}, {(EccLevel.L, QR.V11), 2592},    
        {(EccLevel.H, QR.V12), 1264}, {(EccLevel.Q, QR.V12), 1648}, {(EccLevel.M, QR.V12), 2320}, {(EccLevel.L, QR.V12), 2960},    
        {(EccLevel.H, QR.V13), 1440}, {(EccLevel.Q, QR.V13), 1952}, {(EccLevel.M, QR.V13), 2672}, {(EccLevel.L, QR.V13), 3424},    
        {(EccLevel.H, QR.V14), 1576}, {(EccLevel.Q, QR.V14), 2088}, {(EccLevel.M, QR.V14), 2920}, {(EccLevel.L, QR.V14), 3688},    
        {(EccLevel.H, QR.V15), 1784}, {(EccLevel.Q, QR.V15), 2360}, {(EccLevel.M, QR.V15), 3320}, {(EccLevel.L, QR.V15), 4184},    
        {(EccLevel.H, QR.V16), 2024}, {(EccLevel.Q, QR.V16), 2600}, {(EccLevel.M, QR.V16), 3624}, {(EccLevel.L, QR.V16), 4712},    
        {(EccLevel.H, QR.V17), 2264}, {(EccLevel.Q, QR.V17), 2936}, {(EccLevel.M, QR.V17), 4056}, {(EccLevel.L, QR.V17), 5176},    
        {(EccLevel.H, QR.V18), 2504}, {(EccLevel.Q, QR.V18), 3176}, {(EccLevel.M, QR.V18), 4504}, {(EccLevel.L, QR.V18), 5768},    
        {(EccLevel.H, QR.V19), 2728}, {(EccLevel.Q, QR.V19), 3560}, {(EccLevel.M, QR.V19), 5016}, {(EccLevel.L, QR.V19), 6360},    
        {(EccLevel.H, QR.V20), 3080}, {(EccLevel.Q, QR.V20), 3880}, {(EccLevel.M, QR.V20), 5352}, {(EccLevel.L, QR.V20), 6888},    
    };
    
    /// <summary>
    /// Выбор нужной версии QR-кода и уровня коррекции
    /// </summary>
    private static (string encodingData, EccLevel correctionLevel, QR version) GetEncodingData(string text, string preparedData, EncodingMode codeType, QR version, EccLevel? needCorrectionLevel = null)
    {
        if (version == NA)
            throw new NotSupportedException("QR-code version start with 1!");
       
        var sb = new StringBuilder();
        sb.AppendServiceInformation(codeType, version, text)           
          .Append(preparedData)
          .AlignByByteSize();

        var length = sb.Length;

        if ((int)version > 20)
            throw new NotSupportedException($"Current QR-code does not support version {version} yet!");

        if (needCorrectionLevel.HasValue)
        {
            foreach (var found in _maxData
                .Where(v => v.Key.version >= version && v.Key.correctionLevel >= needCorrectionLevel.Value)
                .Where(l => length < l.Value))
                    return (sb.ToString(), found.Key.correctionLevel, found.Key.version);
        }

        foreach (var found in _maxData
            .Where(v => v.Key.version == version)
            .OrderByDescending(x=>x.Key.correctionLevel)
            .Where(x => length < x.Value))
                return (sb.ToString(), found.Key.correctionLevel, found.Key.version);  

        if ((int)version > 20)
            throw new NotSupportedException($"Current QR-code does not support data length {length} yet!");
        
        return GetEncodingData(text, preparedData, codeType, version + 1, needCorrectionLevel);
    }

    #endregion

    #region Service Information

    /// <summary>
    /// Format information
    /// </summary>
    private static readonly Dictionary<(EccLevel correctionLevel, Mask maskNum), string> _masksAndCorrectionLevel = new()
    {
        {(EccLevel.L, Mask.M0), "111011111000100"},
        {(EccLevel.L, Mask.M1), "111001011110011"},
        {(EccLevel.L, Mask.M2), "111110110101010"},
        {(EccLevel.L, Mask.M3), "111100010011101"},
        {(EccLevel.L, Mask.M4), "110011000101111"},
        {(EccLevel.L, Mask.M5), "110001100011000"},
        {(EccLevel.L, Mask.M6), "110110001000001"},
        {(EccLevel.L, Mask.M7), "110100101110110"},        
        {(EccLevel.M, Mask.M0), "101010000010010"},
        {(EccLevel.M, Mask.M1), "101000100100101"},
        {(EccLevel.M, Mask.M2), "101111001111100"},
        {(EccLevel.M, Mask.M3), "101101101001011"},
        {(EccLevel.M, Mask.M4), "100010111111001"},
        {(EccLevel.M, Mask.M5), "100000011001110"},
        {(EccLevel.M, Mask.M6), "100111110010111"},
        {(EccLevel.M, Mask.M7), "100101010100000"},        
        {(EccLevel.Q, Mask.M0), "011010101011111"},
        {(EccLevel.Q, Mask.M1), "011000001101000"},
        {(EccLevel.Q, Mask.M2), "011111100110001"},
        {(EccLevel.Q, Mask.M3), "011101000000110"},
        {(EccLevel.Q, Mask.M4), "010010010110100"},
        {(EccLevel.Q, Mask.M5), "010000110000011"},
        {(EccLevel.Q, Mask.M6), "010111011011010"},
        {(EccLevel.Q, Mask.M7), "010101111101101"},
        {(EccLevel.H, Mask.M0), "001011010001001"},
        {(EccLevel.H, Mask.M1), "001001110111110"},
        {(EccLevel.H, Mask.M2), "001110011100111"},
        {(EccLevel.H, Mask.M3), "001100111010000"},
        {(EccLevel.H, Mask.M4), "000011101100010"},
        {(EccLevel.H, Mask.M5), "000001001010101"},
        {(EccLevel.H, Mask.M6), "000110100001100"},
        {(EccLevel.H, Mask.M7), "000100000111011"},
    };

    /// <summary>
    /// Информация о маске и уровне коррекции
    /// </summary>
    private static List<byte[]> AddFormatInformation(this List<byte[]> matrix, EccLevel level, QR version, Mask maskNum)
    {
        var maskNumAndCorrectionLevel = _masksAndCorrectionLevel[(level, maskNum)];        
        AddFormatInformation(matrix, maskNumAndCorrectionLevel, version);
        return matrix;      
    }
    
    /// <summary>
    /// Format information - порядок размещения возле верхнего левого паттерна позиционирования
    /// </summary>
    private static readonly Pair[] _masksAndCorrectionLevelTopLeftTemplate = [
                                                                              (02, 10), (03, 10), (04, 10), 
                                                                              (05, 10), (06, 10), (07, 10), 
                                                                              (09, 10), (10, 10), (10, 09),
                                                                              (10, 07), (10, 06), (10, 05),
                                                                              (10, 04), (10, 03), (10, 02),
                                                                             ];

    /// <summary>
    /// Format information - возле других паттернов позиционирования
    /// </summary>
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
    private static List<byte[]> AddFormatInformation(this List<byte[]> matrix, string text, QR version)
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
    private static Predicate<(int,int)> GetMatrixMask(Mask makNum = Mask.M2)
     => makNum switch 
        {
            Mask.M0 => Mask0,
            Mask.M1 => Mask1,    
            Mask.M2 => Mask2,
            Mask.M3 => Mask3,
            Mask.M4 => Mask4,
            Mask.M5 => Mask5,
            Mask.M6 => Mask6,
            Mask.M7 => Mask7,
            _ => throw new ArgumentOutOfRangeException("Incorrect Mask Number")
        };

    /// <summary>
    /// Нанесение макси
    /// </summary>
    private static List<byte[]> MaskInvertMatrix(this List<byte[]> matrix, Predicate<(int,int)> maskFunc)
    {        
        for (int x = BORDER; x < matrix.Count - BORDER; x++)
        {
            for (int y = BORDER; y < matrix.Count - BORDER; y++)
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
        for (int x = BORDER; x < mask.matrix.Count - BORDER; x++)
        {
            cnt = 0;
            current = 3;
            for (int y = BORDER; y < mask.matrix.Count - BORDER; y++)
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

        for (int y = BORDER; y<mask.matrix.Count - BORDER; y++)
        {
            cnt = 0;
            current = 3;
            for (int x = BORDER; x<mask.matrix.Count - BORDER; x++)
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
        for (int x = BORDER; x < mask.matrix.Count - BORDER; x++)
        {
            for (int y = BORDER; y < mask.matrix.Count - BORDER; y++)
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
    private static List<byte[]> GetBestMatrix(this QrCodeData data, ref Mask? maskNum)
    {
        var res = Enumerable
            .Range(0, 8)
            .Select(maskNumber => (maskNumber, (0, data.CreateMatrix((Mask)maskNumber))
                .ScoreByRule1()
                .ScoreByRule4()))
            .MinBy(x=>x.Item2.score);

        maskNum = (Mask)res.maskNumber;
        
        return res.Item2.matrix;
    }

    #endregion
}
