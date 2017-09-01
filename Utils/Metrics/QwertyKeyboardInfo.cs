using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.Metrics
{
    public class QwertyKeyboardInfo
    {
		public static Dictionary<int, HashSet<int>> DistanceCodeKey = new Dictionary<int, HashSet<int>>
		{
            /* '`' */ { 192 , new HashSet<int>(){ 49 }},
            /* '1' */ { 49 , new HashSet<int>(){ 50, 87, 81 }},
            /* '2' */ { 50 , new HashSet<int>(){ 49, 81, 87, 69, 51 }},
            /* '3' */ { 51 , new HashSet<int>(){ 50, 87, 69, 82, 52 }},
            /* '4' */ { 52 , new HashSet<int>(){ 51, 69, 82, 84, 53 }},
            /* '5' */ { 53 , new HashSet<int>(){ 52, 82, 84, 89, 54 }},
            /* '6' */ { 54 , new HashSet<int>(){ 53, 84, 89, 85, 55 }},
            /* '7' */ { 55 , new HashSet<int>(){ 54, 89, 85, 73, 56 }},
            /* '8' */ { 56 , new HashSet<int>(){ 55, 85, 73, 79, 57 }},
            /* '9' */ { 57 , new HashSet<int>(){ 56, 73, 79, 80, 48 }},
            /* '0' */ { 48 , new HashSet<int>(){ 57, 79, 80, 219, 189 }},
            /* '-' */ { 189 , new HashSet<int>(){ 48, 80, 219, 221, 187 }},
            /* '+' */ { 187 , new HashSet<int>(){ 189, 219, 221 }},
            /* 'q' */ { 81 , new HashSet<int>(){ 49, 50, 87, 83, 65 }},
            /* 'w' */ { 87 , new HashSet<int>(){ 49, 81, 65, 83, 68, 69, 51, 50 }},
            /* 'e' */ { 69 , new HashSet<int>(){ 50, 87, 83, 68, 70, 82, 52, 51 }},
            /* 'r' */ { 82 , new HashSet<int>(){ 51, 69, 68, 70, 71, 84, 53, 52 }},
            /* 't' */ { 84 , new HashSet<int>(){ 52, 82, 70, 71, 72, 89, 54, 53 }},
            /* 'y' */ { 89 , new HashSet<int>(){ 53, 84, 71, 72, 74, 85, 55, 54 }},
            /* 'u' */ { 85 , new HashSet<int>(){ 54, 89, 72, 74, 75, 73, 56, 55 }},
            /* 'i' */ { 73 , new HashSet<int>(){ 55, 85, 74, 75, 76, 79, 57, 56 }},
            /* 'o' */ { 79 , new HashSet<int>(){ 56, 73, 75, 76, 186, 80, 48, 57 }},
            /* 'p' */ { 80 , new HashSet<int>(){ 57, 79, 76, 186, 222, 219, 189, 48 }},
            /* '[' */ { 219 , new HashSet<int>(){ 48, 186, 222, 221, 187, 189 }},
            /* ']' */ { 221 , new HashSet<int>(){ 189, 219, 187 }},
            /* 'a' */ { 65 , new HashSet<int>(){ 81, 87, 83, 88, 90 }},
            /* 's' */ { 83 , new HashSet<int>(){ 81, 65, 90, 88, 67, 68, 69, 87, 81 }},
            /* 'd' */ { 68 , new HashSet<int>(){ 87, 83, 88, 67, 86, 70, 82, 69 }},
            /* 'f' */ { 70 , new HashSet<int>(){ 69, 68, 67, 86, 66, 71, 84, 82 }},
            /* 'g' */ { 71 , new HashSet<int>(){ 82, 70, 86, 66, 78, 72, 89, 84 }},
            /* 'h' */ { 72 , new HashSet<int>(){ 84, 71, 66, 78, 77, 74, 85, 89 }},
            /* 'j' */ { 74 , new HashSet<int>(){ 89, 72, 78, 77, 188, 75, 73, 85 }},
            /* 'k' */ { 75 , new HashSet<int>(){ 85, 74, 77, 188, 190, 76, 79, 73 }},
            /* 'l' */ { 76 , new HashSet<int>(){ 73, 75, 188, 190, 191, 186, 80, 79 }},
            /* ';' */ { 186 , new HashSet<int>(){ 79, 76, 190, 191, 222, 219, 80 }},
            /* '\''*/ { 222 , new HashSet<int>(){ 80, 186, 191, 221, 219 }},
            /* 'z' */ { 90 , new HashSet<int>(){ 65, 83, 88 }},
            /* 'x' */ { 88 , new HashSet<int>(){ 90, 65, 83, 68, 67 }},
            /* 'c' */ { 67 , new HashSet<int>(){ 88, 83, 68, 70, 86 }},
            /* 'v' */ { 86 , new HashSet<int>(){ 67, 68, 70, 71, 66 }},
            /* 'b' */ { 66 , new HashSet<int>(){ 86, 70, 71, 72, 78 }},
            /* 'n' */ { 78 , new HashSet<int>(){ 66, 71, 72, 74, 77 }},
            /* 'm' */ { 77 , new HashSet<int>(){ 78, 72, 74, 75, 188 }},
            /* '<' */ { 188 , new HashSet<int>(){ 77, 74, 75, 76, 190 }},
            /* '>' */ { 190 , new HashSet<int>(){ 188, 75, 76, 186, 191 }},
            /* '?' */ { 191 , new HashSet<int>(){ 190, 76, 186, 222 }},
		};

	}
}
