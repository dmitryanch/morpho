using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Keyboard
{
	public class Qwerty
	{
		public static Dictionary<byte, HashSet<byte>> DistanceCodeKey = new Dictionary<byte, HashSet<byte>>
		{
            /* '`' */ { 192 , new HashSet<byte>(){ 49, 81 }},
            /* '1' */ { 49 , new HashSet<byte>(){ 50, 87, 81 }},
            /* '2' */ { 50 , new HashSet<byte>(){ 49, 81, 87, 69, 51 }},
            /* '3' */ { 51 , new HashSet<byte>(){ 50, 87, 69, 82, 52 }},
            /* '4' */ { 52 , new HashSet<byte>(){ 51, 69, 82, 84, 53 }},
            /* '5' */ { 53 , new HashSet<byte>(){ 52, 82, 84, 89, 54 }},
            /* '6' */ { 54 , new HashSet<byte>(){ 53, 84, 89, 85, 55 }},
            /* '7' */ { 55 , new HashSet<byte>(){ 54, 89, 85, 73, 56 }},
            /* '8' */ { 56 , new HashSet<byte>(){ 55, 85, 73, 79, 57 }},
            /* '9' */ { 57 , new HashSet<byte>(){ 56, 73, 79, 80, 48 }},
            /* '0' */ { 48 , new HashSet<byte>(){ 57, 79, 80, 219, 189 }},
            /* '-' */ { 189 , new HashSet<byte>(){ 48, 80, 219, 221, 187 }},
            /* '+' */ { 187 , new HashSet<byte>(){ 189, 219, 221 }},
            /* 'q' */ { 81 , new HashSet<byte>(){ 49, 50, 87, 83, 65 }},
            /* 'w' */ { 87 , new HashSet<byte>(){ 49, 81, 65, 83, 68, 69, 51, 50 }},
            /* 'e' */ { 69 , new HashSet<byte>(){ 50, 87, 83, 68, 70, 82, 52, 51 }},
            /* 'r' */ { 82 , new HashSet<byte>(){ 51, 69, 68, 70, 71, 84, 53, 52 }},
            /* 't' */ { 84 , new HashSet<byte>(){ 52, 82, 70, 71, 72, 89, 54, 53 }},
            /* 'y' */ { 89 , new HashSet<byte>(){ 53, 84, 71, 72, 74, 85, 55, 54 }},
            /* 'u' */ { 85 , new HashSet<byte>(){ 54, 89, 72, 74, 75, 73, 56, 55 }},
            /* 'i' */ { 73 , new HashSet<byte>(){ 55, 85, 74, 75, 76, 79, 57, 56 }},
            /* 'o' */ { 79 , new HashSet<byte>(){ 56, 73, 75, 76, 186, 80, 48, 57 }},
            /* 'p' */ { 80 , new HashSet<byte>(){ 57, 79, 76, 186, 222, 219, 189, 48 }},
            /* '[' */ { 219 , new HashSet<byte>(){ 48, 186, 222, 221, 187, 189 }},
            /* ']' */ { 221 , new HashSet<byte>(){ 189, 219, 187 }},
            /* 'a' */ { 65 , new HashSet<byte>(){ 81, 87, 83, 88, 90 }},
            /* 's' */ { 83 , new HashSet<byte>(){ 81, 65, 90, 88, 67, 68, 69, 87, 81 }},
            /* 'd' */ { 68 , new HashSet<byte>(){ 87, 83, 88, 67, 86, 70, 82, 69 }},
            /* 'f' */ { 70 , new HashSet<byte>(){ 69, 68, 67, 86, 66, 71, 84, 82 }},
            /* 'g' */ { 71 , new HashSet<byte>(){ 82, 70, 86, 66, 78, 72, 89, 84 }},
            /* 'h' */ { 72 , new HashSet<byte>(){ 84, 71, 66, 78, 77, 74, 85, 89 }},
            /* 'j' */ { 74 , new HashSet<byte>(){ 89, 72, 78, 77, 188, 75, 73, 85 }},
            /* 'k' */ { 75 , new HashSet<byte>(){ 85, 74, 77, 188, 190, 76, 79, 73 }},
            /* 'l' */ { 76 , new HashSet<byte>(){ 73, 75, 188, 190, 191, 186, 80, 79 }},
            /* ';' */ { 186 , new HashSet<byte>(){ 79, 76, 190, 191, 222, 219, 80 }},
            /* '\''*/ { 222 , new HashSet<byte>(){ 80, 186, 191, 221, 219 }},
            /* 'z' */ { 90 , new HashSet<byte>(){ 65, 83, 88 }},
            /* 'x' */ { 88 , new HashSet<byte>(){ 90, 65, 83, 68, 67 }},
            /* 'c' */ { 67 , new HashSet<byte>(){ 88, 83, 68, 70, 86 }},
            /* 'v' */ { 86 , new HashSet<byte>(){ 67, 68, 70, 71, 66 }},
            /* 'b' */ { 66 , new HashSet<byte>(){ 86, 70, 71, 72, 78 }},
            /* 'n' */ { 78 , new HashSet<byte>(){ 66, 71, 72, 74, 77 }},
            /* 'm' */ { 77 , new HashSet<byte>(){ 78, 72, 74, 75, 188 }},
            /* '<' */ { 188 , new HashSet<byte>(){ 77, 74, 75, 76, 190 }},
            /* '>' */ { 190 , new HashSet<byte>(){ 188, 75, 76, 186, 191 }},
            /* '?' */ { 191 , new HashSet<byte>(){ 190, 76, 186, 222 }},
		};

	}
}
