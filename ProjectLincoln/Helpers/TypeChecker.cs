using System;

namespace ProjectLincoln.Helpers {
    public class TypeChecker {

        /// <summary>
        /// Checks if the provided object is numeric
        /// </summary>
        /// <param name="o">The object to check</param>
        /// <returns>The IsNumeric status</returns>
        public static bool IsNumeric (Object o) {
            switch (Type.GetTypeCode(o.GetType())) {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if the provided object is boolean
        /// </summary>
        /// <param name="o">The object to check</param>
        /// <returns>The IsBoolean status</returns>
        public static bool IsBoolean (Object o) {
            switch (Type.GetTypeCode(o.GetType())) {
                case TypeCode.Boolean:
                    return true;
                default:
                    return false;
            }
        }
    }
}
