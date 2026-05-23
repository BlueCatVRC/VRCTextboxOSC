using SharpOSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCTextboxOSC
{
    public class OscMessageWithUTF8Strings : OscPacket
    {
        public string Address;

        public List<object> Arguments;

        public OscMessageWithUTF8Strings(string address, params object[] args)
        {
            Address = address;
            Arguments = new List<object>();
            Arguments.AddRange(args);
        }

        public override byte[] GetBytes()
        {
            List<byte[]> list = new List<byte[]>();
            List<object> list2 = Arguments;
            int num = 0;
            string text = ",";
            int num2 = 0;
            while (num2 < list2.Count)
            {
                object obj = list2[num2];
                string text2 = ((obj != null) ? obj.GetType().ToString() : "null");
                switch (text2)
                {
                    case "System.Int32":
                        text += "i";
                        list.Add(OscPacket.setInt((int)obj));
                        break;
                    case "System.Single":
                        if (float.IsPositiveInfinity((float)obj))
                        {
                            text += "I";
                            break;
                        }

                        text += "f";
                        list.Add(OscPacket.setFloat((float)obj));
                        break;
                    case "System.String":
                        text += "s";

                        // Manually do the work of setString in OscPacket, but using UTF8 rather than ASCII.

                        byte[] bytes = Encoding.UTF8.GetBytes((string)obj);
                        int n = ((bytes.Length + 4) / 4) * 4;
                        byte[] arr = new byte[n];

                        Array.Copy(bytes, arr, bytes.Length);

                        list.Add(arr);
                        break;
                    case "System.Byte[]":
                        text += "b";
                        list.Add(OscPacket.setBlob((byte[])obj));
                        break;
                    case "System.Int64":
                        text += "h";
                        list.Add(OscPacket.setLong((long)obj));
                        break;
                    case "System.UInt64":
                        text += "t";
                        list.Add(OscPacket.setULong((ulong)obj));
                        break;
                    case "SharpOSC.Timetag":
                        text += "t";
                        list.Add(OscPacket.setULong(((Timetag)obj).Tag));
                        break;
                    case "System.Double":
                        if (double.IsPositiveInfinity((double)obj))
                        {
                            text += "I";
                            break;
                        }

                        text += "d";
                        list.Add(OscPacket.setDouble((double)obj));
                        break;
                    case "SharpOSC.Symbol":
                        text += "S";
                        list.Add(OscPacket.setString(((Symbol)obj).Value));
                        break;
                    case "System.Char":
                        text += "c";
                        list.Add(OscPacket.setChar((char)obj));
                        break;
                    case "SharpOSC.RGBA":
                        text += "r";
                        list.Add(OscPacket.setRGBA((RGBA)obj));
                        break;
                    case "SharpOSC.Midi":
                        text += "m";
                        list.Add(OscPacket.setMidi((Midi)obj));
                        break;
                    case "System.Boolean":
                        text += (((bool)obj) ? "T" : "F");
                        break;
                    case "null":
                        text += "N";
                        break;
                    case "System.Object[]":
                    case "System.Collections.Generic.List`1[System.Object]":
                        if ((object)obj.GetType() == typeof(object[]))
                        {
                            obj = ((object[])obj).ToList();
                        }

                        if (Arguments != list2)
                        {
                            throw new Exception("Nested Arrays are not supported");
                        }

                        text += "[";
                        list2 = (List<object>)obj;
                        num = num2;
                        num2 = 0;
                        continue;
                    default:
                        throw new Exception("Unable to transmit values of type " + text2);
                }

                num2++;
                if (list2 != Arguments && num2 == list2.Count)
                {
                    text += "]";
                    list2 = Arguments;
                    num2 = num + 1;
                }
            }

            int num3 = ((Address.Length != 0 && Address != null) ? Utils.AlignedStringLength(Address) : 0);
            int num4 = Utils.AlignedStringLength(text);
            int num5 = num3 + num4 + list.Sum((byte[] x) => x.Length);
            byte[] array = new byte[num5];
            num2 = 0;
            Encoding.ASCII.GetBytes(Address).CopyTo(array, num2);
            num2 += num3;
            Encoding.ASCII.GetBytes(text).CopyTo(array, num2);
            num2 += num4;
            foreach (byte[] item in list)
            {
                item.CopyTo(array, num2);
                num2 += item.Length;
            }

            return array;
        }
    }
}
