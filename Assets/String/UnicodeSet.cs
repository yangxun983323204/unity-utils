using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace YX
{
    public static class UnicodeSet
    {
        public enum BlockType
        {
[Range("0000—007F")]基本拉丁字母,
[Range("0080—00FF")]拉丁文补充1,
[Range("0100—017F")]拉丁文扩展A,
[Range("0180—024F")]拉丁文扩展B,
[Range("0250—02AF")]国际音标扩展,
[Range("02B0—02FF")]占位修饰符号,
[Range("0300—036F")]结合附加符号,
[Range("0370—03FF")]希腊字母及科普特字母,
[Range("0400—04FF")]西里尔字母,
[Range("0500—052F")]西里尔字母补充,
[Range("0530—058F")]亚美尼亚字母,
[Range("0590—05FF")]希伯来文,
[Range("0600—06FF")]阿拉伯文,
[Range("0700—074F")]叙利亚文,
[Range("0750—077F")]阿拉伯文补充,
[Range("0780—07BF")]它拿字母,
[Range("07C0—07FF")]西非书面语言,
[Range("0800—083F")]撒玛利亚字母,
[Range("0840—085F")]Mandaic,
[Range("0860—086F")]SyriacSupplement,
[Range("08A0—08FF")]阿拉伯语扩展,
[Range("0900—097F")]天城文,
[Range("0980—09FF")]孟加拉文,
[Range("0A00—0A7F")]果鲁穆奇字母,
[Range("0A80—0AFF")]古吉拉特文,
[Range("0B00—0B7F")]奥里亚文,
[Range("0B80—0BFF")]泰米尔文,
[Range("0C00—0C7F")]泰卢固文,
[Range("0C80—0CFF")]卡纳达文,
[Range("0D00—0D7F")]马拉雅拉姆文,
[Range("0D80—0DFF")]僧伽罗文,
[Range("0E00—0E7F")]泰文,
[Range("0E80—0EFF")]老挝文,
[Range("0F00—0FFF")]藏文,
[Range("1000—109F")]缅甸文,
[Range("10A0—10FF")]格鲁吉亚字母,
[Range("1100—11FF")]谚文字母,
[Range("1200—137F")]埃塞俄比亚语,
[Range("1380—139F")]埃塞俄比亚语补充,
[Range("13A0—13FF")]切罗基字母,
[Range("1400—167F")]统一加拿大原住民音节文字,
[Range("1680—169F")]欧甘字母,
[Range("16A0—16FF")]卢恩字母,
[Range("1700—171F")]他加禄字母,
[Range("1720—173F")]哈努诺文,
[Range("1740—175F")]布迪文,
[Range("1760—177F")]塔格巴努亚文,
[Range("1780—17FF")]高棉文,
[Range("1800—18AF")]蒙古文,
[Range("18B0—18FF")]统一加拿大原住民音节文字扩展,
[Range("1900—194F")]林布文,
[Range("1950—197F")]德宏傣文,
[Range("1980—19DF")]新傣仂文,
[Range("19E0—19FF")]高棉文符号,
[Range("1A00—1A1F")]布吉文,
[Range("1A20—1AAF")]老傣文,
[Range("1AB0—1AFF")]CombiningDiacriticalMarksExtended,
[Range("1B00—1B7F")]巴厘字母,
[Range("1B80—1BBF")]巽他字母,
[Range("1BC0—1BFF")]巴塔克文,
[Range("1C00—1C4F")]雷布查字母,
[Range("1C50—1C7F")]Ol_Chiki,
[Range("1C80—1C8F")]CyrillicExtendedC,
[Range("1C90—1CBF")]GeorgianExtended,
[Range("1CC0—1CCF")]巽他字母补充,
[Range("1CD0—1CFF")]吠陀梵文,
[Range("1D00—1D7F")]语音学扩展,
[Range("1D80—1DBF")]语音学扩展补充,
[Range("1DC0—1DFF")]结合附加符号补充,
[Range("1E00—1EFF")]拉丁文扩展附加,
[Range("1F00—1FFF")]希腊语扩展,
[Range("2000—206F")]常用标点,
[Range("2070—209F")]上标及下标,
[Range("20A0—20CF")]货币符号,
[Range("20D0—20FF")]组合用记号,
[Range("2100—214F")]字母式符号,
[Range("2150—218F")]数字形式,
[Range("2190—21FF")]箭头,
[Range("2200—22FF")]数学运算符,
[Range("2300—23FF")]杂项工业符号,
[Range("2400—243F")]控制图片,
[Range("2440—245F")]光学识别符,
[Range("2460—24FF")]带圈或括号的字母数字,
[Range("2500—257F")]制表符,
[Range("2580—259F")]方块元素,
[Range("25A0—25FF")]几何图形,
[Range("2600—26FF")]杂项符号,
[Range("2700—27BF")]印刷符号,
[Range("27C0—27EF")]杂项数学符号A,
[Range("27F0—27FF")]追加箭头A,
[Range("2800—28FF")]盲文点字模型,
[Range("2900—297F")]追加箭头B,
[Range("2980—29FF")]杂项数学符号B,
[Range("2A00—2AFF")]追加数学运算符,
[Range("2B00—2BFF")]杂项符号和箭头,
[Range("2C00—2C5F")]格拉哥里字母,
[Range("2C60—2C7F")]拉丁文扩展C,
[Range("2C80—2CFF")]科普特字母,
[Range("2D00—2D2F")]格鲁吉亚字母补充,
[Range("2D30—2D7F")]提非纳文,
[Range("2D80—2DDF")]埃塞俄比亚语扩展,
[Range("2DE0—2DFF")]西里尔字母扩展,
[Range("2E00—2E7F")]追加标点,
[Range("2E80—2EFF")]中日韩部首补充,
[Range("2F00—2FDF")]康熙部首,
[Range("2FF0—2FFF")]表意文字描述符,
[Range("3000—303F")]中日韩符号和标点,
[Range("3040—309F")]日文平假名,
[Range("30A0—30FF")]日文片假名,
[Range("3100—312F")]注音字母,
[Range("3130—318F")]谚文兼容字母,
[Range("3190—319F")]象形字注释标志,
[Range("31A0—31BF")]注音字母扩展,
[Range("31C0—31EF")]中日韩笔画,
[Range("31F0—31FF")]日文片假名语音扩展,
[Range("3200—32FF")]带圈中日韩字母和月份,
[Range("3300—33FF")]中日韩字符集兼容,
[Range("3400—4DBF")]中日韩统一表意文字扩展A,
[Range("4DC0—4DFF")]易经六十四卦符号,
[Range("4E00—9FFF")]中日韩统一表意文字,
[Range("A000—A48F")]彝文音节,
[Range("A490—A4CF")]彝文字根,
[Range("A4D0—A4FF")]Lisu,
[Range("A500—A63F")]老傈僳文,
[Range("A640—A69F")]西里尔字母扩展B,
[Range("A6A0—A6FF")]巴姆穆语,
[Range("A700—A71F")]声调修饰字母,
[Range("A720—A7FF")]拉丁文扩展D,
[Range("A800—A82F")]锡尔赫特文,
[Range("A830—A83F")]印第安数字,
[Range("A840—A87F")]八思巴文,
[Range("A880—A8DF")]索拉什特拉,
[Range("A8E0—A8FF")]天城文扩展,
[Range("A900—A92F")]克耶字母,
[Range("A930—A95F")]勒姜语,
[Range("A960—A97F")]谚文字母扩展A,
[Range("A980—A9DF")]爪哇语,
[Range("A9E0—A9FF")]MyanmarExtended_B,
[Range("AA00—AA5F")]鞑靼文,
[Range("AA60—AA7F")]缅甸语扩展,
[Range("AA80—AADF")]越南傣文,
[Range("AAE0—AAFF")]曼尼普尔文扩展,
[Range("AB00—AB2F")]埃塞俄比亚文,
[Range("AB30—AB6F")]LatinExtended_E,
[Range("AB70—ABBF")]CherokeeSupplement,
[Range("ABC0—ABFF")]曼尼普尔文,
[Range("AC00—D7AF")]谚文音节,
[Range("D7B0—D7FF")]HangulJamoExtended_B,
[Range("D800—DB7F")]代理对高位字,
[Range("DB80—DBFF")]代理对私用区高位字,
[Range("DC00—DFFF")]代理对低位字,
[Range("E000—F8FF")]私用区,
[Range("F900—FAFF")]中日韩兼容表意文字,
[Range("FB00—FB4F")]字母表达形式_拉丁字母连字_亚美尼亚字母连字_希伯来文表现形式,
[Range("FB50—FDFF")]阿拉伯文表达形式A,
[Range("FE00—FE0F")]异体字选择符,
[Range("FE10—FE1F")]竖排形式,
[Range("FE20—FE2F")]组合用半符号,
[Range("FE30—FE4F")]中日韩兼容形式,
[Range("FE50—FE6F")]小写变体形式,
[Range("FE70—FEFF")]阿拉伯文表达形式B,
[Range("FF00—FFEF")]半角及全角形式,
[Range("FFF0—FFFF")]特殊,
        }

        internal class RangeAttribute : Attribute
        {
            public string RangStr { get; private set; }
            private Vector2Int _value;
            public Vector2Int Value { get { return _value; } }
            public RangeAttribute(string rangStr)
            {
                RangStr = rangStr;
                var array = rangStr.Split('—');
                _value.x = int.Parse(array[0], System.Globalization.NumberStyles.AllowHexSpecifier);
                _value.y = int.Parse(array[1], System.Globalization.NumberStyles.AllowHexSpecifier);
            }
        }

        internal class SubBlock
        {
            public BlockType Type;
            public Vector2Int[] Ranges;
            public SubBlock(BlockType type, params Vector2Int[] ranges)
            {
                Type = type;
                Ranges = ranges;
            }
        }

        private static readonly SubBlock[] _noCharInBlocks = new SubBlock[] {
            new SubBlock(BlockType.基本拉丁字母, new Vector2Int(0,0x1F)),
            new SubBlock(BlockType.拉丁文扩展A, new Vector2Int(0x80,0x9F)),
            new SubBlock(BlockType.结合附加符号, new Vector2Int(0x34F,0x34F)),
            new SubBlock(BlockType.希腊字母及科普特字母, new Vector2Int(0x378,0x379), new Vector2Int(0x380,0x383), new Vector2Int(0x38B,0x38B), new Vector2Int(0x38D,0x38D), new Vector2Int(0x3A2,0x3A2)),
            new SubBlock(BlockType.亚美尼亚字母, new Vector2Int(0x530,0x530), new Vector2Int(0x557,0x558), new Vector2Int(0x58B,0x58C)),
            new SubBlock(BlockType.希伯来文, new Vector2Int(0x590,0x590), new Vector2Int(0x5C8,0x5CF), new Vector2Int(0x5EB,0x5EE), new Vector2Int(0x5F5,0x5FF)),
            new SubBlock(BlockType.阿拉伯文, new Vector2Int(0x61C,0x61D)),
            new SubBlock(BlockType.叙利亚文, new Vector2Int(0x70E,0x70E), new Vector2Int(0x74B,0x74C)),
            new SubBlock(BlockType.它拿字母, new Vector2Int(0x7B2,0x7BF)),
            new SubBlock(BlockType.西非书面语言, new Vector2Int(0x7FB,0x7FC)),
            new SubBlock(BlockType.撒玛利亚字母, new Vector2Int(0x82E,0x82F), new Vector2Int(0x83F,0x83F)),
            /*...*/
            new SubBlock(BlockType.谚文字母, new Vector2Int(0x115F,0x1160), new Vector2Int(0x83F,0x83F)),
            /*...*/
            new SubBlock(BlockType.蒙古文, new Vector2Int(0x180B,0x180F), new Vector2Int(0x181A,0x181F), new Vector2Int(0x1879,0x187F), new Vector2Int(0x18AB,0x18AF)),
            /*...*/
            new SubBlock(BlockType.中日韩部首补充, new Vector2Int(0x2E9A,0x2E9A), new Vector2Int(0x2EF4,0x2EFF)),
            new SubBlock(BlockType.康熙部首, new Vector2Int(0x2FD6,0x2FDF)),
            new SubBlock(BlockType.表意文字描述符, new Vector2Int(0x2FFC,0x2FFF)),
            new SubBlock(BlockType.日文平假名, new Vector2Int(0x3040,0x3040), new Vector2Int(0x3097,0x3098)),
            new SubBlock(BlockType.注音字母, new Vector2Int(0x3100,0x3104)),
            new SubBlock(BlockType.谚文兼容字母, new Vector2Int(0x3130,0x3130), new Vector2Int(0x318F,0x318F)),
            new SubBlock(BlockType.中日韩笔画, new Vector2Int(0x31E4,0x31EF)),
            new SubBlock(BlockType.带圈中日韩字母和月份, new Vector2Int(0x321F,0x321F)),
            new SubBlock(BlockType.彝文字根, new Vector2Int(0xA4C7,0xA4CF)),
            /*...*/
        };

        internal struct BlockInfo
        {
            public BlockType type;
            public int Count;
            public int CharCount;
            public Vector2Int Range;
            public SubBlock NoCharInfo;
        }

        private static Dictionary<BlockType, BlockInfo> _blocksInfo = new Dictionary<BlockType, BlockInfo>(20);

        public class BlocksGroupContext
        {
            internal BlockType[] Blocks;
            internal float[] Weights;
            internal BlockInfo[] BlocksInfo;
        }
        /// <summary>
        /// 获取常用语言数组
        /// </summary>
        public static BlockType[] GetCommonUseBlocks()
        {
            return new BlockType[]
            {
                BlockType.基本拉丁字母, BlockType.拉丁文补充1, BlockType.拉丁文扩展A, BlockType.拉丁文扩展B, BlockType.国际音标扩展,
                BlockType.占位修饰符号,
                BlockType.希腊字母及科普特字母,
                BlockType.西里尔字母, BlockType.西里尔字母补充,
                BlockType.中日韩部首补充, BlockType.康熙部首, BlockType.表意文字描述符, BlockType.中日韩符号和标点,
                BlockType.日文平假名, BlockType.日文片假名, BlockType.注音字母, BlockType.谚文兼容字母, BlockType.中日韩统一表意文字,
            };
        }
        /// <summary>
        /// 生成语言数组的context
        /// </summary>
        public static BlocksGroupContext GenerateContext(BlockType[] group)
        {
            int n = group.Length;
            var ctx = new BlocksGroupContext() { Blocks = (BlockType[])group.Clone() };
            ctx.BlocksInfo = new BlockInfo[n];
            ctx.Weights = new float[n];
            int sum = 0;
            for (int i = 0; i < n; i++)
            {
                ctx.BlocksInfo[i] = GetBlockInfo(ctx.Blocks[i]);
                sum += ctx.BlocksInfo[i].CharCount;
            }

            for (int i = 0; i < n; i++)
            {
                ctx.Weights[i] = ctx.BlocksInfo[i].CharCount / (float)sum;
            }

            return ctx;
        }

        static StringBuilder _sb = new StringBuilder(10);
        /// <summary>
        /// 使用指定的语言数组的context生成随机文字
        /// </summary>
        public static string Random(BlocksGroupContext ctx, int count)
        {
            _sb.Clear();
            for (int i = 0; i < count; i++)
            {
                float r = UnityEngine.Random.value;
                float sum = 0;
                for (int j = 0; j < ctx.Blocks.Length; j++)
                {
                    sum += ctx.Weights[j];
                    if (sum>= r)
                    {
                        bool noChar = true;
                        int value = -1;
                        var info = ctx.BlocksInfo[j];
                        do
                        {
                            value = UnityEngine.Random.Range(ctx.BlocksInfo[j].Range.x, ctx.BlocksInfo[j].Range.y);
                            noChar = false;
                            if (info.NoCharInfo != null && info.NoCharInfo.Ranges != null)
                            {
                                foreach (var sub in info.NoCharInfo.Ranges)
                                {
                                    if (value >= sub.x && value <= sub.y)
                                    {
                                        noChar = true;
                                        break;
                                    }
                                }
                            }
                        } while (noChar);
                        Debug.Assert(value >= 0, value);
                        var chr = Encoding.Unicode.GetString(BitConverter.GetBytes(value), 0, 2);// 只使用了2字节，int是4字节
                        Debug.AssertFormat(chr.Length == 1, "->{0}<-",chr);
                        _sb.Append(chr);
                        break;
                    }
                }
            }
            Debug.AssertFormat(_sb.Length == count, "string length:{0}, count:{1}", _sb.Length, count);
            return _sb.ToString();
        }

        internal static BlockInfo GetBlockInfo(BlockType type)
        {
            BlockInfo info;
            if (_blocksInfo.TryGetValue(type, out info))
            {
                return info;
            }
            var attr = GetRange(type);
            Debug.Assert(attr != null);
            info = new BlockInfo();
            info.type = type;
            info.Range = attr.Value;
            info.Count = info.Range.y - info.Range.x + 1;
            var noChar = Array.Find(_noCharInBlocks, n => n.Type == type);
            info.NoCharInfo = noChar;
            if (info.NoCharInfo != null)
            {
                int noCharCount = 0;
                foreach (var sub in info.NoCharInfo.Ranges)
                {
                    noCharCount += (sub.y - sub.x + 1);
                }

                info.CharCount = info.Count - noCharCount;
                Debug.Assert(info.CharCount >= 0);
            }
            else
                info.CharCount = info.Count;

            _blocksInfo.Add(type, info);
            return info;
        }

        internal static RangeAttribute GetRange(BlockType value)
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            return enumType.GetField(name).GetCustomAttributes(false).OfType<RangeAttribute>().SingleOrDefault();
        }
    }
}
