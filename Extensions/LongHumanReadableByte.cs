using System;

namespace Firefly.Extensions{
    public static class LongHumanReadableByte{
        public static string ToHumanReadableBytes(this long number)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            var len = (double)number;
            int order = 0;
            while (len >= 1024 && ++order < sizes.Length) {
                len = len/1024;
            }

            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }
    }
}